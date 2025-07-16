using BookingsApi.Models;

namespace BookingsApi.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private static readonly List<Booking> _bookings = new();
        private static int _nextId = 1;

        public IEnumerable<Booking> GetAll() => _bookings;

        public Booking? GetById(int id) => _bookings.FirstOrDefault(b => b.Id == id);

        public Booking Add(Booking booking)
        {
            booking.Id = _nextId++;
            _bookings.Add(booking);
            return booking;
        }

        public void Delete(int id)
        {
            var booking = GetById(id);
            if (booking != null)
                _bookings.Remove(booking);
        }
    }
}
