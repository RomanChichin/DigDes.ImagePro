using System;
using System.Collections.Generic;
using System.Linq;
using ImagePro.Model;
using ImagePro.DataSQL;
using ImagePro.Exeptions;
using NUnit.Framework;



namespace ImagePro.Tests  //тестовая таблица должна быть без FK? а то иначе приходится очень сложно создавать фейковые посты и комменты
{
    [TestFixture]
    public class RepositoryTest
    {
        //Исходные параметры
        Random rand = new Random((int)DateTime.Now.Ticks);
        private readonly string _connection = @"Data Source=(local)\SQLEXPRESS;
                                                Initial Catalog=DB_TEST;
                                                Integrated Security=True";
        private Repository database;

        private string CreateRandomString()
        {
            var result = Guid.NewGuid().ToString().Substring(20);

            return result;
        }

        private byte[] CreateRandomByteArray()
        {
            int length = rand.Next(3 * 1024 * 1024, 10 * 1024 * 1024);

            byte[] array = new byte[length];

            rand.NextBytes(array);

            return array;
        }
        private User CreateFakeUser()
        {
            return new User()
            {
                Nickname = CreateRandomString(),
                Email = CreateRandomString(),
                About = CreateRandomString()
            };
        }

        private Post CreateFakePost()
        {
            var user = CreateFakeUser();
            user = database.AddNewUser(user);

            return new Post()
            {
                UserID = user.UserId,
                Photo = CreateRandomByteArray()
            };
        }

        private Comment CreateFakeComment()
        {
            var user = CreateFakeUser();
            user = database.AddNewUser(user);

            var post = CreateFakePost();
            post = database.AddNewPost(post);

            return new Comment()
            {
                PostID = post.PostId,
                UserID = user.UserId,
                Text = CreateRandomString()
            };
        }

        [SetUp]
        public void InitilizeConnectionToDatabase()
        {
            database = new Repository(_connection);
        }

        //Как отчистить всю БД сразу?  или что сдесь можно сделать после окончания метода?
        //[TearDown]
        //private void CleanDatabaseAfterTest()
        //{
        //    using (var sqlconnection = new SqlConnection(_connection))
        //    {
        //        sqlconnection.Open();
        //        using (var command = sqlconnection.CreateCommand())
        //        {
        //            command.CommandText = @"";
        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}

        [Test]
        public void AddNewUser_TryToCreateNewUser_returnNewUser()
        {
            //Arrange
            var user = CreateFakeUser();

            //Act
            user = database.AddNewUser(user);

            //Asserts
            var resultUser = database.GetUserById(user.UserId);
            Assert.AreEqual(user.Nickname, resultUser.Nickname);
        }

        [Test]
        public void AddNewUser_TryToCreateUserWithSameName_ExceptionExpected()
        {
            //Arrange
            var user = CreateFakeUser();

            //Act
            var newuser = database.AddNewUser(user);

            var ex = Assert.Catch(() => database.AddNewUser(user));

            //Assert
            StringAssert.Contains("UNIQUE KEY", ex.Message);
        }

        [Test]
        public void DeleteUser_TryToDeleteUnexistedUser_ExceptionExpected()
        {
            //Arrange

            //Act
            var ex = Assert.Catch(() => database.DeleteUser(Guid.NewGuid()));

            //Assert
            StringAssert.Contains("Запрашиваемого пользователя не существует.", ex.Message);
        }

        [Test]
        public void DeleteUser_TryToDeleteUserWithEmptyId_ExceptionExpected()
        {
            //Arrange

            //Act
            var ex = Assert.Catch(() => database.DeleteUser(Guid.Empty));

            //Assert
            StringAssert.Contains("Попытка выполнения операции по некорректному Id объекта.", ex.Message);
        }

        [Test]
        public void EditUserInformation_TryToEditInformation_returnUpdatedUser()
        {
            //Arrange
            var user = CreateFakeUser();
            user = database.AddNewUser(user);

            var initialNicknameValue = user.Nickname;

            //Act
            user.Nickname = CreateRandomString();

            database.EditUserInformation(user);

            var updatedUser = database.GetUserById(user.UserId);

            //Assert
            StringAssert.AreNotEqualIgnoringCase(initialNicknameValue, updatedUser.Nickname);
        }

