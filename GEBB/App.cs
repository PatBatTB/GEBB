using Com.GitHub.PatBatTB.GEBB.Domain;
using Com.GitHub.PatBatTB.GEBB.Services;
using Telegram.Bot;

namespace Com.GitHub.PatBatTB.GEBB;

internal class App
{
    private readonly ITelegramBotClient _botClient;
    private readonly Handler _handler;

    internal App()
    {
        _botClient = new TelegramBotClient(BotConfig.BotToken);
        _handler = new Handler();
    }

    internal async Task Run()
    {
        using CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;
        AppDomain.CurrentDomain.ProcessExit += (_,_) => _handler.ExitHandler(cts);
        Console.CancelKeyPress += (_,_) => cts.Cancel();
        
        _botClient.StartReceiving(
            updateHandler: _handler.UpdateHandler,
            errorHandler: _handler.ErrorHandler,
            receiverOptions: BotConfig.ReceiverOptions,
            cancellationToken: token);

        try
        {
            await Task.Delay(-1, token).WaitAsync(token);
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine("Exiting...");
        }
    }
}