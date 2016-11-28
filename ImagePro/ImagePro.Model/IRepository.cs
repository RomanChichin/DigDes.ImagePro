using System;
using System.Collections.Generic;


namespace ImagePro.Model
{
    public interface IRepository
    {
        //Users Methods
        User AddNewUser(User user);
        User GetUserById(Guid userId);
        void DeleteUser(Guid userId);
        void PressAndRemoveLikeToPost(Guid userId, Guid postId);
        void EditUserInformation(User user);
        void SubscribeToUser(Guid subscriberId, Guid publisherId);
        void UnSubscribeFromUser(Guid subscriberId, Guid publisherId);
        IEnumerable<User> GetAllSubscribers(Guid publisherId);

            //Posts Methods
        Post AddNewPost(Post post);
        Post GetPostById(Guid postId);
        void DeletePost(Guid postId);
        IEnumerable<Comment> GetPostComments(Guid postId, int commentsNumber = 10);   //можно ли в интерфейсе делать опциональный параметр?
        IEnumerable<string> GetPostHashTags(Guid postId);
        void AddHashTagToPost(Guid postId, string hashTagText);

        //Comments Methods
        Comment GetCommentById(Guid commentId);
        Comment AddNewComment(Comment comment);
        void DeleteComment(Guid commentId);
        void EditComment(Comment comment);

    }
}
