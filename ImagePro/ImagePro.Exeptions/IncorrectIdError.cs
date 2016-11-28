using System;

namespace ImagePro.Exeptions
{
    public class IncorrectIdError : Exception
    {
        private string _message = "Попытка выполнения операции по некорректному Id объекта.";

        public override string Message
        {
            get { return _message; }
        }
    }
}
