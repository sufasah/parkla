using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Core.DTOs;

namespace Parkla.Business.Concrete;
public class CollectorService : ICollectorService
{
    private readonly IValidator<ParkSpaceStatusDto> _parkSpaceStatusValidator;

    public CollectorService(
        IValidator<ParkSpaceStatusDto> parkSpaceStatusValidator
    ) {
        _parkSpaceStatusValidator = parkSpaceStatusValidator;
    }
    public void CollectParkSpaceStatus(ParkSpaceStatusDto dto)
    {
        var validationResult = _parkSpaceStatusValidator.Validate(dto);
    }

    public void CollectParkSpaceStatus(IEnumerable<ParkSpaceStatusDto> dtos)
    {
        foreach (var dto in dtos)
            CollectParkSpaceStatus(dto);
    }
}