using System;
using System.Web.Http;
using ImagePro.Model;
using ImagePro.DataSQL;
using NLog;


namespace ImagePro.Controllers
{
    public class LikeController : ApiController
    {
        static readonly IRepository _repository = new Repository(@"Data Source=(local)\SQLEXPRESS;
                                                                 Initial Catalog=DB;
                                                                 Integrated Security=True");
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        [HttpPost]
        //  [Route("api/HashTag/post/{userid}/{postid}")]
        public void LikeToPost(Guid userId, Guid postId)
        {
            _repository.PressAndRemoveLikeToPost(userId, postId);
        }

        [HttpPost]
        //  [Route("api/HashTag/comment/{userid}/{postid}")]
        public void LikeToComment(Guid userId, Guid commentId)
        {
            throw new NotImplementedException();
        }
    }
}
