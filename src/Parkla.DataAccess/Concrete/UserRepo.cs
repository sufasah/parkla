using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Parkla.Core.DTOs;
using Parkla.Core.Entities;
using Parkla.Core.Enums;
using Parkla.Core.Exceptions;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;

namespace Parkla.DataAccess.Concrete;

public class UserRepo<TContext> : EntityRepoBase<User, TContext>, IUserRepo
    where TContext: DbContext, new()
{
    public override async Task<User?> GetAsync<Tkey>(
        Tkey id, 
        CancellationToken cancellationToken = default
    )   where Tkey: struct 
    {
        int iid = (int)(object)id;
        using var context = new TContext();
        User? result = await context.Set<User>()
            .AsNoTracking()
            .Include(x => x.City)
            .Include(x => x.District)
            .SingleOrDefaultAsync(x => x.Id == iid, cancellationToken)
            .ConfigureAwait(false);
            
        return result;
    }

    public async Task<User?> LoadMoneyAsync(int id, float amount, CancellationToken cancellationToken)
    {
        using var context = new TContext();
        
        User? userClone = null;
        while(!cancellationToken.IsCancellationRequested) {
            var user = await context.Set<User>()
                .Include(x => x.City)
                .Include(x => x.District)
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if(user == null)
                throw new ParklaConcurrentDeletionException("The user to load money was deleted by another user");

            var result = context.Entry(user);
            userClone = (User)result.CurrentValues.Clone().ToObject();

            user.Wallet += amount;

            try {
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return user;
            } catch(DbUpdateConcurrencyException) {
                context.ChangeTracker.Clear();
            }
        }
        return userClone;
    }

    public override async Task<User> UpdateAsync(
        User user,
        Expression<Func<User, object?>>[] updateProps,
        bool updateOtherProps = true,
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();
        User userClone = user;
        while(!cancellationToken.IsCancellationRequested) {
            var result = context.Entry(user);
            userClone = (User)result.CurrentValues.Clone().ToObject();
            result.State = updateOtherProps ? EntityState.Modified : EntityState.Unchanged;
            foreach (var prop in updateProps)
                result.Property(prop).IsModified = !updateOtherProps;

            try {
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return await context.Set<User>()
                    .Include(x => x.City)
                    .Include(x => x.District)
                    .FirstAsync(x => x.Equals(user),cancellationToken)
                    .ConfigureAwait(false);
            } catch(DbUpdateConcurrencyException e) {
                var entry = e.Entries.Single();
                await entry.ReloadAsync(cancellationToken).ConfigureAwait(false);
                userClone.xmin = ((User)entry.Entity).xmin;
                context.ChangeTracker.Clear();
            }
        }
        return userClone;
    }

    public async Task<DashboardDto> GetDashboardAsync(int id, CancellationToken cancellationToken) {
        using var context = new TContext();
        var aMonthAgo = DateTime.UtcNow.Subtract(new TimeSpan(30,0,0,0));
        aMonthAgo = new DateTime(
            aMonthAgo.Year,
            aMonthAgo.Month,
            aMonthAgo.Day,
            0,0,0
        );

        var result = await context.Set<User>()
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Include(x => x.Parks)
                .ThenInclude(x => x.Areas)
                .ThenInclude(x => x.Spaces)
                .ThenInclude(x => x.Pricing)
            .Include(x => x.Parks)
                .ThenInclude(x => x.Areas)
                .ThenInclude(x => x.Spaces)
                .ThenInclude(x => x.RealSpace)
            .Include(x => x.Parks)
                .ThenInclude(x => x.Areas)
                .ThenInclude(x => x.Spaces)
                .ThenInclude(x => x.ReceivedSpaceStatusses)
            .Include(x => x.Parks)
                .ThenInclude(x => x.Areas)
                .ThenInclude(x => x.Spaces)
                .ThenInclude(x => x.Reservations!)
                .ThenInclude(x => x.User)
                .ThenInclude(x => x.City)
            .GroupBy(x => 1)
            .Select(g => new {
                ParksGroup = g.SelectMany(x => x.Parks)
                    .GroupBy(x => 1)
                    .Select(g2 => new {
                        AreasGroup = g2.SelectMany(x => x.Areas)
                            .GroupBy(x => 1)
                            .Select(g3 => new {
                                SpacesGroup = g3.SelectMany(x => x.Spaces)
                                    .GroupBy(x => 1)
                                    .Select(g4 => new {
                                        ReceivedSpaceStatussesGroup = g4.SelectMany(x => x.ReceivedSpaceStatusses)
                                            .GroupBy(x => 1)
                                            .Select(g5 => new {
                                                MinStatusDataTransferDelayInSeconds = g5.Min(x => (x.ReceivedTime!.Value - x.StatusDataTime!.Value).TotalSeconds),
                                                AverageStatusDataTransferDelayInSeconds = g5.Average(x => (x.ReceivedTime!.Value - x.StatusDataTime!.Value).TotalSeconds),
                                                MaxStatusDataTransferDelayInSeconds = g5.Max(x => (x.ReceivedTime!.Value - x.StatusDataTime!.Value).TotalSeconds),
                                                TotalCarsUsedSpaces = g5.Where(x => x.OldSpaceStatus == SpaceStatus.OCCUPIED && x.NewSpaceStatus == SpaceStatus.EMPTY).Count()
                                            })
                                            .FirstOrDefault(),
                                        ReservationsGroup = g4.SelectMany(x => x.Reservations!)
                                            .GroupBy(x => 1)
                                            .Select(g5 => new {
                                                ReservationCount = g5.Count(),
                                                
                                                TotalEarning = g5.Where(x => x.EndTime < DateTime.UtcNow)
                                                    .Sum(x => (x.EndTime!.Value - x.StartTime!.Value).TotalHours * x.Space!.Pricing!.Price
                                                        * (TimeUnit.MINUTE == x.Space!.Pricing!.Unit ? 60 : 1)
                                                        / (TimeUnit.DAY == x.Space!.Pricing!.Unit ? 24 : 1)
                                                        / (TimeUnit.MONTH == x.Space!.Pricing!.Unit ? 720 : 1)
                                                        / x.Space!.Pricing!.Amount),
                                                MostActive10User = g5.GroupBy(x => x.UserId)
                                                    .Select(g6 => new {
                                                        User = g6.First(x => x.UserId == g6.Key).User!,
                                                        ReservationCount = g6.Count()
                                                    })
                                                    .OrderByDescending(x => x.ReservationCount)
                                                    .Take(10)
                                                    .ToList()
                                            })
                                            .FirstOrDefault(),
                                        Count = g4.Count(),
                                        ReservedSpaceCount = g4.Where(x => x.Reservations!.Where(y => y.EndTime > DateTime.UtcNow).Any()).Count(),
                                    })
                                    .FirstOrDefault(),
                                Count = g3.Count(), 
                                ReservationEnabledCount = g3.Where(x => x.ReservationsEnabled == true).Count()
                            })
                            .FirstOrDefault(),
                        Count = g2.Count(),
                        MinPrice = g2.Min(x => x.MinPrice),
                        AvaragePrice = g2.Average(x => x.AvaragePrice),
                        MaxPrice = g2.Max(x => x.MaxPrice),
                        EmptySpaceCount = g2.Sum(x => (long?)x.EmptySpace),
                        OccupiedSpaceCount = g2.Sum(x => (long?)x.OccupiedSpace),
                    })
                    .FirstOrDefault(),
                Top10PopularParks = g.SelectMany(x => x.Parks)
                    .GroupBy(x => x.Id)
                    .Select(g2 => new {
                        Park = g2.First(x => x.Id == g2.Key),
                        ReservationCount = g2.SelectMany(x => x.Areas)
                            .SelectMany(x => x.Spaces)
                            .SelectMany(x => x.Reservations!)
                            .Count()
                    })
                    .OrderByDescending(x => x.ReservationCount)
                    .Take(10)
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
        
        var dto = new DashboardDto();

        if(result != null) {
            dto.TopPopularParks = result.Top10PopularParks.Select(x => x.Park).ToList();
            if(result.ParksGroup != null) {
                var parksGroup = result.ParksGroup;

                dto.TotalParks = parksGroup.Count;
                dto.MinParkPrice = parksGroup.MinPrice;
                dto.AverageParkPrice = parksGroup.AvaragePrice;
                dto.MaxParkPrice = parksGroup.MaxPrice;
                dto.TotalEmptySpaces = parksGroup.EmptySpaceCount ?? 0;
                dto.TotalOccupiedSpaces = parksGroup.OccupiedSpaceCount ?? 0;
                
                if(parksGroup.AreasGroup != null) {
                    var areasGroup = parksGroup.AreasGroup;

                    dto.TotalAreas = areasGroup.Count;
                    dto.TotalReservationEnabledAreas = areasGroup.ReservationEnabledCount;

                    if(areasGroup.SpacesGroup != null) {
                        var spacesGroup = areasGroup.SpacesGroup;

                        dto.TotalSpaces = spacesGroup.Count;
                        dto.TotalReservedSpaces = spacesGroup.ReservedSpaceCount;

                        if(spacesGroup.ReceivedSpaceStatussesGroup != null) {
                            var receivedSpaceStatussesGroup = spacesGroup.ReceivedSpaceStatussesGroup;

                            dto.MinStatusDataTransferDelayInSeconds = receivedSpaceStatussesGroup.MinStatusDataTransferDelayInSeconds;
                            dto.AverageStatusDataTransferDelayInSeconds = receivedSpaceStatussesGroup.AverageStatusDataTransferDelayInSeconds;
                            dto.MaxStatusDataTransferDelayInSeconds = receivedSpaceStatussesGroup.MaxStatusDataTransferDelayInSeconds;
                            dto.TotalCarsUsedSpaces = receivedSpaceStatussesGroup.TotalCarsUsedSpaces;
                        }

                        if(spacesGroup.ReservationsGroup != null) {
                            var reservationsGroup = spacesGroup.ReservationsGroup;

                            dto.MostActiveUsers = reservationsGroup.MostActive10User.Select(x => {
                                x.User.Address = null;
                                x.User.Birthdate = null;
                                x.User.Phone = null;
                                x.User.Password = null;
                                x.User.Wallet = null;
                                x.User.RefreshTokenSignature = null;
                                x.User.VerificationCode = null;
                                return x.User;
                            }).ToList();
                            dto.TotalReservations = reservationsGroup.ReservationCount;
                            dto.TotalEarning = reservationsGroup.TotalEarning ?? 0;
                        }
                    }
                }
            }
        }

        var seriesResult = await context.Set<ParkSpace>()
            .AsNoTracking()
            .Include(x => x.Area!)
            .ThenInclude(x => x.Park)
            .Include(x => x.Pricing)
            .Include(x => x.ReceivedSpaceStatusses)
            .Include(x => x.Reservations)
            .Where(x => x.Area!.Park!.UserId == id)
            .GroupBy(x => 1)
            .Select(g => new {
                SpaceUsageSeriesRaw = g.SelectMany(x => x.ReceivedSpaceStatusses)
                    .Where(x => x.StatusDataTime > aMonthAgo && (x.NewRealSpaceStatus == SpaceStatus.OCCUPIED || (x.OldRealSpaceStatus == SpaceStatus.OCCUPIED && x.NewRealSpaceStatus == SpaceStatus.EMPTY)))
                    .OrderBy(x => x.StatusDataTime)
                    .ToList(),
                EarningSeries = g.SelectMany(x => x.Reservations!)
                    .Where(x => x.StartTime < DateTime.UtcNow && x.StartTime >= aMonthAgo)
                    .GroupBy(x => new DateTime(x.StartTime!.Value.Year, x.StartTime.Value.Month, x.StartTime.Value.Day, 0,0,0, DateTimeKind.Utc))
                    .Select(g2 => new {
                        X = g2.Key,
                        Y = g2.Sum(x => (x.EndTime!.Value - x.StartTime!.Value).TotalHours * x.Space!.Pricing!.Price
                            * (TimeUnit.MINUTE == x.Space!.Pricing!.Unit ? 60 : 1)
                            / (TimeUnit.DAY == x.Space!.Pricing!.Unit ? 24 : 1)
                            / (TimeUnit.MONTH == x.Space!.Pricing!.Unit ? 720 : 1)
                            / x.Space!.Pricing!.Amount)
                    }),
                CarCountUsedSpaceSeries = g.SelectMany(x => x.ReceivedSpaceStatusses)
                    .Where(x => x.StatusDataTime > aMonthAgo && x.OldRealSpaceStatus != SpaceStatus.OCCUPIED && x.NewRealSpaceStatus == SpaceStatus.OCCUPIED)
                    .GroupBy(x => new DateTime(x.StatusDataTime!.Value.Year, x.StatusDataTime.Value.Month, x.StatusDataTime.Value.Day, 0,0,0, DateTimeKind.Utc))
                    .Select(g2 => new {
                        X = g2.Key,
                        Y = g2.Count()
                    }),
                                
            })
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
        
        if(seriesResult != null) {
            dto.CarCountUsedSpacePerDay = seriesResult.CarCountUsedSpaceSeries.Select(x => new TimeSeriesData(x.X, x.Y)).ToList();
            dto.TotalEarningPerDay = seriesResult.EarningSeries.Select(x => new TimeSeriesData(x.X, x.Y ?? 0)).ToList();

            var pairs = seriesResult.SpaceUsageSeriesRaw
                .Where(x => x.NewRealSpaceStatus == SpaceStatus.EMPTY && x.OldRealSpaceStatus == SpaceStatus.OCCUPIED)
                .Select((x,i) => {
                    var found = seriesResult.SpaceUsageSeriesRaw
                        .Where((y,i2) => y.SpaceId == x.SpaceId && y.NewRealSpaceStatus == SpaceStatus.OCCUPIED && i2 < i)
                        //.Max(Comparer<ReceivedSpaceStatus>.Create((v1,v2) => v1.StatusDataTime!.Value.CompareTo(v2.StatusDataTime!.Value)));
                        .Last();
                    if(found == null) return null;
                    return new {
                        FromOccupied = found,
                        ToEmpty = x
                    };
                })
                .Where(x => x != null);

            
        }

        return dto;
    }

    public void Test(int id) {
        using var context = new TContext();
        var aMonthAgo = DateTime.UtcNow.Subtract(new TimeSpan(30,0,0,0));
        aMonthAgo = new DateTime(
            aMonthAgo.Year,
            aMonthAgo.Month,
            aMonthAgo.Day,
            0,0,0
        );
/*TotalCarsUsedSpaces = g5.Where(x => x.OldSpaceStatus == SpaceStatus.OCCUPIED && x.NewSpaceStatus == SpaceStatus.EMPTY).Count()
    TotalEarning = g5.Where(x => x.EndTime < DateTime.UtcNow)
        .Sum(x => (x.EndTime!.Value - x.StartTime!.Value).TotalHours * x.Space!.Pricing!.Price
            * (TimeUnit.MINUTE == x.Space!.Pricing!.Unit ? 60 : 1)
            / (TimeUnit.DAY == x.Space!.Pricing!.Unit ? 24 : 1)
            / (TimeUnit.MONTH == x.Space!.Pricing!.Unit ? 720 : 1)
            / x.Space!.Pricing!.Amount)*/        
//Time = new DateTime(x.StatusDataTime!.Value.Year, x.StatusDataTime.Value.Month, x.StatusDataTime.Value.Day, 0,0,0, DateTimeKind.Utc)
        
    }
}