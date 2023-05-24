using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Models
{
    public class Comment
    {
        public string CommentId { get; set; }
        public string CommentContent { get; set; }
        public string UserId { get; set; }
        public string PostId { get; set; }
        public long CreateTime { get; set; }
    }
}