namespace TaskManagerWeb.Api.Models
{
    public class CommentVM
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public int TaskItemId { get; set; }
        public string TaskItemTitle { get; set; }
        public string? AppUserId { get; set; }
        public string AppUserEmail { get; set; }
    }
}
