using FlowDesk.Api.Models;

namespace FlowDesk.Api.Dtos;

public record KiVorschlagDto(
    int Id,
    int AnfrageId,
    string? VorgeschlagenerTitel,
    Prioritaet? VorgeschlagenePrioritaet,
    List<string> VorgeschlageneAbteilungen,
    string? FehlendeInformationen,
    string? VorgeschlageneNaechsteSchritte,
    bool ManuellePruefungErforderlich,
    Pruefstatus Pruefstatus);

public record KiEntscheidungRequest(
    Pruefstatus Pruefstatus,
    string? Titel,
    Prioritaet? Prioritaet,
    List<int>? AbteilungIds);
