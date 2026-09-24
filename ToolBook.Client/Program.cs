var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();


var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] 
              ?? throw new InvalidOperationException("API BaseUrl mangler");

builder.Services.AddHttpClient("ToolBookApi", client =>
{
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseRouting();
app.UseHttpsRedirection();


app.UseAuthorization();
app.UseSession();
app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Tool}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();