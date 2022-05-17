using System.Linq.Expressions;
using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.Core.Helpers;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class RealParkSpaceService : EntityServiceBase<RealParkSpace>, IRealParkSpaceService
{
    private readonly IRealParkSpaceRepo _realParkSpaceRepo;
    private readonly IValidator<RealParkSpace> _validator;

    public RealParkSpaceService(
        IRealParkSpaceRepo realParkSpaceRepo, 
        IValidator<RealParkSpace> validator
    ) : base(realParkSpaceRepo, validator)
    {
        _realParkSpaceRepo = realParkSpaceRepo;
        _validator = validator;
    }

    public async Task<PagedList<RealParkSpace>> GetPageAsync(
        int parkId, 
        int nextRecord, 
        int pageSize, 
        string? search, 
        string? orderBy, 
        bool ascending, 
        CancellationToken cancellationToken = default
    ) {
        NullOrTrim(ref search);
        NullOrTrim(ref orderBy);

        Expression eFilter = (ParkArea x) => x.ParkId == parkId;
        if(search != null) {
            eFilter = (RealParkSpace x) => (
                x.Name!.ToLower().Contains(search) ||
                x.SpaceId.ToString()!.ToLower().Contains(search) ||
                x.Id.ToString()!.ToLower().Contains(search)
            ) && x.ParkId == parkId;
        }

        return await _realParkSpaceRepo.GetListAsync(
            nextRecord,
            pageSize,
            (Expression<Func<RealParkSpace,bool>>)eFilter,
            GetPropertyLambdaExpression(orderBy),
            ascending,
            cancellationToken
        );
    }
}