<%@ Control Language="c#" Inherits="Subtext.Web.UI.Controls.Day" %>
<%@ Import Namespace = "Subtext.Framework" %>
			<span class="daylink"><asp:HyperLink Runat="server" title="Day Archive" BorderWidth="0" ID="ImageLink" ><asp:Literal ID = "DateTitle" Runat = "server" /></asp:HyperLink></span>

			<asp:Repeater runat="Server" Runat="server" ID="DayList" OnItemCreated="PostCreated">
				<ItemTemplate>
					<div class="blogpost">
						<h1><span class="title"><asp:HyperLink Runat="server" ID="TitleUrl" /></span></h1>
						<asp:Literal  runat="server" ID="PostText" />
						<p class="postfooter">
							<asp:Literal ID = "PostDesc" Runat = "server" />
						</p>
					</div> <!-- end #blogpost -->
				</ItemTemplate>
			</asp:Repeater>
