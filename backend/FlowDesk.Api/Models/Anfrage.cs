namespace FlowDesk.Api.Models;

public class Anfrage
{
    public int Id { get; set; }
    public required string OriginalText { get; set; }
    public string? Titel { get; set; }
    public int StandortId { get; set; }
    public Standort Standort { get; set; } = null!;
    public AnfrageStatus Status { get; set; } = AnfrageStatus.Neu;
    public Prioritaet? Prioritaet { get; set; }
    public DateTime ErstelltAm { get; set; }
    public DateTime AktualisiertAm { get; set; }
    public ICollection<Abteilung> Abteilungen { get; set; } = new List<Abteilung>();
    public KiVorschlag? KiVorschlag { get; set; }
}
