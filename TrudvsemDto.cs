using System.Text.Json.Serialization;

namespace TgBot;

public class TrudvsemResponse
{
    [JsonPropertyName("meta")] public Meta Meta { get; set; } = new();
    [JsonPropertyName("results")] public Results Results { get; set; } = new();
}

public class Meta
{
    [JsonPropertyName("total")] public int Total { get; set; }
}

public class Results
{
    [JsonPropertyName("vacancies")] public List<VacancyWrapper> Vacancies { get; set; } = new();
}

public class VacancyWrapper
{
    [JsonPropertyName("vacancy")] public Vacancy Vacancy { get; set; } = new();
}

public class Vacancy
{
    [JsonPropertyName("job-name")] public string JobName { get; set; } = string.Empty;
    [JsonPropertyName("salary")] public string Salary { get; set; } = string.Empty;
    [JsonPropertyName("vac_url")] public string VacUrl { get; set; } = string.Empty;
    [JsonPropertyName("company")] public Company Company { get; set; } = new();
    [JsonPropertyName("region")] public Region Region { get; set; } = new();
    [JsonPropertyName("duty")] public string? Duty { get; set; }
}

public class Company
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}

public class Region
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}