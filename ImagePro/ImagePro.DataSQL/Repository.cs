using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using ImagePro.Model;
using ImagePro.Exeptions;
using NLog;

namespace ImagePro.DataSQL
{
    public class Repository : IRepository
    {
        private readonly string _connectionString;
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private Stopwatch _stopWatch;
        private byte[] MaxPhotoSize = new byte[1024 * 1024 * 10];

        public Repository(string newconnection)// переписать короче?
        {
            if (newconnection != string.Empty &&
                CheckConnectionToDatabase(newconnection))
            {
                _connectionString = newconnection;
            }
            else
            {
                throw new ErrorConnectingToDatabase();
            }
        }

        public string Connection
        {
            get { return _connectionString; }
        }
        private void WriteOperationTimeToLog(Stopwatch stopwatch)
        {
            _logger.Debug("Operation completed, time: " + stopwatch.ElapsedMilliseconds + " misec");
        }

        private bool CheckConnectionToDatabase(string connection) //вынести close в блок finally?
        {
            try
            {
                var newconnection = new SqlConnection(connection);
                newconnection.Open();
                newconnection.Close();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool IsUserExists(Guid userId)
        {
            bool result;

            if (userId != Guid.Empty)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT UserID FROM Users
                                            WHERE UserID = @ID";
                        command.Parameters.AddWithValue("@ID", userId);

                        using (var reader = command.ExecuteReader())
                        {
                            result = reader.HasRows;
                        }
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public bool IsPostExists(Guid postId)
        {
            bool result;

            if (postId != Guid.Empty)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT PostID FROM Posts
                                            WHERE PostID = @ID";
                        command.Parameters.AddWithValue("@ID", postId);

                        using (var reader = command.ExecuteReader())
                        {
                            result = reader.HasRows; //что будет, если здесь поставить return?
                        }
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public bool IsCommentExists(Guid commentId)
        {
            bool result;

            if (commentId != Guid.Empty)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT CommentID FROM Comments
                                            WHERE CommentID = @ID";
                        command.Parameters.AddWithValue("@ID", commentId);

                        using (var reader = command.ExecuteReader())
                        {
                            result = reader.HasRows;
                        }
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }

        public bool IsUserPressedLike(Guid userId, Guid postId)
        {
            bool result;

            if (userId == Guid.Empty || postId == Guid.Empty)
            {
                throw new IncorrectIdError();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT * FROM Likes
                                            WHERE PostID=@postid AND UserID=@userid";
                    command.Parameters.AddWithValue("@postid", postId);
                    command.Parameters.AddWithValue("@userid", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        result = reader.HasRows;
                    }
                }
            }

            return result;
        }

        public bool IsSubscriber(Guid publisherId, Guid subscriberId)
        {
            bool result;

            if (publisherId == Guid.Empty || subscriberId == Guid.Empty)
            {
                throw new IncorrectIdError();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT * FROM Subscribers
                                            WHERE PublisherID=@pubId AND SubscriberID=@subId";
                    command.Parameters.AddWithValue("@pubId", publisherId);
                    command.Parameters.AddWithValue("@subId", subscriberId);

                    using (var reader = command.ExecuteReader())
                    {
                        result = reader.HasRows;
                    }
                }
            }

            return result;
        }
         
        /// <summary>
         ///  Позволяет узнать, есть ли у поста данный хештег.
         /// </summary>
         /// <param name="postId"></param>
         /// <param name="hashTag"></param>
         /// <returns></returns>
        public bool IsPostHashtag(Guid postId, string hashTag)
        {
            bool result;

            if (postId == Guid.Empty)
            {
                throw new IncorrectIdError();
            }
            if (hashTag == String.Empty)
            {
                throw new ArgumentNullException();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT HashTagText FROM HashTags ht
                                            INNER JOIN HashTagsToPosts htp
                                            ON ht.HashTagID=htp.HashTagID
                                            WHERE PostID=@postId AND HashTagText=@text";
                    command.Parameters.AddWithValue("@postId", postId);
                    command.Parameters.AddWithValue("@text", hashTag);

                    using (var reader = command.ExecuteReader())
                    {
                        result = reader.HasRows;
                    }
                }
            }

            return result;
        }
            



        /// <summary> 
        /// Добавление нового пользователя в базу.
        ///  </summary>
        /// <param name="user"></param>
        /// <returns>User</returns>
        public User AddNewUser(User user) //зачем возвращать юзера? Можно ли заносить юзеру в базу при создании?
        {
            _logger.Debug("Recieved a request to ADD new user");  //почему то приходится передавать About хотя он null

            _stopWatch = Stopwatch.StartNew();

            if (user == null)
            {
                throw new NullReferenceException();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    user.UserId = Guid.NewGuid();
                    user.RegistrationDate = DateTime.Now.Date;


                    command.CommandText = @"INSERT INTO Users
                                           (UserID, Nickname, RegistrationDate,Email,About)
                                            VALUES
                                           (@id, @name, @date, @email, @about)";

                    command.Parameters.AddWithValue("@id", user.UserId);
                    command.Parameters.AddWithValue("@name", user.Nickname);
                    command.Parameters.AddWithValue("@date", user.RegistrationDate);
                    command.Parameters.AddWithValue("@email", user.Email);
                    command.Parameters.AddWithValue("@about", user.About);

                    command.ExecuteNonQuery();
                }
            }

            _stopWatch.Stop();

            WriteOperationTimeToLog(_stopWatch);
            _logger.Info(string.Format("New user was added | ID={0} | Nickname={1}", user.UserId, user.Nickname));

            return user;
        }

