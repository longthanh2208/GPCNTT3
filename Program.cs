using AssetManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký DbContext với connection string từ appsettings.json
builder.Services.AddDbContext<Session3Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cho phép Android emulator (10.0.2.2) gọi HTTP
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cho phép Kestrel lắng nghe trên mọi card mạng (0.0.0.0) để Android Emulator (10.0.2.2) truy cập được
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5285);
});

var app = builder.Build();

// Bật Swagger ở mọi môi trường (tiện test)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
