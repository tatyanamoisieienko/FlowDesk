namespace FlowDesk.Api.Services;

public interface IKiVorschlagService
{
    KiVorschlagErgebnis Analysiere(string originalText);
}
