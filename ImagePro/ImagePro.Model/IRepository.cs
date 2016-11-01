using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagePro.Model
{
    public interface IRepository
    {
        User AddUser(User user);
        User GetUser(Guid userId);
        void DeleteUser(Guid userId);

        Post AddPost(Post post);
        Post GetPost(Guid postId);
        void DeletePost(Guid postId);

        IEnumerable<Comment> GetAllComments(Guid postID);
        Comment GetComment(Guid commentID);
        Comment AddComment(Comment comment);
        void DeleteComment(Guid commentId);
    }
}
