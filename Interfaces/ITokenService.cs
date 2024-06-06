using Kallum.Models;

namespace Kallum.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
