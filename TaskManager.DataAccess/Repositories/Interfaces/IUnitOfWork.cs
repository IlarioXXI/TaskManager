using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.DataAccess.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IAppUserRepository AppUser { get; }
        IToDoRepository TaskToDo { get; }
        ICommentRepository Comment { get; }
        void Save();
    }
}
