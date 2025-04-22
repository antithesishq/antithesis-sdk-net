namespace Bank;

public interface ITellerGrain : IGrainWithIntegerKey
{
    [Transaction(TransactionOption.Create)]
    Task Transfer(IAccountGrain from, IAccountGrain to, int cents);
}