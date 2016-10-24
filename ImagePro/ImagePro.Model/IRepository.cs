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

        Post AddPost(Post post);

        Comment AddComment(Comment comment);

        User GetUser(Guid userId);

        Post GetPost(Guid postId);

        bool DeletePost(Guid postId);


    }
}
