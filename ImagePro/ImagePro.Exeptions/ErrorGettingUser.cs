using System;

namespace ImagePro.Exeptions
{
    public class ErrorGettingUser : Exception
    {
        private string _message = "Запрашиваемого пользователя не существует.";

        public override string Message
        {
            get { return _message; }
        }
    }
}
