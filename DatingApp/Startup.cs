using DatingApp.Application.Interfaces;
using DatingApp.Core.Entities;
using DatingApp.Core.Interfaces;
using DatingApp.Infrastructure.Data;
using DatingApp.Infrastructure.Service;
using DatingApp.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

using FluentValidation.AspNetCore;
using FluentValidation;
using DatingApp.Application.Validation;
using DatingApp.Core.DTO;
using DatingApp.Infrastructure.Repositories;
using DatingApp.Application.DTO;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using DatingApp.Infrastructure.Persistence.Repositories;
using System.Collections.Generic;

namespace DatingApp
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ITokenService, TokenService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IFriendRepository, FriendRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            services.AddTransient<IPostService, PostService>();
            services.AddTransient<ILikeService, LikeService>();
            services.AddTransient<ICommentService, CommentService>();


            services.AddDbContext<DataContext>(option => {
                option.UseSqlServer(_config.GetConnectionString("MyDB"));
            });
            services.AddControllers();
            services.AddCors();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>{
                options.TokenValidationParameters = new TokenValidationParameters{
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DatingApp", Version = "v1" });
            //});

            // Config to send mail
            services.AddOptions(); // Active Options
            var mailsettings = _config.GetSection("MailSettings");  // read config
            services.Configure<MailSettings>(mailsettings);
            services.AddTransient<ISendMailService, SendMailService>();
            services.AddTransient<IAccountService, AccountService>();          

            // Validation with fluent api
            services.AddMvc().AddFluentValidation();
            services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidation>();
            services.AddScoped<IValidator<ProfileDto>, ProfileDtoValidation>();
            

            services.AddDirectoryBrowser();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DatingApp", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                        {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                    }
                });
            });

            //config signalR
            services.AddSignalR();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DatingApp v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<BroadcastHub>("/notify");
            });


            //config static folder to get images
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"app/Share/Images")), RequestPath = new PathString("/images")
            });
        }
    }
}
