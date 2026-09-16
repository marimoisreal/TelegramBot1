using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


var builder = WebApplication.CreateBuilder(args);

// 1. Получаем токен из конфигурации
var botToken = builder.Configuration["BotConfiguration:BotToken"]
    ?? throw new ArgumentNullException("BotToken is missing");

// 2. Регистрируем сервисы
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Собираем приложение (ТОЛЬКО ПОСЛЕ ЭТОГО можно использовать app)
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 4. Запуск бота (после builder.Build)
var botClient = app.Services.GetRequiredService<ITelegramBotClient>();

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

// Запуск приёма сообщений
botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    errorHandler: HandleErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

Console.WriteLine("--> Бот успешно запущен и ждет сообщений в Telegram! <--");

// 5. Запуск веб-сервера
app.Run();

// 6. Логика обработки сообщений
async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message) return;


    var chatId = message.Chat.Id;
    var username = message.Chat.Username ?? message.Chat.FirstName ?? chatId.ToString();
    
    if(message.Text is not { } messageText)
    {
        await bot.SendMessage(
            chatId: chatId,
            text: "Im working with text messages only!", 
            cancellationToken: cancellationToken);
        return;
    }

    Console.WriteLine($"[Log] Message from {username}: {messageText}");

    var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[] {
        new KeyboardButton[] {"/start", "/help"},
        new KeyboardButton[] {"/info"}
    })
    {
        ResizeKeyboard = true
    };


    switch (messageText.Trim().ToLower())
    {
        case "/start":
            await bot.SendMessage(
                chatId: chatId,
                text: $"Hello {message.Chat.FirstName}! Can i help you? ^_^",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken
                );
            break;


        case "/help":
            await bot.SendMessage(
                chatId: chatId,
                text: " --Available commands:--\n\n" +
                "/start - Start the bot\n" +
                "/help - Show the help menu\n" +
                "/info - Bot info",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken
                );
            break;

        case "/info":
            await bot.SendMessage(
                chatId: chatId,
                text: $"That's test bot which is created only in tesc case!",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken
                );
            break;
        default:
            await bot.SendMessage(
                chatId:chatId,
                text: $"You wrote down default text: *{messageText}*\n\nTry to use /help to find out more commands!",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken
                );
            break;

    }


}

// 7. Логика обработки ошибок
Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
{
    Console.WriteLine($"[Ошибка] {exception.Message}");
    return Task.CompletedTask;
}