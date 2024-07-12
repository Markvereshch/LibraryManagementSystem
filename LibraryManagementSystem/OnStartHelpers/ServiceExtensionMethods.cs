using FluentValidation;
using FluentValidation.AspNetCore;
using LibraryManagementSystem.Configuration;
using LibraryManagementSystem.FluentValidators;
using LMS_BusinessLogic.Interfaces;
using LMS_BusinessLogic.Services;
using LMS_BusinessLogic;
using LMS_DataAccess.Data;
using LMS_DataAccess.Interfaces;
using LMS_DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using LMS_DataAccess.Entities;

namespace LibraryManagementSystem.ServiceRegistration
{
    public static class ServiceExtensionMethods
    {
        public static void AddAndValidateOptions(this IServiceCollection services)
        {
            services
                .AddOptions<Files>()
                .BindConfiguration("Files")
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services
                .AddOptions<ConnectionStrings>()
                .BindConfiguration("ConnectionStrings")
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
        public static void AddFluentValidators(this IServiceCollection services)
        {
            services
                .AddValidatorsFromAssemblyContaining<BookDTOValidator>()
                .AddValidatorsFromAssemblyContaining<BookOperationsDTOValidator>()
                .AddValidatorsFromAssemblyContaining<BookCollectionDTOValidator>()
                .AddValidatorsFromAssemblyContaining<BookCollectionOperationsDTOValidator>();
            services
                .AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
        }
        public static void AddDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        }
        public static void AddDistributedSQLCache(this IServiceCollection services, string connectionString)
        {
            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = connectionString;
                options.SchemaName = "dbo";
                options.TableName = "LMSCache";
            });
            services.AddScoped<ICaching<Book>, BookCachingRepository>();
            services.AddScoped<ICaching<BookCollection>, BookCollectionCachingRepository>();
        }
        public static void AddAutomapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(BusinessLayerMapper), typeof(PresentationLayerMapper));
        }
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IBookCollectionService, BookCollectionService>();
            services.AddScoped<IBookCollectionRepository, BookCollectionRepository>();
        }
    }
}
