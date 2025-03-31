using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DataAccess.Entities;
using TaskManager.DataAccess.Repositories.Interfaces;

namespace TaskManager.DataAccess.Repositories
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        private readonly AppDbContext _db;
        public TeamRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
