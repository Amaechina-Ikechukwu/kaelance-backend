namespace Kallum.Models
{
    public class UserAccount
    {
        public Guid UserAccountId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PassWord { get; set; }
        public string PhoneNumber { get; set; }
        public List<UserBankAccountInformation> UserBankAccounts { get; set; }

    }
}
