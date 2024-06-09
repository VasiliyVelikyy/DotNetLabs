namespace BankAccount
{
    public class Account
    {
        public string Customer { get; private set; }
        public double Balance { get; private set; }
        public Account(string customer, double balance)
        {
            // TODO: Complete member initialization
            this.Customer = customer;
            this.Balance = balance;
        }

        public void Credit(double amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException("amount");
            }
            Balance += amount;
        }


    }
}
