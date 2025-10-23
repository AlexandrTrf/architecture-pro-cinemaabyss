namespace events.Models
{
    // Common Event container
    public class Event
    {
        public string Id { get; set; } = default!;          // e.g. "movie-123-viewed-<guid>"
        public string Type { get; set; } = default!;        // movie | user | payment
        public DateTimeOffset Timestamp { get; set; }
        public object Payload { get; set; } = default!;
    }

    public class EventResponse
    {
        public string Status { get; set; } = "success";
        public int Partition { get; set; }
        public long Offset { get; set; }
        public Event Event { get; set; } = default!;
    }

    public class Error
    {
        public string ErrorMessage { get; set; } = default!;
    }

    // MovieEvent DTO
    public class MovieEvent
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = default!;
        public string Action { get; set; } = default!; // e.g. "viewed"
        public int? UserId { get; set; }
        public float? Rating { get; set; }
        public string[]? Genres { get; set; }
        public string? Description { get; set; }
    }

    // UserEvent DTO
    public class UserEvent
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string Action { get; set; } = default!;
        public DateTimeOffset Timestamp { get; set; }
    }

    // PaymentEvent DTO
    public class PaymentEvent
    {
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = default!;
        public DateTimeOffset Timestamp { get; set; }
        public string? MethodType { get; set; }
    }
    
    
}