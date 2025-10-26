<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wfmLoginME.aspx.cs" Inherits="NeoSpinMSS.wfmLoginME" %>
<%@ Register Assembly="NDLogin" TagPrefix="ndasp" Namespace="NDLogin" %>
<!doctype html public "-//w3c//dtd xhtml 1.1//en" "http://www.w3.org/tr/xhtml11/dtd/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>PERSLink Web Self Service - Member Portal External User Login</title>
      <script type="text/javascript">
          function SetWindowName() {
              if (document.getElementById("hfldLoginWindowName").value !== "") {
                  window.name = document.getElementById("hfldLoginWindowName").value;
              }
          }
          //PIR 18492 As discussed with vasu,due to security concern below free third party api code is commented.
          //function LoadUserLocation() {
              //var xmlhttp;
              //if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
              //    xmlhttp = new XMLHttpRequest();
              //}
              //else {// code for IE6, IE5
              //    xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
              //}
             
              //xmlhttp.onreadystatechange = function () {
              //    if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
              //        var data = JSON.parse(xmlhttp.responseText); 
              //        document.getElementById("hfldUserCity").value = data.city.names.en;
              //        document.getElementById("hfldUserCountry").value = data.country.names.en;
              //        document.getElementById("hfldUserRegionName").value = data.subdivisions[0].names.en;
              //    }
              //}

              //xmlhttp.open("GET", "https://geoip.nekudo.com/api/full", true)
              //xmlhttp.send()
          //}
      </script>
</head>
<body class="LoginEBody" onload="SetWindowName()">
    <form id="wfmLoginMEForm" runat="server">
    <div>
        <table border="0" align="center" cellpadding="0" cellspacing="0" Width="100%" height="599" class="LoginEControlBG">
            <tr>
                <td align="center">
                    <table>
                    <tr><td>
                    <ndasp:NDLogin CssClass="NDLogin" ID="NDLoginInstance" runat="server" Agency="ITD" AgencyURL="http://www.nd.gov/itd" AppName="MyTestingApp" AppType = "P" />
                    <asp:HiddenField runat="server" ID="hfldLoginWindowName" />
                    <asp:HiddenField runat="server" ID="hfldUserCity" />
                    <asp:HiddenField runat="server" ID="hfldUserCountry" />
                    <asp:HiddenField runat="server" ID="hfldUserRegionName" />
					<asp:Label runat="server" ID="lblError" ForeColor="Red" />
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
                window.history.pushState(null, "", window.location.href);
                window.onpopstate = function () {
                    window.history.pushState(null, "", window.location.href);
                };
                
            </script>
    </div>
    </form>
</body>
</html>
