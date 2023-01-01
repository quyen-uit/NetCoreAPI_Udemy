using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public AppUser Author { get; set; }

        public Guid ActivityId { get; set; }
        public Activity Activity { get; set; }

    }
}
