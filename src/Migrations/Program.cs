using DbUp;

var cs = args.FirstOrDefault() ?? Environment.GetEnvironmentVariable("DB_CONNECTION")!;
var upgrader = DeployChanges.To
    .SqlDatabase(cs)
    .WithScriptsEmbeddedInAssembly(typeof(Program).Assembly)
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();
Environment.ExitCode = result.Successful ? 0 : -1;
