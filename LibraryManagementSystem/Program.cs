using LibraryManagementSystem;
using LibraryManagementSystem.Middleware;
using LibraryManagementSystem.ModelBinders;
using LMS_BusinessLogic;
using LMS_BusinessLogic.Interfaces;
using LMS_BusinessLogic.Services;
using LMS_DataAccess.Data;
using LMS_DataAccess.Interfaces;
using LMS_DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    //options.ModelBinderProviders.Insert(0, new BookModelBinderProvider()); //This was a bad idea. This binder blocks JSON reading in methods that work with Book model.
}).AddNewtonsoftJson();

//Connection to the database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});

//Automapper
builder.Services.AddAutoMapper(typeof(BusinessLayerMapper), typeof(PresentationLayerMapper));

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookCollectionService, BookCollectionService>();
builder.Services.AddScoped<IBookCollectionRepository, BookCollectionRepository>();

//Logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.File(builder.Configuration["Files:LoggingFile"], rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

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
