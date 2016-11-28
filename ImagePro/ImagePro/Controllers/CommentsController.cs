using System;
using System.Collections.Generic;
using System.Web.Http;
using ImagePro.Model;
using ImagePro.DataSQL;

namespace ImagePro.Controllers
{
    public class CommentsController : ApiController
    {

        static readonly IRepository _repository = new Repository(@"Data Source=(local)\SQLEXPRESS;
                                                                 Initial Catalog=DB;
                                                                 Integrated Security=True");

        [HttpGet]
        //  [Route("api/Comments/{id}")]
        public Comment GetComment(Guid commentId)
        {
            return _repository.GetCommentById(commentId);
        }

        [HttpGet]
        //  [Route("api/Comments/postcomments/{id}")]
        public IEnumerable<Comment> GetAllPostComments(Guid postId)
        {
            return _repository.GetPostComments(postId);
        }

        [HttpPost]
        //  [Route("api/Comments/comment")]
        public Comment AddComment(Comment comment)
        {
            return _repository.AddNewComment(comment);
        }

        [HttpPut]
        //[Route("api/Comments/comment")]
        public void EditComment(Comment comment)
        {
            _repository.EditComment(comment);
        }

        [HttpDelete]
        //  [Route("api/Comments/{id}")]
        public void DeleteComment(Guid commentId)
        {
            _repository.DeleteComment(commentId);
        }

    }
}
