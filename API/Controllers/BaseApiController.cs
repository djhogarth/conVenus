using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    //This the superclass all other controllers and is used to minimize repeated code
    public class BaseApiController: ControllerBase 
    {
        
    }
}