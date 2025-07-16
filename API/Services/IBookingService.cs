using BookingsApi.Models;

namespace BookingsApi.Services
{
    public interface IBookingService
    {
        IEnumerable<Booking> GetAll();
        Booking? GetById(int id);
        Booking Create(Booking booking);
        void Cancel(int id);
        bool HasOverlap(int roomId, DateTime from, DateTime to);
    }

}
