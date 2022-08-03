using Application.Activities;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Extensions
{
    public static class ApplicationServiceExtensionsBase
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.IgnoreObsoleteActions();
                c.IgnoreObsoleteProperties();
                c.CustomSchemaIds(type => type.FullName);
            });

            services.AddDbContext<DataContext>( opt =>
            {
                 opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("https://localhost:7180");
                });
            });
            services.AddMediatR(typeof(List.Handler).Assembly);
            services.AddMediatR(typeof(Detail.Handler).Assembly);
            services.AddMediatR(typeof(Create.Command).Assembly);
            services.AddMediatR(typeof(Edit.Command).Assembly);

            services.AddAutoMapper(typeof(MappingProfiles).Assembly);

            return services;
        }
    }
}