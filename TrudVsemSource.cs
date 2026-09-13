using System.Text.Json;

namespace TgBot;

public class TrudvsemVacancySource : IVacancySource
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TrudvsemVacancySource> _logger;

    public TrudvsemVacancySource(IHttpClientFactory httpClientFactory, ILogger<TrudvsemVacancySource> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public string SourceName => "trudvsem";

    public async Task<List<VacancyDTO>> SearchAsync(string query, int limit, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient();
        var url = $"https://opendata.trudvsem.ru/api/v1/vacancies?offset=0&limit={limit}";

        var json = await client.GetStringAsync(url, ct);
        var response = JsonSerializer.Deserialize<TrudvsemResponse>(json);

        var result = new List<VacancyDTO>();
        if (response?.Results?.Vacancies == null) return result;

        foreach (var wrapper in response.Results.Vacancies)
        {
            var v = wrapper?.Vacancy;
            if (v == null) continue;

            result.Add(new VacancyDTO
            {
                Title = v.JobName ?? "",
                Company = v.Company?.Name ?? "",
                Region = v.Region?.Name ?? "",
                Salary = v.Salary ?? "",
                Url = v.VacUrl ?? "",
                Source = SourceName,
                Description = v.Duty ?? ""
            });
        }
        return result;
    }
}