using FlowDesk.Api.Data;
using FlowDesk.Api.Dtos;
using FlowDesk.Api.Models;
using FlowDesk.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Api.Controllers;

[ApiController]
[Route("api/anfragen")]
public class AnfragenController(FlowDeskDbContext db, IKiVorschlagService kiService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AnfrageListItemDto>>> GetAlle()
    {
        var anfragen = await db.Anfragen
            .Include(a => a.Standort)
            .OrderByDescending(a => a.ErstelltAm)
            .ToListAsync();

        return Ok(anfragen.Select(ToListItemDto).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AnfrageDetailDto>> GetById(int id)
    {
        var anfrage = await LadeAnfrageMitDetails(id);
        if (anfrage is null) return NotFound();

        return Ok(ToDetailDto(anfrage));
    }

    [HttpPost]
    public async Task<ActionResult<AnfrageDetailDto>> Erstellen(AnfrageErstellenRequest request)
    {
        var standort = await db.Standorte.FindAsync(request.StandortId);
        if (standort is null) return BadRequest("Unbekannter Standort.");

        var jetzt = DateTime.UtcNow;
        var anfrage = new Anfrage
        {
            OriginalText = request.OriginalText,
            StandortId = standort.Id,
            Standort = standort,
            Status = AnfrageStatus.Neu,
            ErstelltAm = jetzt,
            AktualisiertAm = jetzt
        };

        db.Anfragen.Add(anfrage);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = anfrage.Id }, ToDetailDto(anfrage));
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<AnfrageDetailDto>> StatusAktualisieren(int id, StatusAktualisierenRequest request)
    {
        var anfrage = await LadeAnfrageMitDetails(id);
        if (anfrage is null) return NotFound();

        anfrage.Status = request.Status;
        anfrage.AktualisiertAm = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Ok(ToDetailDto(anfrage));
    }

    [HttpPost("{id:int}/ki-analyse")]
    public async Task<ActionResult<KiVorschlagDto>> KiAnalyse(int id)
    {
        var anfrage = await db.Anfragen
            .Include(a => a.KiVorschlag)
            .ThenInclude(k => k!.VorgeschlageneAbteilungen)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (anfrage is null) return NotFound();

        var ergebnis = kiService.Analysiere(anfrage.OriginalText);
        var abteilungen = await db.Abteilungen
            .Where(a => ergebnis.VorgeschlageneAbteilungen.Contains(a.Name))
            .ToListAsync();

        if (anfrage.KiVorschlag is null)
        {
            var vorschlag = new KiVorschlag { AnfrageId = anfrage.Id };
            UebernehmeErgebnis(vorschlag, ergebnis, abteilungen);
            db.KiVorschlaege.Add(vorschlag);
            anfrage.KiVorschlag = vorschlag;
        }
        else
        {
            UebernehmeErgebnis(anfrage.KiVorschlag, ergebnis, abteilungen);
        }

        await db.SaveChangesAsync();
        return Ok(ToVorschlagDto(anfrage.KiVorschlag));
    }

    [HttpPost("{id:int}/ki-entscheidung")]
    public async Task<ActionResult<AnfrageDetailDto>> KiEntscheidung(int id, KiEntscheidungRequest request)
    {
        var anfrage = await LadeAnfrageMitDetails(id);
        if (anfrage is null) return NotFound();
        if (anfrage.KiVorschlag is null) return BadRequest("Es liegt noch kein KI-Vorschlag für diese Anfrage vor.");

        var vorschlag = anfrage.KiVorschlag;

        switch (request.Pruefstatus)
        {
            case Pruefstatus.Angenommen:
                anfrage.Titel = vorschlag.VorgeschlagenerTitel;
                anfrage.Prioritaet = vorschlag.VorgeschlagenePrioritaet;
                anfrage.Abteilungen = vorschlag.VorgeschlageneAbteilungen.ToList();
                anfrage.AktualisiertAm = DateTime.UtcNow;
                break;

            case Pruefstatus.Geaendert:
                if (request.AbteilungIds is not null)
                {
                    var abteilungen = await db.Abteilungen
                        .Where(a => request.AbteilungIds.Contains(a.Id))
                        .ToListAsync();

                    if (abteilungen.Count != request.AbteilungIds.Distinct().Count())
                        return BadRequest("Unbekannte Abteilung angegeben.");

                    anfrage.Abteilungen = abteilungen;
                }

                anfrage.Titel = request.Titel ?? vorschlag.VorgeschlagenerTitel;
                anfrage.Prioritaet = request.Prioritaet ?? vorschlag.VorgeschlagenePrioritaet;
                anfrage.AktualisiertAm = DateTime.UtcNow;
                break;

            case Pruefstatus.Abgelehnt:
                break;

            default:
                return BadRequest("Ungültiger Prüfstatus für eine Entscheidung.");
        }

        vorschlag.Pruefstatus = request.Pruefstatus;
        await db.SaveChangesAsync();

        return Ok(ToDetailDto(anfrage));
    }

    private Task<Anfrage?> LadeAnfrageMitDetails(int id) =>
        db.Anfragen
            .Include(a => a.Standort)
            .Include(a => a.Abteilungen)
            .Include(a => a.KiVorschlag)
            .ThenInclude(k => k!.VorgeschlageneAbteilungen)
            .FirstOrDefaultAsync(a => a.Id == id);

    private static void UebernehmeErgebnis(KiVorschlag vorschlag, KiVorschlagErgebnis ergebnis, List<Abteilung> abteilungen)
    {
        vorschlag.VorgeschlagenerTitel = ergebnis.VorgeschlagenerTitel;
        vorschlag.VorgeschlagenePrioritaet = ergebnis.VorgeschlagenePrioritaet;
        vorschlag.VorgeschlageneAbteilungen = abteilungen;
        vorschlag.FehlendeInformationen = ergebnis.FehlendeInformationen;
        vorschlag.VorgeschlageneNaechsteSchritte = ergebnis.VorgeschlageneNaechsteSchritte;
        vorschlag.ManuellePruefungErforderlich = ergebnis.ManuellePruefungErforderlich;
        vorschlag.Pruefstatus = Pruefstatus.Ausstehend;
    }

    private static AnfrageListItemDto ToListItemDto(Anfrage anfrage) => new(
        anfrage.Id,
        anfrage.Titel,
        anfrage.Standort.Name,
        anfrage.Status,
        anfrage.Prioritaet,
        anfrage.ErstelltAm);

    private static AnfrageDetailDto ToDetailDto(Anfrage anfrage) => new(
        anfrage.Id,
        anfrage.OriginalText,
        anfrage.Titel,
        anfrage.StandortId,
        anfrage.Standort.Name,
        anfrage.Status,
        anfrage.Prioritaet,
        anfrage.ErstelltAm,
        anfrage.AktualisiertAm,
        anfrage.Abteilungen.Select(a => a.Name).ToList(),
        anfrage.KiVorschlag is null ? null : ToVorschlagDto(anfrage.KiVorschlag));

    private static KiVorschlagDto ToVorschlagDto(KiVorschlag vorschlag) => new(
        vorschlag.Id,
        vorschlag.AnfrageId,
        vorschlag.VorgeschlagenerTitel,
        vorschlag.VorgeschlagenePrioritaet,
        vorschlag.VorgeschlageneAbteilungen.Select(a => a.Name).ToList(),
        vorschlag.FehlendeInformationen,
        vorschlag.VorgeschlageneNaechsteSchritte,
        vorschlag.ManuellePruefungErforderlich,
        vorschlag.Pruefstatus);
}
