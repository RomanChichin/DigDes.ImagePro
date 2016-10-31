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
        bool DeleteUser(Guid userId);

        Post AddPost(Post post);
        Post GetPost(Guid postId);
        bool DeletePost(Guid postId);

        Comment AddComment(Comment comment);
        bool DeleteComment(Guid commentId);

     

       

      


    }
}
