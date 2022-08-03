using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Core
{
    public class AppException
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }

        public AppException(int status, string message, string details = null)
        {
            Status = status;
            Message = message;
            Details = details;
        }
    }
}
