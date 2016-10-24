using System;
using System.Collections.Generic;
using System.Linq;

namespace ImagePro.Model
{
   public class Post   //Надо констркутор ?
    {
        public Guid PostId { get; set; }
        public Guid UserID { get; set; }
        public byte[] Photo { get; set; }
        public DateTime DateOfPublication { get; set; }
        public string[] Hashtags { get; set; }
    }
}
