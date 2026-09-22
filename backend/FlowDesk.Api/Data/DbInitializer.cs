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
            var fuerth = db.Standorte.Single(s => s.Name == "Fürth");
            var nuernberg = db.Standorte.Single(s => s.Name == "Nürnberg");
            var erlangen = db.Standorte.Single(s => s.Name == "Erlangen");
            var jetzt = DateTime.UtcNow;

            db.Anfragen.AddRange(
                new Anfrage
                {
                    OriginalText = "Eine neue Mitarbeiterin beginnt nächste Woche in der Praxis in Fürth. Sie hat jedoch noch keinen Zugang zum System und ihr Arbeitsplatz muss noch vorbereitet werden.",
                    StandortId = fuerth.Id,
                    Status = AnfrageStatus.Neu,
                    ErstelltAm = jetzt,
                    AktualisiertAm = jetzt
                },
                new Anfrage
                {
                    OriginalText = "Der Drucker am Empfang in der Praxis Nürnberg druckt seit heute Morgen keine Dokumente mehr aus. Mehrere Kolleginnen und Kollegen sind betroffen.",
                    StandortId = nuernberg.Id,
                    Status = AnfrageStatus.Neu,
                    ErstelltAm = jetzt,
                    AktualisiertAm = jetzt
                },
                new Anfrage
                {
                    OriginalText = "Für die Praxis in Erlangen wird eine aktualisierte Vorlage für das interne Urlaubsantragsformular benötigt. Die aktuelle Version enthält noch veraltete Kontaktdaten der Verwaltung.",
                    StandortId = erlangen.Id,
                    Status = AnfrageStatus.Neu,
                    ErstelltAm = jetzt,
                    AktualisiertAm = jetzt
                });

            db.SaveChanges();
        }
    }
}
