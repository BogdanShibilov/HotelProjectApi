using HotelProjectApi.Filters;
using HotelProjectApi.Infrastructure;
using HotelProjectApi.Models;
using HotelProjectApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;

namespace HotelProjectApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<HotelInfo>(builder.Configuration.GetSection("Info"));

            builder.Services.AddScoped<IRoomService, DefaultRoomService>();

            builder.Services.AddDbContext<HotelApiDbContext>(options =>
            {
                options.UseInMemoryDatabase("hoteldb");
            });

            builder.Services.AddMvc(options =>
            {
                options.Filters.Add<JsonExceptionFilter>();
                options.Filters.Add<RequireHttpsOrCloseAttribute>();
                options.Filters.Add<LinkRewritingFilter>();
            });

            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAny",
                    policy => policy.AllowAnyOrigin());
            });

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new MediaTypeApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            });

            builder.Services.AddAutoMapper(options =>
            {
                options.AddProfile<MappingProfile>();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAny");

            app.UseAuthorization();


            app.MapControllers();

            InitializeDatabase(app);

            app.Run();
        }

        public static void InitializeDatabase(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var dbContext = services.GetRequiredService<HotelApiDbContext>();
                SeedData.InitializeAsync(dbContext).Wait();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during seeding database.");
            }
        }
    }
}