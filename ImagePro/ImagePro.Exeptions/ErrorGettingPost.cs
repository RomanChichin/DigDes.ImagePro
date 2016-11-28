using System;

namespace ImagePro.Exeptions
{
   public class ErrorGettingPost : Exception
    {
        private string _message = "Запрашиваемого поста не существует.";

        public override string Message
        {
            get { return _message; }
        }

    }
}
