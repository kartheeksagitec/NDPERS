<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wfmRequestPerslinkAccess.aspx.cs" Inherits="NeoSpinMSS.wfmRequestPerslinkAccess" %>

<%@ Register TagPrefix="swc" Namespace="Sagitec.WebControls" Assembly="SagitecWebControls, Version=6.0.0.0, Culture=neutral, PublicKeyToken=2FAFCCD2A44457D5" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title>PERSLink Web Self Service - Request Online Access</title>
    <style type="text/css">
    <!--
        body {
	        background-color: #d7dadd;
        }
        html
        {
            height:auto !important;
        }
    -->
    </style>
    <script type="text/javascript">
        function btnClose_Click() {
            window.close();
        }

        function SetWindowName() {
            if (document.getElementById("hfldLoginWindowName").value !== "") {
                window.name = document.getElementById("hfldLoginWindowName").value;
                sessionStorage.setItem("WindowName", window.name);
            }
        }
    </script>
</head>
<body onload="SetWindowName()">
    <form id="form2" runat="server" defaultbutton="btnSubmit">
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
                                    <td width="1">
                                    </td>
                                    <td width="678" rowspan="2" valign="top" align="left">
                                        <table width="100%" border="0" cellspacing="10" cellpadding="0">
                                            <tr>
                                                <td>
                                                    <asp:Panel runat="server" ID="pnlBase" DefaultButton="btnRequestAccess">
                                                        <table border="0" align="left" cellpadding="10" cellspacing="2">
                                                            <tr>
                                                                <td colspan="4">
                                                                    <swc:sfwLabel Text="Please enter all following information: " runat="server" ID="SfwLabelTOP"
                                                                        Font-Size="12px" Font-Bold="true" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right" style="width:30%;">
                                                                    <swc:sfwLabel ID="lblPersonID" runat="server" Text="Last Name :" CssClass="loginTxt"></swc:sfwLabel>
                                                                </td>
                                                                <td valign="top" colspan="3">
                                                                    <swc:sfwTextBox autocomplete="off" CssClass="Logintxtbox" MaxLength="50" runat="server"
                                                                        ID="txbLastName" />
                                                                    <asp:RequiredFieldValidator ID="rfvLastName" ControlToValidate="txbLastName" Text="*"
                                                                        ErrorMessage="Last Name is required" runat="server" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="right">
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
                                                                <td class="contentLoginError" colspan="4">
                                                                    <asp:Label runat="server" ID="lblError" ForeColor="Red" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td valign="top" align="right">
                                                                    &nbsp;
                                                                </td>
                                                                <td valign="top" colspan="3" align="left">
                                                                    <swc:sfwButton runat="server" Text="Mail My Member ID to Me" ID="btnRequestAccess" OnClick="btnRequestAccess_Click"
                                                                        CssClass="buttonbgRequest" />
                                                                    <swc:sfwButton runat="server" Text="Close" ID="SfwButtonClose" OnClientClick="btnClose_Click();"
                                                                        CssClass="buttonbgRequest" />
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td></td>
                                                                <td valign="top" colspan="3" align="left">
                                                                    <asp:Label runat="server" ID="lblMessage" Text="If you need immediate access to Member Self Service (MSS), please call the NDPERS office at 701-328-3900" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td height="20" valign="top" style="padding-left: 10px; padding-top: 20px;" align="center">
                                                    <div align="center">
                                                        <span class="footertxt">Privacy Policy &amp; Disclaimer </span><span class="footertxt">
                                                            | NDPERS &copy; <%= DateTime.Now.Year%> </span>
                                                    </div>
                                                </td>
                                                <td> <asp:HiddenField runat="server" ID="hfldLoginWindowName" />
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
    </form>
</body>
</html>