using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;

namespace TaskManager.Services.Interfaces
{
    public interface ITaskItemService
    {
        IEnumerable<TaskItem> GetAll();
        TaskItem GetById(int id);
        TaskItem Upsert(TaskItem taskItem);
        bool Delete(int id);

    }
}
