using Microsoft.EntityFrameworkCore;
using TranslationApi.Data;
using TranslationApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Налаштування бази даних SQLite (зберігання історії)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=translations.db"));

// Реєстрація сервісу перекладу (MyMemory API)
builder.Services.AddHttpClient<ITranslationService, MyMemoryService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Генерація документації API

var app = builder.Build();

// Включаємо Swagger для тестування методів у браузері
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers(); // Підключення контролерів

app.Run();