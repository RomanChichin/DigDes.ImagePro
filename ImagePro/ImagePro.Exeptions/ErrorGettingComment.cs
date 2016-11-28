using System;

namespace ImagePro.Exeptions
{
    public class ErrorGettingComment : Exception
    {
        private string _message = "Запрашиваемого комментария не существует.";

        public override string Message
        {
            get { return _message; }
        }
    }
}
