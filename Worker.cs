using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using System.Text.Json;

namespace TgBot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    IServiceScopeFactory _scopeFactory;
    IHttpClientFactory _httpClientFactory;
    private readonly IEnumerable<IVacancySource> _sources;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory, IHttpClientFactory httpClientFactory, IEnumerable<IVacancySource> sources)
    {
        _logger = logger;
        _configuration = configuration;
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
        _sources = sources;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var botToken = _configuration["BotToken"];

        if (string.IsNullOrEmpty(botToken))
        {
            _logger.LogError("Токен не найден! Проверь user-secrets.");
            return;
        }

        var botClient = new TelegramBotClient(botToken);

        _logger.LogInformation("Бот запущен и слушает...");
        foreach (var source in _sources)                       // обходим источники
        {
            var vacancies = await source.SearchAsync("", 5, stoppingToken);  // источник ВЕРНУЛ список

            foreach (var v in vacancies)                       // ← ВОТ ЗДЕСЬ вывод
                _logger.LogInformation("[{Source}] {Title} | {Salary} | {Region}",
                   v.Source, v.Title, v.Salary, v.Region);
        }
        // Оба «номера телефона» кладём в одну папку-обработчик
        var handler = new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync);

        // Нанимаем охрану: папка + настройки по умолчанию + лампочка
        botClient.StartReceiving(handler, new ReceiverOptions(), stoppingToken);
        // Держим смену живой, пока не загорится лампочка (Ctrl+C)
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // лампочка загорелась — спокойно завершаем смену
        }
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {

        if (update.Message is not { } message)
            return;

        _logger.LogInformation("Сообщение: {Text}", message.Text);

        using var scope = _scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var resume = new Resume
        {
            ResumeText = message.Text ?? string.Empty,
            ChatId = message.Chat.Id,
            ReceivedAt = DateTime.UtcNow
        };
        dbContext.Resumes.Add(resume);
        await dbContext.SaveChangesAsync();

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: $"Резюме №{resume.Id} сохранено",
            cancellationToken: cancellationToken);

    }

    private Task HandleErrorAsync(
        ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError("Ошибка polling: {Message}", exception.Message);
        return Task.CompletedTask;
    }
}