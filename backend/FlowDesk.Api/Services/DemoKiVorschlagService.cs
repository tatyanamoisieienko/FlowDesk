using FlowDesk.Api.Models;

namespace FlowDesk.Api.Services;

public class DemoKiVorschlagService : IKiVorschlagService
{
    private sealed record Kategorie(string Abteilung, string TitelFragment, string NaechsterSchritt, string[] Schluesselwoerter);

    // Regelbasierte Demo-Zuordnung: neue Kategorie = neuer Eintrag, keine if/else-Kette.
    private static readonly Kategorie[] Kategorien =
    [
        new(
            Abteilung: "IT",
            TitelFragment: "IT-Anfrage",
            NaechsterSchritt: "IT-Abteilung informieren und technisches Problem prüfen lassen.",
            Schluesselwoerter:
            [
                "drucker", "software", "laptop", "computer", "pc", "passwort", "login",
                "systemzugang", "zugang", "internet", "wlan", "e-mail", "fehler",
            ]),
        new(
            Abteilung: "Personal",
            TitelFragment: "Personalanfrage",
            NaechsterSchritt: "Personalabteilung informieren und weiteres Vorgehen abstimmen.",
            Schluesselwoerter:
            [
                "mitarbeiter", "mitarbeiterin", "onboarding", "arbeitsvertrag",
                "urlaub", "krankmeldung", "personal", "bewerbung",
            ]),
        new(
            Abteilung: "Verwaltung",
            TitelFragment: "Verwaltungsanfrage",
            NaechsterSchritt: "Verwaltung informieren und benötigte Unterlagen bereitstellen.",
            Schluesselwoerter: ["formular", "vorlage", "dokument", "organisation", "termin", "antrag", "bescheinigung"]),
        new(
            Abteilung: "Gebäudemanagement",
            TitelFragment: "Gebäudeanfrage",
            NaechsterSchritt: "Gebäudemanagement informieren und Reparatur bzw. Prüfung vor Ort veranlassen.",
            Schluesselwoerter:
            [
                "heizung", "lampe", "licht", "tür", "fenster", "wasser",
                "toilette", "reparatur", "raum", "klimaanlage",
            ]),
        new(
            Abteilung: "Finanzen",
            TitelFragment: "Finanzanfrage",
            NaechsterSchritt: "Finanzabteilung informieren und Beleg bzw. Zahlung prüfen lassen.",
            Schluesselwoerter: ["rechnung", "zahlung", "kosten", "budget", "erstattung", "überweisung"]),
    ];

    private static readonly string[] DringendSchluesselwoerter =
        [
            "dringend", "sofort", "ausfall", "funktioniert nicht", "komplett ausgefallen",
            "ausgefallen", "defekt", "kaputt", "nicht erreichbar",
        ];

    public KiVorschlagErgebnis Analysiere(string originalText)
    {
        var text = originalText.ToLowerInvariant();

        var erkannteKategorien = Kategorien
            .Where(kategorie => kategorie.Schluesselwoerter.Any(text.Contains))
            .ToList();

        if (erkannteKategorien.Count == 0)
        {
            return new KiVorschlagErgebnis(
                VorgeschlagenerTitel: null,
                VorgeschlagenePrioritaet: null,
                VorgeschlageneAbteilungen: [],
                FehlendeInformationen: "Die Anfrage ist zu allgemein formuliert. Bitte ergänzen Sie weitere Informationen zum Anliegen.",
                VorgeschlageneNaechsteSchritte: null,
                ManuellePruefungErforderlich: true);
        }

        var istDringend = DringendSchluesselwoerter.Any(text.Contains);

        return new KiVorschlagErgebnis(
            VorgeschlagenerTitel: string.Join(" + ", erkannteKategorien.Select(kategorie => kategorie.TitelFragment)),
            VorgeschlagenePrioritaet: istDringend ? Prioritaet.Hoch : Prioritaet.Normal,
            VorgeschlageneAbteilungen: erkannteKategorien.Select(kategorie => kategorie.Abteilung).ToArray(),
            FehlendeInformationen: null,
            VorgeschlageneNaechsteSchritte: string.Join(" ", erkannteKategorien.Select(kategorie => kategorie.NaechsterSchritt)),
            ManuellePruefungErforderlich: false);
    }
}
