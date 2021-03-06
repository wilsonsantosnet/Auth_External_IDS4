using IdentityModel;
using Sso.Server.Api.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Sso.Server.Api
{
    public class UserCredentialServices : IUserCredentialServices
    {

        public async Task<UserCredential> Auth(string userName, string password)
        {

            return await Task.Run(() =>
            {
                var UserCredential = default(UserCredential);

                var userAdmin = Config.GetUsers()
                    .Where(_ => _.Username == userName)
                    .Where(_ => _.Password == password)
                    .SingleOrDefault();

                if (userAdmin.IsNotNull())
                    UserCredential = userAdmin;

                return UserCredential;
            });

        }

        public async Task<UserCredential> AuthByExternalLogin(string email)
        {
            return await Task.Run(() =>
            {
                var UserCredential = default(UserCredential);

                var userAdmin = Config.GetUsers()
                    .Where(_ => _.Claims
                        .Where(__=>__.Type == JwtClaimTypes.Email)
                        .Where(__=>__.Value == email).IsAny())
                    .SingleOrDefault();

                if (userAdmin.IsNotNull())
                    UserCredential = userAdmin;

                return UserCredential;
            });
        }
    }
}
