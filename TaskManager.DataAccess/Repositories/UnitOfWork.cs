using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Repositories.Interfaces;

namespace TaskManager.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        
        public IAppUserRepository AppUser { get; private set; }
        public IToDoRepository TaskToDo { get; private set; }
        public ICommentRepository Comment { get; private set; }



        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            AppUser = new AppUserRepository(_db);
            TaskToDo = new TaskToDoRepository(_db);
            Comment = new CommentRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
