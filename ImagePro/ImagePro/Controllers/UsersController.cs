using System;
using System.Web.Http;
using ImagePro.Model;
using ImagePro.DataSQL;

namespace ImagePro.Controllers
{
    public class UsersController : ApiController
    {
        //public User GetUser()
        //{
        //    return new User()
        //    {
        //      Nickname = "Nik",
        //      UserId = Guid.NewGuid(),
        //      RegistrationDate = DateTime.Now
        //    };
        //}

        //public User GetUser(int id)
        //{
        //    return new User()
        //    {
        //        Nickname = "NikID",
        //        UserId = Guid.NewGuid(),
        //        RegistrationDate = DateTime.Now

        //    };
        //}

//        private const string _connectionString = @"Data Source=(local)\SQLEXPRESS;
//                                                   Initial Catalog=DB;
//                                                   Integrated Security=True";

//        private readonly IRepository _repository;

//        public UsersController()
//        {
//            _repository = new ImagePro.DataSQL.Repository(_connectionString);
//        }

        static readonly IRepository _repository = new Repository(@"Data Source=(local)\SQLEXPRESS;
                                                                 Initial Catalog=DB;
                                                                 Integrated Security=True");

        [HttpGet]
        public User GetUser(Guid userID)
        {
            return _repository.GetUser(userID);
        }

        [HttpPost]
        public User AddUser(User user)
        {
            return _repository.AddUser(user);
        }

        [HttpDelete]
        public void DeleteUser(Guid userID)
        {
            _repository.DeleteUser(userID);
        }

    }
}
