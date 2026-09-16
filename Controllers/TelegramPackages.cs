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

}
