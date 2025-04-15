using TaskManager.Models;

namespace TaskManager.Services.ServicesInterfaces
{
    public interface ICommentService /*: IGenericService<Comment,CommentVM>*/
    {
        IEnumerable<Comment> GetAllByTaskId(int taskItemId);
        Comment Upsert(Comment comment);
        Comment Delete(int id);

    }
}
