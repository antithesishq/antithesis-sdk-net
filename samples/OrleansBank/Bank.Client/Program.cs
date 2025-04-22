using Bank;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .UseOrleansClient(client =>
    {
        client.UseLocalhostClustering()
            .UseTransactions();
    })
    .UseConsoleLifetime()
    .Build();

await host.StartAsync();

var client = host.Services.GetRequiredService<IClusterClient>();
var transactionClient = host.Services.GetRequiredService<ITransactionClient>();
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

var accountKeys = new[] { "Xaawo", "Pasqualino", "Derick", "Ida", "Stacy", "Xiao" };
var random = Random.Shared;

foreach (var key in accountKeys)
{
    var account = client.GetGrain<IAccountGrain>(key);

    await transactionClient.RunTransaction(
        TransactionOption.Create,
        async () =>
        {
            if (await account.GetBalanceCents() == 0)
                await account.Deposit(500);
        });
}

while (!lifetime.ApplicationStopping.IsCancellationRequested)
{
    Console.WriteLine();

    // This is O(n) when O(1) is possible with simple but additional code.
    random.Shuffle(accountKeys);

    var fromAccount = client.GetGrain<IAccountGrain>(accountKeys[0]);
    var toAccount = client.GetGrain<IAccountGrain>(accountKeys[1]);

    await ConsoleWriteBalances();

    async Task ConsoleWriteBalances() =>
        Console.WriteLine($"{fromAccount.GetPrimaryKeyString()} balance: {await fromAccount.GetBalanceCents()}\n" +
                          $"{toAccount.GetPrimaryKeyString()} balance: {await toAccount.GetBalanceCents()}");

    try
    {
        var teller = client.GetGrain<ITellerGrain>(0);
        int transferCents = random.Next(200);

        Console.WriteLine($"\tXfer: {transferCents}");

        await teller.Transfer(fromAccount, toAccount, transferCents);

        await ConsoleWriteBalances();
    }
    catch (Exception exception)
    {
        Console.WriteLine($"{exception.Message}");

        if (exception.InnerException is { } inner)
            Console.WriteLine($"\tInnerException: {inner.Message}\n");
    }

    await Task.Delay(TimeSpan.FromMilliseconds(200));
}