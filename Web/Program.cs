using EsunLikeList.Business;
using EsunLikeList.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var cs = builder.Configuration.GetConnectionString("Default")!;
builder.Services.AddSingleton<ILikeListRepository>(_ => new LikeListRepository(cs));
builder.Services.AddScoped<ILikeListService, LikeListService>();

var app = builder.Build();
app.UseRouting();
app.MapControllerRoute("default", "{controller=LikeList}/{action=Index}/{id?}");
app.Run();
