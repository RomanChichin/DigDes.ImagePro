using System;

namespace ImagePro.Exeptions
{
    public class ErrorTryingToSubscribe : Exception
    {
        private string _message = "Попытка повторной подписки на пользователя.";

        public override string Message
        {
            get { return _message; }
        }
    }
}
