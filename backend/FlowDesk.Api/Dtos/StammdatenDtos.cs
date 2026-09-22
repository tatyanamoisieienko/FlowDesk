namespace FlowDesk.Api.Dtos;

public record StandortDto(int Id, string Name);

public record AbteilungDto(int Id, string Name);

public record StammdatenDto(List<StandortDto> Standorte, List<AbteilungDto> Abteilungen);
