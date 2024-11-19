
using System.ComponentModel.Design;
using System.Runtime.InteropServices.JavaScript;
namespace _8_Event_Args
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CreditCard creditCard = new CreditCard("1234 5678 9012 3456", "Bill Gates", new DateTime(2025, 12, 31), "1234", 100000, 2000,5000);
            User user1 = new User("Bill Gates", new Account(100), creditCard);
            user1.creditCard.Notify += DisplayCardMessage;
            user1.account.Notify += DisplayMessage;
            user1.AccountAction += DisplayUserMessage;
            
            user1.Put(100);
            user1.Take(120);
            user1.Take(100);
            user1.Put(100);
            user1.creditCard.Deposit(6000); 
            user1.creditCard.Snatie(8500);
            user1.creditCard.Credit(200000);
            user1.creditCard.Deposit(60000);
            creditCard.ChangePin();
            static void DisplayMessage(object? sender, AccoutEventArgs e)
            {
                Console.WriteLine("Ответ банка: " + e.Message + e.Sum + ". Баланс средств: " + e.Balance);
                Console.WriteLine("---------------------------------------------------------------------");
                Console.ResetColor();
            }

            static void DisplayUserMessage(object? sender, AccoutEventArgs e)
            {
                if (sender is User user)
                {
                    Console.WriteLine(e.Message + e.Sum + " клиентом: " + user.UserName);
                }
            }

            static void DisplayCardMessage(object? sender, CardEventArgs e)
            {
                Console.WriteLine($"Карта: {e.Message}. Сумма: {e.Amount}. Баланс: {e.Balance}. Лимит: {e.Limit}");
            }
        }
    }

    public class Account
    {
        public EventHandler<AccoutEventArgs>? Notify;
        public int Balance { get; set; } = 0;

        public Account(int sum) { this.Balance = sum; }

        public void Put(int sum)
        {
            Balance += sum;
            Notify?.Invoke(this, new AccoutEventArgs("Положено на счет ", sum, Balance));
        }

        public void Take(int sum)
        {
            if (Balance >= sum)
            {
                Balance -= sum;
                Notify?.Invoke(this, new AccoutEventArgs("Снято со счета ", sum, Balance));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Notify?.Invoke(this, new AccoutEventArgs("Недостаточно денег для снятия", sum, Balance));
            }
        }
    }

    public class AccoutEventArgs : EventArgs
    {
        public string? Message { get; set; }
        public int Sum { get; set; }
        public int Balance { get; set; }

        public AccoutEventArgs(string? message, int sum, int balance)
        {
            Message = message;
            Sum = sum;
            Balance = balance;
        }
    }

    public class CreditCard
    {
        public EventHandler<CardEventArgs>? Notify;
        public string Number { get; set; }
        public string FIO { get; set; }
        public DateTime Srok { get; set; }
        public string PIN { get; set; }
        public decimal Limit { get; set; }
        public decimal Balance { get; set; }
        public decimal ZadannaSumma { get; set; }

        public CreditCard(string number, string fio, DateTime srok, string pin, decimal limit, decimal balance,decimal zadannaSumma)
        {
            Number = number;
            FIO = fio;
            Srok = srok;
            PIN = pin;
            Limit = limit;
            Balance = balance;
            ZadannaSumma = zadannaSumma;
        }

        public void Deposit(decimal amount)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Balance += amount;
            Notify?.Invoke(this, new CardEventArgs($"Пополнение на карту: {Number}", amount, Balance,Limit));
            CheckSumm();
        }

        public void Snatie(decimal amount)
        {
            if (Balance >= amount)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Balance -= amount;
                Notify?.Invoke(this, new CardEventArgs($"Снятие с карты: {Number}", amount, Balance,Limit));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Notify?.Invoke(this, new CardEventArgs("Недостаточно денег для снятия", amount, Balance,Limit));
            }
            CheckSumm();
        }

        public void Credit(decimal amount)
        {
            if (Balance + Limit >= amount)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                decimal availableFunds = Balance + Limit;
                Balance  -= amount;
                Notify?.Invoke(this, new CardEventArgs($"Кредитные средства использованы с карты: {Number}", amount, Balance, Limit));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Notify?.Invoke(this, new CardEventArgs("Недостаточно денег для снятия включая кредитные средства", amount, Balance, Limit));
            }
            CheckSumm();
        }

        public void CheckSumm()
        {
            if (Balance >= ZadannaSumma)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Notify?.Invoke(this, new CardEventArgs($"Достигнута целевая сумма:",0,Balance, Limit));

            }
        }

        public void ChangePin()
        {
            Console.WriteLine("Новый PIN: ");
            string? newPin = Console.ReadLine();
            if(int.TryParse(newPin,out int NewPin)&& NewPin>=1000 && NewPin<=9999)
            {
                string oldPin = PIN; 
                PIN = NewPin.ToString();
                Console.ForegroundColor = ConsoleColor.Green;
                Notify?.Invoke(this, new CardEventArgs($"PIN-код изменён. Старый PIN: {oldPin}, Новый PIN: {PIN}", 0, Balance, Limit));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("PIN не удалось изменить");
                
            }
        }
        
    }

    public class CardEventArgs : EventArgs
    {
        public string Message { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public decimal Limit { get; set; }

        public CardEventArgs(string message, decimal amount, decimal balance,decimal limit)
        {
            Message = message;
            Amount = amount;
            Balance = balance;
            Limit = limit;
        }
    }

    public class User
    {
        public EventHandler<AccoutEventArgs>? AccountAction;
        public string? UserName { get; set; }
        public Account account { get; set; }
        public CreditCard creditCard { get; set; }

        public User(string? userName, Account account, CreditCard creditCard)
        {
            UserName = userName;
            this.account = account;
            this.creditCard = creditCard;
        }

        public void Take(int sum)
        {
            AccountAction?.Invoke(this, new AccoutEventArgs("Попытка снятия со счета суммы в размере ", sum, account.Balance));
            account.Take(sum);
        }

        public void Put(int sum)
        {
            AccountAction?.Invoke(this, new AccoutEventArgs("Попытка положить на счет суммы в размере ", sum, account.Balance));
            account.Put(sum);
        }
    }
}

