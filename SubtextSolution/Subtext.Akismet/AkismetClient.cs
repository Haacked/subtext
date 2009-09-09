using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Web;
using Subtext.Akismet.Properties;

namespace Subtext.Akismet
{
    /// <summary>
    /// The client class used to communicate with the 
    /// <see href="http://akismet.com/">Akismet</see> service.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Akismet")]
    [Serializable]
    public class AkismetClient
    {
        const string checkUrlFormat = "http://{0}.rest.akismet.com/1.1/comment-check";
        const string submitHamUrlFormat = "http://{0}.rest.akismet.com/1.1/submit-ham";
        const string submitSpamUrlFormat = "http://{0}.rest.akismet.com/1.1/submit-spam";
        static readonly Uri verifyUrl = new Uri("http://rest.akismet.com/1.1/verify-key");
        static readonly string version = typeof(HttpClient).Assembly.GetName().Version.ToString();
        string apiKey;
        Uri checkUrl;
        [NonSerialized] private HttpClient httpClient;
        Uri submitHamUrl;
        Uri submitSpamUrl;
        string userAgent;

        protected AkismetClient()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AkismetClient"/> class.
        /// </summary>
        /// <param name="apiKey">The Akismet API key.</param>
        /// <param name="blogUrl">The root url of the blog.</param>
        public AkismetClient(string apiKey, Uri blogUrl)
            : this(apiKey, blogUrl, new HttpClient())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AkismetClient"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor takes in all the dependencies to allow for 
        /// dependency injection and unit testing. Seems like overkill, 
        /// but it's worth it.
        /// </remarks>
        /// <param name="apiKey">The Akismet API key.</param>
        /// <param name="blogUrl">The root url of the blog.</param>
        /// <param name="httpClient">Client class used to make the underlying requests.</param>
        public AkismetClient(string apiKey, Uri blogUrl, HttpClient httpClient)
        {
            if(apiKey == null)
            {
                throw new ArgumentNullException("apiKey");
            }

            if(blogUrl == null)
            {
                throw new ArgumentNullException("blogUrl");
            }

            if(httpClient == null)
            {
                throw new ArgumentNullException("httpClient");
            }

            this.apiKey = apiKey;
            BlogUrl = blogUrl;
            this.httpClient = httpClient;
            Timeout = 5000; /* default */
            SetServiceUrls();
        }

        /// <summary>
        /// Gets or sets the Akismet API key.
        /// </summary>
        /// <value>The API key.</value>
        public string ApiKey
        {
            get { return apiKey ?? string.Empty; }
            set
            {
                apiKey = value ?? string.Empty;
                SetServiceUrls();
            }
        }

        /// <summary>
        /// Gets or sets the Usera Agent for the Akismet Client.  
        /// Do not confuse this with the user agent for the comment 
        /// being checked.
        /// </summary>
        /// <value>The API key.</value>
        public string UserAgent
        {
            get { return userAgent ?? BuildUserAgent("Subtext", version); }
            set { userAgent = value; }
        }

        /// <summary>
        /// Gets or sets the timeout in milliseconds for the http request to Akismet. 
        /// By default 5000 (5 seconds).
        /// </summary>
        /// <value>The timeout.</value>
        public int Timeout { get; set; }

        /// <summary>
        /// Gets or sets the root URL to the blog.
        /// </summary>
        /// <value>The blog URL.</value>
        public Uri BlogUrl { get; set; }

        /// <summary>
        /// Gets or sets the proxy to use.
        /// </summary>
        /// <value>The proxy.</value>
        public IWebProxy Proxy { get; set; }

        void SetServiceUrls()
        {
            submitHamUrl = new Uri(String.Format(CultureInfo.InvariantCulture, submitHamUrlFormat, apiKey));
            submitSpamUrl = new Uri(String.Format(CultureInfo.InvariantCulture, submitSpamUrlFormat, apiKey));
            checkUrl = new Uri(String.Format(CultureInfo.InvariantCulture, checkUrlFormat, apiKey));
        }

