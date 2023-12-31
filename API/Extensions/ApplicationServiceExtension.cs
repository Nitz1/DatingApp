﻿using System.Reflection;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API;

public static class ApplicationServiceExtension
{
   public static IServiceCollection AddApplicationServices(this IServiceCollection services,
   IConfiguration configuration)
   {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddDbContext<DataContext>(opt=>
            {
                opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            });
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();
        // services.AddScoped<IUserRepository, UserRepository>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
        services.AddScoped<IPhotoService,PhotoService>();
        services.AddScoped<LogUserActivity>();
        // services.AddScoped<ILikesRepository, LikesRepository>();
        // services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();
        return services;
   }
}
