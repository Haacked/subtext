#region Disclaimer/Info
///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at SourceForge at http://sourceforge.net/projects/subtext
// The development mailing list is at subtext-devs@lists.sourceforge.net 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Configuration;
using Subtext.Extensibility.Providers;
using Subtext.Framework;
using Subtext.Framework.Configuration;
using Subtext.Framework.Text;
using log4net;
using Subtext.Framework.Security;

namespace Subtext.Web.Pages
{
	/// <summary>
	/// Summary description for login.
	/// </summary>
	public partial class login : System.Web.UI.Page
	{
		private readonly static ILog log = new Framework.Logging.Log();

		#region Declared Controls
		#endregion
	
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		protected void btnLogin_Click(object sender, EventArgs e)
		{
			BlogInfo currentBlog = Config.CurrentBlog;
			string returnUrl = Request.QueryString["ReturnURL"];
			if(currentBlog == null || (returnUrl != null && StringHelper.Contains(returnUrl, "HostAdmin", StringComparison.InvariantCultureIgnoreCase)))
			{
				if(!AuthenticateHostAdmin())
				{
					log.Warn("HostAdmin login failure for " + tbUserName.Text);
					Message.Text = "That&#8217;s not it<br />";
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
                if (SecurityHelper.Authenticate(tbUserName.Text, tbPassword.Text, chkRememberMe.Checked))
				{
					ReturnToUrl(currentBlog.AdminHomeVirtualUrl);
					return;
				}
				else
				{
					log.Warn("Admin login failure for " + tbUserName.Text);
					Message.Text = "That&#8217;s not it<br />";
				}
			}
		}

		protected void lbSendPassword_Click(object sender, EventArgs e)
		{
			BlogInfo info = Config.CurrentBlog;
			bool messageSent = false;
			string password;

			if (info != null)
			{
				//Try Admin login.
				if (String.Equals(tbUserName.Text, info.UserName, StringComparison.InvariantCultureIgnoreCase))
				{
					if (info.IsPasswordHashed)
					{
						password = SecurityHelper.ResetPassword();
					}
					else
					{
						password = info.Password;
					}

					log.Warn("Admin password was reset for " + info.UserName);

					string message = "Here is your blog login information:\nUserName: {0}\nPassword: {1}\n\nNote: your old password will no longer work.";
					EmailProvider mail = EmailProvider.Instance();

					string To = info.Email;
					string From = mail.AdminEmail;
					string Subject = "Login Credentials";
					string Body = string.Format(message, info.UserName, password);
					mail.Send(To, From, Subject, Body);
					Message.Text = "Login Credentials Sent<br />";
					messageSent = true;
				}
			}

			if (String.Equals(tbUserName.Text, HostInfo.Instance.HostUserName, StringComparison.InvariantCultureIgnoreCase))
			{
				//Try Host Admin login.
				if (Config.Settings.UseHashedPasswords)
					password = SecurityHelper.ResetHostAdminPassword();
				else
					password = HostInfo.Instance.Password;

				log.Warn("HostAdmin password was reset for " + HostInfo.Instance.HostUserName);

				string message = "Here is your Host Admin Login information:\nUserName: {0}\nPassword: {1}\n\nPlease disregard this message if you did not request it.";
				EmailProvider mail = EmailProvider.Instance();

				string hostAdminEmail = ConfigurationManager.AppSettings["HostEmailAddress"];
				if (hostAdminEmail == null || hostAdminEmail.Length == 0 || hostAdminEmail.IndexOf('@') <= 0) //Need better email validation. I know!
				{
					log.Debug("No Host Email Address specified in Web.config");
					Message.Text = "Sorry, but I don&#8217;t know where to send the email.  Please specify a Host Email Address in Web.config. It is the AppSetting &#8220;HostEmailAddress&#8221;";
					return;
				}
				string To = hostAdminEmail;
				string From = mail.AdminEmail;
				string Subject = "Subtext Host Admin Login Credentials";
				string Body = string.Format(message, HostInfo.Instance.HostUserName, password);
				mail.Send(To, From, Subject, Body);
				Message.Text = "Login Credentials Sent<br />";
				messageSent = true;
			}

			if (!messageSent)
			{
				log.Warn("Failed request to reset password for " + tbUserName.Text);
				Message.Text = "I don't know you";
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

