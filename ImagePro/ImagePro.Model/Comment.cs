using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagePro.Model
{
   public class Comment
    {
        public Guid CommentId { get; set; }

        public Guid PostID { get; set; }

        public Guid UserID { get; set; }

        public string Text { get; set; }
        public DateTime DateOfPublication { get; set; }
    }
}
