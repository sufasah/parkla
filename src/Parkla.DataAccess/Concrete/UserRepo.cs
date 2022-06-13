using System.Linq.Expressions;
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
        var aMonthAgo = DateTime.UtcNow.Subtract(new TimeSpan(30,0,0,0)).Date;

        var query = context.Set<User>()
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
                .ThenInclude(x => x!.User)
                .ThenInclude(x => x!.City)
            .GroupBy(x => 1);

        var result = await query.Select(
            g => new {
                ParkCount = g.First().Parks
                    .Count,
                ParkMinPrice = g.First().Parks
                    .Min(x => x.MinPrice),
                ParkAveragePrice = g.First().Parks
                    .Average(x => x.AveragePrice),
                ParkMaxPrice = g.First().Parks
                    .Max(x => x.MaxPrice),
                ParkEmptySpaceCount = g.First().Parks
                    .Sum(x => (long?)x.EmptySpace),
                ParkOccupiedSpaceCount = g.First().Parks
                    .Sum(x => (long?)x.OccupiedSpace),
                Top10PopularParks = g.First().Parks
                    .Select(x => new {
                        Park = x,
                        ReservationCount = x.Areas
                            .SelectMany(x => x.Spaces)
                            .SelectMany(x => x.Reservations!)
                            .Count()
                    })
                    .OrderByDescending(x => x.ReservationCount)
                    .Take(10)
                    .ToList(),
                AreaCount = g.First().Parks
                    .SelectMany(x => x.Areas)
                    .Count(),
                AreaReservationEnabledCount = g.First().Parks
                    .SelectMany(x => x.Areas)
                    .Where(x => x.ReservationsEnabled == true)
                    .Count(),
                SpaceCount = g.First().Parks
                    .SelectMany(x => x.Areas)
                    .SelectMany(x => x.Spaces)
                    .Count(),
                ReservedSpaceCount = g.First().Parks
                    .SelectMany(x => x.Areas)
                    .SelectMany(x => x.Spaces)
                    .Where(x => x.Reservations!.Where(y => y.EndTime > DateTime.UtcNow).Any())
                    .Count(),
                MinStatusDataTransferDelayInSeconds = g.First().Parks
                    .SelectMany(x => x.Areas)
                    .SelectMany(x => x.Spaces)
                    .SelectMany(x => x.ReceivedSpaceStatusses)
                    .Min(x => (double?)(x.ReceivedTime!.Value - x.StatusDataTime!.Value).TotalSeconds),// for empty result cast nullable
                AverageStatusDataTransferDelayInSeconds = g.First().Parks
                    .SelectMany(x => x.Areas)
                    .SelectMany(x => x.Spaces)
                    .SelectMany(x => x.ReceivedSpaceStatusses)
                    .Average(x => (double?)(x.ReceivedTime!.Value - x.StatusDataTime!.Value).TotalSeconds),
                MaxStatusDataTransferDelayInSeconds = g.First().Parks
                    .SelectMany(x => x.Areas)
                    .SelectMany(x => x.Spaces)
                    .SelectMany(x => x.ReceivedSpaceStatusses)
                    .Max(x => (double?)(x.ReceivedTime!.Value - x.StatusDataTime!.Value).TotalSeconds),
                TotalCarsUsedSpaces = g.First().Parks
                    .SelectMany(x => x.Areas)
                    .SelectMany(x => x.Spaces)
                    .SelectMany(x => x.ReceivedSpaceStatusses)
                    .Where(x => x.OldSpaceStatus == SpaceStatus.OCCUPIED && x.NewSpaceStatus == SpaceStatus.EMPTY).Count(),
                ReservationCount = g.First().Parks
                    .SelectMany(x => x.Areas)
                    .SelectMany(x => x.Spaces)
                    .SelectMany(x => x.Reservations!)
                    .Count(),
                TotalEarning = g.First().Parks
                    .SelectMany(x => x.Areas)
                    .SelectMany(x => x.Spaces)
                    .SelectMany(x => x.Reservations!)
                    .Where(x => x.StartTime < DateTime.UtcNow.AddDays(1).Date)
                    .Sum(x => (x.EndTime!.Value - x.StartTime!.Value).TotalHours * x.Space!.Pricing!.Price
                        * (TimeUnit.MINUTE == x.Space!.Pricing!.Unit ? 60 : 1)
                        / (TimeUnit.DAY == x.Space!.Pricing!.Unit ? 24 : 1)
                        / (TimeUnit.MONTH == x.Space!.Pricing!.Unit ? 720 : 1)
                        / x.Space!.Pricing!.Amount),
                MostActive10User = g.First().Parks
                    .SelectMany(x => x.Areas)
                    .SelectMany(x => x.Spaces)
                    .SelectMany(x => x.Reservations!)
                    .Select(x => x.User!)
                    .GroupBy(x => x.Id)
                    .Select(g2 => new {
                        User = g2.First()!,
                        ReservationCount = g2.First()!.Reservations.Count()
                    })
                    .OrderByDescending(x => x.ReservationCount)
                    .Take(10)
                    .ToList()
            }
        )
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);
        
        var dto = new DashboardDto(aMonthAgo);

        if(result != null) {
            dto.TopPopularParks = result.Top10PopularParks.Select(x => x.Park).ToList();

            dto.TotalParks = result.ParkCount;
            dto.MinParkPrice = result.ParkMinPrice;
            dto.AverageParkPrice = result.ParkAveragePrice;
            dto.MaxParkPrice = result.ParkMaxPrice;
            dto.TotalEmptySpaces = result.ParkEmptySpaceCount ?? 0;
            dto.TotalOccupiedSpaces = result.ParkOccupiedSpaceCount ?? 0;

            dto.TotalAreas = result.AreaCount;
            dto.TotalReservationEnabledAreas = result.AreaReservationEnabledCount;

            dto.TotalSpaces = result.SpaceCount;
            dto.TotalReservedSpaces = result.ReservedSpaceCount;

            dto.MinStatusDataTransferDelayInSeconds = result.MinStatusDataTransferDelayInSeconds;
            dto.AverageStatusDataTransferDelayInSeconds = result.AverageStatusDataTransferDelayInSeconds;
            dto.MaxStatusDataTransferDelayInSeconds = result.MaxStatusDataTransferDelayInSeconds;
            dto.TotalCarsUsedSpaces = result.TotalCarsUsedSpaces;

            dto.MostActiveUsers = result.MostActive10User.Select(x => {
                x.User.Address = null;
                x.User.Birthdate = null;
                x.User.Phone = null;
                x.User.Password = null;
                x.User.Wallet = null;
                x.User.RefreshTokenSignature = null;
                x.User.VerificationCode = null;
                return x.User;
            }).ToList();
            dto.TotalReservations = result.ReservationCount;
            dto.TotalEarning = result.TotalEarning ?? 0;
        }

        var seriesQuery = context.Set<ParkSpace>()
            .AsNoTracking()
            .Include(x => x.Area!)
            .ThenInclude(x => x.Park)
            .Include(x => x.Pricing)
            .Include(x => x.ReceivedSpaceStatusses)
            .Include(x => x.Reservations)
            .Where(x => x.Area!.Park!.UserId == id)
            .GroupBy(x => 1);

        var spaceUsageSeriesRaw = await seriesQuery.Select(g =>
                g.SelectMany(x => x.ReceivedSpaceStatusses)
                    .Where(x => x.StatusDataTime > aMonthAgo && (x.NewRealSpaceStatus == SpaceStatus.OCCUPIED || (x.OldRealSpaceStatus == SpaceStatus.OCCUPIED && x.NewRealSpaceStatus == SpaceStatus.EMPTY)))
                    .OrderBy(x => x.StatusDataTime)
                    .ToList())
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
        
        var earningSeries = await seriesQuery.Select(g => 
                g.SelectMany(x => x.Reservations!)
                    .Where(x => x.StartTime < DateTime.UtcNow.AddDays(1).Date && x.StartTime >= aMonthAgo && x.Space!.PricingId != null)
                    .Select(x => new {
                        Reservation = x,
                        Pricing = x.Space!.Pricing!
                    })
                    .ToList())
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        var carCountUsedSpaceSeries = await seriesQuery.Select(g => 
                g.SelectMany(x => x.ReceivedSpaceStatusses)
                    .Where(x => x.StatusDataTime > aMonthAgo && x.OldRealSpaceStatus == SpaceStatus.OCCUPIED && x.NewRealSpaceStatus == SpaceStatus.EMPTY)
                    .ToList())
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
        
        if(earningSeries != null)
            earningSeries
                .GroupBy(x => x.Reservation.StartTime!.Value.Date)
                .Select(g => new TimeSeriesData(g.Key, g.Sum(x => Pricing.GetPricePerHour(x.Pricing) * (x.Reservation.EndTime!.Value - x.Reservation.StartTime!.Value).TotalHours)))
                .ToList()
                .ForEach(x => dto.TotalEarningPerDay[(int)(x.X-aMonthAgo).TotalDays].Y = x.Y);
        
        if(carCountUsedSpaceSeries != null)
            carCountUsedSpaceSeries
                .GroupBy(x => x.StatusDataTime!.Value.Date)
                .Select(g => new TimeSeriesData(g.Key, g.Count()))
                .ToList()
                .ForEach(x => dto.CarCountUsedSpacePerDay[(int)(x.X-aMonthAgo).TotalDays].Y = x.Y);
        
        if(spaceUsageSeriesRaw != null ) {
            var pairedSet = new HashSet<ReceivedSpaceStatus>();

            var pairs = spaceUsageSeriesRaw
                .Where(x => x.NewRealSpaceStatus == SpaceStatus.EMPTY && x.OldRealSpaceStatus == SpaceStatus.OCCUPIED)
                .Select((x,i) => {
                    var found = spaceUsageSeriesRaw
                        .Where((y,i2) => y.SpaceId == x.SpaceId && y.NewRealSpaceStatus == SpaceStatus.OCCUPIED && i2 < i && !pairedSet.Contains(y))
                        //.Max(Comparer<ReceivedSpaceStatus>.Create((v1,v2) => v1.StatusDataTime!.Value.CompareTo(v2.StatusDataTime!.Value)));
                        .LastOrDefault();
                    if(found == null) return null;

                    pairedSet.Add(found);
                    return new {
                        FromOccupied = found,
                        ToEmpty = x
                    };
                })
                .Where(x => x != null);

            foreach (var pair in pairs) {
                var start = pair!.FromOccupied.StatusDataTime!.Value;
                var end = pair.ToEmpty.StatusDataTime!.Value;

                var iter = start.AddHours(1);
                iter = iter.AddMinutes(-iter.Minute);
                iter = iter.AddSeconds(-iter.Second);
                iter = iter.AddMilliseconds(-iter.Millisecond);

                if(end <= iter) iter = end;
                var minutes = (iter-start).TotalMinutes;
                dto.SpaceUsageTimePercentagesPerWeekday[start.Hour][(int)start.DayOfWeek] += minutes;
                if(end == iter) continue;
                
                do{
                    var next = iter.AddHours(1);
                    if(end <= next) next = end;
                    minutes = (next-iter).TotalMinutes;
                    dto.SpaceUsageTimePercentagesPerWeekday[iter.Hour][(int)iter.DayOfWeek] += minutes;
                    iter = next;
                } while(iter < end);
            }

            var totalWeight = dto.SpaceUsageTimePercentagesPerWeekday.Sum(x => x.Sum());
            dto.SpaceUsageTimePercentagesPerWeekday.ForEach(x => {
                for(var i = 0; i < 7; i++)
                    x[i] = x[i] / totalWeight * 100;
            });

            foreach (var pair in pairs)
            {
                var start = pair!.FromOccupied.StatusDataTime!.Value;
                var end = pair.ToEmpty.StatusDataTime!.Value;

                var iter = start.AddDays(1);
                iter = iter.Date;

                if(end <= iter) iter = end;
                var minutes = (iter-start).TotalMinutes;
                var mams = (MinAvgMaxSum)dto.SpaceUsageTimePerDay[(int)(start-aMonthAgo).TotalDays].Y;
                mams.Min = Math.Min(mams.Min, minutes);
                mams.Max = Math.Max(mams.Max, minutes);
                mams.Sum += minutes;
                mams.Count++;
                if(end == iter) continue;

                do {
                    var next = iter.AddDays(1);
                    if(end <= next) next = end;
                    minutes = (next-iter).TotalMinutes;
                    mams = (MinAvgMaxSum)dto.SpaceUsageTimePerDay[(int)(iter-aMonthAgo).TotalDays].Y;
                    mams.Min = Math.Min(mams.Min, minutes);
                    mams.Max = Math.Max(mams.Max, minutes);
                    mams.Sum += minutes;
                    mams.Count++;
                    iter = next;
                } while(iter < end);
            }
        }

        dto.SpaceUsageTimePerDay.ForEach(x => {
            var mams = (MinAvgMaxSum)x.Y;
            if(mams.Count > 0) {
                mams.Avg = mams.Sum / mams.Count;
            }
            else {
                mams.Min = 0;
                mams.Avg = 0;
                mams.Max = 0;
            }
        });

        return dto;
    }
}