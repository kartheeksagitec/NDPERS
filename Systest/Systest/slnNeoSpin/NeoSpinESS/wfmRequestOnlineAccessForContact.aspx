<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wfmRequestOnlineAccessForContact.aspx.cs" Inherits="NeoSpinESS.wfmRequestOnlineAccessForContact" %>

<%@ Register TagPrefix="swc" Namespace="Sagitec.WebControls" Assembly="SagitecWebControls, Version=6.0.0.0, Culture=neutral, PublicKeyToken=2FAFCCD2A44457D5" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="sfwPageHeader" runat="server">
    <title>PERSLink Web Self Service - Request Online Access</title>
    <script type="text/javascript">
          function SetWindowName() {
              if (document.getElementById("hfldLoginWindowName").value !== "") {
                  window.name = document.getElementById("hfldLoginWindowName").value;
              }
          }
      </script>
</head>
<body class="LoginBody">
    <form id="wfmNeoSpinBase" runat="server" submitdisabledcontrols="true">
    <asp:Panel runat="server" ID="pnlBase" DefaultButton="btnRequestAccess">
        <table border="0" align="center" cellpadding="0" cellspacing="0" class="LoginBgESS">
            <tr style="height: 135px;">
                <td>
                </td>
            </tr>
            <tr style="height: 20px;">
                <td colspan="2" style="padding-left: 30px; padding-right: 30px;">
                    <swc:sfwLabel runat="server" ID="lblError" Font-Bold="true" ForeColor="Red" />
                </td>
            </tr>
             <tr style="height: 20px;">
                <td>
                </td>
            </tr>
            <tr style="height: 25px;">
                <td valign="top" align="right">
                    <swc:sfwLabel ID="lblOrgCode" runat="server" Text="Employer Org Code ID :" CssClass="loginTxt"
                        Font-Bold="true"></swc:sfwLabel>
                </td>
                <td valign="top">
                    <swc:sfwTextBox autocomplete="off" CssClass="Logintxtbox" MaxLength="10" runat="server"
                        ID="txbOrgCode" />
                    <asp:RequiredFieldValidator ID="rfvOrgCode" ControlToValidate="txbOrgCode" Text="*"
                        ErrorMessage="Org code is required" runat="server" />
                </td>
            </tr>
            <tr style="height: 25px;">
                <td valign="top" align="right">
                    <swc:sfwLabel ID="lblContactID" runat="server" Text="Contact ID:" CssClass="loginTxt"
                        Font-Bold="true"></swc:sfwLabel>
                </td>
                <td valign="top">
                    <swc:sfwTextBox autocomplete="off" CssClass="Logintxtbox" MaxLength="10" runat="server"
                        ID="txbContactID" />
                    <asp:RequiredFieldValidator ID="rfvContact" ControlToValidate="txbContactID" Text="*"
                        ErrorMessage="Contact is required" runat="server" />
                </td>
            </tr>
            <tr style="height: 25px;">
                <td colspan="2" style="padding-left: 30px; padding-right: 30px;" valign="top ">
                    <swc:sfwLabel ID="Label1" runat="server" Text="Please answer question below" CssClass="loginTxt"
                        Font-Bold="true"></swc:sfwLabel>
                </td>
            </tr>
            <tr style="height: 25px;">
                <td valign="top" align="right" style="padding-left: 30px;">
                    <swc:sfwDropDownList ID="ddlQuestions" runat="server">
                    </swc:sfwDropDownList>
                </td>
                <td valign="top">
                    <swc:sfwTextBox autocomplete="off" CssClass="Logintxtbox" MaxLength="100" runat="server"
                        ID="txbAnswer" />
                    <br />
                    <br />
                    <swc:sfwButton runat="server" Text="Request Online Access" ID="btnRequestAccess"
                        OnClick="btnRequestAccess_Click" />
                    <asp:HiddenField runat="server" ID="hfldLoginWindowName" />
                </td>
            </tr>
            <tr style="height: 50px;">
                <td>
                </td>
            </tr>
        </table>
    </asp:Panel>
    </form>
</body>
</html>
