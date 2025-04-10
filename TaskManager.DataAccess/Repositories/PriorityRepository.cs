using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.Models;

namespace TaskManager.DataAccess.Repositories
{
    public class PriorityRepository : GenericRepository<Priority>,IPriorityRepository
    {
        private readonly AppDbContext _db;
        public PriorityRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
