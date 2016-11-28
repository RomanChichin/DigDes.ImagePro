using System;

namespace ImagePro.Exeptions
{
   public class ErrorUploadingPhoto : Exception
    {
        private string _message = "Загружаемая фоторграфия имеет размер больше предельного.";
                                                                                            
        public override string Message
        {
            get { return _message; }
        }
    }
}
