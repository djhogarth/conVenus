namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(UpdateUserActivity))]

    //This the superclass all other controllers and is used to minimize repeated code
    public class BaseApiController: ControllerBase
    {

    }
}
