using System;


namespace ImagePro.Exeptions
{
    public class ErrorCreatingUser : Exception
    {
        private string _message = "Пользователь с таким никнеймом уже существует.";

        public override string Message
        {
            get { return _message; }
        }
    }
}
