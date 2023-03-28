using AdventureWorksApi.Models;
using AdventureWorksApi.Functions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AdventureWorksLt2019Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AdventureWorksLt2019Context"))
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/Address/Read", AddressFunctions.ReadAddress);
app.MapDelete("/Address/Delete", AddressFunctions.DeleteAddress);
app.MapPut("/Address/Update", AddressFunctions.UpdateAddress);
app.MapPost("/Address/Create", AddressFunctions.CreateAddress);

app.Run();
