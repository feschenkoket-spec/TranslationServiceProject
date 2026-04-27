using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranslationApi.Data;
using TranslationApi.Models;
using TranslationApi.Services;

namespace TranslationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslateController : ControllerBase
    {
        private readonly ITranslationService _service;
        private readonly AppDbContext _db;

        public TranslateController(ITranslationService service, AppDbContext db)
        {
            _service = service;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> TranslateAndSave([FromBody] TranslateRequest request)
        {
            var result = await _service.TranslateAsync(request.Text, request.From, request.To);

            var record = new TranslationRecord
            {
                ChatId = request.ChatId,
                OriginalText = request.Text,
                TranslatedText = result.Data.TranslatedText,
                SourceLang = request.From,
                TargetLang = request.To,
                CreatedAt = DateTime.Now
            };

            _db.Translations.Add(record);
            await _db.SaveChangesAsync();
            return Ok(record);
        }

        [HttpGet("history/{chatId}")]
        public IActionResult GetHistory(long chatId)
        {
            var history = _db.Translations.Where(t => t.ChatId == chatId).OrderByDescending(t => t.CreatedAt).Take(10).ToList();
            return Ok(history);
        }

        [HttpDelete("history/{id}")]
        public async Task<IActionResult> DeleteHistoryItem(int id)
        {
            var record = await _db.Translations.FindAsync(id);
            if (record == null) return NotFound();
            _db.Translations.Remove(record);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("history/clear/{chatId}")]
        public async Task<IActionResult> ClearUserHistory(long chatId)
        {
            var userHistory = _db.Translations.Where(t => t.ChatId == chatId);
            _db.Translations.RemoveRange(userHistory);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("quote")]
        public async Task<IActionResult> GetQuote()
        {
            var quote = await _service.GetRandomQuoteAsync();
            return Ok(quote);
        }

        [HttpGet("country/{name}")]
        public async Task<IActionResult> GetCountry(string name)
        {
            var country = await _service.GetCountryByNameAsync(name);
            if (country == null) return NotFound();
            return Ok(country);
        }
    }
}