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
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Subtext.Extensibility;
using Subtext.Framework.Components;
using Subtext.Framework.Configuration;
using System.Web.Routing;

namespace Subtext.Framework.Format
{
	/// <summary>
	/// Default Implemenation of UrlFormats
	/// </summary>
	public class UrlFormats
	{
		/// <summary>
		/// Returns a <see cref="DateTime"/> instance parsed from the url.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <returns></returns>
		public static DateTime DateFromUrl(string url)
		{
			string date = GetRequestedFileName(url);
			CultureInfo en = new CultureInfo("en-US");
			switch(date.Length)
			{
				case 8:
					return DateTime.ParseExact(date, "MMddyyyy", en);
				case 6:
					return DateTime.ParseExact(date, "MMyyyy", en);
				default:
					throw new Exception("Invalid Date Format");
			}
		}

		/// <summary>
		/// Gets the name of the requested file.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <returns></returns>
		public static string GetRequestedFileName(string uri)
		{
			return Path.GetFileNameWithoutExtension(uri);
		}
		
		/// <summary>
		/// Parses out the subfolder of the blog from the requested URL.  It 
		/// simply searches for the first "folder" after the host and 
		/// Request.ApplicationPath.
		/// </summary>
		/// <remarks>
		/// <p>
		/// For example, if a blog is hosted at the virtual directory http://localhost/Subtext.Web/ and 
		/// request is made for http://localhost/Subtext.Web/, the subfolder name is "" (empty string). 
		/// Howver, a request for http://localhost/Subtext.Web/MyBlog/ would return "MyBlog" as the 
		/// subfolder.
		/// </p>
		/// <p>
		/// Likewise, if a blog is hosted at http://localhost/, a request for http://localhost/MyBlog/ 
		/// would return "MyBlog" as the subfolder.
		/// </p>
		/// </remarks>
		/// <param name="rawUrl">The raw url.</param>
		/// <param name="applicationPath">The virtual application name as found in the Request.ApplicationName property.</param>
		/// <returns></returns>
		public static string GetBlogSubfolderFromRequest(string rawUrl, string applicationPath)
		{
			if(rawUrl == null)
				throw new ArgumentNullException("rawUrl", "The path cannot be null.");

			if(applicationPath == null)
				throw new ArgumentNullException("applicationPath", "The app should not be null.");

			// The {0} represents a potential virtual directory
			string urlPatternFormat = "{0}/(?<app>.*?)/";

			//Remove any / from App.
			string cleanApp = "/" + StripSurroundingSlashes(applicationPath);
			if(cleanApp == "/")
				cleanApp = string.Empty;
			string appRegex = Regex.Escape(cleanApp);

			string urlRegexPattern = string.Format(CultureInfo.InvariantCulture, urlPatternFormat, appRegex);
			
			Regex urlRegex = new Regex(urlRegexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			Match match = urlRegex.Match(rawUrl);
			if(match.Success)
			{
				return match.Groups["app"].Value;
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// From Jason Block @ http://www.angrycoder.com/article.aspx?cid=5&y=2003&m=4&d=15
		/// Basically, it's [Request.UrlReferrer] doing a lazy initialization of its internal 
		/// _referrer field, which is a Uri-type class. That is, it's not created until it's 
		/// needed. The point is that there are a couple of spots where the UriFormatException 
		/// could leak through. One is in the call to GetKnownRequestHeader(). _wr is a field 
		/// of type HttpWorkerRequest. 36 is the value of the HeaderReferer constant - since 
		/// that's being blocked in this case, it may cause that exception to occur. However, 
		/// HttpWorkerRequest is an abstract class, and it took a trip to the debugger to find 
		/// out that _wr is set to a System.Web.Hosting.ISAPIWorkerRequestOutOfProc object. 
		/// This descends from System.Web.Hosting.ISAPIWorkerRequest, and its implementation 
		/// of GetKnownRequestHeader() didn't seem to be the source of the problem. 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static Uri GetUriReferrerSafe(HttpRequest request)
		{
			try {
				return request.UrlReferrer;
			}
			catch(UriFormatException)
			{
				return null;
			}
    	}

        public static Uri GetUriReferrerSafe(HttpRequestBase request)
        {
            try {
                return request.UrlReferrer;
            }
            catch (UriFormatException)
            {
                return null;
            }
        }

		/// <summary>
		/// Strips the surrounding slashes from the specified string.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <returns></returns>
		public static string StripSurroundingSlashes(string target)
		{
			if(target == null)
				throw new ArgumentNullException("target", "The target to strip slashes from is null.");

			if(target.EndsWith("/"))
				target = target.Remove(target.Length - 1, 1);
			if(target.StartsWith("/"))
				target = target.Remove(0, 1);

			return target;
		}

		/// <summary>
		/// Return the url with the http://host stripped off the front. The given url
		/// may or maynot have the http://host on it.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string StripHostFromUrl(string url)
		{
			string fullHost = string.Format("{0}://{1}", HttpContext.Current.Request.Url.Scheme, Config.CurrentBlog.Host);
			
			if(url.StartsWith(fullHost))
			{
				// use Length b/c we want to leave the beginning "/" character on newUrl
				url = url.Substring(fullHost.Length);

                //Remove port number is present
                if (url.StartsWith(":"))
                {
                    int firstSlash = url.IndexOf('/');
                    url = url.Substring(firstSlash, url.Length - firstSlash);
                }
			}
			return url;
		}

		/// <summary>
		/// Parses out the host from an external URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		public static string GetHostFromExternalUrl(string url)
		{
			string hostDelim = "://";
			
			int hostStart = url.IndexOf(hostDelim);
			hostStart = (hostStart < 0) ? 0 : hostStart + 3;

			int hostEnd = url.IndexOf("/", hostStart);
			
			return (hostEnd < 0) ? url.Substring(hostStart) : url.Substring(hostStart, hostEnd-hostStart);
		}


		#region Url Shortener
		//Original code from: Mads Kristensen
		//http://blog.madskristensen.dk/post/Resolve-and-shorten-URLs-in-Csharp.aspx
		private static readonly Regex regex = new Regex("((http://|www\\.)([A-Z0-9.-:]{1,})\\.[0-9A-Z?;~&#=\\-_\\./]{2,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static readonly string link = "<a href=\"{0}{1}\">{2}</a>";

		public static string ResolveLinks(string body)
		{
			if (string.IsNullOrEmpty(body))
				return body;

			foreach (Match match in regex.Matches(body)) {
				if (!match.Value.Contains("://")) {
					body = body.Replace(match.Value, string.Format(link, "http://", match.Value, ShortenUrlForDisplay(match.Value, 50)));
				}
				else {
					body = body.Replace(match.Value, string.Format(link, string.Empty, match.Value, ShortenUrlForDisplay(match.Value, 50)));
				}
			}

			return body;
		}

		public static string ShortenUrlForDisplay(string url, int max)
		{
			if (url.Length <= max)
				return url;

			// Remove the protocal
			int startIndex = url.IndexOf("://");
            if (startIndex > -1) {
                url = url.Substring(startIndex + 3);
            }

			if (url.Length <= max)
				return url;

			// Remove the folder structure
			int firstIndex = url.IndexOf("/") + 1;
			int lastIndex = url.LastIndexOf("/");
            if (firstIndex < lastIndex) {
                url = url.Replace(url.Substring(firstIndex, lastIndex - firstIndex), "...");
            }

            if (url.Length <= max) {
                return url;
            }

			// Remove URL parameters
			int queryIndex = url.IndexOf("?");
			if (queryIndex > -1)
				url = url.Substring(0, queryIndex);

			if (url.Length <= max)
				return url;

			// Remove URL fragment
			int fragmentIndex = url.IndexOf("#");
			if (fragmentIndex > -1)
				url = url.Substring(0, fragmentIndex);

			if (url.Length <= max)
				return url;

			// Shorten page
			firstIndex = url.LastIndexOf("/") + 1;
			lastIndex = url.LastIndexOf(".");
			if (lastIndex - firstIndex > 10)
			{
				string page = url.Substring(firstIndex, lastIndex - firstIndex);
				int length = url.Length - max + 3;
				if(length<page.Length)//If page length is longer then max length, can not shorten it
					url = url.Replace(page, "..." + page.Substring(length));
			}

			return url;
		}
		#endregion

	}
}
