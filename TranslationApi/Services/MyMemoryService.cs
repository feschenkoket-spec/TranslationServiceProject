using Newtonsoft.Json;
using TranslationApi.Models;

namespace TranslationApi.Services
{
    public interface ITranslationService
    {
        Task<TranslationResponse> TranslateAsync(string text, string fromLang, string toLang);
        Task<QuoteInfo> GetRandomQuoteAsync();
        Task<CountryInfo> GetCountryByNameAsync(string name);
    }

    public class MyMemoryService : ITranslationService
    {
        private readonly HttpClient _httpClient;

        public MyMemoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TranslationResponse> TranslateAsync(string text, string fromLang, string toLang)
        {
            var url = $"https://api.mymemory.translated.net/get?q={text}&langpair={fromLang}|{toLang}";
            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TranslationResponse>(json);
        }

        public async Task<QuoteInfo> GetRandomQuoteAsync()
        {
            var response = await _httpClient.GetAsync("https://dummyjson.com/quotes/random");
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<QuoteInfo>(json);
        }

        public async Task<CountryInfo> GetCountryByNameAsync(string name)
        {
            var response = await _httpClient.GetAsync($"https://restcountries.com/v3.1/name/{name}?fullText=true");
            var json = await response.Content.ReadAsStringAsync();
            var countries = JsonConvert.DeserializeObject<List<CountryInfo>>(json);
            return countries?.FirstOrDefault();
        }
    }
}