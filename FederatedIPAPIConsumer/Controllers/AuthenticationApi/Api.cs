using FederatedIPAuthenticationService.Models;
using FederatedIPAuthenticationService.Web.ConsumerAPI;
using ServiceLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FederatedIPAPIConsumer.Controllers.AuthenticationApi
{
    public class AuthenticationController : ConsumerAuthenticationApiAuthenticationController
    {
        private IUserManagmentService UserManagmentService => Services.Get<IUserManagmentService>();
        private ConsumerUser ResolveUserWithBasicAuth(string username, string password)
        {
            if (username != null && password == "1234")
            {
                return UserManagmentService.ResolveUser(username, password);
            }
            return null;
        }
        private ConsumerUser ResolveUserWithEmailAuth(string email, string password)
        {
            if (email != null && password == "1234")
            {
                return UserManagmentService.ResolveUser(email, password);
            }
            return null;
        }
        protected override ConsumerUser ResolveAuthenticatedUser(ProviderAuthenticationCredentials providerAuthenticationCredentials, IEnumerable<string> externalAuthAuthorizedUser)
        {
            if (providerAuthenticationCredentials.TestUserGuid is Guid userGuid)
            {
                return UserManagmentService.GetTestUser(userGuid);
            }
            else if (!string.IsNullOrWhiteSpace(providerAuthenticationCredentials.Username) && !string.IsNullOrWhiteSpace(providerAuthenticationCredentials.Password))
            {
                return ResolveUserWithBasicAuth(providerAuthenticationCredentials.Username, providerAuthenticationCredentials.Password);
            }
            else if (!string.IsNullOrWhiteSpace(providerAuthenticationCredentials.Email) && !string.IsNullOrWhiteSpace(providerAuthenticationCredentials.Password))
            {
                return ResolveUserWithEmailAuth(providerAuthenticationCredentials.Email, providerAuthenticationCredentials.Password);
            }
            else if (externalAuthAuthorizedUser != null)
            {
                CACAuthorizedUser user = new CACAuthorizedUser(externalAuthAuthorizedUser);
                return UserManagmentService.ResolveUser(user.EDIPI);
            }
            return null;
        }
    }
    public class TestUsersController : ConsumerAuthenticationApiTestUsersController
    {
        private IUserManagmentService UserManagmentService => Services.Get<IUserManagmentService>();
        protected override IEnumerable<ConsumerUser> ResolveTestUsers() => UserManagmentService.GetTestUsers();
    }
    public class PrivacyNoticeController : ConsumerAuthenticationApiPrivacyNoticeController
    {
        private static string PrivacyPolicy = "Lorem ipsum dolor sit amet, ea vel phaedrum deterruisset, ne qui discere ullamcorper, te commodo habemus imperdiet vel. Sea id dicat integre, no his duis error evertitur. Vim tritani blandit molestiae ei, quodsi virtute ad sed. Id quis torquatos pri, eum eu vide tota suscipiantur, veri idque et nam. Qui ex denique constituto, mei alia veniam putent no.\n" +
            "Ei case rebum oratio cum, assum equidem pri ne, vix et vidit velit ancillae.Nemore scripta reprimique at eum. Sint meis rationibus id vix, dolore essent dignissim an vel.Te his exerci reprehendunt. Has ei meis consul adversarium, ex quod tincidunt pro, dicit impetus ei eam.\n" +
"Minim tincidunt disputando mei an.Ex unum nostro cum, insolens antiopam ex pro, reque iudicabit mea ex. Duo habemus consequat ut, ne vidisse meliore molestie vel.Summo graece et ius, ex epicurei indoctum pericula qui, qui et nusquam conclusionemque. Minimum consequat mea te.\n" +
"At congue animal labitur cum, summo quidam ocurreret vel ex.Pri inani voluptua sensibus ei, ea qui sonet perpetua evertitur, an nam melius latine. Quem dicat qui in, at agam eloquentiam ius. Id vide erant molestie pro, an semper fabulas consectetuer vim, atqui aliquip assueverit ad pri.In mei cetero albucius.\n" +
"Eros option sea id, ex aliquam perpetua electram mea.Oratio omittam mnesarchum mea no, ei adhuc suavitate has, etiam virtute ei qui. Consetetur intellegam mel in, per esse mutat an. An utroque cotidieque theophrastus vis.At vel utamur verterem, ea legere eligendi qualisque cum, esse lobortis urbanitas ut mei.";
        protected override string GetPrivacyNotice() => PrivacyPolicy;
    }
    public class SiteMetaController : ConsumerAuthenticationApiSiteMetaController { }
    


}