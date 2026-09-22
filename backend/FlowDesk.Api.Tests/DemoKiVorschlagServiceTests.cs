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
}
