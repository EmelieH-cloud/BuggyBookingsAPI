using BookingsApi.Models;
using BookingsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly BookingService _service;

        public BookingsController(BookingService service)
        {
            _service = service;
        }


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

        [HttpDelete("{id:int}")]
        public IActionResult Cancel(int id)
        {
            var existing = _service.GetById(id);

            if (existing == null)
            {
                return NotFound();
            }

            _service.Cancel(id);
            return NoContent();
        }

    }
}
