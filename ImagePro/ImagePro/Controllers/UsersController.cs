using System;
using System.Collections.Generic;
using System.Web.Http;
using ImagePro.Model;
using ImagePro.DataSQL;
using ImagePro.Filters;
using NLog;

namespace ImagePro.Controllers
{
    public class UsersController : ApiController
    {
        static readonly IRepository _repository = new Repository(@"Data Source=(local)\SQLEXPRESS;
                                                                 Initial Catalog=DB;
                                                                 Integrated Security=True");
        

        [HttpGet]
        //  [Route("api/Users/{id}")]
        [ErrorHandler]
        public User GetUser(Guid userID)
        {
            return _repository.GetUserById(userID);
        }

        [HttpGet]
        // [Route("api/Users/subscribers/{id}")]
        public IEnumerable<User> GetUserSubscribers(Guid userId)
        {
            return _repository.GetAllSubscribers(userId);
        }

        [HttpPost]
        // [Route("api/Users/user")]
        public User CreateUser(User user)
        {
            return _repository.AddNewUser(user);
        }

        [HttpPost]
        // [Route("api/Users/sub/{sid}/{pid}")]
        public void Subscribe(Guid subscriberId, Guid publisherId)
        {
            _repository.SubscribeToUser(subscriberId, publisherId);
        }

        [HttpDelete]
        // [Route("api/Users/unsub/{sid}/{pid}")]
        public void Unsubscribe(Guid subscriberId, Guid publisherId)
        {
            _repository.UnSubscribeFromUser(subscriberId, publisherId);
        }

        [HttpDelete]
        //  [Route("api/Users/{id}")]
        public void DeleteUser(Guid userID)
        {
            _repository.DeleteUser(userID);
        }

        [HttpPut]
        //  [Route("api/Users/user")]
        public void EditUser(User user)
        {
            _repository.EditUserInformation(user);
        }










    }
}
