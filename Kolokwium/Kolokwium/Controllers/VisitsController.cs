using Microsoft.AspNetCore.Mvc;
using Kolokwium.Models;
using Kolokwium.Services;

namespace Kolokwium.Controllers;


[Route("api/[controller]")]
[ApiController]
public class VisitsController : ControllerBase
{
    private IDbService _iDbService; // rename
    
    public VisitsController(IDbService iDbService)
    {
        _iDbService = iDbService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> getVisit(int id)
    {
        var visit = await _iDbService.GetVisitAsync(id);

        return Ok(visit);
    }

    [HttpPost]
    public async Task<IActionResult> newVisit(NewVisitsDto newVisitsDto)
    {
        await _iDbService.AddNewNewVisitsDtoAsync(newVisitsDto);

        return Created();
    }
    

}