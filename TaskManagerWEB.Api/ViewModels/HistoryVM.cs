using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Models;

namespace TaskManagerWEB.Api.ViewModels
{
    public class HistoryVM
    {
        public int Id { get; set; }
        public string FromStatus { get; set; }
        public string ToStatus { get; set; }
        public DateTime ChangeDate { get; set; }
        public int TaskItemId { get; set; }
        public string TaskItemName { get; set; }
        public string AppUserId { get; set; }
        public string AppUserEmail { get; set; }
    }
}
