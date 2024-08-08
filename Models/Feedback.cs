namespace Carbon_inventory_platform.Models
{
    // Models/Feedback.cs
    public class Feedback
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
