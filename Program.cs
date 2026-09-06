using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

var botToken = builder.Configuration["BotConfiguration:BotToken"] ?? throw new ArgumentNullException("BotToken is missing");

builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));

builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
