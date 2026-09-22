using FlowDesk.Api.Data;
using FlowDesk.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Api.Controllers;

[ApiController]
[Route("api/stammdaten")]
public class StammdatenController(FlowDeskDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<StammdatenDto>> GetStammdaten()
    {
        var standorte = await db.Standorte
            .OrderBy(s => s.Name)
            .Select(s => new StandortDto(s.Id, s.Name))
            .ToListAsync();

        var abteilungen = await db.Abteilungen
            .OrderBy(a => a.Name)
            .Select(a => new AbteilungDto(a.Id, a.Name))
            .ToListAsync();

        return Ok(new StammdatenDto(standorte, abteilungen));
    }
}
