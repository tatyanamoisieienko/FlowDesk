using FlowDesk.Api.Models;
using FlowDesk.Api.Services;

namespace FlowDesk.Api.Tests;

public class DemoKiVorschlagServiceTests
{
    private readonly DemoKiVorschlagService _service = new();

    [Fact]
    public void Eindeutige_It_Anfrage_Wird_Als_It_Erkannt()
    {
        var ergebnis = _service.Analysiere("Der Drucker am Empfang funktioniert seit heute Morgen nicht mehr.");

        Assert.False(ergebnis.ManuellePruefungErforderlich);
        Assert.Contains("IT", ergebnis.VorgeschlageneAbteilungen);
    }

    [Fact]
    public void Unklare_Anfrage_Fuehrt_Zu_ManuellePruefungErforderlich()
    {
        var ergebnis = _service.Analysiere("Bitte kümmert euch bei Gelegenheit darum, danke.");

        Assert.True(ergebnis.ManuellePruefungErforderlich);
        Assert.Empty(ergebnis.VorgeschlageneAbteilungen);
    }

    [Fact]
    public void Mehrere_Kategorien_Werden_Gemeinsam_Erkannt()
    {
        var ergebnis = _service.Analysiere("Die neue Mitarbeiterin benötigt einen Laptop für den ersten Arbeitstag.");

        Assert.False(ergebnis.ManuellePruefungErforderlich);
        Assert.Contains("Personal", ergebnis.VorgeschlageneAbteilungen);
        Assert.Contains("IT", ergebnis.VorgeschlageneAbteilungen);
    }

    [Fact]
    public void Dringende_Anfrage_Erhaelt_Hohe_Prioritaet()
    {
        var ergebnis = _service.Analysiere("Die Heizung ist komplett ausgefallen, das ist dringend.");

        Assert.Equal(Prioritaet.Hoch, ergebnis.VorgeschlagenePrioritaet);
    }

    [Fact]
    public void Finanzanfrage_Wird_Als_Finanzen_Erkannt()
    {
        var ergebnis = _service.Analysiere("Für die Erstattung der Reisekosten fehlt noch die Rechnung.");

        Assert.False(ergebnis.ManuellePruefungErforderlich);
        Assert.Contains("Finanzen", ergebnis.VorgeschlageneAbteilungen);
    }

    [Fact]
    public void Ausgefallene_Heizung_Erhaelt_Hohe_Prioritaet()
    {
        var ergebnis = _service.Analysiere("Die Heizung im Behandlungsraum ist ausgefallen.");

        Assert.False(ergebnis.ManuellePruefungErforderlich);
        Assert.Contains("Gebäudemanagement", ergebnis.VorgeschlageneAbteilungen);
        Assert.Equal(Prioritaet.Hoch, ergebnis.VorgeschlagenePrioritaet);
    }
}
