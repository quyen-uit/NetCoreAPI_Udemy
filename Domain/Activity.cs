namespace Domain
{
    public class Activity
    {
        public Activity()
        {
            ActivityAttendees = new List<ActivityAttendee>();
            Comments = new List<Comment>();
        }

        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string? Category { get; set; }
        public string? City { get; set; }
        public string? Venue { get; set; }
        public bool IsCancelled { get; set; }
        public ICollection<ActivityAttendee>? ActivityAttendees { get; set; } 
        public ICollection<Comment>? Comments { get; set; }
    }
}