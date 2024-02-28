using Class_system_Backstage_pj.Areas.ordering_system.Data;
using Class_system_Backstage_pj.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using text_loginWithBackgrount.Data;
using text_loginWithBackgrount.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddControllers(); //�VDI�e�����U����һݪ��A��
builder.Services.AddDbContext<studentContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StudentDB")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:KEY"]))
    };
})
.AddCookie(options =>
{
    // Cookie �]�m
    options.LoginPath = new PathString("/Login/StudentIndex");
    options.AccessDeniedPath = new PathString("/Template/Index");
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "verification"; // �]�w Session Cookie ���W��
    options.IdleTimeout = TimeSpan.FromMinutes(10); // �]�w Session ���O�ɮɶ��� 10 ����
    options.Cookie.HttpOnly = true; // ����Ȥ�ݸ}���X�� Session Cookie
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // �n�D Session Cookie �ȳz�L HTTPS �ǿ�
});
builder.Services.AddTransient<IEmailSender, Emailsender>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Template}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();