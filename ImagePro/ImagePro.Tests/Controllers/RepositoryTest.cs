using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImagePro.Model;
using ImagePro.DataSQL;

namespace ImagePro.Tests
{
    [TestClass]
    public class RepositoryTest
    {
        //Исходные параметры
        private string _connection = @"Data Source=(local)\SQLEXPRESS;
                                       Initial Catalog=DB;
                                       Integrated Security=True";

        #region UserTests

        [TestMethod]
        public void GetUser_FalseGuid_returnException()
        {
            //Arrange
            var repository = new Repository(_connection);
            bool result;

            //Act
            try
            {
                User returnedUser = repository.GetUser(Guid.NewGuid());
                result = true;
            }
            catch
            {
                result = false;
            }

            //Asserts
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AddUser_SomeUser_returnUser()
        {
            //Arrange
            var user = new User
            {
                Nickname = Guid.NewGuid().ToString().Substring(20)
            };
            var repository = new Repository(_connection);

            //Act
            user = repository.AddUser(user);

            //Asserts
            var resultUser = repository.GetUser(user.UserId);
            Assert.AreEqual(user.Nickname, resultUser.Nickname);
        }

        [TestMethod]
        public void AddUser_TwoUsersWithSameNames_returnError()
        {
            //Arrange
            var repository = new Repository(_connection);

            var user1 = new User()
            {
                Nickname = Guid.NewGuid().ToString().Substring(20)
            };
            var user2 = new User()
            {
                Nickname = user1.Nickname
            };

            bool result;

            //Act
            repository.AddUser(user1);

            try
            {
                repository.AddUser(user2);
                result = false;
            }
            catch
            {
                result = true;
            }

            //Asserts
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void DeleteUser_SomeUser_OK()
        {
            //Arrange
            var user = new User
            {
                Nickname = Guid.NewGuid().ToString().Substring(20)
            };
            var repository = new Repository(_connection);

            bool result;

            //Act
            user = repository.AddUser(user);

            try
            {
                repository.DeleteUser(user.UserId);
                repository.GetUser(user.UserId);
                result = false;
            }
            catch
            {
                result = true;
            }

            //Asserts
            Assert.IsTrue(result);
        }

        //[TestMethod]    Как?
        //public void DeleteUser_SomeUser_returnException()
        //{
        //    //Arrange
        //    var repository = new Repository(_connection);

        //    Guid id = Guid.NewGuid();

        //    bool result;

        //    //Act
        //    try
        //    {
        //        repository.DeleteUser(id);
        //        repository.GetUser(id);
        //        result = true;
        //    }
        //    catch
        //    {
        //        result = false;
        //    }

        //    //Asserts
        //    Assert.IsFalse(result);
        //}

        #endregion


        [TestMethod]
        public void AddComment_SomeComment_Comment()
        {
            //Arrange
            var repository = new Repository(_connection);

            var comment = new Comment()
            {
                PostID = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
                Text = "Abra-kadabra"
            }
            ;

            //Act
            comment = repository.AddComment(comment);

            //Asserts
            var newcomment = repository.GetComment(comment.CommentId);
            Assert.AreEqual(comment.CommentId, newcomment.CommentId);
        }



        //[TestMethod]  //надо проверить каскадное удаление
        //public void DeletePost_SomePost_returnTRUE()
        //{
        //    //Arrange
        //    var post = new Post
        //    {
        //        PostId = Guid.NewGuid()
        //    };

        //    //Act
        //    var repository = new Repository(_connection);
        //    bool result = repository.DeletePost(post.PostId);

        //    //Asserts
        //    Assert.IsTrue(result);

        //}

        //[TestMethod]
        //public void DeletePost_SomeGuid_returnFALSE()
        //{
        //    //Arrange

        //    var guid = Guid.NewGuid();

        //    //Act
        //    var repository = new Repository(_connection);
        //    bool result = repository.DeletePost(guid);

        //    //Asserts
        //    Assert.IsFalse(result);


        //}

    }
}

