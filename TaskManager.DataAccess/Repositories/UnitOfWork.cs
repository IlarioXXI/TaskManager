using Microsoft.EntityFrameworkCore;
using TaskManager.DataAccess.Interfaces;

namespace TaskManager.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        
        public IAppUserRepository AppUser { get; private set; }
        public ITaskItemRepository TaskItem { get; private set; }
        public ICommentRepository Comment { get; private set; }
        public ITeamRepository Team { get; private set; }
        public IPriorityRepository Priority { get; private set; }
        public IStatusRepository Status { get; private set; }
        public IHistoryRepository History { get; private set; }



        public UnitOfWork(AppDbContext db)
        {

            _db = db;
            AppUser = new AppUserRepository(_db);
            TaskItem = new TaskItemRepository(_db);
            Comment = new CommentRepository(_db);
            Team = new TeamRepository(_db);
            Priority = new PriorityRepository(_db);
            Status = new StatusRepository(_db); 
            History = new HistoryRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }

        //public IGenericRepository<T> GetRepository<T>() where T : class
        //{
        //    return new GenericRepository<T>(_db);
        //}
    }
}
