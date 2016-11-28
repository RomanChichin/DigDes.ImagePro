using System;

namespace ImagePro.Model
{
   public class User
    {
        public Guid UserId { get; set; }
        public string Nickname { get; set; }
        public DateTime RegistrationDate { get; set; }

        public string Email { get; set; }

        public string About { get; set; }
        
    }
}
