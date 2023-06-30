using HotelProjectApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelProjectApi
{
    public class HotelApiDbContext : DbContext
    {
        public HotelApiDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<RoomEntity> Rooms { get; set; }
    }
}
