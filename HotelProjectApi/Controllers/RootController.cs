using HotelProjectApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace HotelProjectApi.Controllers
{
    [Route("/")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = nameof(GetRoot))]
        [ProducesResponseType(200)]
        public IActionResult GetRoot()
        {
            var response = new RootResponse
            {
                Self = Link.To(nameof(GetRoot)),
                Rooms = Link.To(nameof(RoomsController.GetRooms)),
                Info = Link.To(nameof(InfoController.GetInfo)),
            };

            return Ok(response);
        }
    }
}
