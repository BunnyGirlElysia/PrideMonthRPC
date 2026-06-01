using DiscordRPC;
var now = DateTime.Now;
var midnightLocal = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Local);

// flag can be set in order var -> cli -> interactive (if available). Fallsback to LGBTQ
string? input = Environment.GetEnvironmentVariable("PRIDE_FLAG")
                ?? (args.Length > 0 ? args[0] : null);

if (input == null)
{
    Console.WriteLine("Choose flag: 1 - lesbian, 2 - trans, other - lgbtq");
    input = Console.ReadLine();
}

string pic = "";
string pictext = "";

switch (input?.Trim().ToLowerInvariant())
{
    case "1":
    case "lesbian":
        pic = "lesbianflag";
        pictext = "Lesbian";
        break;
    case "2":
    case "trans":
        pic = "transflag";
        pictext = "Transgender";
        break;
    default:
        pic = "lgbtqflag";
        pictext = "LGBTQ";
        break;
}

var client = new DiscordRpcClient("1510964108432769114");

client.Initialize();
client.SetPresence(new RichPresence()
{
    Details = "You're Valid!",
    State = "meow",
    Assets = new Assets
    {
        LargeImageKey = pic,
        LargeImageText = pictext
    },
    Timestamps = new Timestamps
    {
        Start = midnightLocal.ToUniversalTime()
    },
    Buttons = new Button[]
    {
        new Button() { Label = "github", Url = "https://github.com/BunnyGirlElysia/PrideMonthRPC" }
    }
});

// block forever so we can run as a service
var exit = new ManualResetEventSlim(false);
AppDomain.CurrentDomain.ProcessExit += (_, _) => exit.Set();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; exit.Set(); };
exit.Wait();

client.Dispose();
