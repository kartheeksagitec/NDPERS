<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wfmRequestOnlineAccessForMember.aspx.cs" Inherits="NeoSpinMSS.wfmRequestOnlineAccessForMember" %>

<%@ Register TagPrefix="swc" Namespace="Sagitec.WebControls" Assembly="SagitecWebControls, Version=6.0.0.0, Culture=neutral, PublicKeyToken=2FAFCCD2A44457D5" %>
<%@ Register Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title>PERSLink Web Self Service - Request Online Access</title>
    <script type="text/javascript">
          function SetWindowName() {
              if (document.getElementById("hfldLoginWindowName").value !== "") {
                  window.name = document.getElementById("hfldLoginWindowName").value;
            }
          }
    </script>
</head>
<body>
    <form id="form2" runat="server" defaultbutton="btnSubmit">
    <asp:Panel runat="server" ID="pnlBase" DefaultButton="btnRequestAccess">
        <table width="778" border="0" align="center" cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <table width="710" border="0" align="center" cellpadding="0" cellspacing="0">
                        <tr>
                            <td width="10" height="13">
                                <img src="images/Border_Top_Left.jpg" alt="Left" width="13" height="13" />
                            </td>
                            <td background="images/Border_Top_middle.jpg">
                            </td>
                            <td width="10" height="13">
                                <img src="images/Border_Top_Right.jpg" width="15" height="13" />
                            </td>
                        </tr>
                        <tr>
                            <td background="images/Border_Center_Left.jpg">
                                &nbsp;
                            </td>
                            <td bgcolor="#FFFFFF">
                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td colspan="3">
                                            <div align="center">
                                                <img src="images/NDPERSImgLogo.png" width="676" height="212" /></div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td width="678" valign="top" align="left">
                                            <table border="0" align="left" cellpadding="10" cellspacing="2">
                                                <tr>
                                                    <td valign="top" align="right">
                                                        <swc:sfwLabel ID="lblPersonID" runat="server" Text="Member ID :" CssClass="loginTxt"></swc:sfwLabel>
                                                    </td>
                                                    <td valign="top" colspan="3">
                                                        <swc:sfwTextBox autocomplete="off" CssClass="Logintxtbox" MaxLength="10" runat="server"
                                                            ID="txbPERSLinkID" />
                                                        <asp:RequiredFieldValidator ID="rfvPERSLinkID" ControlToValidate="txbPERSLinkID"
                                                            Text="*" ErrorMessage="Member ID is required" runat="server" />
                                                        <a href="wfmRequestPerslinkAccess.aspx">Forgot Member ID?</a>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top" align="right">
                                                        <swc:sfwLabel ID="lblLast4DigitsSSN" runat="server" Text="Last 4 digits of SSN :"
                                                            CssClass="loginTxt"></swc:sfwLabel>
                                                    </td>
                                                    <td valign="top" colspan="3">
                                                        <swc:sfwTextBox autocomplete="off" CssClass="Logintxtbox" MaxLength="100" runat="server"
                                                            ID="txbLast4DigitsSSN" />
                                                        <asp:RequiredFieldValidator ID="rfvSSN" ControlToValidate="txbLast4DigitsSSN" Text="*"
                                                            ErrorMessage="Last 4 digits of SSN is required" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top" align="right">
                                                        <swc:sfwLabel ID="lblDOB" runat="server" Text="Date of Birth :" CssClass="loginTxt"></swc:sfwLabel>
                                                    </td>
                                                    <td valign="top">
                                                        <swc:sfwDropDownList ID="ddlMonth" runat="server" CssClass="LoginSelectBox">
                                                        </swc:sfwDropDownList>
                                                        <br />
                                                        <swc:sfwLabel ID="lblMonth" runat="server" Text="Month" CssClass="loginTxtLabel"></swc:sfwLabel>
                                                    </td>
                                                    <td valign="top">
                                                        <swc:sfwDropDownList ID="ddlDay" runat="server" CssClass="LoginSelectBox">
                                                        </swc:sfwDropDownList>
                                                        <br />
                                                        <swc:sfwLabel ID="lblDay" runat="server" Text="Day" CssClass="loginTxtLabel"></swc:sfwLabel>
                                                    </td>
                                                    <td valign="top">
                                                        <swc:sfwDropDownList ID="ddlYear" runat="server" Width="77px" CssClass="LoginSelectBox">
                                                        </swc:sfwDropDownList>
                                                        <br />
                                                        <swc:sfwLabel ID="lblYear" runat="server" Text="Year" CssClass="loginTxtLabel"></swc:sfwLabel>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="contentLoginError" colspan="5">
                                                        <asp:Label runat="server" ID="lblError" ForeColor="Red" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top" align="right">
                                                        &nbsp;
                                                    </td>
                                                    <td valign="top" colspan="3">
                                                        <swc:sfwButton runat="server" Text="Request Online Access" ID="btnRequestAccess"
                                                            OnClick="btnRequestAccess_Click" CssClass="buttonbgRequest" />
                                                    </td>
                                                    <asp:HiddenField runat="server" ID="hfldLoginWindowName" />
                                                </tr>
                                            </table>
                                            <table width="678" rowspan="2" valign="top" align="center">
                                                <tr>
                                                    <td height="20" valign="top" style="padding-left: 10px; padding-top: 20px;" align="center">
                                                        <div align="center">
                                                            <span class="footertxt">Privacy Policy &amp; Disclaimer </span><span class="footertxt">
                                                                | NDPERS &copy; <%= DateTime.Now.Year %> </span>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td background="images/Border_Center_Right.jpg">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <img src="images/Border_Bottom_Left.jpg" width="13" height="15" />
                            </td>
                            <td background="images/Border_Bottom_Center.jpg">
                            </td>
                            <td>
                                <img src="images/Border_Bottom_Right.jpg" width="15" height="15" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td height="25">
                    <div align="center">
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    </form>
</body>
</html>
