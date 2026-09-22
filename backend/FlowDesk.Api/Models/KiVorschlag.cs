namespace FlowDesk.Api.Models;

public class KiVorschlag
{
    public int Id { get; set; }
    public int AnfrageId { get; set; }
    public Anfrage Anfrage { get; set; } = null!;
    public string? VorgeschlagenerTitel { get; set; }
    public Prioritaet? VorgeschlagenePrioritaet { get; set; }
    public ICollection<Abteilung> VorgeschlageneAbteilungen { get; set; } = new List<Abteilung>();
    public string? FehlendeInformationen { get; set; }
    public string? VorgeschlageneNaechsteSchritte { get; set; }
    public bool ManuellePruefungErforderlich { get; set; }
    public Pruefstatus Pruefstatus { get; set; } = Pruefstatus.Ausstehend;
}
