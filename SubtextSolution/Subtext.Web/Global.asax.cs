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
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.Routing;
using log4net;
using Subtext.Framework;
using Subtext.Framework.Configuration;
using Subtext.Framework.Data;
using Subtext.Framework.Exceptions;
using Subtext.Framework.Logging;
using Subtext.Framework.Providers;
using Subtext.Framework.Web.HttpModules;
using System.Web.Mvc;
using Subtext.Web.Infrastructure;
using Subtext.Infrastructure;

namespace Subtext.Web
{
    public class Global : HttpApplication
    {
        //This call is to kickstart log4net.
        //log4net Configuration Attribute is in AssemblyInfo
        private readonly static ILog log = new Log(LogManager.GetLogger(typeof(Global)));

        bool _logInitialized = false;

        /// <summary>
        /// Method called by the application on startup.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Start(Object sender, EventArgs e)
        {
            _logInitialized = true;
            Routes.RegisterRoutes(RouteTable.Routes);
            Bootstrapper.InitializeKernel(new Dependencies());
            var factory = new SubtextControllerFactory(Bootstrapper.Kernel);
            ControllerBuilder.Current.SetControllerFactory(factory);
        }
	
        /// <summary>
        /// <para>
        /// This is used to vary partial caching of ASCX controls and ASPX pages on a per blog basis.  
        /// You can see this in action via the [PartialCaching] attribute.
        /// </para>
        /// <para>
        /// Provides an application-wide implementation of the <see cref="P:System.Web.UI.PartialCachingAttribute.VaryByCustom"/> property.
        /// </para>
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> that contains information about the current Web request</param>
        /// <param name="custom"></param>
        /// <returns>
        /// If the value of the <paramref name="custom"/> parameter is "browser", the browser's
        /// <see cref="System.Web.HttpBrowserCapabilities.Type"/> ; otherwise,
        /// <see langword="null"/> .
        /// </returns>
        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            if(custom == "Blogger")
            {
                return Config.CurrentBlog.Id.ToString(CultureInfo.InvariantCulture);
            }

            return base.GetVaryByCustomString(context, custom);
        }

        private const string ErrorPageLocation = "~/SystemMessages/error.aspx";
        private const string BadConnectionStringPage = "~/SystemMessages/CheckYourConnectionString.aspx";
        private const string DatabaseLoginFailedPage = "~/SystemMessages/DatabaseLoginFailed.aspx";

        /// <summary>
        /// Method called during at the beginning of each request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            if (!_logInitialized) {
                //This line will trigger the configuration.
                log.Info("Application_Start - This is not a malfunction.");
                _logInitialized = true;
            }
            
            //KLUDGE: This is required due to a bug in Log4Net 1.2.9.
            // This should be fixed in the next release.
            Log.SetBlogIdContext(NullValue.NullInt32);
        }

        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if(exception is HttpUnhandledException)
            {
                if(exception.InnerException == null)
                {
                    Server.Transfer(ErrorPageLocation, false);
                    return;
                }
                exception = exception.InnerException;
            }

            BaseCommentException commentException = exception as BaseCommentException;
            if (commentException != null)
            {
                string message = "Comment exception thrown and handled in Global.asax.";
                if(HttpContext.Current != null && HttpContext.Current.Request != null)
                {
                    message += "-- User Agent: " + HttpContext.Current.Request.UserAgent;
                }
                log.Info(message, commentException);
            }
			
            //Sql Exception and request is for "localhost"
            SqlException sqlExc = exception as SqlException;
            if (sqlExc != null)
            {
                if (sqlExc.Number == (int)SqlErrorMessage.SqlServerDoesNotExistOrAccessDenied
                    || (sqlExc.Number == (int)SqlErrorMessage.CouldNotFindStoredProcedure && sqlExc.Message.IndexOf("'blog_GetConfig'") > 0)
                    )
                {
                    // Probably a bad connection string.
                    Server.Transfer(BadConnectionStringPage);
                    return;
                }

                if (sqlExc.Number == (int)SqlErrorMessage.LoginFailsCannotOpenDatabase
                    || sqlExc.Number == (int)SqlErrorMessage.LoginFailed
                    || sqlExc.Number == (int)SqlErrorMessage.LoginFailedInvalidUserOfTrustedConnection
                    || sqlExc.Number == (int)SqlErrorMessage.LoginFailedNotAssociatedWithTrustedConnection
                    || sqlExc.Number == (int)SqlErrorMessage.LoginFailedUserNameInvalid
                    )
                {
                    // Probably a bad connection string.
                    Server.Transfer(DatabaseLoginFailedPage);
                    return;
                }
            }

            InstallationManager installManager = new InstallationManager(Subtext.Extensibility.Providers.Installation.Provider);
            BlogRequest blogRequest = BlogRequest.Current;
            if (blogRequest.RequestLocation != RequestLocation.Installation && blogRequest.RequestLocation != RequestLocation.Upgrade) {
                if (installManager.InstallationActionRequired(exception, VersionInfo.FrameworkVersion))
                {
                    Response.Redirect("~/Install/", true);
                    return;
                }

                // User could be logging into the HostAdmin.
                if(exception.GetType() == typeof(BlogDoesNotExistException))
                {
                    Response.Redirect("~/Install/BlogNotConfiguredError.aspx", true);
                    return;
                }
            }

            if(blogRequest.RequestLocation != RequestLocation.SystemMessages)
            {
                if(exception.GetType() == typeof(BlogInactiveException))
                {
                    HttpContext.Current.Response.Redirect("~/SystemMessages/BlogNotActive.aspx");
                }
            }

            if(exception is InvalidOperationException && exception.Message.IndexOf("ConnectionString") >= 0)
            {
                // Probably a missing connection string.
                Server.Transfer(BadConnectionStringPage);
                return;
            }

            if(exception is ArgumentException 
               && (
                      exception.Message.IndexOf("Keyword not supported") >= 0
                      ||	exception.Message.IndexOf("Invalid value for key") >= 0
                  )
                )
            {
                // Probably a malformed connection string.
                Server.Transfer(BadConnectionStringPage);
                return;
            }

            if(exception is HttpException)
            {
                if(((HttpException)exception).GetHttpCode() == 404)
                {
                    return;
                }
            }

            // I don't know that Context can ever be null in the pipe, but we'll play it
            // extra safe. If customErrors are off, we'll just let ASP.NET default happen.
            if (Context != null && Context.IsCustomErrorEnabled)
            {
                Server.Transfer(ErrorPageLocation, false);
            }
            else
            {
                log.Error("Unhandled Exception trapped in Global.asax", exception);
            }
        }

        /// <summary>
        /// Handles the End event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_End(Object sender, EventArgs e)
        {
            _logInitialized = false;
        }
    }
}