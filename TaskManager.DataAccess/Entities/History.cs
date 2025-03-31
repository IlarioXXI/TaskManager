using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.DataAccess.Entities
{
    public class History
    {
        [Key]
        public int Id { get; set; }
        public string FromStatus { get; set; }
        public string ToStatus { get; set; }
        public DateTime ChangeDate { get; set; }
        public int TaskItemId { get; set; }
        [ForeignKey("TaskItemId")]
        public TaskItem TaskItem { get; set; }

    }
}
