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
using LMS_BusinessLogic.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

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
            services
                .AddOptions<CachingOptions>()
                .BindConfiguration("CachingOptions")
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
        public static void AddDistributedSQLCache(this IServiceCollection services, string connectionString, IOptions<CachingOptions> caching)
        {
            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = connectionString;
                options.SchemaName = "dbo";
                options.TableName = "LMSCache";
            });
            services.AddScoped<ICaching<BookCollection>>(provider =>
            {
                var cache = provider.GetRequiredService<IDistributedCache>();
                var cachePrefix = "BookCollection_";
                return new CachingRepository<BookCollection>(cache, cachePrefix, caching.Value.CollectionTimespan);
            });
            services.AddScoped<ICaching<Book>>(provider =>
            {
                var cache = provider.GetRequiredService<IDistributedCache>();
                var cachePrefix = "Book_";
                return new CachingRepository<Book>(cache, cachePrefix, caching.Value.BookTimespan);
            });
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
