using Com.GitHub.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Services.Handlers;
using Telegram.Bot;

namespace Com.GitHub.PatBatTB.GEBB;

public class App
{
    private readonly ITelegramBotClient _botClient;
    private readonly ReceivingHandler _receivingHandler;

    public App()
    {
        _botClient = new TelegramBotClient(BotConfig.BotToken);
        _receivingHandler = new ReceivingHandler();
    }

    public async Task Run()
    {
        using CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;
        AppDomain.CurrentDomain.ProcessExit += (_, _) => _receivingHandler.HandleExitSignal(cts);
        Console.CancelKeyPress += (_, _) => _receivingHandler.HandleExitSignal(cts);

        _botClient.StartReceiving(
            _receivingHandler.HandleUpdate,
            _receivingHandler.HandleError,
            receiverOptions: BotConfig.ReceiverOptions,
            cancellationToken: token);
        Console.WriteLine("Bot is running.");
        try
        {
            await Task.Delay(-1, token).WaitAsync(token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Exiting...");
        }
    }
}