using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Entities;
using TaskManager.DataAccess.Repositories.Interfaces;

namespace TaskManager.DataAccess.Repositories
{
    public class StatusRepository : GenericRepository<Status>,IStatusRepository
    {
        private readonly AppDbContext _db;
        public StatusRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
