<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1.Default" ViewStateMode="Disabled"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Summer Gallery</title>
    <link href="Content/styles/main.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Summer Gallery</h1>
        <div id="BigImageArea">
            <asp:Image ID="BigImage" runat="server" Visible="true" />
        </div>

        <div id="thumbnails">
            <asp:Repeater ID="Repeater1" runat="server" ItemType="System.String" SelectMethod="ThumbnailRepeater_GetData">
                <HeaderTemplate><ul></HeaderTemplate>
                <ItemTemplate>
                    <asp:HyperLink ID="ImageHyperLink" runat="server" NavigateUrl='<%# "?" + Item.ToString() %>'>
                        <asp:Image ID="Image1" runat="server" ImageUrl='<%# "~/Content/images/thumbnails/" + Item.ToString() %>' />
                    </asp:HyperLink>
                </ItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
        </div>

        <div id="uploadControls">
            <asp:FileUpload ID="ImageFileUpload" runat="server" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Ingen fil vald." ControlToValidate="ImageFileUpload" Display="None" />            
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Filen måste vara jpg/gif/png." ControlToValidate="ImageFileUpload" Display="None" ValidationExpression="^.*\.(gif|jpg|png|jpeg)$" />

            <asp:Button ID="UploadButton" runat="server" Text="Upload" OnClick="UploadButton_Click" />

            <asp:PlaceHolder ID="SuccessPlaceHolder" runat="server">
                <div id="successDiv">
                    <div id="closeSuccess">
                        x
                    </div>
                        <asp:Label ID="SuccessLabel" runat="server" Text="" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="ErrorPlaceHolder" runat="server" Visible="true">
                <div id="errorDiv">
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                </div>
            </asp:PlaceHolder>
        </div>

    </div>
    <script src="Scripts/main.js"></script>
    </form>
</body>
</html>
