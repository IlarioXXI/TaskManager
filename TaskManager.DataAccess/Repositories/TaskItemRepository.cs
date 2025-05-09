﻿
using TaskManager.DataAccess.Interfaces;
using TaskManager.Models;

namespace TaskManager.DataAccess.Repositories
{
    public class TaskItemRepository : GenericRepository<TaskItem>, ITaskItemRepository
    {
        private readonly AppDbContext _db;
        public TaskItemRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public void Add(TaskItem taskItem, int priorityId, int statusId)
        {
            //var priority = _db.Priority.FirstOrDefault(p=>p.Id == priorityId);
            //var status = _db.Status.FirstOrDefault(s=>s.Id == statusId);
            //taskItem.Priority = priority;
            //taskItem.StatusId = statusId;

            _db.TaskItems.Add(taskItem);
        }

    }
}
