using Microsoft.AspNetCore.Mvc.RazorPages;
using Parkla.Web.Controllers;

namespace Parkla.ParkSimulator.Pages;

public class IndexModel : PageModel
{

    private readonly ILogger<IndexModel> _logger;

    public string GetParkId => SimulateController.ParkId;

    public List<Tuple<int,string>> GetRealSpaces => SimulateController.RealSpaces;
    
    public string GetProtocol => SimulateController.InitialProtocol.ToString();

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }

    

}
