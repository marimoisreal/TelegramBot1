using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MyTelegramBot.Controllers;

[ApiController]
[Route("api/webhook")]

public class WebhookController : ControllerBase 
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(ITelegramBotClient botClient, ILogger<WebhookController> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        if (update.Message is { Text: not null} message)
        {
            _logger.LogInformation("Получено сообщение: {Text} от {ChatId}", message.Text, message.Chat.Id);

            await _botClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"Вы сказали {message.Text}"
                );
        }

        return Ok();
    }

}
