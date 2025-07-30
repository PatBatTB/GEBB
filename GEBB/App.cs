using Com.GitHub.PatBatTB.GEBB.Domain;
using Com.Github.PatBatTB.GEBB.Services;
using Com.Github.PatBatTB.GEBB.Services.Handlers;
using log4net;
using Telegram.Bot;

namespace Com.GitHub.PatBatTB.GEBB;

public class App
{
    private readonly ITelegramBotClient _botClient;
    private readonly ReceivingHandler _receivingHandler;
    private readonly AlarmSendService _alarmSendService;
    private readonly ILog log = LogManager.GetLogger(typeof(App));

    public App()
    {
        _botClient = new TelegramBotClient(BotConfig.BotToken);
        _receivingHandler = new ReceivingHandler();
        _alarmSendService = new AlarmSendService();
    }

    public async Task Run()
    {
        using CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;
        AppDomain.CurrentDomain.ProcessExit += (_, _) => _receivingHandler.HandleExitSignal(cts);
        Console.CancelKeyPress += (_, _) => _receivingHandler.HandleExitSignal(cts);

        RunBot(token);
        RunAlarmService(token);
        try
        {
            await Task.Delay(-1, token).WaitAsync(token);
        }
        catch (OperationCanceledException)
        {
            log.Info("Exiting the App");
        }
    }

    private void RunBot(CancellationToken token)
    {
        _botClient.StartReceiving(
            _receivingHandler.HandleUpdate,
            _receivingHandler.HandleError,
            receiverOptions: BotConfig.ReceiverOptions,
            cancellationToken: token);
        log.Info("The bot is running");
    }

    private void RunAlarmService(CancellationToken token)
    {
        Task.Run(() => _alarmSendService.Start(_botClient, token), token);
        log.Info("The alarm service is running");
    }
}