        [Test]
        public void SubscribeToUser_TryUserToSubscribeAnother_returnTrue()
        {
            //Arrange
            var publisher = CreateFakeUser();
            var subscriber = CreateFakeUser();
            bool isSubscribe;

            //Act
            publisher = database.AddNewUser(publisher);
            subscriber = database.AddNewUser(subscriber);

            database.SubscribeToUser(subscriber.UserId, publisher.UserId);

            isSubscribe = database.IsSubscriber(publisher.UserId, subscriber.UserId);

            //Assert
            Assert.IsTrue(isSubscribe);
        }

        [Test]
        public void SubscribeToUser_TryToSubscribeMoreThenOneTime_ExceptionExpected()
        {
            //Arrange
            var publisher = CreateFakeUser();
            var subscriber = CreateFakeUser();

            //Act
            publisher = database.AddNewUser(publisher);
            subscriber = database.AddNewUser(subscriber);

            database.SubscribeToUser(subscriber.UserId, publisher.UserId);
            var ex = Assert.Catch<ErrorTryingToSubscribe>(
                () => database.SubscribeToUser(subscriber.UserId, publisher.UserId));

            //Assert
            StringAssert.Contains("Попытка повторной подписки на пользователя.", ex.Message);
        }

        [Test]
        public void UnSubscribeFromUser_TryToUnsubscribe_returnTrue()
        {
            //Arrange
            var publisher = CreateFakeUser();
            var subscriber = CreateFakeUser();
            bool isSubscribe;

            //Act
            publisher = database.AddNewUser(publisher);
            subscriber = database.AddNewUser(subscriber);

            database.SubscribeToUser(subscriber.UserId, publisher.UserId);
            database.UnSubscribeFromUser(subscriber.UserId, publisher.UserId);

            isSubscribe = database.IsSubscriber(publisher.UserId, subscriber.UserId);

            //Assert
            Assert.IsTrue(!isSubscribe);
        }

        [Test]
        public void UnSubscribeFromUser_TryToUnsubscribeFromUnsubscribedUser_ExeptionExpexted()
        {
            //Arrange
            var subscriber = CreateFakeUser();
            var randomUser = CreateFakeUser();

            //Act
            subscriber = database.AddNewUser(subscriber);
            randomUser = database.AddNewUser(randomUser);

            var ex = Assert.Catch<ErrorTryingToUnsubscribe>(
                () => database.UnSubscribeFromUser(subscriber.UserId, randomUser.UserId));

            //Assert
            StringAssert.Contains("Попытка отписаться от пользователя, на которого нет подписки.", ex.Message);
        }

