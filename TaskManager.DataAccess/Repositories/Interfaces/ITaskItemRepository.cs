using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Entities;

namespace TaskManager.DataAccess.Repositories.Interfaces
{
    public interface ITaskItemRepository : IGenericRepository<TaskItem>
    {
        public void Add(TaskItem taskItem, int propertyId, int statusId);
    }
}
