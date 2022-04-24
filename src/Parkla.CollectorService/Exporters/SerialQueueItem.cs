
using System.IO.Ports;
using Parkla.CollectorService.DTOs;

namespace Parkla.CollectorService.Exporters;
public class SerialQueueItem
{
    public SerialPort SerialPort { get; set; }
    public ParkSpaceStatusDto Dto { get; set; }
}