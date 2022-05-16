using System.Linq.Expressions;
using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class ParkSpaceService : EntityServiceBase<ParkSpace>, IParkSpaceService
{
    private readonly IParkSpaceRepo _parkSpaceRepo;
    private readonly IValidator<ParkSpace> _validator;

    public ParkSpaceService(
        IParkSpaceRepo parkSpaceRepo, 
        IValidator<ParkSpace> validator
    ) : base(parkSpaceRepo, validator)
    {
        _parkSpaceRepo = parkSpaceRepo;
        _validator = validator;
    }

    public async Task<List<ParkSpace>> GetAllAsync(
        int? areaId,
        bool includeReservations,
        CancellationToken cancellationToken = default
    ) {
        var include = new List<Expression<Func<ParkSpace,object>>>();

        if(includeReservations)
            include.Add(x => x.Reservations!);
        
        return await _parkSpaceRepo.GetListAsync(
            include.ToArray(),
            areaId == null ? null : (ParkSpace x) => x.AreaId == areaId,
            cancellationToken
        ).ConfigureAwait(false);
    }
}