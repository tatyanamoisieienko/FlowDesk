using FlowDesk.Api.Models;

namespace FlowDesk.Api.Dtos;

public record AnfrageListItemDto(
    int Id,
    string? Titel,
    string OriginalText,
    string Standort,
    AnfrageStatus Status,
    Prioritaet? Prioritaet,
    DateTime ErstelltAm);

public record AnfrageDetailDto(
    int Id,
    string OriginalText,
    string? Titel,
    int StandortId,
    string Standort,
    AnfrageStatus Status,
    Prioritaet? Prioritaet,
    DateTime ErstelltAm,
    DateTime AktualisiertAm,
    List<string> Abteilungen,
    KiVorschlagDto? KiVorschlag);

public record AnfrageErstellenRequest(string OriginalText, int StandortId);

public record StatusAktualisierenRequest(AnfrageStatus Status);
