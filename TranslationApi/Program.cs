using Microsoft.EntityFrameworkCore;
using TranslationApi.Data;
using TranslationApi.Services;

var builder = WebApplication.CreateBuilder(args);
// Підключення бази даних SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=translations.db"));
// Add services to the container.
builder.Services.AddHttpClient<ITranslationService, MyMemoryService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
