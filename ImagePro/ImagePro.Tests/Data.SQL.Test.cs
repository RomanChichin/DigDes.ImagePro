using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImagePro.Model;
using ImagePro.Data.SQL;

namespace ImagePro.Tests
{
    [TestClass]
    public class RepositoryTest
    {
        //Исходные параметры
        private string _connection = @"Data Source=(local)\SQLEXPRESS;
                                       Initial Catalog=DB;
                                       Integrated Security=True";

        string[] names = { "Jo", "Nick", "Sara", "Lulu", "Eva", "Alex", "John" };
        Random rand = new Random((int)DateTime.Now.Ticks);


        [TestMethod]
        public void AddUser_SomeUser_returnUser()     //почему затемнен?
        {

            //Arrange
            var user = new User
            {
                Nickname = names[rand.Next(0, 5)]
            };
            var repository = new Repository(_connection);

            //Act
            user = repository.AddUser(user);

            //Asserts

            var resultUser = repository.GetUser(user.UserId);
            Assert.AreEqual(user.Nickname, resultUser.Nickname);


        }

        [TestMethod]  //надо проверить каскадное удаление
        public void DeletePost_SomePost_returnTRUE()
        {
            //Arrange
            var post = new Post
            {
                PostId = Guid.NewGuid()
            };

            //Act
            var repository = new Repository(_connection);
            bool result = repository.DeletePost(post.PostId);

            //Asserts
            Assert.IsTrue(result);

        }

        [TestMethod]  
        public void DeletePost_SomeGuid_returnFALSE()
        {
            //Arrange

            var guid = Guid.NewGuid();

            //Act
            var repository = new Repository(_connection);
            bool result = repository.DeletePost(guid);

            //Asserts
            Assert.IsFalse(result);
          

        }

    }
}
