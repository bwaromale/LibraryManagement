using AutoMapper;
using LibraryManagement;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.Repository.Implementation;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<LibraryContext>(
    option
        =>
    {
        option.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultSQLConnection")
            );
    });
builder.Services.AddScoped <IRepository<Book>, Repository<Book>>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();


builder.Services.AddAutoMapper(typeof(MappingConfig));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
