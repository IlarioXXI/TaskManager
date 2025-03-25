using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Entities;
using TaskManager.DataAccess.Repositories.Interfaces;

namespace TaskManager.DataAccess.Repositories
{
    public class TaskToDoRepository : GenericRepository<TaskToDo>, IToDoRepository
    {
        private readonly AppDbContext _db;
        public TaskToDoRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(TaskToDo entity)
        {
            dbSet.Update(entity);
        }
    }
}
