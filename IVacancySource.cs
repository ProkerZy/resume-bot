namespace TgBot;

public interface IVacancySource
{
    string SourceName { get; }
    Task<List<VacancyDTO>> SearchAsync(string query, int limit, CancellationToken ct);
}