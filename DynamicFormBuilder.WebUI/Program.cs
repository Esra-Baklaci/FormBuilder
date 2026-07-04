using DynamicFormBuilder.Business;
using DynamicFormBuilder.DataAccess;
using DynamicFormBuilder.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddBusinessServices(builder.Configuration);
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var useSqlite = string.Equals(
        builder.Configuration.GetValue<string>("DatabaseProvider"),
        "Sqlite",
        StringComparison.OrdinalIgnoreCase);

    if (useSqlite)
        db.Database.EnsureCreated();
    else
        db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
