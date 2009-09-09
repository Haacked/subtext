#region Disclaimer/Info

///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at Google Code at http://code.google.com/p/subtext/
// The development mailing list is at subtext-devs@lists.sourceforge.net 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using System.Web;
using DotNetOpenAuth.OpenId.RelyingParty;
using log4net;
using Subtext.Framework.Logging;
using Subtext.Framework.Security;
using Subtext.Framework.Text;
using Subtext.Framework.Web.Handlers;
using Subtext.Web.Properties;

namespace Subtext.Web.Pages
{
    /// <summary>
    /// Summary description for login.
    /// </summary>
    public partial class login : SubtextPage
    {
        private readonly static ILog log = new Log();
        private static readonly string loginFailedMessage = Resources.Login_Failed + "<br />";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if(!IsPostBack)
            {
                HttpCookie cookie = Request.Cookies["__OpenIdUrl__"];
                if(cookie != null)
                {
                    btnOpenIdLogin.Text = cookie.Value;
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string returnUrl = Request.QueryString["ReturnURL"];
            if(Blog == null ||
               (returnUrl != null && returnUrl.Contains("HostAdmin", StringComparison.OrdinalIgnoreCase)))
            {
                if(!AuthenticateHostAdmin())
                {
                    log.Warn("HostAdmin login failure for " + tbUserName.Text);
                    Message.Text = loginFailedMessage;
                    return;
                }
                else
                {
                    ReturnToUrl("~/HostAdmin/Default.aspx");
                    return;
                }
            }
            else
            {
                if(SubtextContext.HttpContext.Authenticate(Blog, tbUserName.Text, tbPassword.Text, chkRememberMe.Checked))
                {
                    ReturnToUrl(AdminUrl.Home());
                    return;
                }
                else
                {
                    log.Warn("Admin login failure for " + tbUserName.Text);
                    Message.Text = loginFailedMessage;
                }
            }
        }

        protected void btnOpenIdLogin_LoggingIn(object sender, OpenIdEventArgs e)
        {
            if(btnOpenIdLogin.RememberMe)
            {
                var openIdCookie = new HttpCookie("__OpenIdUrl__", btnOpenIdLogin.Text);
                openIdCookie.Expires = DateTime.Now.AddDays(14);
                Response.Cookies.Add(openIdCookie);
            }
        }

        protected void btnOpenIdLogin_LoggedIn(object sender, OpenIdEventArgs e)
        {
            e.Cancel = true;
            if(e.Response.Status == AuthenticationStatus.Authenticated &&
               SecurityHelper.Authenticate(e.ClaimedIdentifier, btnOpenIdLogin.RememberMe))
            {
                ReturnToUrl(AdminUrl.Home());
            }
            else
            {
                openIdMessage.Text = Resources.Login_AuthenticationFailed;
            }
        }

        private void ReturnToUrl(string defaultReturnUrl)
        {
            if(Request.QueryString["ReturnURL"] != null && Request.QueryString["ReturnURL"].Length > 0)
            {
                log.Debug("redirecting to " + Request.QueryString["ReturnURL"]);
                Response.Redirect(Request.QueryString["ReturnURL"], false);
                return;
            }
            else
            {
                log.Debug("redirecting to " + defaultReturnUrl);
                Response.Redirect(defaultReturnUrl, false);
                return;
            }
        }

        private bool AuthenticateHostAdmin()
        {
            return SecurityHelper.AuthenticateHostAdmin(tbUserName.Text, tbPassword.Text, chkRememberMe.Checked);
        }
    }
}