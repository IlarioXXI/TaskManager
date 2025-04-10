using TaskManager.Models;

namespace TaskManager.Services.ServicesInterfaces
{
    public interface ICommentService
    {
        IEnumerable<Comment> GetAllByTaskId(int taskItemId);
        Task<Comment> UpsertAsync(Comment comment);
        Comment Delete(int taskId);
    }
}
