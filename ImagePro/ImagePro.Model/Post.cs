using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagePro.Model
{
   public class Post
    {
        public Guid PostId { get; set; }
        public Guid UserID { get; set; }
        public byte[] Photo { get; set; }
        public DateTime DateOfPublication { get; set; }
        public string[] Hashtags { get; set; }
    }
}