        /// <summary> 
        /// Возвращает информацию о пользователе по указанному Id.
        ///  </summary>
        /// <param name="userid"></param>
        /// <returns>User</returns>
        public User GetUserById(Guid userId)
        {
            User userResult = null;

            _logger.Debug("Recieved a request to GET user with ID=" + userId);

            _stopWatch = Stopwatch.StartNew();

            if (userId == Guid.Empty)
            {
                throw new IncorrectIdError();
            }

            if (IsUserExists(userId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT * FROM Users
                                                WHERE UserID = @ID";
                        command.Parameters.AddWithValue("@ID", userId);

                        using (var reader = command.ExecuteReader())
                        {
                            reader.Read();

                            userResult = new User
                            {
                                UserId = reader.GetGuid(0),
                                Nickname = reader.GetString(1),
                                RegistrationDate = reader.GetDateTime(2),
                                Email = reader.GetString(3),
                                About = reader.GetString(4)
                            };
                        }

                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("Information about user sent | userID={0}", userId));

                return userResult;
            }
            else
            {
                throw new ErrorGettingUser();
            }
        }

        /// <summary>
        /// Удаление пользователя с указанным Id.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public void DeleteUser(Guid userId)
        {
            _logger.Debug("Recieved a request to DELETE user with ID=" + userId);

            _stopWatch = Stopwatch.StartNew();

            if (userId == Guid.Empty)
            {
                throw new IncorrectIdError();
            }

            if (IsUserExists(userId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"DELETE FROM Users
                                                WHERE UserID = @id";
                        command.Parameters.AddWithValue("@id", userId);

                        command.ExecuteNonQuery();
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("User was deleted | userID={0}", userId));
            }
            else
            {
                throw new ErrorGettingUser();
            }

        }

        /// <summary>
        /// Позволяет пользователю поставить Like посту.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postId"></param>
        public void PressAndRemoveLikeToPost(Guid userId, Guid postId)   //не слишком ли сложно?
        {
            _logger.Debug(string.Format("Recieved a request to LIKE post with ID={0}", postId));

            _stopWatch = Stopwatch.StartNew();

            if (userId == Guid.Empty || postId == Guid.Empty) //как упростить запись условий?
            {
                throw new IncorrectIdError();
            }

            if (!IsUserExists(userId) || !IsPostExists(postId))
            {
                throw new ErrorGettingUser();
            }

            if (IsUserPressedLike(userId, postId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"DELETE FROM Likes
                                                WHERE PostID=@postID AND UserID=@userID)";
                        command.Parameters.AddWithValue("@postID", postId);
                        command.Parameters.AddWithValue("@userID", userId);

                        command.ExecuteNonQuery();
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("Like was unmarked | postID={0} | userID={1}", postId, userId));
            }
            else
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"INSERT INTO Likes
                                                (PostID,UserID)
                                                VALUES
                                                (@postID,@userID)";
                        command.Parameters.AddWithValue("@postID", postId);
                        command.Parameters.AddWithValue("@userID", userId);

