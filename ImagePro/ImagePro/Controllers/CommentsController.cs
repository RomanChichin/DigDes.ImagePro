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
        public Comment GetComment(Guid commentID)
        {
            return _repository.GetComment(commentID);
        }

        //[HttpGet]
        //public IEnumerable<Comment> GetAllComments(Guid postID)
        //{
        //    return _repository.GetAllComments(postID);
        //}


        [HttpPost]
        public Comment AddComment(Comment comment)
        {
            return _repository.AddComment(comment);
        }

        [HttpDelete]
        public void DeleteComment(Guid commentID)
        {
            _repository.DeleteComment(commentID);
        }


    }
}
