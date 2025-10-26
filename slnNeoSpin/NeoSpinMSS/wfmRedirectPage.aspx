<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wfmRedirectPage.aspx.cs" Inherits="NeoSpinMSS.wfmRedirectPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <style type="text/css">
        .style1
        {
            height: 19px;
        }
    </style>
    <script type="text/javascript">
          function SetWindowName() {
              if (document.getElementById("hfldLoginWindowName").value !== "") {
                  window.name = document.getElementById("hfldLoginWindowName").value;
            }
          }
    </script>
    <LINK rel=stylesheet type=text/css href="App_Themes/Green/Green.css">
</head>
<body class="LoginBody" BackColor="#C7E8C1" >
    <form id="wfmLoginMIForm" runat="server">
    <div>
    <table>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    <tr><td></td></tr>
    </table>
        <table border="0" align="center" cellpadding="0" cellspacing="0" height="100px;" width="1000px;" style="margin-left: 0px; margin-top: 75px; margin-right: 0px; margin-bottom: 0px;"  >
        <tr><td> </td></tr>
            
            <tr><td class="style1"></td></tr>
            <tr align ="center">                
            <td bgcolor="#C7E8C1"  style="border:50px;"  ><br />
              <asp:Label ID="Label5" runat="server" BackColor="#C7E8C1" Text="ATTENTION:  PERSLink Member Self Service has moved!"  Font-Size="X-Large"></asp:Label> 
              <br />&nbsp;
    </td></tr>
    <tr><td></td></tr>
    <tr align ="center">                
                <td bgcolor="#333333">
        <p><br /><asp:Label ID="Label6" runat="server" 
            Text="Please add the following link to your favorites and delete the old link:" 
            ForeColor="White"></asp:Label>
    </p>
    <p >
        <asp:Label ID="Label7" runat="server" 
            Text="https://perslink.nd.gov/perslinkmss/wfmloginme.aspx" ForeColor="White"></asp:Label>
    </p>
   <p >
        <asp:HyperLink ID="HyperLink1" runat="server" 
            NavigateUrl="https://perslink.nd.gov/perslinkmss/wfmloginme.aspx " 
            ForeColor="#FFFFCC">Click Here</asp:HyperLink>
        &nbsp;<asp:Label ID="Label8" runat="server" Text=" to be redirected to the new site." ForeColor="White"></asp:Label>
       <asp:HiddenField runat="server" ID="hfldLoginWindowName" />
    </p> 
    <br />
                </td>                
            </tr>  
            <tr><td bgcolor=""height="10px;"></td></tr>                
        </table>
    </div>
    </form>
</body>
</html>
