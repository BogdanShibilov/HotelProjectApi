using HotelProjectApi.Models;

namespace HotelProjectApi.Services
{
    public interface IRoomService
    {
        Task<Room> GetRoomAsync(Guid id);
    }
}
