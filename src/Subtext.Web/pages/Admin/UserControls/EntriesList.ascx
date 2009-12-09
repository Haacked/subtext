﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EntriesList.ascx.cs" Inherits="Subtext.Web.Admin.UserControls.EntriesList" %>

<h2 id="title" runat="server">Title</h2>

<asp:Repeater id="rprSelectionList" runat="server">
	<HeaderTemplate>
		<table id="Listing" class="listing highlightTable" cellspacing="0" cellpadding="0" border="0">
			<tr>
				<th>Description</th>
				<th width="50">Active</th>
				<th width="75">Web Views</th>
				<th width="75">Agg Views</th>
				<th width="50">Referrals</th>
				<th width="50">&nbsp;</th>
				<th width="50">&nbsp;</th>
			</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td>
			    <asp:HyperLink runat="server" NavigateUrl='<%# Url.EntryUrl((IEntryIdentity)Container.DataItem) %>' ToolTip="View Entry" >
			        <%# GetEntry(Container.DataItem).Title %>
			    </asp:HyperLink>
			</td>
			<td>
				<%# IsActiveText(Container.DataItem)%>
			</td>												
			<td>
				<%# GetEntry(Container.DataItem).WebCount %>
			</td>
			<td>
				<%# GetEntry(Container.DataItem).AggCount %>
			</td>				
			<td>
				<a href="<%# ReferrersUrl(Container.DataItem) %>" title="View Referrals">View</a>
			</td>				
			<td>
				<a href="<%# PostsEditUrl(Container.DataItem) %>" title="Edit Post">Edit</a>
			</td>
			<td>
				<asp:LinkButton id="lnkDelete" CausesValidation="False" CommandName="Delete" CommandArgument='<%# GetEntry(Container.DataItem).Id %>' Text="Delete" runat="server" CssClass="confirm-delete" />
			</td>
		</tr>
	</ItemTemplate>
	<AlternatingItemTemplate>
		<tr class="alt">
			<td>
				<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Url.EntryUrl((IEntryIdentity)Container.DataItem) %>' ToolTip="View Entry" >
			        <%# GetEntry(Container.DataItem).Title %>
			    </asp:HyperLink>
			</td>
			<td>
				<%# IsActiveText(Container.DataItem)%>
			</td>
			<td>
				<%# GetEntry(Container.DataItem).WebCount %>
			</td>
			<td>
				<%# GetEntry(Container.DataItem).AggCount %>
			</td>					
			<td>
				<a href="../Referrers.aspx?EntryID=<%# GetEntry(Container.DataItem).Id %>" title="View Referrals">View</a>
			</td>				
			<td>
				<a href="Edit.aspx?PostId=<%# Eval("Id") %>" title="Edit Post">Edit</a>
			</td>
			<td>
				<asp:LinkButton id="lnkDeleteAlt" CausesValidation="False" CommandName="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Id") %>' Text="Delete" runat="server" CssClass="confirm-delete" />
			</td>
		</tr>
	</AlternatingItemTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
</asp:Repeater>

<p id="NoMessagesLabel" runat="server" visible="false">No entries found.</p>
	
<st:PagingControl id="resultsPager" runat="server" 
		PrefixText="<div>Goto page</div>" 
		LinkFormatActive='<a href="{0}" class="Current">{1}</a>' 
		UrlFormat="Default.aspx?pg={0}" 
		CssClass="Pager" />
<br class="clear" />
