using System;
using System.Collections.Generic;
using System.Web.Http;
using ImagePro.Model;
using ImagePro.DataSQL;
using NLog;

namespace ImagePro.Controllers
{
    public class PostsController : ApiController
    {
        static readonly IRepository _repository = new Repository(@"Data Source=(local)\SQLEXPRESS;
                                                                 Initial Catalog=DB;
                                                                 Integrated Security=True");
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        public Post GetPost(Guid postId)
        {
            return _repository.GetPostById(postId);
        }

        [HttpPost]
        public Post CreatePost(Post post)
        {
            return _repository.AddNewPost(post);
        }

        [HttpDelete]
        public void DeletePost(Guid postId)
        {
            _repository.DeletePost(postId);
        }
    }
}
