using Clifton.CommonWeb.Authentication;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Clifton.CommonWeb.Controllers {
    public abstract class BaseController : ApiController {
        protected string ConnectionString { get { return ConfigurationManager.ConnectionStrings["Default"].ConnectionString; } }

        protected bool RememberMe {
            get {
                bool rememberMe;

                HttpCookie rememberMeCookie = HttpContext.Current.Request.Cookies[CookieNames.RememberMe];
                if (rememberMeCookie == null || string.IsNullOrWhiteSpace(rememberMeCookie.Value) || !bool.TryParse(rememberMeCookie.Value, out rememberMe)) {
                    return false;
                }

                return bool.Parse(rememberMeCookie.Value);
            }
        }

        protected CookieHeaderValue CreateCookie(string key, string value, DateTime? expires = null) {
            var cookieHeader = new CookieHeaderValue(key, value) { Path = "/" };
            if (expires.HasValue) {
                cookieHeader.Expires = new DateTimeOffset(expires.Value);
            }
            return cookieHeader;
        }

        protected CookieHeaderValue CreateExpiredCookie(string key) {
            return new CookieHeaderValue(key, "") { Path = "/", Expires = DateTime.MinValue };
        }

        protected IHttpActionResult CreateSession(Guid accountId, string clientId) {
            Guid sessionId;
            DateTime? expires;

            AccountSession.Begin(accountId, clientId, RememberMe, out sessionId, out expires);

            var responseMsg = Request.CreateResponse<string>(null);

            responseMsg.Headers.AddCookies(new CookieHeaderValue[] {
                CreateCookie(CookieNames.AccountId, accountId.ToString(), expires),
                CreateCookie(CookieNames.SessionId, sessionId.ToString(), expires)
            });

            return ResponseMessage(responseMsg);
        }

        [Obsolete("Get rid of inline SQL where possible; use stored procedures")]
        protected int ExecuteNonQuery(SqlConnection connection, string query) {
            SqlCommand cmd = new SqlCommand(query, connection);
            connection.Open();
            int rowsAffected = cmd.ExecuteNonQuery();
            connection.Close();
            return rowsAffected;
        }
    }
}