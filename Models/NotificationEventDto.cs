namespace Yrke.Models
{
    public class NotificationEventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Link { get; set; }
        public bool IsRead { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }
}
