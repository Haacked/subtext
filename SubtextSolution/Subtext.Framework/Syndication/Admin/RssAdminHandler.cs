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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Subtext.Extensibility;
using Subtext.Framework.Components;
using Subtext.Framework.Configuration;
using Subtext.Framework.Logging;
using Subtext.Framework.Providers;

namespace Subtext.Framework.Syndication.Admin
{
    public class RssAdminHandler : EntryCollectionHandler<object>
    {
        int count;
        string[] filters;
        string rssType = "";
        string title = "";

        public RssAdminHandler(ISubtextContext subtextContext) : base(subtextContext)
        {
        }

        protected override bool RequiresAdminRole
        {
            get { return true; }
        }

        protected override BaseSyndicationWriter SyndicationWriter
        {
            get
            {
                IList feed = GetFeedEntriesSimple();
                if(feed is ICollection<FeedbackItem>)
                {
                    //TODO: Test the admin feeds
                    var entry = new Entry(PostType.None);
                    entry.Title = title;
                    entry.Body = string.Empty;

                    var feedback = (ICollection<FeedbackItem>)feed;
                    return new CommentRssWriter(HttpContext.Response.Output, feedback, entry, SubtextContext);
                }
                if(feed is ICollection<Referrer>)
                {
                    var referrers = (ICollection<Referrer>)feed;
                    DateTime lastReferrer = NullValue.NullDateTime;
                    if(referrers.Count > 0)
                    {
                        lastReferrer = referrers.First().LastReferDate;
                    }
                    return new ReferrerRssWriter(HttpContext.Response.Output, referrers, lastReferrer, UseDeltaEncoding,
                                                 SubtextContext);
                }
                if(feed is ICollection<LogEntry>)
                {
                    var entries = (ICollection<LogEntry>)feed;
                    return new LogRssWriter(HttpContext.Response.Output, entries, UseDeltaEncoding, SubtextContext);
                }
                return null;
            }
        }

        protected override bool IsMainfeed
        {
            get { return false; }
        }

        protected override bool IsLocalCacheOK()
        {
            string dt = LastModifiedHeader;

            if(dt != null)
            {
                IList ec = GetFeedEntriesSimple();

                if(ec != null && ec.Count > 0)
                {
                    //Get the first entry.
                    object entry = default(object);
                    //TODO: Probably change GetFeedEntries to return ICollection<Entry>
                    foreach(object en in ec)
                    {
                        entry = en;
                        break;
                    }
                    return
                        DateTime.Compare(DateTime.Parse(dt, CultureInfo.InvariantCulture),
                                         ConvertLastUpdatedDate(GetItemCreatedDate(entry))) == 0;
                }
            }
            return false;
        }

        protected void SetOptions()
        {
            if(!Int32.TryParse(HttpContext.Request.QueryString["Count"], out count))
            {
                count = Config.Settings.ItemCount;
            }

            //TODO: Use route data instead.
            if(Regex.IsMatch(HttpContext.Request.Url.PathAndQuery, "ModeratedCommentRss", RegexOptions.IgnoreCase))
            {
                title = "Comments requiring your approval.";
                filters = new string[] {"NeedsModeration"};
                rssType = "Comment";
                return;
            }

            if(Regex.IsMatch(HttpContext.Request.Url.PathAndQuery, "ReferrersRss", RegexOptions.IgnoreCase))
            {
                title = "Referrals";
                rssType = "Referral";
                return;
            }

            if(Regex.IsMatch(HttpContext.Request.Url.PathAndQuery, "ErrorsRss", RegexOptions.IgnoreCase))
            {
                title = "Errors";
                rssType = "Log";
                return;
            }

            title = HttpContext.Request["Title"];
            rssType = HttpContext.Request.QueryString["Type"];

            string qryFilters = HttpContext.Request.QueryString["Filter"];
            if(String.IsNullOrEmpty(qryFilters))
            {
                filters = new string[] {};
            }
            else
            {
                filters = qryFilters.Split('+');
            }
        }

        protected override void ProcessFeed()
        {
            SetOptions();
            base.ProcessFeed();
        }

        protected override ICollection<object> GetFeedEntries()
        {
            throw new NotImplementedException();
        }

        protected IList GetFeedEntriesSimple()
        {
            if(String.IsNullOrEmpty(rssType))
            {
                throw new ArgumentNullException("rssType");
            }

            ObjectProvider repository = ObjectProvider.Instance();

            switch(rssType)
            {
                case "Comment":
                    FeedbackStatusFlag flags = FeedbackStatusFlag.None;

                    foreach(string filter in filters)
                    {
                        if(Enum.IsDefined(typeof(FeedbackStatusFlag), filter))
                        {
                            flags |= (FeedbackStatusFlag)Enum.Parse(typeof(FeedbackStatusFlag), filter, true);
                        }
                    }

                    ICollection<FeedbackItem> moderatedFeedback = repository.GetPagedFeedback(0, count, flags,
                                                                                              FeedbackStatusFlag.None,
                                                                                              FeedbackType.None);
                    return (IList)moderatedFeedback;

                case "Referral":
                    //TODO: Fix!
                    ICollection<Referrer> referrers = repository.GetPagedReferrers(0, count, NullValue.NullInt32);
                    return (IList)referrers;
                case "Log":
                    ICollection<LogEntry> entries = LoggingProvider.Instance().GetPagedLogEntries(0, count);
                    return (IList)entries;
            }


            return null;
        }

        protected override DateTime GetItemCreatedDate(object item)
        {
            if(item is FeedbackItem)
            {
                return ((FeedbackItem)item).DateCreated;
            }
            if(item is Referrer)
            {
                return ((Referrer)item).LastReferDate;
            }
            if(item is LogEntry)
            {
                return ((LogEntry)item).Date;
            }
            return DateTime.Now;
        }
    }
}