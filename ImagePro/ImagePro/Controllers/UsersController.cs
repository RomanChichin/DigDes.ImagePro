using System;
using System.Web.Http;
using ImagePro.Model;
using ImagePro.DataSQL;
using System.Net;

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

            if (userID == null) //для чего тогда вообще эта проверка?
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            else
            {
                try
                {
                    return _repository.GetUser(userID);
                }
                catch
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound); //почему ничего не выскакивает?
                }
            }


        }

        [HttpPost]
        public User AddUser(User user)
        {
            try
            {
                return _repository.AddUser(user);
            }
            catch
            {
              //что тут можно бросить?
                throw new NotImplementedException();
            }

        }

        [HttpDelete]
        public void DeleteUser(Guid userID)
        {
            _repository.DeleteUser(userID);
        }

    }
}
