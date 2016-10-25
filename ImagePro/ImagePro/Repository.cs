using System;
using System.Data.SqlClient;
using ImagePro.Model;
using System.IO;

namespace ImagePro.Data.SQL
{
    public class Repository : IRepository
    {

        public Post GetPost(Guid postId)
        {
            throw new NotImplementedException();
        }

        private readonly string _connectionString;

        public string Connection   // свойства до или после конструкторов?
        {
            get
            {
                return _connectionString;
            }
        }

        public Repository(string newconnection)
        {
            if (newconnection == string.Empty)   // or write  == null?
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


        #region-----------------------------AddUser----------------------------------
        /// <summary>
        /// Добавление User в базу
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User AddUser(User user)    //зачем возвращать юзера? Можно ли заносить юзеру в базу при создании?
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

         //доделать, чтобы была фотография
        #region----------------------ADDPOST-------------------------------------
        public Post AddPost(Post post)  
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    post.PostId = Guid.NewGuid();

                    command.CommandText = @"INSERT INTO Users
                                          (UserID, Nickname, RegistrationDate)
                                          VALUES
                                          (@id, @name, @date)";
                    throw new NotImplementedException();
                }
            }
        }
        #endregion--------------------------------------------------------------------

        public Comment AddComment(Comment comment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {

                    throw new NotImplementedException();
                }

            }
            
        }

        public bool DeletePost(Guid postId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"DELETE FROM Posts
                                            WHERE PostID = @id";

                    command.Parameters.AddWithValue("@id", postId);

                    try
                    {
                        command.ExecuteNonQuery();
                        return true;
                    }
                    catch(Exception e)
                    {
                       //something
                        return false;
                    }
                }
            }
        }





        static void Main()
        {


        }
    }
}
