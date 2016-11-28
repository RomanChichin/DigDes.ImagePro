using System;


namespace ImagePro.Exeptions
{
   public class ErrorTryingToUnsubscribe : Exception
    {
        private string _message = "Попытка отписаться от пользователя, на которого нет подписки.";

        public override string Message
        {
            get { return _message; }
        }
    }
}
