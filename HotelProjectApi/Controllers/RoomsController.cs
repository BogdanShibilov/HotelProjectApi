using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelProjectApi.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        [HttpGet(Name = nameof(GettRooms))]
        public IActionResult GettRooms()
        {
            throw new NotImplementedException();
        }
    }
}
