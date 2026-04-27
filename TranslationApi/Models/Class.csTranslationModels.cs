using Newtonsoft.Json;
using System.Collections.Generic;

namespace TranslationApi.Models
{
    public class TranslationResponse
    {
        [JsonProperty("responseData")]
        public ResponseData Data { get; set; }
    }

    public class ResponseData
    {
        [JsonProperty("translatedText")]
        public string TranslatedText { get; set; }
    }

    public class QuoteInfo
    {
        [JsonProperty("quote")]
        public string Content { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
    }

    public class CountryInfo
    {
        [JsonProperty("name")]
        public CountryName Name { get; set; }
        [JsonProperty("population")]
        public long Population { get; set; }
    }

    public class CountryName
    {
        [JsonProperty("common")]
        public string Common { get; set; }
    }

    public class TranslateRequest
    {
        public long ChatId { get; set; }
        public string Text { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}