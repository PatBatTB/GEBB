using Com.GitHub.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Services.Handlers;
using Telegram.Bot;

namespace Com.GitHub.PatBatTB.GEBB;

internal class App
{
    private readonly ITelegramBotClient _botClient;
    private readonly ReceivingHandler _receivingHandler;

    internal App()
    {
        _botClient = new TelegramBotClient(BotConfig.BotToken);
        _receivingHandler = new ReceivingHandler();
    }

    internal async Task Run()
    {
        using CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;
        AppDomain.CurrentDomain.ProcessExit += (_, _) => _receivingHandler.ExitHandler(cts);
        Console.CancelKeyPress += (_, _) => cts.Cancel();

        _botClient.StartReceiving(
            _receivingHandler.UpdateHandler,
            _receivingHandler.ErrorHandler,
            receiverOptions: BotConfig.ReceiverOptions,
            cancellationToken: token);

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