﻿using System;
using System.Collections.Generic;
using System.Linq;
using TaskManager.DataAccess.Interfaces;
using TaskManager.Models;

namespace TaskManager.DataAccess.Repositories
{
    public class AppUserRepository : GenericRepository<AppUser>,IAppUserRepository
    {
        private readonly AppDbContext _db;
        public AppUserRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
