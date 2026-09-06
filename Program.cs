using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


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
    if (message.Text is not { } messageText) return;

    var chatId = message.Chat.Id;
    Console.WriteLine($"[Лог] Сообщение от {message.Chat.Username ?? chatId.ToString()}: {messageText}");

    await bot.SendMessage(
        chatId: chatId,
        text: $"Вы написали: {messageText}",
        cancellationToken: cancellationToken
    );
}

// 7. Логика обработки ошибок
Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
{
    Console.WriteLine($"[Ошибка] {exception.Message}");
    return Task.CompletedTask;
}