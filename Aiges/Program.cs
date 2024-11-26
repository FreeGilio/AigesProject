using Aiges.DataAccess.DB;
using Aiges.DataAccess.Repositories;
using Aiges.Core.Interfaces;
using Aiges.Core.Services;
using Aiges.Core.Models;

var builder = WebApplication.CreateBuilder(args);

// Register DatabaseConnection 
builder.Services.AddSingleton<DatabaseConnection>(provider =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new DatabaseConnection(connectionString);
});

//Register repositories
builder.Services.AddScoped<IProjectRepo,ProjectRepo>();
builder.Services.AddScoped<IProjectCategoryRepo, ProjectCategoryRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IImageRepo, ImageRepo>();
builder.Services.AddScoped<IEventRepo, EventRepo>();
builder.Services.AddScoped<IReplyRepo, ReplyRepo>();

//Register services
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<ProjectCategoryService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<ReplyService>();

// Configure session with a longer timeout
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
