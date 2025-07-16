using BookingsApi.Models;
using BookingsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly BookingService _service = new BookingService();

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var booking = _service.GetById(id);
            return booking == null ? NotFound() : Ok(booking);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Booking booking)
        {
            if (_service.HasOverlap(booking.RoomId, booking.From, booking.To))
            {
                return Conflict("Booking overlaps an existing reservation.");
            }

            var created = _service.Create(booking);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
    }
}
