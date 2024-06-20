namespace Kallum.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PassWord { get; set; }
        public string PhoneNumber { get; set; }
        public BankAccount BankAccount { get; set; }
        public BalanceDetails BalanceDetails { get; set; }
    }
}
