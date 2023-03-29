using AdventureWorksApi.Models;
using AdventureWorksApi.Functions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;

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
app.MapGet("/Customer/Details/{CustomerId}", AddressFunctions.CustomerDetails);
app.MapGet("/Address/Details/{AddressId}", AddressFunctions.AddressDetails);


app.MapGet("/Product/Read", ProductFunctions.ReadProduct);
app.MapDelete("/Product/Delete", ProductFunctions.DeleteProduct);
app.MapPut("/Product/Update", ProductFunctions.UpdateProduct);
app.MapPost("/Product/Create", ProductFunctions.CreateProduct);

app.MapGet("/Order/Read", OrderFunctions.ReadOrder);
app.MapDelete("/Order/Delete", OrderFunctions.DeleteOrder);
app.MapPut("/Order/Update", OrderFunctions.UpdateOrder);
app.MapPost("/Order/Create", OrderFunctions.CreateOrder);






app.Run();
