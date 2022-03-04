using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Errors
{
    //The object we return for any exception we get
    public class ApiExceptions
    {
        public ApiExceptions(int statusCode, string message = null, string details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
        public int StatusCode { get; set; }

        public string Message { get; set; }

        // basically the stack trace
        public string Details { get; set; }
    }
}
