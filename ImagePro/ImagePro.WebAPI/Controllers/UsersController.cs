using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImagePro.Model;

namespace ImagePro.WebAPI.Controllers
{
    public class UsersController : ApiController
    {
        private const string _connectionString = @"Data Source=(local)\SQLEXPRESS;
                                                   Initial Catalog=DB;
                                                   Integrated Security=True";

        private readonly IRepository _repository;

        public UsersController()
        {
            _repository = new ImagePro.Data.SQL.Repository(_connectionString);
        }

        [HttpPost]
        public User CreateUser(User user)
        {
            return _repository.AddUser(user);
        }

        [HttpPost]
        public Post CreatePost(Post post)
        {
            return _repository.AddPost(post);
        }

        [HttpPost]
        public Comment CreateComment(Comment comment)
        {
            return _repository.AddComment(comment);
        }

    }
}
