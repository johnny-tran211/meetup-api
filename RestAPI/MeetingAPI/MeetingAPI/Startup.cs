using FluentValidation;
using FluentValidation.AspNetCore;
using MeetingAPI.Authorization;
using MeetingAPI.Entities;
using MeetingAPI.Filters;
using MeetingAPI.Identity;
using MeetingAPI.Models;
using MeetingAPI.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => {
                // Add Global Filter
                options.Filters.Add(typeof(ExceptionFilter));
            }).AddFluentValidation();
            services.AddDbContext<MeetupContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            // Create jwt options from appsettings.json
            var jwtOptions = new JwtOptions();
            Configuration.GetSection("Jwt").Bind(jwtOptions);

            services.AddSingleton(jwtOptions);

            // Add Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg => 
            {
                cfg.RequireHttpsMetadata = false;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    /*
                    ValidateIssuer = true, // generates the token
                    ValidateAudience = true, // Validate the recipient of the token is authorized to receive
                    ValidateLifetime = true, // Check if the token is not expired and the signing key of the issuer is valid
                    ValidateIssuerSigningKey = true, // Validate signature of the token
                    */
                    ValidIssuer = jwtOptions.JwtIssuer,
                    ValidAudience = jwtOptions.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.JwtKey))
                };
            });

            // Config Authorization
            services.AddAuthorization(options => {
                options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "VN"));
                options.AddPolicy("AtLeast18", builder => builder.AddRequirements(new MinimumAgeRequirement(18)));
            });

            // Add AutoMapper
            services.AddAutoMapper(this.GetType().Assembly);

            // Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "MeetupAPI", Version = "v1"});

            });
            // Add Dependency Injection
            services.AddScoped<MeetupSeeder>();
            services.AddScoped<TimeTrackFilter>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserValidator>();
            services.AddScoped<IValidator<MeetupQuery>, MeetupQueryValidator>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IAuthorizationHandler, MinimumAgeHandler>();
            services.AddScoped<IAuthorizationHandler, MeetupResourceOperationHandler>();

            // Add Cross-Orgin Resource Sharing
            services.AddCors(options =>
            {
                options.AddPolicy("FrontEndClient",
                    builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins(""));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //RunMigrations(context);
            app.UseResponseCaching();

            app.UseStaticFiles();

            // Use Cross-Orgin Resource Sharing
            app.UseCors("FrontEndClient");



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Use Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MeetupAPI v1");
            });

            app.UseAuthentication();

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //meetupSeeder.Seed();
        }

        /*private void RunMigrations(MeetupContext context)
        {
            var pendingMigrations = context.Database.GetPendingMigrations();
            if (pendingMigrations.Any()) 
            {
                context.Database.Migrate();
            }

        }*/
    }
}
