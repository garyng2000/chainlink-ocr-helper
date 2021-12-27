<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Test.aspx.cs" Inherits="Test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox ID="cTestJSON" TextMode="MultiLine" Width="800" Height="600" runat="server">

            </asp:TextBox>
            <asp:Button ID="cTestBtn" runat="server" Text="Send" OnClick="TestBtn_Click" />
            <asp:TextBox ID="cResult" TextMode="MultiLine" Width="800" Height="600" runat="server">

            </asp:TextBox>
        </div>
    </form>
</body>
</html>
