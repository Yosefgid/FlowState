using FlowState;
using FlowState.Services;
using FlowState.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IToDoTaskRepo, ToDoTaskRepo>();
builder.Services.AddScoped<IGoogleService, GoogleService>();
builder.Services.AddScoped<IGoogleCalendarClient, GoogleCalendarClient>();
builder.Services.AddControllers();
builder.Services.AddScoped<IToDoTaskService, ToDoTaskService>();
builder.Services.AddScoped<IToDoTaskRepo, ToDoTaskRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IAuthServices, AuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MyDbContext>(options =>
   options.UseInMemoryDatabase("TestDb"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
