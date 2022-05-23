using System.IO.Ports;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Parkla.Core.DTOs;
using Parkla.Core.Enums;
using Parkla.Web.Controllers;

namespace Parkla.ParkSimulator.Pages;

public class IndexModel : PageModel
{

    private readonly ILogger<IndexModel> _logger;

    public string GetParkId => SimulateController.ParkId;

    public List<Tuple<int,string>> GetRealSpaces => SimulateController.RealSpaces;
    

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }

    

}
