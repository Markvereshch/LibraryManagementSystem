using LibraryManagementSystem.Configuration;
using LibraryManagementSystem.Middleware;
using LibraryManagementSystem.ServiceRegistration;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    //options.ModelBinderProviders.Insert(0, new BookModelBinderProvider()); //This was a bad idea. This binder blocks JSON reading in methods that work with Book model.
}).AddNewtonsoftJson();

// Adding services
builder.Services.AddAndValidateOptions();
builder.Services.AddFluentValidators();

var dbOptions = new ConnectionStrings();
builder.Configuration.Bind("ConnectionStrings", dbOptions);
IOptions<ConnectionStrings> connectionStrings = Options.Create(dbOptions);

var filesOptions = new Files();
builder.Configuration.Bind("Files", filesOptions);
IOptions<Files> files = Options.Create(filesOptions);

builder.Services.AddDbContext(connectionStrings.Value.DefaultSQLConnection);
builder.Services.AddDistributedSQLCache(connectionStrings.Value.DefaultSQLConnection);
builder.Services.AddAutomapper();
builder.Services.AddRepositories();

// Logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.File(files.Value.LoggingFile, rollingInterval: RollingInterval.Day)
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
