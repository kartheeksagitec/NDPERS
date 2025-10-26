<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wfmLoginEE.aspx.cs" Inherits="NeoSpinESS.wfmLoginEE" %>

<%@ Register Assembly="NDLogin" TagPrefix="ndasp" Namespace="NDLogin" %>
<!doctype html public "-//w3c//dtd xhtml 1.1//en" "http://www.w3.org/tr/xhtml11/dtd/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>PERSLink Web Self Service - Employer Portal External User Login</title>
      <script type="text/javascript">
          function SetWindowName() {
              if (document.getElementById("hfldLoginWindowName").value !== "") {
                  window.name = document.getElementById("hfldLoginWindowName").value;
              }
          }
      </script>
</head>
<body class="LoginEBody" onload="SetWindowName()">
    <form id="wfmLoginEEForm" runat="server">
    <div>
       <table border="0" align="center" cellpadding="0" cellspacing="0" Width="100%" height="599" class="LoginEControlBG">
            <tr>
                <td align="center">
                    <table>
                    <tr><td>
                        <ndasp:NDLogin CssClass="NDLogin" ID="NDLoginInstance" runat="server" Agency="ITD" AgencyURL="http://www.nd.gov/itd" AppName="MyTestingApp" AppType = "B" />
                        <asp:HiddenField runat="server" ID="hfldLoginWindowName" />
                    </td></tr>
                    </table>                    
                </td>
            </tr>
        </table>
        <script type="text/javascript">
            sessionStorage.clear();
                var element = document.getElementById("NDLoginInstance_txtPassword");
                if (element !== null && element !== undefined)
                    element.setAttribute("autocomplete", "off");
            </script>
    </div>
    </form>
</body>
</html>