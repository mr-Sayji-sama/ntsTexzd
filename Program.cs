using Microsoft.EntityFrameworkCore; 
using ntsTexzd;
using ntsTexzd.data;

var builder = WebApplication.CreateBuilder(args);

//подключение базы данных
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlite(connectionString));

// Регистрируем DatabaseInitializer в DI
builder.Services.AddTransient<DatabaseInitializer>();

builder.WebHost.UseWebRoot("wwwroot");

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllers();

var app = builder.Build();

// ==== Инициализация базы данных ====
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    initializer.Initialize();   
}

Console.WriteLine("База данных создана и заполнена."); 

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

//app.UseAuthorization();

app.MapRazorPages();

app.MapControllers(); 

app.Run();
