
using System.IO.Ports;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Exporters;
public class SerialQueueItem
{
    public SerialPort SerialPort { get; set; }
    public ParkSpaceStatusDto Dto { get; set; }
}