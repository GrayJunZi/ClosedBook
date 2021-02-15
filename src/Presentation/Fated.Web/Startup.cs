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
                // 密码中是否要求包含数字
                options.Password.RequireDigit = false;
                // 密码中要求的最小长度
                options.Password.RequiredLength = 1;
                // 密码中是否要求包含小写字符
                options.Password.RequireLowercase = false;
                // 密码中是否要求包含大写字符
                options.Password.RequireUppercase = false;
                // 密码中是否要求包含特殊字符
                options.Password.RequireNonAlphanumeric = false;
                // 密码中要求的唯一字符数
                options.Password.RequiredUniqueChars = 1;

                // 新用户是否默认锁定
                options.Lockout.AllowedForNewUsers = true;
                // 密码错误可重试次数
                options.Lockout.MaxFailedAccessAttempts = 5;
                // 密码错误默认锁定时长
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                // 用户名中允许使用字符
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                // 是否要求唯一邮箱
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie 名称
                options.Cookie.Name = "Fated";
                // 仅 HTTP 获取 Cookie
                options.Cookie.HttpOnly = true;
                // Cookie 过期时间
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

                // 登录地址
                options.LoginPath = "/Account/Login";
                // 登出地址
                options.LogoutPath = "/Account/Logout";
                // 拒绝访问地址
                options.AccessDeniedPath = "/Account/AccessDenied";
                // 弹性到期时间，自动续期
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
