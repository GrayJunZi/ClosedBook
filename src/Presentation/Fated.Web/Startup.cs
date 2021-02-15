using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fated.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Fated.Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;

namespace Fated.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // �������Ƿ�Ҫ���������
                options.Password.RequireDigit = false;
                // ������Ҫ�����С����
                options.Password.RequiredLength = 1;
                // �������Ƿ�Ҫ�����Сд�ַ�
                options.Password.RequireLowercase = false;
                // �������Ƿ�Ҫ�������д�ַ�
                options.Password.RequireUppercase = false;
                // �������Ƿ�Ҫ����������ַ�
                options.Password.RequireNonAlphanumeric = false;
                // ������Ҫ���Ψһ�ַ���
                options.Password.RequiredUniqueChars = 1;

                // ���û��Ƿ�Ĭ������
                options.Lockout.AllowedForNewUsers = true;
                // �����������Դ���
                options.Lockout.MaxFailedAccessAttempts = 5;
                // �������Ĭ������ʱ��
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                // �û���������ʹ���ַ�
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                // �Ƿ�Ҫ��Ψһ����
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie ����
                options.Cookie.Name = "Fated";
                // �� HTTP ��ȡ Cookie
                options.Cookie.HttpOnly = true;
                // Cookie ����ʱ��
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

                // ��¼��ַ
                options.LoginPath = "/Account/Login";
                // �ǳ���ַ
                options.LogoutPath = "/Account/Logout";
                // �ܾ����ʵ�ַ
                options.AccessDeniedPath = "/Account/AccessDenied";
                // ���Ե���ʱ�䣬�Զ�����
                options.SlidingExpiration = true;
            });

            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://localhost:5001/";
                    options.ClientId = Configuration["Authentication:OpenIdConnect:ClientId"];
                    options.ClientSecret = Configuration["Authentication:OpenIdConnect:ClientSecret"];
                    options.ResponseMode = OpenIdConnectResponseType.Code;
                    options.SaveTokens = true;
                })
                .AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                    options.ClientId = Configuration["Authentication:MicrosoftAccount:ClientId"];
                    options.ClientSecret = Configuration["Authentication:MicrosoftAccount:ClientSecret"];
                })
                .AddLinkedIn(options =>
                {
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                    options.ClientId = Configuration["Authentication:LinkedIn:ClientId"];
                    options.ClientSecret = Configuration["Authentication:LinkedIn:ClientSecret"];
                })
                .AddGitHub(options =>
                {
                    options.ClientId = Configuration["Authentication:GitHub:ClientId"];
                    options.ClientSecret = Configuration["Authentication:GitHub:ClientSecret"];
                });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
