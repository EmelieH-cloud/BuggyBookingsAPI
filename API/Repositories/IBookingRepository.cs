using BookingsApi.Models;

namespace BookingsApi.Repositories
{
    public interface IBookingRepository
    {
        IEnumerable<Booking> GetAll();
        Booking? GetById(int id);
        Booking Add(Booking booking);
        void Delete(int id);
    }

}
