namespace Bank;

public interface IAccountGrain : IGrainWithStringKey
{
    [Transaction(TransactionOption.CreateOrJoin)]
    Task<int> GetBalanceCents();

    [Transaction(TransactionOption.Join)]
    Task Deposit(int cents);

    [Transaction(TransactionOption.Join)]
    Task Withdraw(int cents);
}