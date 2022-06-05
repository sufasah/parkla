using System.Net;
using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.Core.Exceptions;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;

namespace Parkla.DataAccess.Concrete;

public class ReservationRepo<TContext> : EntityRepoBase<Reservation, TContext>, IReservationRepo 
    where TContext: DbContext, new()
{
    public override async Task<Reservation> AddAsync(
        Reservation reservationParam, 
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();
        var reservationClone = reservationParam;

        while(!cancellationToken.IsCancellationRequested) {
            var result = context.Add(reservationClone);
            reservationClone = (Reservation)result.CurrentValues.Clone().ToObject();
            var reservation = result.Entity;
            
            await result.Reference(x => x.Space)
                .Query()
                .Include(x => x.Pricing)
                .LoadAsync(cancellationToken)
                .ConfigureAwait(false);        
            await result.Reference(x => x.User)
                .LoadAsync(cancellationToken)
                .ConfigureAwait(false);

            if(reservation.Space == null)
                throw new ParklaException("Reservation space is not found", HttpStatusCode.BadRequest);
            
            if(reservation.User == null)
                throw new ParklaException("Reservation user is not found", HttpStatusCode.BadRequest);

            if(reservation.Space.Pricing == null)
                throw new ParklaException("Space pricing is not defined", HttpStatusCode.BadRequest);
            
            var isReservedByOthers = await context.Set<Reservation>()
                .AsNoTracking()
                .AnyAsync(x => 
                    x.SpaceId == reservation.SpaceId &&
                    ((x.StartTime <= reservation.StartTime && reservation.StartTime <= x.EndTime) ||
                    (x.StartTime <= reservation.EndTime && reservation.EndTime <= x.EndTime) ||
                    (reservation.StartTime <= x.StartTime && x.EndTime <= reservation.StartTime)),
                    cancellationToken)
                .ConfigureAwait(false);
            
            if(isReservedByOthers)
                throw new ParklaException("Space is reserved by another one whose time interval overlaps the inteval of the reservation.", HttpStatusCode.NotAcceptable);

            var space = reservation.Space;
            var pricing = space.Pricing;
            var user = reservation.User;

            var pricePerHour = Pricing.GetPricePerHour(pricing);
            var totalHours = (reservation.EndTime!.Value - reservation.StartTime!.Value).TotalHours;
            var payment = pricePerHour * (float)totalHours;

            if(totalHours == 0)
                throw new ParklaException("Reservation must be a time interval. Time difference of the reservation is zero.", HttpStatusCode.NotAcceptable);

            if(user.Wallet < payment) 
                throw new ParklaException("User does not have enough money for reservation", HttpStatusCode.BadRequest);

            user.Wallet -= payment;

            try {
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                reservation.User = new User() {
                    Id = reservation.User!.Id,
                    Name = reservation.User!.Name,
                    Surname = reservation.User!.Surname,
                    Username = reservation.User!.Username
                };
                return reservation;
            }
            catch(DbUpdateConcurrencyException) {
                context.ChangeTracker.Clear();
            }
        }
        return reservationClone;
    }

    public async Task<Reservation?> DeleteReservationAsync(
        Reservation reservationParam,
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();

        while(!cancellationToken.IsCancellationRequested) {
            var reservation = await context.Set<Reservation>()
                .Where(x => x.Id == reservationParam.Id)
                .Include(x => x.Space!)
                .ThenInclude(x => x.Pricing)
                .Include(x => x.User)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if(reservation == null)
                throw new ParklaConcurrentDeletionException("The reservation has already been deleted by another user");
            
            if(reservation.EndTime <= DateTime.UtcNow)
                throw new ParklaException("Only reservations which are not ended can be deleted", HttpStatusCode.BadRequest);
            
            var entry = context.Entry(reservation);
            entry.State = EntityState.Deleted;

            if(reservation.EndTime > DateTime.UtcNow) {
                var timeIntervalAsHour = reservation.EndTime!.Value.Subtract(DateTime.UtcNow > reservation.StartTime ? DateTime.UtcNow : reservation.StartTime!.Value).TotalHours;
                reservation.User!.Wallet += Pricing.GetPricePerHour(reservation.Space!.Pricing!) * (float)timeIntervalAsHour;
                context.Entry(reservation.User).State = EntityState.Modified;
            }

            try {
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return reservation;
            }
            catch(DbUpdateConcurrencyException) {
                context.ChangeTracker.Clear();
            }
        }
        return null;
    }

    public async Task<List<Reservation>> UserReservationListAsync(
        int userId, 
        CancellationToken cancellationToken
    ) {
        using var context = new TContext();
        var result = await context.Set<Reservation>()
            .AsNoTracking()
            .Include(x => x.Space!)
            .ThenInclude(x => x.Area!)
            .ThenInclude(x => x.Park)
            .Include(x => x.User)
            .Include(x => x.Space!)
            .ThenInclude(x => x.Pricing)
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        
        return result;
    }


}