                        command.ExecuteNonQuery();
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("Like was marked | postID={0} | userID={1}", postId, userId));
            }
        }

        /// <summary>
        /// Редактирование информации пользователя.
        /// </summary>
        /// <param name="userId"></param>
        public void EditUserInformation(User user)
        {
            _logger.Debug("Recieved a request to EDIT user information with ID=" + user.UserId);

            _stopWatch = Stopwatch.StartNew();

            if (IsUserExists(user.UserId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"UPDATE Users
                                               SET 
                                               Nickname=@name,Email=@email,About=@about
                                               WHERE UserID=@userid";

                        command.Parameters.AddWithValue("@name", user.Nickname);
                        command.Parameters.AddWithValue("@email", user.Email);
                        command.Parameters.AddWithValue("@about", user.About);
                        command.Parameters.AddWithValue("@userid", user.UserId);

                        command.ExecuteNonQuery();
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("User information was updated | userID={0}", user.UserId));
            }
            else
            {
                throw new ErrorGettingUser();
            }
        }

        /// <summary>
        /// Подписка пользователя на обновления другого пользователя.
        /// </summary>
        /// <param name="userId"></param>
        public void SubscribeToUser(Guid subscriberId, Guid publisherId)
        {
            _logger.Debug(string.Format("Recieved a request to SUBSCRIBE to user with ID={0}", publisherId));

            _stopWatch = Stopwatch.StartNew();

            if (subscriberId == Guid.Empty || publisherId == Guid.Empty) //как упростить запись условий?
            {
                throw new IncorrectIdError();
            }

            if (!IsUserExists(publisherId) || !IsUserExists(subscriberId))
            {
                throw new ErrorGettingUser();
            }

            if (!IsSubscriber(publisherId, subscriberId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"INSERT INTO Subscribers
                                            (PublisherID,SubscriberID)
                                            VALUES
                                            (@pubID,@subID)";
                        command.Parameters.AddWithValue("@pubID", publisherId);
                        command.Parameters.AddWithValue("@subID", subscriberId);

                        command.ExecuteNonQuery();
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("User subscribe | subscriberIdID={0} | publisherId={1}", subscriberId, publisherId));

            }
            else
            {
                throw new ErrorTryingToSubscribe();
            }
        }

        /// <summary>
        /// Отписка пользователя от обновлений другого пользователя.
        /// </summary>
        /// <param name="subscriberId"></param>
        /// <param name="publisherId"></param>
        public void UnSubscribeFromUser(Guid subscriberId, Guid publisherId)
        {
            _logger.Debug(string.Format("Recieved a request to UNSUBSCRIBE from user with ID={0}", publisherId));

            _stopWatch = Stopwatch.StartNew();

            if (subscriberId == Guid.Empty || publisherId == Guid.Empty) //как упростить запись условий?
            {
                throw new IncorrectIdError();
            }

            if (!IsUserExists(publisherId) || !IsUserExists(subscriberId))
            {
                throw new ErrorGettingUser();
            }

            if (IsSubscriber(publisherId, subscriberId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"DELETE FROM Subscribers
                                            WHERE PublisherID=@pubID AND SubscriberID=@subID";
                        command.Parameters.AddWithValue("@pubID", publisherId);
                        command.Parameters.AddWithValue("@subID", subscriberId);

                        command.ExecuteNonQuery();
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("User unsubscribe | subscriberIdID={0} | publisherId={1}", subscriberId, publisherId));
            }
            else
            {
                throw new ErrorTryingToUnsubscribe();
            }
        }

        /// <summary>
        /// Возвращает список всех подписчиков.
        /// </summary>
        /// <param name="publisherId"></param>
        /// <returns></returns>
        public IEnumerable<User> GetAllSubscribers(Guid publisherId)
        {
            List<User> subscribers = null;

            _logger.Debug("Recieved a request to GET SUBSCRIBERS user with ID=" + publisherId);

            _stopWatch = Stopwatch.StartNew();

            if (publisherId == Guid.Empty)
            {
                throw new IncorrectIdError();
            }

            if (IsUserExists(publisherId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT UserID,Nickname,RegistrationDate,Email,About FROM Users 
                                                INNER JOIN Subscribers 
                                                ON SubscriberID = UserID
                                                WHERE PublisherID=@id";

                        command.Parameters.AddWithValue("@id", publisherId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                subscribers.Add(new User()
                                {
                                    UserId = reader.GetGuid(0),
                                    Nickname = reader.GetString(1),
                                    RegistrationDate = reader.GetDateTime(2),
                                    Email = reader.GetString(3),
                                    About = reader.GetString(4)
                                });
                            }
                        }
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("User subscribers sent | userID={0}", publisherId));

                return subscribers;
            }
            else
            {
                throw new ErrorGettingUser();
            }
        }


        /// <summary>
        /// Создание нового поста.
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public Post AddNewPost(Post post) //что желать с хештегами?
        {
            _logger.Debug("Recieved a request to ADD new post");

            _stopWatch = Stopwatch.StartNew();

            if (post == null)
            {
                throw new NullReferenceException();
            }

            if (post.Photo.Length > MaxPhotoSize.Length)
            {
                throw new ErrorUploadingPhoto();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    post.PostId = Guid.NewGuid();
                    post.DateOfPublication = DateTime.Now;

                    command.CommandText = @"INSERT INTO Posts
                                            (PostID, UserID, Photo, DateOfPublication)
                                            VALUES
                                            (@postid, @userid, @photo, @date)";

                    command.Parameters.AddWithValue("@postid", post.PostId);
                    command.Parameters.AddWithValue("@userid", post.UserID);
                    command.Parameters.AddWithValue("@photo", post.Photo);
                    command.Parameters.AddWithValue("@date", post.DateOfPublication);

                    command.ExecuteNonQuery();
                }
            }

            _stopWatch.Stop();

            WriteOperationTimeToLog(_stopWatch);
            _logger.Info(string.Format("Post was added | postID={0} | userID = {1}", post.PostId, post.UserID));

            return post;
        }

        /// <summary>
        /// Получение поста по указанному Id.
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public Post GetPostById(Guid postId)
        {
            Post postResult = null;

            _logger.Debug("Recieved a request to GET post with ID=" + postId);

            _stopWatch = Stopwatch.StartNew();

            if (postId == Guid.Empty)
            {
                throw new IncorrectIdError();
            }

            if (IsPostExists(postId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT * FROM Posts
                                                WHERE PostID = @id";

                        command.Parameters.AddWithValue("@id", postId);

                        using (var reader = command.ExecuteReader())
                        {
                            reader.Read();

                            postResult = new Post()
                            {
                                PostId = reader.GetGuid(0),
                                UserID = reader.GetGuid(1),
                                Photo = (byte[])reader.GetValue(2),                    //как вытащить массив байт?
                                DateOfPublication = reader.GetDateTime(3)
                            };
                        }
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("Information about post sent | postID={0}", postId));

                return postResult;
            }
            else
            {
                throw new ErrorGettingPost();
            }
        }

        /// <summary>
        /// Удаление пользователя по указаннному Id.
        /// </summary>
        /// <param name="postId"></param>
        public void DeletePost(Guid postId)
        {
            _logger.Debug("Recieved a request to DELETE post with ID=" + postId);

            _stopWatch = Stopwatch.StartNew();

            if (postId == Guid.Empty)
            {
                throw new NullReferenceException();
            }

            if (IsPostExists(postId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"DELETE FROM Posts
                                                WHERE PostID = @id";

                        command.Parameters.AddWithValue("@id", postId);

                        command.ExecuteNonQuery();
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("Post was deleted | postID={0}", postId));
            }
            else
            {
                throw new ErrorGettingPost();
            }

        }

        /// <summary>
        /// Получить все комментарии к фотографии.
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="commentsNumber"></param>
        /// <returns></returns>
        public IEnumerable<Comment> GetPostComments(Guid postId, int commentsNumber = 10)  //можно ли в интерфейсе делать опциональный параметр?
        {
            List<Comment> comments = null;

            _logger.Debug("Recieved a request to GET COMMENTS from post with ID=" + postId);

            _stopWatch = Stopwatch.StartNew();

            if (postId == Guid.Empty)
            {
                throw new NullReferenceException();
            }

            if (IsPostExists(postId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT * FROM Comments
                                                WHERE PostID = @id";

                        command.Parameters.AddWithValue("@id", postId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                comments.Add(new Comment()
                                {
                                    CommentId = reader.GetGuid(0),
                                    PostID = reader.GetGuid(1),
                                    UserID = reader.GetGuid(2),
                                    DateOfPublication = reader.GetDateTime(3),
                                    Text = reader.GetString(4)
                                });
                            }
                        }
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("Post comments sent | postID={0}", postId));

                return comments;
            }
            else
            {
                throw new ErrorGettingPost();
            }
        }

        /// <summary>
        /// Получить все хэштеги к посту.
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetPostHashTags(Guid postId) //что здесь делать с возвращаемым Null?
        {
            List<string> hashtags = null;

            _logger.Debug("Recieved a request to GET HASHTAGS from post with ID=" + postId);

            _stopWatch = Stopwatch.StartNew();

            if (postId == Guid.Empty)
            {
                throw new NullReferenceException();
            }

            if (IsPostExists(postId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT HashTagText FROM HashTags ht
                                                INNER JOIN HashTagsToPosts hp
                                                ON ht.HashTagID = hp.HashTagID
                                                WHERE PostID=@id";

                        command.Parameters.AddWithValue("@id", postId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                hashtags.Add(reader.GetString(0));
                            }
                        }
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("Post hashtags sent | postID={0}", postId));

                return hashtags;
            }
            else
            {
                throw new ErrorGettingPost();
            }
        }

        /// <summary>
        /// Добавить HashTag к посту.
        /// </summary>
        /// <param name="postId"></param>
        public void AddHashTagToPost(Guid postId, string hashTagText) //или hashtag сделать как сласс?
        {
            _logger.Debug("Recieved a request to ADD new hashtag to post");

            _stopWatch = Stopwatch.StartNew();

            if (hashTagText == null)
            {
                throw new NullReferenceException();
            }
            if (postId == Guid.Empty)
            {
                throw new IncorrectIdError();
            }

            var hashTagId = Guid.NewGuid();

            if (IsPostExists(postId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"INSERT INTO HashTags
                                               (HashTagID,HashTagText)
                                               VALUES
                                               (@id, @hashtagtext)";

                        command.Parameters.AddWithValue("@id", hashTagId);
                        command.Parameters.AddWithValue("@hashtagtext", hashTagText);

                        command.ExecuteNonQuery();
                    }

                    using (var newcommand = connection.CreateCommand())
                    {
                        newcommand.CommandText = @"INSERT INTO HashTagsToPosts
                                                   (HashTagID,PostID)
                                                   VALUES
                                                   (@id,@postid)";

                        newcommand.Parameters.AddWithValue("@id", hashTagId);
                        newcommand.Parameters.AddWithValue("@postid", postId);

                        newcommand.ExecuteNonQuery();
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("Hashtag was added | postID={0} | hashtagText={1}", postId, hashTagText));
            }
            else
            {
                throw new ErrorGettingPost();
            }
        }

        /// <summary>
        /// Добавить новый комментарий к посту.
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public Comment AddNewComment(Comment comment)
        {
            _logger.Debug("Recieved a request to ADD new comment");

            _stopWatch = Stopwatch.StartNew();

            if (comment == null)
            {
                throw new NullReferenceException();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {

                    comment.CommentId = Guid.NewGuid();
                    comment.DateOfPublication = DateTime.Now;

                    command.CommandText = @"INSERT INTO Comments
                                          (CommentID, PostID, UserID, DateOfPublication, Comment)
                                          VALUES
                                          (@commentid, @postid, @userid, @date, @comment)";

                    command.Parameters.AddWithValue("@commentid", comment.CommentId);
                    command.Parameters.AddWithValue("@postid", comment.PostID);
                    command.Parameters.AddWithValue("@userid", comment.UserID);
                    command.Parameters.AddWithValue("@date", comment.DateOfPublication);
                    command.Parameters.AddWithValue("@comment", comment.Text);

                    command.ExecuteNonQuery();
                }
            }

            _stopWatch.Stop();

            WriteOperationTimeToLog(_stopWatch);
            _logger.Info(string.Format("New comment was added | commentID={0} | postID={1} | userID={2}", comment.CommentId, comment.PostID, comment.UserID));

            return comment;
        }

        /// <summary>
        /// Удалить комментарий к посту.
        /// </summary>
        /// <param name="commentID"></param>
        public void DeleteComment(Guid commentId)
        {
            _logger.Debug("Recieved a request to DELETE comment with ID=" + commentId);

            _stopWatch = Stopwatch.StartNew();

            if (commentId == Guid.Empty)
            {
                throw new IncorrectIdError();
            }

            if (IsCommentExists(commentId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"DELETE FROM Comments
                                                WHERE CommentID = @id";

                        command.Parameters.AddWithValue("@id", commentId);

                        command.ExecuteNonQuery();
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("Comment was deleted | commentID={0}", commentId));
            }
            else
            {
                throw new ErrorGettingComment();
            }
        }

        /// <summary>
        /// Получить комментарий по указанному Id.
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        public Comment GetCommentById(Guid commentId)
        {
            Comment commentResult = null;

            _logger.Debug("Recieved a request to GET comment with ID=" + commentId);

            _stopWatch = Stopwatch.StartNew();

            if (commentId == Guid.Empty)
            {
                throw new IncorrectIdError();
            }

            if (IsCommentExists(commentId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT * FROM Comments
                                                WHERE CommentID = @id";

                        command.Parameters.AddWithValue("@id", commentId);

                        using (var reader = command.ExecuteReader())
                        {
                            reader.Read();

                            commentResult = new Comment()
                            {
                                CommentId = reader.GetGuid(0),
                                PostID = reader.GetGuid(1),
                                UserID = reader.GetGuid(2),
                                DateOfPublication = reader.GetDateTime(3),
                                Text = reader.GetString(4)
                            };
                        }
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("Information about comment sent | commentID={0}", commentId));

                return commentResult;
            }
            else
            {
                throw new ErrorGettingComment();
            }
        }

        /// <summary>
        /// Редактирование комментария.
        /// </summary>
        /// <param name="commentId"></param>
        public void EditComment(Comment comment)
        {
            _logger.Debug("Recieved a request to EDIT comment with ID=" + comment.CommentId);

            _stopWatch = Stopwatch.StartNew();

            if (IsCommentExists(comment.CommentId))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"UPDATE Comments
                                               SET 
                                               Comment=@comment
                                               WHERE CommentID=@commentid";

                        command.Parameters.AddWithValue("@commentid", comment.CommentId);
                        command.Parameters.AddWithValue("@comment", comment.Text);

                        command.ExecuteNonQuery();
                    }
                }

                _stopWatch.Stop();

                WriteOperationTimeToLog(_stopWatch);
                _logger.Info(string.Format("Comment was updated | commentID={0}", comment.CommentId));
            }
            else
            {
                throw new ErrorGettingComment();
            }
        }
    }
}
