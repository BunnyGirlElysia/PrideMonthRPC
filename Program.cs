using DiscordRPC;

class RpcConfig
{
  public string State { get; set; } = "meow";

  public string BigFlag { get; set; } = "lgbtqflag";
  public string BigFlagText { get; set; } = "LGBTQ";

  public string SmallFlag { get; set; } = "lgbtqflag";
  public string SmallFlagText { get; set; } = "LGBTQ";
}

class Program
{
  protected static string configFolder = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
      "PrideMonthRPC"
  );

  protected static string configPath = Path.Combine(configFolder, "config.txt");

  static void Main(string[] args)
  {
    Directory.CreateDirectory(configFolder);

    bool headless = args.Contains("--bg");

    var cfg = LoadConfig(configPath);

    if (!headless)
    {
      Console.Write("State: ");
      var state = Console.ReadLine();
      if (!string.IsNullOrWhiteSpace(state))
        cfg.State = state;

      Console.Write("Big flag: ");
      var big = ResolveFlag(Console.ReadLine());
      cfg.BigFlag = big.flag;
      cfg.BigFlagText = big.text;

      Console.Write("Small flag: ");
      var small = ResolveFlag(Console.ReadLine());
      cfg.SmallFlag = small.flag;
      cfg.SmallFlagText = small.text;

      SaveConfig(configPath, cfg);
    }

    var now = DateTime.Now;
    var midnight = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Local);

    var client = new DiscordRpcClient("1510964108432769114");
    client.Initialize();

    ApplyRpc(client, cfg, midnight);

    var exit = new ManualResetEventSlim(false);
    AppDomain.CurrentDomain.ProcessExit += (_, _) => exit.Set();
    Console.CancelKeyPress += (_, e) =>
    {
      e.Cancel = true;
      exit.Set();
    };

    exit.Wait();

    client.Dispose();
  }

  static (string flag, string text) ResolveFlag(string? input)
  {
    return input?.Trim().ToLowerInvariant() switch
    {
      "1" or "lesbian" => ("lesbianflag", "Lesbian"),
      "2" or "trans" => ("transflag", "Transgender"),
      "3" or "progress" => ("progressflag", "Progress"),
      "lgbtq" or "4" or _ => ("lgbtqflag", "LGBTQ")
    };
  }

  static RpcConfig LoadConfig(string path)
  {
    if (!File.Exists(path))
      return new RpcConfig();

    var lines = File.ReadAllLines(path);

    return new RpcConfig
    {
      State = lines.ElementAtOrDefault(0) ?? "meow",
      BigFlag = lines.ElementAtOrDefault(1) ?? "lgbtqflag",
      BigFlagText = lines.ElementAtOrDefault(2) ?? "LGBTQ",
      SmallFlag = lines.ElementAtOrDefault(3) ?? "lgbtqflag",
      SmallFlagText = lines.ElementAtOrDefault(4) ?? "LGBTQ"
    };
  }

  static void SaveConfig(string path, RpcConfig cfg)
  {
    File.WriteAllLines(path,
      [
        cfg.State,
            cfg.BigFlag,
            cfg.BigFlagText,
            cfg.SmallFlag,
            cfg.SmallFlagText
      ]);
  }

  static void ApplyRpc(DiscordRpcClient client, RpcConfig cfg, DateTime midnightLocal)
  {
    client.SetPresence(new RichPresence
    {
      Details = "You're Valid <3",
      State = cfg.State,
      Assets = new Assets
      {
        LargeImageKey = cfg.BigFlag,
        LargeImageText = cfg.BigFlagText,
        SmallImageKey = cfg.SmallFlag,
        SmallImageText = cfg.SmallFlagText
      },
      Buttons = new[]
        {
                new Button
                {
                    Label = "github project",
                    Url = "https://github.com/BunnyGirlElysia/PrideMonthRPC"
                }
            },
      Timestamps = new Timestamps
      {
        Start = midnightLocal.ToUniversalTime()
      }
    });
  }
}
