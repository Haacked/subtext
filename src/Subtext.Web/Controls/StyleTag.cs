#region Disclaimer/Info

///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at Google Code at http://code.google.com/p/subtext/
// The development mailing list is at subtext@googlegroups.com 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Subtext.Framework.Web;

namespace Subtext.Web.Controls
{
    /// <summary>
    /// This is used to render a client side script tag using the 
    /// the ~/ syntax for the script path.
    /// </summary>
    public class StyleTag : HtmlControl
    {
        /// <summary>
        /// Gets or sets the SRC, the path to the script file.
        /// </summary>
        /// <value></value>
        public string Href
        {
            get
            {
                if(IsAttributeDefined("href"))
                {
                    return ConvertToAppPath(Attributes["href"]);
                }
                return string.Empty;
            }
            set
            {
                if(value.Length == 0)
                {
                    value = null;
                }

                Attributes["href"] = value;
            }
        }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        /// <value></value>
        public override string TagName
        {
            get
            {
                //TODO: shouldn't this be "style", or "link"?
                return "script";
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/>
        /// event. Sets the <see cref="Control.EnableViewState"/> property to false.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            EnableViewState = false;
            base.OnInit(e);
        }

        /// <summary>
        /// Renders this script tag.
        /// </summary>
        /// <param name="writer">Writer.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            const string format = @"<link href=""{0}"" type=""text/css"" rel=""stylesheet""></link>";
            writer.Write(string.Format(CultureInfo.InvariantCulture, format, Href));
        }

        /// <summary>
        /// Converts to app path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        static string ConvertToAppPath(string path)
        {
            return HttpHelper.ExpandTildePath(path);
        }

        bool IsAttributeDefined(string name)
        {
            return ControlHelper.IsAttributeDefined(this, name);
        }
    }
}