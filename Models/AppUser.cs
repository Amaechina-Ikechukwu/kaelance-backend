using Microsoft.AspNetCore.Identity;

namespace Kallum.Models
{
    public class AppUser : IdentityUser
    {
        public List<UserBankAccountInformation> FullInformation { get; set; } = new List<UserBankAccountInformation>();
    }
}
