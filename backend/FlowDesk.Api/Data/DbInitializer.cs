using FlowDesk.Api.Models;

namespace FlowDesk.Api.Data;

public static class DbInitializer
{
    public static void Seed(FlowDeskDbContext db)
    {
        if (!db.Standorte.Any())
        {
            db.Standorte.AddRange(
                new Standort { Name = "Nürnberg" },
                new Standort { Name = "Fürth" },
                new Standort { Name = "Erlangen" });
            db.SaveChanges();
        }

        if (!db.Abteilungen.Any())
        {
            db.Abteilungen.AddRange(
                new Abteilung { Name = "IT" },
                new Abteilung { Name = "Personal" },
                new Abteilung { Name = "Verwaltung" },
                new Abteilung { Name = "Gebäudemanagement" },
                new Abteilung { Name = "Finanzen" });
            db.SaveChanges();
        }

        if (!db.Anfragen.Any())
        {
            var nuernberg = db.Standorte.Single(s => s.Name == "Nürnberg");
            var fuerth = db.Standorte.Single(s => s.Name == "Fürth");
            var erlangen = db.Standorte.Single(s => s.Name == "Erlangen");

            var it = db.Abteilungen.Single(a => a.Name == "IT");
            var personal = db.Abteilungen.Single(a => a.Name == "Personal");
            var verwaltung = db.Abteilungen.Single(a => a.Name == "Verwaltung");
            var gebaeudemanagement = db.Abteilungen.Single(a => a.Name == "Gebäudemanagement");
            var finanzen = db.Abteilungen.Single(a => a.Name == "Finanzen");

            var jetzt = DateTime.UtcNow;

            Anfrage NeueAnfrage(
                int stundenZurueck,
                string originalText,
                Standort standort,
                AnfrageStatus status,
                string? titel = null,
                Prioritaet? prioritaet = null,
                params Abteilung[] abteilungen)
            {
                var erstelltAm = jetzt.AddHours(-stundenZurueck);
                return new Anfrage
                {
                    OriginalText = originalText,
                    StandortId = standort.Id,
                    Status = status,
                    Titel = titel,
                    Prioritaet = prioritaet,
                    Abteilungen = abteilungen.ToList(),
                    ErstelltAm = erstelltAm,
                    AktualisiertAm = erstelltAm
                };
            }

            db.Anfragen.AddRange(
                // IT – bestätigt, hohe Priorität
                NeueAnfrage(
                    132,
                    "Der Drucker am Empfang ist seit heute Morgen komplett ausgefallen. Mehrere Kolleginnen können dadurch keine Unterlagen mehr ausdrucken.",
                    nuernberg,
                    AnfrageStatus.InBearbeitung,
                    "Drucker-Ausfall am Empfang",
                    Prioritaet.Hoch,
                    it),

                // IT/Personal – unbestätigt
                NeueAnfrage(
                    120,
                    "Ein neuer Mitarbeiter kommt nicht in sein E-Mail-Postfach, offenbar funktioniert das Passwort nicht mehr.",
                    fuerth,
                    AnfrageStatus.Neu),

                // Personal – Onboarding, unbestätigt
                NeueAnfrage(
                    108,
                    "Eine neue Mitarbeiterin beginnt nächste Woche in der Praxis in Fürth. Sie hat jedoch noch keinen Zugang zum System und ihr Arbeitsplatz muss noch vorbereitet werden.",
                    fuerth,
                    AnfrageStatus.Neu),

                // Personal – bestätigt, niedrige Priorität
                NeueAnfrage(
                    96,
                    "Eine Mitarbeiterin aus der Erlanger Praxis hat einen Urlaubsantrag für die kommenden zwei Wochen eingereicht und bittet um zeitnahe Freigabe.",
                    erlangen,
                    AnfrageStatus.Erledigt,
                    "Urlaubsantrag freigegeben",
                    Prioritaet.Niedrig,
                    personal),

                // Personal – unbestätigt
                NeueAnfrage(
                    84,
                    "Ein Mitarbeiter hat sich heute krankgemeldet und wird voraussichtlich die ganze Woche ausfallen. Es muss kurzfristig eine Vertretung organisiert werden.",
                    nuernberg,
                    AnfrageStatus.InBearbeitung),

                // Verwaltung/Personal – unbestätigt
                NeueAnfrage(
                    72,
                    "Für die Praxis in Erlangen wird eine aktualisierte Vorlage für das interne Urlaubsantragsformular benötigt. Die aktuelle Version enthält noch veraltete Kontaktdaten der Verwaltung.",
                    erlangen,
                    AnfrageStatus.Neu),

                // Verwaltung – bestätigt, niedrige Priorität
                NeueAnfrage(
                    60,
                    "Für eine Fortbildung wurde ein Antrag auf Dienstreise gestellt. Die notwendigen Dokumente wurden bereits eingereicht und geprüft.",
                    nuernberg,
                    AnfrageStatus.Erledigt,
                    "Dienstreiseantrag geprüft",
                    Prioritaet.Niedrig,
                    verwaltung),

                // Gebäudemanagement – bestätigt, hohe Priorität
                NeueAnfrage(
                    48,
                    "Die Heizung im Behandlungsraum 2 ist ausgefallen. Die Patientinnen und Patienten klagen bereits über die Kälte im Raum.",
                    fuerth,
                    AnfrageStatus.InBearbeitung,
                    "Heizungsausfall Behandlungsraum 2",
                    Prioritaet.Hoch,
                    gebaeudemanagement),

                // Gebäudemanagement – unbestätigt
                NeueAnfrage(
                    36,
                    "Im Wartezimmer flackert seit einigen Tagen die Lampe und die Toilette im Erdgeschoss lässt sich nur noch schwer spülen.",
                    erlangen,
                    AnfrageStatus.Neu),

                // Finanzen – unbestätigt
                NeueAnfrage(
                    24,
                    "Für eine Fortbildung wurde in Vorleistung gezahlt. Die Rechnung liegt vor und es wird um Erstattung der Kosten gebeten.",
                    nuernberg,
                    AnfrageStatus.Neu),

                // Finanzen – bestätigt, normale Priorität
                NeueAnfrage(
                    12,
                    "Eine Lieferantenrechnung für medizinisches Verbrauchsmaterial muss überwiesen werden, bevor das vereinbarte Zahlungsziel überschritten wird.",
                    fuerth,
                    AnfrageStatus.Erledigt,
                    "Lieferantenrechnung beglichen",
                    Prioritaet.Normal,
                    finanzen),

                // Unklarer Fall – keine erkennbaren Schlüsselwörter
                NeueAnfrage(
                    2,
                    "Bitte kümmern.",
                    erlangen,
                    AnfrageStatus.Neu));

            db.SaveChanges();
        }
    }
}
