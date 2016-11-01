using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagePro.Model
{
   public class User
    {
        public Guid UserId { get; set; }
        public string Nickname { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
