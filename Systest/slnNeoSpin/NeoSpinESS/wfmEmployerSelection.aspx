<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wfmEmployerSelection.aspx.cs" Inherits="NeoSpinESS.wfmEmployerSelection" %>

<%@ Register TagPrefix="swc" Namespace="Sagitec.WebControls" Assembly="SagitecWebControls, Version=6.0.0.0, Culture=neutral, PublicKeyToken=2FAFCCD2A44457D5" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="sfwPageHeader" runat="server">
    <link href="Styles/Login.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css?family=Open+Sans" rel="stylesheet">
    <title>PERSLink Web Self Service - Employer Selection</title>
     <script type="text/javascript">
          function SetWindowName() {
              if (document.getElementById("hfldLoginWindowName").value !== "") {
                  window.name = document.getElementById("hfldLoginWindowName").value;
              }
          }
      </script>
</head>
<body class="LoginBody LoginBg">
    <div class="login-wrapper">
         <div class="login-header">
            <img src="Images/logo.png" />
        </div>
        <div class="login-inner-wrap">
            <%--<div class="logn-name">
                    <h3>Employer Selection</h3> 
                    <hr />
                
                </div>--%>
                <form id="wfmNeoSpinBase" runat="server" submitdisabledcontrols="true">
                <asp:Panel runat="server" ID="pnlBase" DefaultButton="btnSelect">
                    <table border="0" align="center" cellpadding="0" cellspacing="0" class="LoginBgESS">
                        <%--<tr height="30px">
                            <td>
                            </td>
                        </tr>--%>
                        
                        <tr>
                            <td valign="top" align="right" style="text-align:left;padding-bottom: 10px;">
                                <swc:sfwLabel CssClass="loginTxt" ID="lblContactID" runat="server" Font-Bold="true"
                                    Text="Select Employer : " />
                            </td>
                        </tr>  
                        <tr>                            
                            <td valign="top">
                                <swc:sfwDropDownList ID="ddlEmployers" runat="server">
                                </swc:sfwDropDownList>
                                <br></br>
                                <asp:Label runat="server" ID="lblError" Font-Size ="14px" ForeColor="Red" />
                                <asp:Button ID="btnSelect" CssClass="login-btn" runat="server" Text="Select" OnClick="btnSelect_Click" />
                                <asp:HiddenField runat="server" ID="hfldLoginWindowName" />
                            </td>
                        </tr>
                        <tr style="height: 20px">
                            <td>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                </form>
            </div>
    </div>
</body>
</html>
