using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GiveAndReceive.Models
{
    public class Post
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int Love { get; set; }
        public int Comment { get; set; }
        public long CreateTime { get; set; }
    }
}