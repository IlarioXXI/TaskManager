using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.DataAccess.Interfaces
{
    public interface IUnitOfWork
    {
        IAppUserRepository AppUser { get; }
        ITaskItemRepository TaskItem { get; }
        ICommentRepository Comment { get; }
        ITeamRepository Team { get; }
        IStatusRepository Status { get; }
        IPriorityRepository Priority { get; }
        IHistoryRepository History { get; }

        //IGenericRepository<T> GetRepository<T>() where T : class;
        void Save();
    }
}