        [Test] //может есть что то получше?  + проверить написание
        public void GetAllSubscribers_TryToGetAllUserSubscribers_returnFalse()
        {
            //Arrange
            List<User> subscribers = new List<User>(rand.Next(10, 20));
            var publisher = CreateFakeUser();
            var resultOfEverySubscriber = new bool[subscribers.Capacity];
            bool result;

            //Act
            publisher = database.AddNewUser(publisher);

            for (int i = 0; i < subscribers.Capacity; i++)
            {
                subscribers.Add(CreateFakeUser());

                subscribers[i] = database.AddNewUser(subscribers[i]);

                database.SubscribeToUser(subscribers[i].UserId, publisher.UserId);
            }

            for (int i = 0; i < subscribers.Capacity; i++)
            {
                resultOfEverySubscriber[i] =
                    database.IsSubscriber(publisher.UserId, subscribers[i].UserId);
            }

            result = resultOfEverySubscriber.Contains(false);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void AddNewPost_TryToAddNewPostWithNormalPhotoSize_returnNewPost()
        {
            //Arrage
            var post = CreateFakePost();

            //Act
            post = database.AddNewPost(post);

            var returnedPost = database.GetPostById(post.PostId);

            //Assert
            Assert.AreEqual(post.UserID, returnedPost.UserID);
        }

        [Test]
        public void AddNewPost_TryToAddNewPostWithEnormousPhotoSize_ExceptionExpected()
        {
            //Arrage
            var post = CreateFakePost();
            byte[] largePhoto = new byte[16 * 1024 * 1024];

            //Act
            rand.NextBytes(largePhoto);
            post.Photo = largePhoto;

            var ex = Assert.Catch<ErrorUploadingPhoto>(
                () => database.AddNewPost(post));

            //Assert
            StringAssert.Contains("Загружаемая фоторграфия имеет размер больше предельного.", ex.Message);
        }

        [Test]
        public void DeletePost_DeleteSomePost_returnFalse()
        {
            //Arrange
            var post = CreateFakePost();
            bool result;

            post = database.AddNewPost(post);

            //Act
            database.DeletePost(post.PostId);

            result = database.IsPostExists(post.PostId);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeletePost_DeleteUnexistedPost_ErrorExpected()
        {
            //Arrange

            //Act
            var ex = Assert.Catch<ErrorGettingPost>(
                () => database.DeletePost(Guid.NewGuid()));

            //Assert
            StringAssert.Contains("Запрашиваемого поста не существует.", ex.Message);
        }

        [Test]
        public void AddNewComment_TryToCreateNewComment_returnTrue()
        {
            //Arrange
            var comment = CreateFakeComment();

            //Act
            comment = database.AddNewComment(comment);

            var returnedComment = database.GetCommentById(comment.CommentId);

            //Assert
            Assert.AreEqual(comment.Text, returnedComment.Text);
        }

        [Test]
        public void DeleteComment_TryToDeleteSomeComment_returnFalse()
        {
            //Arrange
            var comment = CreateFakeComment();
            comment = database.AddNewComment(comment);
            bool result;

            //Act
            database.DeleteComment(comment.CommentId);

            result = database.IsCommentExists(comment.CommentId);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteComment_TryToDeleteUnexistedComment_ExceptionExpected()
        {
            //Arrange

            //Act
            var ex = Assert.Catch<ErrorGettingComment>(
                () => database.DeleteComment(Guid.NewGuid()));

            //Assert
            StringAssert.Contains("Запрашиваемого комментария не существует.", ex.Message);
        }

        [Test]
        public void EditComment_TryToEditComment_returnTrue()
        {
            //Arrange
            var comment = CreateFakeComment();
            comment = database.AddNewComment(comment);
            var initialCommentText = comment.Text;

            //Act
            comment.Text = CreateRandomString();
            database.EditComment(comment);

            var updatedComment = database.GetCommentById(comment.CommentId);

            //Assert
            StringAssert.AreNotEqualIgnoringCase(initialCommentText, updatedComment.Text);

        }

        [Test]  //Es uno plobleme
        public void GetPostComments_TryToGetAllPostComments_returnFalse()
        {
            //Arrange
            var post = CreateFakePost();
            post = database.AddNewPost(post);
            List<Comment> comments = new List<Comment>(rand.Next(5, 10));
            Comment comment;
            int j = 0;

            bool[] everyCommentResult = new bool[comments.Capacity];
            bool result;


            //Act
            for (int i = 0; i < comments.Capacity; i++)
            {
                comment = CreateFakeComment();
                comment.PostID = post.PostId;

                comments.Add(comment);

                database.AddNewComment(comments[i]);
            }

            var returnedComments = database.GetPostComments(post.PostId);

            foreach (var comt in returnedComments)
            {
                everyCommentResult[j] = (comt.PostID == post.PostId);
                j++;
            }

            result = everyCommentResult.Contains(false);

            //Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void AddHashTagToPost_TryToAddNewHashTag_returnTrue()
        {
            //Arrange
            var post = CreateFakePost();
            post = database.AddNewPost(post);
            var hashTag = CreateRandomString();
            bool result;

            //Act
            database.AddHashTagToPost(post.PostId,hashTag);

            result = database.IsPostHashtag(post.PostId, hashTag);

            //Assert
            Assert.IsTrue(result);
        }
    }
}

