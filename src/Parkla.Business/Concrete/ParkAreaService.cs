using System.Linq.Expressions;
using System.Net;
using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.Core.Exceptions;
using Parkla.Core.Helpers;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class ParkAreaService : EntityServiceBase<ParkArea>, IParkAreaService
{
    private readonly IParkAreaRepo _parkAreaRepo;
    private readonly IValidator<ParkArea> _validator;

    public ParkAreaService(
        IParkAreaRepo parkAreaRepo, 
        IValidator<ParkArea> validator
    ) : base(parkAreaRepo, validator)
    {
        _parkAreaRepo = parkAreaRepo;
        _validator = validator;
    }

    public async Task DeleteAsync(ParkArea parkArea, int userId, CancellationToken cancellationToken = default)
    {
        await ThrowIfUserNotMatch((int)parkArea.Id!, userId, cancellationToken).ConfigureAwait(false);
        await base.DeleteAsync(parkArea, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ParkArea> UpdateAsync(ParkArea parkArea, int userId, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(parkArea, o => o.IncludeRuleSets("update","id"), cancellationToken).ConfigureAwait(false);
        if (!result.IsValid)
            throw new ParklaException(result.ToString(), HttpStatusCode.BadRequest);
        
        var props = new Expression<Func<ParkArea,object?>>[]{
            x => x.Name,
            x => x.Description,
            x => x.ReservationsEnabled
        };

        await ThrowIfUserNotMatch((int)parkArea.Id!, userId, cancellationToken).ConfigureAwait(false);
        return await _parkAreaRepo.UpdateAsync(parkArea, props, false, cancellationToken).ConfigureAwait(false);
    }

    public override async Task<ParkArea> AddAsync(ParkArea entity, CancellationToken cancellationToken = default)
    {
        var result = await _validator.ValidateAsync(entity, o => o.IncludeRuleSets("update").IncludeProperties(x => x.ParkId), cancellationToken).ConfigureAwait(false);
        if (!result.IsValid)
            throw new ParklaException(result.ToString(), HttpStatusCode.BadRequest);
        
        entity.TemplateImage = null;
        entity.StatusUpdateTime = null;
        entity.EmptySpace = 0;
        entity.ReservedSpace = 0;
        entity.OccupiedSpace = 0;
        entity.MinPrice = -1;
        entity.AvaragePrice = -1;
        entity.MaxPrice = -1;
        
        return await _parkAreaRepo.AddAsync(entity,cancellationToken).ConfigureAwait(false);
    }

    private async Task ThrowIfUserNotMatch(int parkAreaId, int userId, CancellationToken cancellationToken) {
        var parkArea = await _parkAreaRepo.GetAsync(
            new Expression<Func<ParkArea, object>>[]{x=>x.Park!},
            x => x.Id == parkAreaId,
            cancellationToken
            ).ConfigureAwait(false);
        
        if(parkArea!.Park!.UserId != userId)
            throw new ParklaException("User requested is not permitted to update or delete other user's parkArea", HttpStatusCode.BadRequest);
    }
}