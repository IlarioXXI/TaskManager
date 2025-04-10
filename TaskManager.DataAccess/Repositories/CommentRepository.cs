
using TaskManager.DataAccess.Repositories.Interfaces;
using TaskManager.Models;

namespace TaskManager.DataAccess.Repositories
{
    public class CommentRepository : GenericRepository<Comment>,ICommentRepository
    {
        private readonly AppDbContext _db;
        public CommentRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
