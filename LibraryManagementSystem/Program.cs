using LibraryManagementSystem.Middleware;
using LibraryManagementSystem.ModelBinders;
using LMS_BusinessLogic.Interfaces;
using LMS_BusinessLogic.Services;
using LMS_DataAccess.Data;
using LMS_DataAccess.Interfaces;
using LMS_DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    //options.ModelBinderProviders.Insert(0, new BookModelBinderProvider()); //This was a bad idea. This binder blocks JSON reading in methods that work with Book model.
}).AddNewtonsoftJson();

//Layouts and DB
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookCollectionService, BookCollectionService>();
builder.Services.AddScoped<IBookCollectionRepository, BookCollectionRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Adding a custom middleware to handle exceptions
app.UseMiddleware<ExceptionHandlingMiddleware>();

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
