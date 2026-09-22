using FlowDesk.Api.Controllers;
using FlowDesk.Api.Data;
using FlowDesk.Api.Models;
using FlowDesk.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Api.Tests;

public class AnfragenControllerTests
{
    [Fact]
    public async Task KiAnalyse_Aendert_Bestaetigte_Anfrage_Nicht_Automatisch()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<FlowDeskDbContext>()
            .UseSqlite(connection)
            .Options;

        using var db = new FlowDeskDbContext(options);
        db.Database.EnsureCreated();

        var standort = new Standort { Name = "Nürnberg" };
        db.Standorte.Add(standort);
        db.Abteilungen.Add(new Abteilung { Name = "IT" });
        await db.SaveChangesAsync();

        var anfrage = new Anfrage
        {
            OriginalText = "Der Drucker am Empfang funktioniert seit heute Morgen nicht mehr.",
            StandortId = standort.Id,
            Status = AnfrageStatus.Neu,
            ErstelltAm = DateTime.UtcNow,
            AktualisiertAm = DateTime.UtcNow
        };
        db.Anfragen.Add(anfrage);
        await db.SaveChangesAsync();

        var controller = new AnfragenController(db, new DemoKiVorschlagService());
        await controller.KiAnalyse(anfrage.Id);

        var neuGeladen = await db.Anfragen
            .Include(a => a.Abteilungen)
            .FirstAsync(a => a.Id == anfrage.Id);

        Assert.Null(neuGeladen.Titel);
        Assert.Null(neuGeladen.Prioritaet);
        Assert.Empty(neuGeladen.Abteilungen);
    }
}
