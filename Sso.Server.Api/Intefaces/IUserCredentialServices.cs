using Sso.Server.Api.Model;
using System.Threading.Tasks;

namespace Sso.Server.Api
{
    public interface IUserCredentialServices
    {
        Task<UserCredential> Auth(string userName, string password);
        Task<UserCredential> AuthByExternalLogin(string email);
    }
}
