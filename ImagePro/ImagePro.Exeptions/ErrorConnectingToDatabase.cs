using System;


namespace ImagePro.Exeptions
{
    public class ErrorConnectingToDatabase : Exception
    {
        private string _message = "Ошибка подключения к базе данных.";

        public override string Message
        {
            get { return _message; }
        }
    }
}
