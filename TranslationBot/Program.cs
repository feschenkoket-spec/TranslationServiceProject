using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TranslationBot
{
    class Program
    {
        private static readonly string BotToken = "8615542214:AAH9Eh-XhJDDec8w1dp9ffhIm5gFjYe7z5o";
        private static readonly string ApiBaseUrl = "https://localhost:7288/api/Translate";
        private static readonly HttpClient _httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            var botClient = new TelegramBotClient(BotToken);
            using var cts = new CancellationTokenSource();

            botClient.StartReceiving(
                HandleUpdateAsync,
                HandlePollingErrorAsync,
                new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
                cts.Token
            );

            var me = await botClient.GetMe();
            Console.WriteLine($"Бот @{me.Username} запущено! Натисни Enter для зупинки.");
            Console.ReadLine();
            cts.Cancel();
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update.Message is not { Text: { } messageText } message) return;
            var chatId = message.Chat.Id;

            try
            {
                if (messageText == "/start" || messageText == "🔄 Почати спочатку")
                {
                    await _httpClient.DeleteAsync($"{ApiBaseUrl}/history/clear/{chatId}");

                    string welcome = "Привіт! Я твій бот-перекладач. ✨\n\n" +
                                     "Я щойно очистив твою історію, щоб ми могли почати з чистого аркуша!\n\n" +
                                     "Просто напиши мені текст для перекладу або скористайся кнопками нижче.\n" +
                                     "(Для інфо про країну пиши: /country Назва)";

                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("🔄 Почати спочатку"),
                            new KeyboardButton("📜 Історія")
                        },
                        new[]
                        {
                            new KeyboardButton("💬 Випадкова цитата")
                        }
                    })
                    {
                        ResizeKeyboard = true
                    };

                    await botClient.SendMessage(chatId, welcome, replyMarkup: replyKeyboard, cancellationToken: ct);
                    return;
                }

                if (messageText == "/history" || messageText == "📜 Історія")
                {
                    var history = await _httpClient.GetFromJsonAsync<TranslationRecord[]>($"{ApiBaseUrl}/history/{chatId}");
                    string res = (history == null || history.Length == 0) ? "Історія порожня." : "Останні переклади:\n";
                    if (history != null) foreach (var h in history) res += $"- {h.OriginalText} ➔ {h.TranslatedText}\n";
                    await botClient.SendMessage(chatId, res, cancellationToken: ct);
                    return;
                }

                if (messageText == "/quote" || messageText == "💬 Випадкова цитата")
                {
                    var q = await _httpClient.GetFromJsonAsync<QuoteInfo>($"{ApiBaseUrl}/quote");
                    await botClient.SendMessage(chatId, $"\"{q.Content}\"\n— {q.Author}", cancellationToken: ct);
                    return;
                }

                if (messageText.StartsWith("/country"))
                {
                    var name = messageText.Split(' ', 2).Length > 1 ? messageText.Split(' ', 2)[1] : "";
                    if (string.IsNullOrEmpty(name)) { await botClient.SendMessage(chatId, "Вкажи країну!"); return; }
                    var c = await _httpClient.GetFromJsonAsync<CountryInfo>($"{ApiBaseUrl}/country/{name}");
                    await botClient.SendMessage(chatId, $"Країна: {c.Name.Common}\nНаселення: {c.Population}", cancellationToken: ct);
                    return;
                }

                var result = await _httpClient.PostAsJsonAsync(ApiBaseUrl, new { ChatId = chatId, Text = messageText, From = "uk", To = "en" });
                var record = await result.Content.ReadFromJsonAsync<TranslationRecord>();
                await botClient.SendMessage(chatId, $"🇬🇧 {record.TranslatedText}", cancellationToken: ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
                await botClient.SendMessage(chatId, "Сталася помилка. Перевір, чи запущено API!", cancellationToken: ct);
            }
        }

        private static Task HandlePollingErrorAsync(ITelegramBotClient b, Exception e, HandleErrorSource s, CancellationToken ct)
        {
            Console.WriteLine(e.Message); return Task.CompletedTask;
        }
    }

    public class TranslationRecord { public string OriginalText { get; set; } public string TranslatedText { get; set; } }
    public class QuoteInfo { public string Content { get; set; } public string Author { get; set; } }
    public class CountryInfo { public CountryName Name { get; set; } public long Population { get; set; } }
    public class CountryName { public string Common { get; set; } }
}