﻿using TaskManager.Models;

namespace TaskManager.DataAccess.Interfaces
{
    public interface ITaskItemRepository : IGenericRepository<TaskItem>
    {
        public void Add(TaskItem taskItem, int propertyId, int statusId);
    }
}
