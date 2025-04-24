using Microsoft.AspNetCore.Mvc;
using QuickCareSim.Application.Interfaces.Services.Core;

namespace QuickCareSim.Presentation.WebApp.Controllers;

public class SimulationController : Controller
{

    private readonly ISimulationInfoService _info;


    public SimulationController(ISimulationInfoService info)
    {
        _info = info;
    }
    
    [HttpGet]
    public async Task<IActionResult> History()
    {
        var list = await _info.GetAllSimulationsAsync();
        return View(list);
    }
}