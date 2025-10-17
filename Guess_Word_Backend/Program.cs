
using Guess_Word_Backend.Data;
using Guess_Word_Backend.Hubs;
using Guess_Word_Backend.Hubs.HubServices;
using Guess_Word_Backend.Hubs.Repositories;
using Guess_Word_Backend.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddSingleton<HubData>();
builder.Services.AddSingleton<OnlinePlayersService>();
builder.Services.AddSingleton<RoomsService>();
builder.Services.AddSingleton<HubData>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Arabic Guess Word API", Version = "v1" });
});

// DB: use SQLite for simplicity; swap to SQL Server / Postgres for prod
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=wordle.db"));

// SignalR
builder.Services.AddSignalR().AddJsonProtocol();

// DI: repositories and services
//builder.Services.AddScoped<IGameRoomRepository, GameRoomRepository>();
//builder.Services.AddScoped<IGameService, GameService>();

// CORS (allow your client origin)
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
        //.AllowCredentials().SetIsOriginAllowed(_ => true)
        ));

var app = builder.Build();

// Apply EF migrations at startup (simple approach)
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    db.Database.Migrate();
//}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.MapControllers();
app.MapHub<GameHub>("/hubs/game");

app.Run();
