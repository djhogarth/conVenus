using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
      //_next manages what comes next in the middleware pipeline
      private readonly RequestDelegate _next;

      //logger is used to log exceptions into the terminal
      private readonly ILogger<ExceptionMiddleware> _logger;

      /* env is used to check what environment we are running in
      (production or development) */
      private readonly IHostEnvironment _env;

      public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env )
          {
        _next = next;
        _logger = logger;
        _env = env;
      }

      public async Task InvokeAsync (HttpContext context){
        //pass the context to the next piece of middleware
        try{
          await _next(context);
        }
        catch(Exception ex)
        {
          //log exception to the terminal
          _logger.LogError(ex, ex.Message);
          context.Response.ContentType = "application/json";
          context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

          /*Check to see what environemnt we are running in.
            Based on the environment, we return
            ApiExceptions with more or less details*/
          var response = _env.IsDevelopment()
            ? new ApiExceptions(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
            : new ApiExceptions(context.Response.StatusCode,"Internal Server Error");

          //we will send back the response in JSON format with CameCase formatting
          var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

          var json = JsonSerializer.Serialize(response, options);

          await context.Response.WriteAsync(json);
        }

      }




    }
}
