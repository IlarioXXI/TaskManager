using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Entities;
using TaskManager.DataAccess.Repositories.Interfaces;

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
