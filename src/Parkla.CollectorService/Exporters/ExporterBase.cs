using System.Text;
using Parkla.CollectorService.OptionsManager;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Exporters;
public abstract class ExporterBase
{
    private readonly object _startLock = new();
    public bool Started { get; private set; } = false;
    private readonly ILogger _logger;

    public ExporterBase(
        ILogger logger
    )
    {
        _logger = logger;
    }
    
    public void Start() {
        lock (_startLock) {
            var typeName = GetType().Name;
            if (Started) {
                _logger.LogWarning("{}.Start: {} is already started.",typeName, typeName);
                return;
            }

            DoStart();
        
            Started = true;
            _logger.LogInformation("START: {} is started", typeName);
        }
    }

    protected abstract void DoStart();

    public abstract Task ExportAsync(ParkSpaceStatusDto dto, ExporterElemBase exporterElemBase);
    public abstract Task ExportAsync(IEnumerable<ParkSpaceStatusDto> dtos, ExporterElemBase exporterElemBase);

    public static string LogStrList(IEnumerable<ParkSpaceStatusDto> dtos, string className, bool successful) {           
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine();
        foreach(var dto in dtos) {
            stringBuilder.AppendFormat("{0} [{1}]: ParkId='{2}', SpaceId='{3}', Status='{4}' is{5} exported\n", 
                className, 
                successful ? "SUCCESS" : "FAIL", 
                dto.Parkid, 
                dto.Spaceid, 
                dto.Status, 
                successful ? "" : " not");
        }
        return stringBuilder.ToString();
    }

}