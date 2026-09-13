namespace TgBot;

public class Resume
{
    public int Id { get; set; }
    public string ResumeText { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public long ChatId { get; set; }
}