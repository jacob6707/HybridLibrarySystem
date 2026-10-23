using HybridLibrarySystem.Core.Interface;
using HybridLibrarySystem.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HybridLibrarySystem.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        services.AddDbContext<LibraryContext>();
        // Register repositories
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        return services;
    }
    public static IServiceCollection AddDataServices(this IServiceCollection services, Action<DbContextOptionsBuilder> dbConfig)
    {
        services.AddDbContext<LibraryContext>(dbConfig);
        // Register repositories
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        return services;
    }
}