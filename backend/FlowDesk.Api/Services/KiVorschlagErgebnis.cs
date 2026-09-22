using FlowDesk.Api.Models;

namespace FlowDesk.Api.Services;

public record KiVorschlagErgebnis(
    string? VorgeschlagenerTitel,
    Prioritaet? VorgeschlagenePrioritaet,
    IReadOnlyCollection<string> VorgeschlageneAbteilungen,
    string? FehlendeInformationen,
    string? VorgeschlageneNaechsteSchritte,
    bool ManuellePruefungErforderlich);
