using Clifton.IO.Authentication;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Clifton.IO.Authorization {
    public struct CookieNames {
        public const string AccountId = "accountId";
        public const string ClientId = "clientId";
        public const string RememberMe = "rememberMe";
        public const string SessionId = "sessionId";
    }

    ///<summary>
    ///Authenticate users calling Web API using a token from their cookie.
    ///</summary>
    /// <see cref="https://msdn.microsoft.com/en-us/library/system.web.http.authorizeattribute(v=vs.118).aspx"/>
    /// <seealso cref="https://github.com/rblaettler/BasicHttpAuthorization"/>
    public class CookieAuthorizeAttribute : System.Web.Http.AuthorizeAttribute {

        // http://stackoverflow.com/questions/13595723/allowanonymous-not-working-with-custom-authorizationattribute
        private static bool SkipAuthorization(HttpActionContext actionContext) {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        public override void OnAuthorization(HttpActionContext actionContext) {
            if (!SkipAuthorization(actionContext)) {
                if (Authenticate(actionContext) == false) {
                    HandleUnauthorizedRequest(actionContext);
                }
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext) {
            throw new HttpResponseException(actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, ""));
        }

        private bool Authenticate(HttpActionContext actionContext) {
            HttpCookie accountCookie = HttpContext.Current.Request.Cookies[CookieNames.AccountId];
            HttpCookie sessionCookie = HttpContext.Current.Request.Cookies[CookieNames.SessionId];
            HttpCookie clientCookie = HttpContext.Current.Request.Cookies[CookieNames.ClientId];

            if (accountCookie == null || string.IsNullOrWhiteSpace(accountCookie.Value) ||
                sessionCookie == null || string.IsNullOrWhiteSpace(sessionCookie.Value) ||
                clientCookie == null || string.IsNullOrWhiteSpace(clientCookie.Value)) {
                return false;
            }

            Guid accountId = Guid.Parse(accountCookie.Value);
            string clientId = clientCookie.Value;
            Guid sessionId = Guid.Parse(sessionCookie.Value);

            if (AccountSession.Validate(accountId, clientId, sessionId)) {
                Account account = Account.Select(accountId);
                HttpContext.Current.User = new UserPrincipal(account, clientId, sessionId);
                return true;
            }
            
            return false;
        }
    }

    /// <see cref="http://msdn.microsoft.com/en-us/library/ms172766%28v=vs.90%29.aspx"/>
    public class UserPrincipal : System.Security.Principal.IPrincipal {
        //TODO: make this the actual account object...?
        private GenericIdentity identityValue;
        public System.Security.Principal.IIdentity Identity {
            get { return identityValue; }
        }

        public Account Account { get; private set; }
        public string ClientId { get; private set; }
        public Guid SessionId { get; private set; }

        public UserPrincipal(Account account, string clientId, Guid sessionId) {
            identityValue = new GenericIdentity(account.email);
            this.Account = account;
            this.ClientId = clientId;
            this.SessionId = sessionId;
        }

        public bool IsInRole(string role) {
            //TODO: check account roles
            return false;
        }
    }
}