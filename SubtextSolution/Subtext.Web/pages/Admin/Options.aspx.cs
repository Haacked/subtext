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
using System.Web.UI.WebControls;

namespace Subtext.Web.Admin.Pages
{
    public partial class AdminOptionsPage : AdminPage
    {
        public AdminOptionsPage()
        {
            TabSectionId = "Options";
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                BindLocalUI();
            }
            BindActionsListUI();
        }

        protected virtual void BindActionsListUI()
        {
            HyperLink lnkConfigure = Utilities.CreateHyperLink("Configure", "Configure.aspx");
            HyperLink lnkSkins = Utilities.CreateHyperLink("Skins", "Skins.aspx");
            HyperLink lnkCustomize = Utilities.CreateHyperLink("Customize", "Customize.aspx");
            HyperLink lnkPreferences = Utilities.CreateHyperLink("Preferences", "Preferences.aspx");
            HyperLink lnkSyndication = Utilities.CreateHyperLink("Syndication", "Syndication.aspx");
            HyperLink lnkComments = Utilities.CreateHyperLink("Comments", "Comments.aspx");
            HyperLink linkKeyWords = Utilities.CreateHyperLink("Key Words", "EditKeyWords.aspx");
            HyperLink lnkSecurity = Utilities.CreateHyperLink("Security", "Security.aspx");
            HyperLink lnkImportExport = Utilities.CreateHyperLink("Import/Export", "ImportExport.aspx");


            // Add the buttons to the PageContainer.
            AdminMasterPage.ClearActions();
            AdminMasterPage.AddToActions(lnkConfigure);
            AdminMasterPage.AddToActions(lnkSkins);
            AdminMasterPage.AddToActions(lnkCustomize);
            AdminMasterPage.AddToActions(lnkPreferences);
            AdminMasterPage.AddToActions(lnkSyndication);
            AdminMasterPage.AddToActions(lnkComments);
            AdminMasterPage.AddToActions(linkKeyWords);
            AdminMasterPage.AddToActions(lnkSecurity);
            AdminMasterPage.AddToActions(lnkImportExport);
        }

        protected virtual void BindLocalUI()
        {
        }
    }
}