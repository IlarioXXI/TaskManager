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
    public class Team
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public List<TaskItem>? TaskItems { get; set; }
        //public string UserIdteamLeader { get; set; }

        public List<AppUser> Users { get; set; } = [];
    }
}
