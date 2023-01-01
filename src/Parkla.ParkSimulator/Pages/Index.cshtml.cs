using Microsoft.AspNetCore.Mvc.RazorPages;
using Parkla.Web.Controllers;

namespace Parkla.ParkSimulator.Pages;

public class IndexModel : PageModel
{

    private readonly ILogger<IndexModel> _logger;
    private readonly IConfiguration configuration;

    public string GetParkId => SimulateController.ParkId;

    public Dictionary<int,string> GetRealSpaces => configuration.GetSection("RealSpaces")
        .GetChildren()
        .ToDictionary(x => int.Parse(x.Key), x => x.Value) ?? new();
    
    public string GetProtocol => SimulateController.InitialProtocol.ToString();

    public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        this.configuration = configuration;
    }

    public void OnGet()
    {

    }

    

}
