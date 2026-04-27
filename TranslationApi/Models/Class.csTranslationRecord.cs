using System;

namespace TranslationApi.Models
{
    public class TranslationRecord
    {
        public int Id { get; set; }
        public long ChatId { get; set; } 
        public string OriginalText { get; set; }
        public string TranslatedText { get; set; }
        public string SourceLang { get; set; }
        public string TargetLang { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}