<%@ Page language="c#" Codebehind="EditGalleries.aspx.cs" AutoEventWireup="false" Inherits="Subtext.Web.Admin.Pages.EditGalleries" %>
<%@ Register TagPrefix="ANW" Namespace="Subtext.Web.Admin.WebUI" Assembly="Subtext.Web.Admin" %>
<ANW:Page runat="server" id="PageContainer" TabSectionID="Galleries" CategoryType="ImageCollection"><ANW:MessagePanel id=Messages runat="server" ErrorIconUrl="~/admin/resources/ico_critical.gif" ErrorCssClass="ErrorPanel" MessageIconUrl="~/admin/resources/ico_info.gif" MessageCssClass="MessagePanel"></ANW:MessagePanel><ANW:AdvancedPanel id=Results runat="server" LinkStyle="Image" LinkImageCollapsed="~/admin/resources/toggle_gray_down.gif" LinkImage="~/admin/resources/toggle_gray_up.gif" LinkBeforeHeader="True" DisplayHeader="True" HeaderCssClass="CollapsibleHeader" HeaderText="Galleries" Collapsible="True"><asp:DataGrid id=dgrSelectionList runat="server" CssClass="Listing" GridLines="None" AutoGenerateColumns="False">
			<AlternatingItemStyle CssClass="Alt"></AlternatingItemStyle>
			<HeaderStyle CssClass="Header"></HeaderStyle>

			<Columns>
				<asp:TemplateColumn HeaderText="Gallery">
					<ItemTemplate>
						<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Title") %>' ID="Label1" NAME="Label1"></asp:Label>
						<br>
						<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>' ID="Label3" NAME="Label1"></asp:Label>
					</ItemTemplate>

					<EditItemTemplate>
						Title<br>
						<asp:TextBox width = "350" id="txbTitle" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Title") %>'></asp:TextBox>
						<br>Description<br>
						<asp:TextBox width = "350" rows="5" textmode="MultiLine" id="txbDescription" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Description") %>'></asp:TextBox>
					</EditItemTemplate>
				</asp:TemplateColumn>

				<asp:TemplateColumn HeaderText="Visible">
					<ItemTemplate>
						<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IsActive") %>' ID="Label2"></asp:Label>
					</ItemTemplate>

					<EditItemTemplate>
						<asp:CheckBox id="ckbIsActive" runat="server" Checked='<%# DataBinder.Eval(Container, "DataItem.IsActive") %>'/>
					</EditItemTemplate>
				</asp:TemplateColumn>
				
				<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="Update" CancelText="Cancel" EditText="Edit" />
				
				<asp:ButtonColumn Text="Delete" CommandName="Delete" />
			</Columns>
		</asp:DataGrid>
		<ANW:AdvancedPanel id=Add runat="server" DisplayHeader="true" HeaderCssClass="CollapsibleTitle" HeaderText="Add New Gallery" Collapsible="False" BodyCssClass="Edit">
		<LABEL class=Block>Title</LABEL> 
		<asp:TextBox id=txbNewTitle runat="server" width="350"></asp:TextBox>&nbsp; 
		Visible <asp:CheckBox id=ckbNewIsActive runat="server" Checked="true"></asp:CheckBox>
		<br>
		<label class="Block">Description (1000 characters including HTML)</lable><br>
		<asp:TextBox id="txbNewDescription" max = "1000"  runat="server" width="450" rows="5" textmode="MultiLine"></asp:TextBox>
<DIV style="MARGIN-TOP: 8px"><asp:linkbutton id=lkbPost runat="server" CssClass="Button" Text="Add"></asp:linkbutton><BR>&nbsp; 
</DIV></ANW:AdvancedPanel></ANW:AdvancedPanel><ASP:Panel id=ImagesDiv runat="server"><ANW:AdvancedPanel id=AddImages runat="server" LinkStyle="Image" LinkImageCollapsed="~/admin/resources/toggle_gray_down.gif" LinkImage="~/admin/resources/toggle_gray_up.gif" LinkBeforeHeader="True" DisplayHeader="true" HeaderCssClass="CollapsibleTitle" HeaderText="Add New Image" Collapsible="True" BodyCssClass="Edit"><LABEL 
class=Block>Local File Location</LABEL> <INPUT class=FileUpload id=ImageFile 
type=file size=82 name=ImageFile runat="server"> <BR class=Clear><LABEL 
class=Block>Image Description</LABEL> <asp:TextBox id=txbImageTitle runat="server" size="82"></asp:TextBox>&nbsp; 
Visible <asp:CheckBox id=ckbIsActiveImage runat="server" Checked="true"></asp:CheckBox>
<DIV style="MARGIN-TOP: 8px"><asp:linkbutton id=lbkAddImage runat="server" CssClass="Button" Text="Add"></asp:linkbutton><BR>&nbsp; 
</DIV></ANW:AdvancedPanel>
<H1><ASP:PlaceHolder id=plhImageHeader runat="server"></ASP:PlaceHolder></H1><ASP:Repeater id=rprImages runat="server"> 
			<HeaderTemplate> 			
				<div class="ImageList">
			</HeaderTemplate>
			<ItemTemplate>
					<div class="ImageThumbnail">
						<div class="ImageThumbnailImage">
							<asp:HyperLink id="lnkThumbnail" runat="server" ImageUrl='<%# EvalImageUrl(Container.DataItem) %>' NavigateUrl='<%# EvalImageNavigateUrl(Container.DataItem) %>'/>
						</div>
						<div class="ImageThumbnailTitle">
							<%# EvalImageTitle(Container.DataItem) %>
							<br>
							<a href='EditImage.aspx?imgid=<%# DataBinder.Eval(Container.DataItem, "ImageID") %>'>Edit</a>
							&nbsp;&bull;&nbsp;
							<asp:linkbutton id="lnkDeleteImage" CommandName="DeleteImage" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageID") %>' Text="Delete" runat="server" />
						</div>
					</div>				
			</ItemTemplate>
			<FooterTemplate>
				</div>
			</FooterTemplate>
		</ASP:Repeater><BR class=Clear></ASP:Panel>
</ANW:Page>
