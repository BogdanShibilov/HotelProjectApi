using HotelProjectApi.Models;

namespace HotelProjectApi
{
    public static class SeedData
    {
        public static async Task InitializeAsync(HotelApiDbContext context)
        {
            await AddTestData(context);
        }

        public static async Task AddTestData(HotelApiDbContext context)
        {
            if (context.Rooms.Any())
            {
                // There is some data in db, so it is very likely not in memory
                return;
            }

            context.Rooms.Add(new RoomEntity
            {
                Id = Guid.Parse("301df04d-8679-4b1b-ab92-0a586ae53d08"),
                Name = "Lux room 420",
                Rate = 69420,
            });

            context.Rooms.Add(new RoomEntity
            {
                Id = Guid.Parse("ee2b83be-91db-4de5-8122-35a9e9195976"),
                Name = "Usual room",
                Rate = 10000,
            });

            await context.SaveChangesAsync();
        }
    }
}
