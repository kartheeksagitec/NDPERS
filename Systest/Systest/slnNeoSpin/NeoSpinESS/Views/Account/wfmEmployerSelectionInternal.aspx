<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Neo.Model.wfmEmployerSelection>" %>

<!DOCTYPE html>
<html>
<head>
    <title>Select Contact</title>
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMLibScript")%>
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMScript")%>  
    <link href="../Styles/Login.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css?family=Open+Sans" rel="stylesheet">
</head>
<body class="LoginBg">
    <% using (Html.BeginForm(new { ReturnUrl = ViewBag.ReturnUrl }))
        { %>
    <%: Html.AntiForgeryToken() %>
    <input id="antiForgeryToken" type="hidden" value="<%: Model.AntiForgeryToken %>" />
     <div id="page-loader" style="display: none;" class="preloader">
        <table width="100%" height="100%">
            <tr>
                <td style="vertical-align: middle; text-align: center">
                    <img alt="loader" src="<%:Url.Content("~/Images/301.gif")%>" />
                </td>
            </tr>

        </table>
    </div>
    <div class="login-wrapper">
        <div class="login-header">
            <img src="../Images/logo.png" />
        </div>
        <div class="login-inner-wrap">
      
        <div role="group" id="wfmSelectContactHolder">
        <div id="LookupHolder"></div>
        <div id="wfmEmployerSelection">
              <div class="login-container">
                
                    <div class="login-content">                        
                     
                        <div class="login-row">
                            <%: Html.DropDownListFor(m => m.ORG_ID,Model.EmployerList,"---Select---",new { @class = "form-control input-xlarge",autofocus="autofocus"})%>                            
                        </div>
                        <div class="logerror">
                            <%= Model.Message %>
                            <%: Html.ValidationMessageFor(m => m.ORG_ID) %>
                        </div>

                        <div class="login-row">                            
                            <input id="Submit1" type="submit" class="login-btn" value="Submit" />
                        </div>
                     </div>
                </div>
            </div>
        </div>
     </div>
   </div>
    <div style="padding: 10px; display: none" id="TabsTree"></div>
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMLibScript")%>
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMScript")%>
    <script src="<%=Url.Content("~/Scripts/App/UserDefinedFunctions.js")%>"></script>
    <script src="<%=Url.Content("~/Scripts/App/Notification.js")%>"></script>
    	  
    <% } %>
</body>

</html>