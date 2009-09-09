using System;
using System.Globalization;
using Subtext.Extensibility;
using Subtext.Framework;
using Subtext.Framework.Format;

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

namespace Subtext.Web.UI.Controls
{
    /// <summary>
    ///		Summary description for ArchiveDay.
    /// </summary>
    public class MonthList : BaseControl
    {
        protected EntryList MonthListings;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if(Context != null)
            {
                DateTime dt = UrlFormats.DateFromUrl(Request.Path);
                MonthListings.DescriptionOnly = true;
                MonthListings.EntryListItems = Entries.GetPostsByDayRange(dt, dt.AddMonths(1), PostType.BlogPost, true);
                MonthListings.EntryListTitle = dt.ToString("y", CultureInfo.CurrentCulture);
            }
        }
    }
}