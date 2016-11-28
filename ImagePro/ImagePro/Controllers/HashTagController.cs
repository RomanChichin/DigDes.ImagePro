using System;
using System.Collections.Generic;
using System.Web.Http;
using ImagePro.Model;
using ImagePro.DataSQL;
using NLog;

namespace ImagePro.Controllers
{
    public class HashTagController : ApiController
    {
        static readonly IRepository _repository = new Repository(@"Data Source=(local)\SQLEXPRESS;
                                                                 Initial Catalog=DB;
                                                                 Integrated Security=True");
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        [HttpGet]
        //  [Route("api/HashTag/{id}")]
        public IEnumerable<string> GetPostHashTags(Guid postId)
        {
            return _repository.GetPostHashTags(postId);
        }

        [HttpPost]
        //  [Route("api/HashTag/{postid}/{hashtag}")]
          public void AddHashTagToPost(Guid postId, string hashTagText)
        {
           _repository.AddHashTagToPost(postId, hashTagText); 
        }
    }
}
