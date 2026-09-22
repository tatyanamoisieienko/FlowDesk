using FlowDesk.Api.Models;

namespace FlowDesk.Api.Services;

public class DemoKiVorschlagService : IKiVorschlagService
{
    private static readonly string[] ItSchluesselwoerter =
        ["drucker", "software", "systemzugang", "zugang zum system"];

    private static readonly string[] PersonalSchluesselwoerter =
        ["neue mitarbeiterin", "neuer mitarbeiter", "onboarding"];

    public KiVorschlagErgebnis Analysiere(string originalText)
    {
        var text = originalText.ToLowerInvariant();

        var istIt = ItSchluesselwoerter.Any(text.Contains);
        var istPersonal = PersonalSchluesselwoerter.Any(text.Contains);

        if (!istIt && !istPersonal)
        {
            return new KiVorschlagErgebnis(
                VorgeschlagenerTitel: null,
                VorgeschlagenePrioritaet: null,
                VorgeschlageneAbteilungen: [],
                FehlendeInformationen: "Die Anfrage ist nicht eindeutig genug für eine automatische Zuordnung.",
                VorgeschlageneNaechsteSchritte: null,
                ManuellePruefungErforderlich: true);
        }

        if (istIt && istPersonal)
        {
            return new KiVorschlagErgebnis(
                VorgeschlagenerTitel: "Onboarding mit Systemzugang",
                VorgeschlagenePrioritaet: Prioritaet.Normal,
                VorgeschlageneAbteilungen: ["Personal", "IT"],
                FehlendeInformationen: null,
                VorgeschlageneNaechsteSchritte: "Zugangsdaten anlegen und Arbeitsplatz vorbereiten.",
                ManuellePruefungErforderlich: false);
        }

        if (istPersonal)
        {
            return new KiVorschlagErgebnis(
                VorgeschlagenerTitel: "Onboarding-Anfrage",
                VorgeschlagenePrioritaet: Prioritaet.Normal,
                VorgeschlageneAbteilungen: ["Personal"],
                FehlendeInformationen: null,
                VorgeschlageneNaechsteSchritte: "Onboarding mit der Personalabteilung abstimmen.",
                ManuellePruefungErforderlich: false);
        }

        return new KiVorschlagErgebnis(
            VorgeschlagenerTitel: "IT-Problem",
            VorgeschlagenePrioritaet: Prioritaet.Normal,
            VorgeschlageneAbteilungen: ["IT"],
            FehlendeInformationen: null,
            VorgeschlageneNaechsteSchritte: "IT-Abteilung zur Fehlerbehebung kontaktieren.",
            ManuellePruefungErforderlich: false);
    }
}
