<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Web.Login" %>

<!DOCTYPE html>
<html>
<head>
    <title>Dev Authentication</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
</head>
<body bgcolor="#FFFFFF" text="#000000">
    <form runat="server">
    <div>
        <table border="0" cellspacing="0" cellpadding="0">
            <tr>
                <td width="80">
                    Username:
                </td>
                <td width="10">
                    &nbsp;
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtUsername" Width="150" />
                </td>
            </tr>
            <tr>
                <td>
                    Password:
                </td>
                <td width="10">
                    &nbsp;
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" Width="150" />
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td width="10">
                    &nbsp;
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="chkPersistLogin" />Remember my credentials
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td width="10">
                    &nbsp;
                </td>
                <td>
                    <asp:Button runat="server" ID="cmdLogin" OnClick="ProcessLogin" Text="Login" />
                </td>
            </tr>
        </table>
        <br />
        <br />
        <div id="divErr" runat="server" enableviewstate="false" />
    </div>
    </form>
</body>
</html>
