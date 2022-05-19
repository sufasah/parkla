using FluentValidation;
using Microsoft.Extensions.Logging;
using Parkla.Business.Abstract;
using Parkla.Core.DTOs;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class CollectorService : ICollectorService
{
    private readonly IValidator<ParkSpaceStatusDto> _parkSpaceStatusValidator;
    private readonly ICollectorRepo _repo;
    private readonly IParklaHubService _hubService;
    private readonly ILogger<CollectorService> _logger;

    public CollectorService(
        IValidator<ParkSpaceStatusDto> parkSpaceStatusValidator,
        ICollectorRepo repo,
        IParklaHubService hubService,
        ILogger<CollectorService> logger
    ) {
        _parkSpaceStatusValidator = parkSpaceStatusValidator;
        _repo = repo;
        _hubService = hubService;
        _logger = logger;
    }
    public async Task CollectParkSpaceStatusAsync(ParkSpaceStatusDto dto)
    {
        var validationResult = _parkSpaceStatusValidator.Validate(dto);
        if(!validationResult.IsValid) {
            _logger.LogInformation("ParkSpaceStatus has not been validated.\n{}",validationResult.ToString());
            return;
        }

        try {
            var result = await _repo.CollectParkSpaceStatusAsync(dto);

            if(result.Item1) {
                if(result.Item2 != null)
                    await _hubService.ParkSpaceChangesAsync(result.Item2!, false);
                if(result.Item3 != null)
                    await _hubService.ParkChangesAsync(result.Item3!, false);
            }
        } catch(Exception e) {
            _logger.LogWarning(e, "ParkSpaceStatus could not persist to the database.");
        }
    }

    public async Task CollectParkSpaceStatusAsync(IEnumerable<ParkSpaceStatusDto> dtos)
    {
        var tasks = new List<Task>();

        foreach (var dto in dtos)
            tasks.Add(CollectParkSpaceStatusAsync(dto));

        await Task.WhenAll(tasks);
    }
}