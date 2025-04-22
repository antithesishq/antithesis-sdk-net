namespace Bank;

using Orleans.Concurrency;
using Orleans.Transactions.Abstractions;

[GenerateSerializer]
public record class Balance
{
    [Id(0)]
    public int Cents { get; set; }
}

[Reentrant]
public sealed class AccountGrain : Grain, IAccountGrain
{
    private readonly ITransactionalState<Balance> _balance;

    public AccountGrain([TransactionalState(nameof(balance))] ITransactionalState<Balance> balance) =>
        _balance = balance ?? throw new ArgumentNullException(nameof(balance));

    public Task Deposit(int cents) =>
        _balance.PerformUpdate(balance => balance.Cents += cents);

    public Task Withdraw(int cents) =>
        _balance.PerformUpdate(balance =>
        {
            if (balance.Cents < cents)
            {
                throw new InvalidOperationException(
                    $"Insufficient balance of {balance.Cents / 100m:C2} for withdrawal of {cents / 100m:C2}.");
            }

            return balance.Cents -= cents;
        });

    public Task<int> GetBalanceCents() =>
        _balance.PerformRead(balance => balance.Cents);
}