        /// <summary>
        /// Helper method for building a user agent string in the format 
        /// preferred by Akismet.
        /// </summary>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="appVersion">The version of the app.</param>
        /// <returns></returns>
        public static string BuildUserAgent(string applicationName, string appVersion)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}/{1} | Akismet/1.11", applicationName, appVersion);
        }

        /// <summary>
        /// Verifies the API key.  You really only need to
        /// call this once, perhaps at startup.
        /// </summary>
        /// <returns></returns>
        /// <exception type="Sytsem.Web.WebException">If it cannot make a request of Akismet.</exception>
        public bool VerifyApiKey()
        {
            string parameters = "key=" + HttpUtility.UrlEncode(ApiKey) + "&blog=" +
                                HttpUtility.UrlEncode(BlogUrl.ToString());
            string result = httpClient.PostRequest(verifyUrl, UserAgent, Timeout, parameters, Proxy);

            if(String.IsNullOrEmpty(result))
            {
                throw new InvalidResponseException(Resources.InvalidResponse_EmptyResponse);
            }

            return String.Equals("valid", result, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks the comment and returns true if it is spam, otherwise false.
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public bool CheckCommentForSpam(IComment comment)
        {
            string result = SubmitComment(comment, checkUrl);

            if(String.IsNullOrEmpty(result))
            {
                throw new InvalidResponseException(Resources.InvalidResponse_EmptyResponse);
            }

            if(result != "true" && result != "false")
            {
                throw new InvalidResponseException(string.Format(CultureInfo.InvariantCulture,
                                                                 Resources.InvalidResponse_PossiblyBadApiKey, result));
            }

            return bool.Parse(result);
        }

        /// <summary>
        /// Submits a comment to Akismet that should have been 
        /// flagged as SPAM, but was not flagged by Akismet.
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public virtual void SubmitSpam(IComment comment)
        {
            SubmitComment(comment, submitSpamUrl);
        }

        /// <summary>
        /// Submits a comment to Akismet that should not have been 
        /// flagged as SPAM (a false positive).
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public void SubmitHam(IComment comment)
        {
            SubmitComment(comment, submitHamUrl);
        }

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        string SubmitComment(IComment comment, Uri url)
        {
            //Not too many concatenations.  Might not need a string builder.
            string parameters = "blog=" + HttpUtility.UrlEncode(BlogUrl.ToString())
                                + "&user_ip=" + comment.IPAddress.ToString()
                                + "&user_agent=" + HttpUtility.UrlEncode(comment.UserAgent);

            if(!String.IsNullOrEmpty(comment.Referrer))
            {
                parameters += "&referer=" + HttpUtility.UrlEncode(comment.Referrer);
            }

            if(comment.Permalink != null)
            {
                parameters += "&permalink=" + HttpUtility.UrlEncode(comment.Permalink.ToString());
            }

            if(!String.IsNullOrEmpty(comment.CommentType))
            {
                parameters += "&comment_type=" + HttpUtility.UrlEncode(comment.CommentType);
            }

            if(!String.IsNullOrEmpty(comment.Author))
            {
                parameters += "&comment_author=" + HttpUtility.UrlEncode(comment.Author);
            }

            if(!String.IsNullOrEmpty(comment.AuthorEmail))
            {
                parameters += "&comment_author_email=" + HttpUtility.UrlEncode(comment.AuthorEmail);
            }

            if(comment.AuthorUrl != null)
            {
                parameters += "&comment_author_url=" + HttpUtility.UrlEncode(comment.AuthorUrl.ToString());
            }

            if(!String.IsNullOrEmpty(comment.Content))
            {
                parameters += "&comment_content=" + HttpUtility.UrlEncode(comment.Content);
            }

            if(comment.ServerEnvironmentVariables != null)
            {
                foreach(string key in comment.ServerEnvironmentVariables)
                {
                    parameters += "&" + key + "=" + HttpUtility.UrlEncode(comment.ServerEnvironmentVariables[key]);
                }
            }

            return httpClient.PostRequest(url, UserAgent, Timeout, parameters).ToLowerInvariant();
        }
    }
}