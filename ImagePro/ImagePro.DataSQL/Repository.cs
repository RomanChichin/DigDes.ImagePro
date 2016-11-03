using System;
using System.Collections.Generic;
using ImagePro.Model;
using System.Data.SqlClient;

namespace ImagePro.DataSQL
{
    public class Repository : IRepository
    {
        private readonly string _connectionString;

        public string Connection // свойства до или после конструкторов?
        {
            get { return _connectionString; }
        }

        public Repository(string newconnection)
        {
            if (newconnection == string.Empty) // or write  == null?
            {
                throw new Exception("Empty connection.");
            }

            //Проверка, получится ли вообще подключиться к базе
            try
            {
                var connection = new SqlConnection(newconnection);
                connection.Open();
                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            _connectionString = newconnection; //проверка входных данных(сделать попытку подключения и try catch?
        }


        #region UserMethods

        #region-----------------------------AddUser----------------------------------

        /// <summary>
        /// Добавление User в базу
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User AddUser(User user) //зачем возвращать юзера? Можно ли заносить юзеру в базу при создании?
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {

                    user.UserId = Guid.NewGuid();

                    command.CommandText = @"INSERT INTO Users
                                          (UserID, Nickname, RegistrationDate)
                                          VALUES
                                          (@id, @name, @date)";

                    command.Parameters.AddWithValue("@id", user.UserId);
                    command.Parameters.AddWithValue("@name", user.Nickname);
                    command.Parameters.AddWithValue("@date", DateTime.Now.Date);

                    command.ExecuteNonQuery();

                    return user;
                }
            }
        }

        #endregion-------------------------------------------------------------------

        #region --------------------------------GetUser-------------------------

        /// <summary>
        /// Возвращает User по указанному Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetUser(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT * FROM Users
                                            WHERE UserID = @ID";
                    command.Parameters.AddWithValue("@ID", id);

                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();

                        return new User
                        {
                            UserId = reader.GetGuid(0),
                            Nickname = reader.GetString(1),
                            RegistrationDate = reader.GetDateTime(2)
                        };
                    }

                }
            }
        }

        #endregion

        #region -----------------------------------DeleteUser-------------------------------------

        /// <summary>
        /// Удаление пользователя по ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public void DeleteUser(Guid userID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {

                    command.CommandText = @"DELETE FROM Users
                                            WHERE UserID = @id";
                    command.Parameters.AddWithValue("@id", userID);

                    command.ExecuteNonQuery();
                }
            }
        }


        #endregion

        #endregion

        #region PostMethods

        #region----------------------AddPost-------------------------------------

        public Post AddPost(Post post)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    post.PostId = Guid.NewGuid();

                    command.CommandText = @"INSERT INTO Posts
                                          (PostID, UserID, Photo, DateOfPublication)
                                          VALUES
                                          (@postid, @userid, @photo,@date)";

                    command.Parameters.AddWithValue("@postid", post.PostId);
                    command.Parameters.AddWithValue("@userid", post.UserID);
                    // command.Parameters.AddWithValue("@photo",);//что  то подставить
                    command.Parameters.AddWithValue("@date", DateTime.Now);

                    command.ExecuteNonQuery();

                    return post;
                }
            }
        }

        #endregion--------------------------------------------------------------------

        #region ---------------GetPost--------------------------------

        public Post GetPost(Guid postId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region -----------------------------DeletePost---------------------------------

        public void DeletePost(Guid postId)
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
        }

        #endregion

        #endregion

        #region CommentMethods

        #region --------------------------AddComment----------------------------

        public Comment AddComment(Comment comment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {

                    comment.CommentId = Guid.NewGuid();

                    command.CommandText = @"INSERT INTO Comments
                                          (CommentID, PostID, UserID, DateOfPublication, Comment)
                                          VALUES
                                          (@commentid, @postid, @userid, @date, @comment)";

                    command.Parameters.AddWithValue("@commentid", comment.CommentId);
                    command.Parameters.AddWithValue("@postid", comment.PostID);
                    command.Parameters.AddWithValue("@userid", comment.UserID);
                    command.Parameters.AddWithValue("@date", DateTime.Now);
                    command.Parameters.AddWithValue("@comment", comment.Text);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch //Какое здесь может быть исключение?
                    {
                        Console.WriteLine("Что то не так");
                    }
                    return comment;
                }
            }
        }

        #endregion

        #region ----------------------------DeleteComment----------------------------------------

        public void DeleteComment(Guid commentID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region --------------------------------GetComment-------------------------

        public Comment GetComment(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT * FROM Comments
                                            WHERE CommentID = @ID";
                    command.Parameters.AddWithValue("@ID", id);

                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();

                        return new Comment()
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
        }

        #endregion

        #region --------------------------------GetAllComments-------------------------

        public IEnumerable<Comment> GetAllComments(Guid postID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                //                using (var command = connection.CreateCommand())
                //                {
                //                    command.CommandText = @"SELECT * FROM Comments
                //                                            WHERE CommentID = @ID";
                //                    command.Parameters.AddWithValue("@ID", id);

                //                    using (var reader = command.ExecuteReader())
                //                    {
                //                        reader.Read();

                //                        return new Comment()
                //                        {
                //                            CommentId = reader.GetGuid(0),
                //                            PostID = reader.GetGuid(1),
                //                            UserID = reader.GetGuid(2),
                //                            DateOfPublication = reader.GetDateTime(3),
                //                            Text = reader.GetString(4)
                //                        };
                //                    }
                //                }
                throw new NotImplementedException();

            }
        }

        #endregion

        #endregion

    }

}
