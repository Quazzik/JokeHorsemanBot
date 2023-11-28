using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using tgbot;
internal class Program
{
    public static string path = @"botvideos\";
    public static int vCount = Directory.GetFiles(path, "*.mp4", SearchOption.AllDirectories).Length;
    public static GameRandom randv = new GameRandom(vCount);

    public static string[] jokesList = System.IO.File.ReadAllText("botjokes.txt").Split('$');
    public static GameRandom randt = new GameRandom(jokesList.Length);

    private readonly static HttpClient _client = new();
    private static TelegramBotClient? _botClient;
    private static List<string> _commandsAvailable = new()
    {
    "/start",
    "/video",
    "/joke"
    };

    private static void Main(string[] args)
    {
        MainAsync().GetAwaiter().GetResult();
    }

    private static async Task MainAsync()
    {
        _botClient = new TelegramBotClient("6772456414:AAEL9sz49afvN3MxCVkyVf0yH3dZGn_9Wn8", _client);
        await Logger.Info($"Запущен прекрасный бот {(await _botClient.GetMeAsync()).Username}");
        _botClient.StartReceiving(UpdateHandler, ErrorHandler);
        Console.ReadLine();

    }
    private static async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
    {
        await (update.Type switch
        {
            UpdateType.Message => ProcedureMessageAsync(update),
            UpdateType.EditedMessage => ProcedureMessageAsync(update, true),
            _ => Task.CompletedTask
        });
    }
    private static async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        throw new NotImplementedException();
    }
    private static async Task ProcedureMessageAsync(Update update, bool isEdited = false)
    {
        var message = isEdited ? update.EditedMessage : update.Message;
        await Logger.Info($"Сообщение пользователя {message.Chat.Username}: '{message.Text}'");
        if (message.Entities != null && message.Entities.Any())
            foreach (var entity in message.Entities)
                await ProcedureCommand(message, entity);
    }
    private static async Task ProcedureCommand(Message msg, MessageEntity entity)
    {
        if (entity.Type != MessageEntityType.BotCommand) return;
        var command = string.Join("", msg.Text.Skip(entity.Offset).Take(entity.Length));
        if (!_commandsAvailable.Contains(command))
        {
            await _botClient.SendTextMessageAsync(msg.Chat, "Неправильная команда");
            return;
        }
        await (command switch
        {
            "/start" => _botClient.SendTextMessageAsync(msg.Chat, $"""
            Доступные команды:
            {string.Join("\n", _commandsAvailable)}
            """),
            "/video" => SendVideoAsync(msg.Chat),
            "/joke" => SendTextAsync(msg.Chat)
        });
    }
    private static async Task SendVideoAsync(Chat chat)
    {
        int videoNum = randv.GetNext();
        using var file = new FileStream(path + "video" + videoNum + ".mp4", FileMode.Open, FileAccess.Read);
        var video = new InputFileStream(file);
        await _botClient.SendVideoAsync(chat, video);
    }
    private static async Task SendTextAsync(Chat chat)
    {
        int jokenum = randt.GetNext();
        await _botClient.SendTextMessageAsync(chat, jokesList[--jokenum]);
    }
}