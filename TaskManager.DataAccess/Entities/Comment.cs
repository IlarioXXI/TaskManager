using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.DataAccess.Entities
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }

        public int TaskToDoId { get; set; }

        [ForeignKey("TaskToDoId")]
        public TaskToDo? TaskToDo { get; set; }

    }
}
