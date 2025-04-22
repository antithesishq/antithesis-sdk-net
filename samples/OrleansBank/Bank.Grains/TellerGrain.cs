namespace Bank;

using Orleans.Concurrency;

[StatelessWorker]
public class TellerGrain : Grain, ITellerGrain
{
    public Task Transfer(IAccountGrain from, IAccountGrain to, int cents) =>
        Task.WhenAll(
            from.Withdraw(cents),
            to.Deposit(cents));
}