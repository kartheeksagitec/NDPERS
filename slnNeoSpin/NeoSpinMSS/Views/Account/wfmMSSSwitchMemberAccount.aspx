<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<NeoSpinMSS.Model.wfmMSSSwitchMemberAccount>" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title></title>
    <%--<link href="../Styles/Login.css" rel="stylesheet" />--%>
</head>
<body>
    <div>
    </div>
</body>
</html>
<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <title>SelectClaimant</title>
    <link href="../Styles/Login.css" rel="stylesheet" />
    <%-- <link href="../Styles/Common.css" rel="stylesheet" />--%>
    <%: System.Web.Optimization.Scripts.Render("../bundles/FMLibScript")%>
    <%: System.Web.Optimization.Scripts.Render("../bundles/FMScript")%>
</head>
<body class="LoginBg">
    <% using (Html.BeginForm(new { ReturnUrl = ViewBag.ReturnUrl }))
       { %>
    <%: Html.AntiForgeryToken()%>
    <input id="antiForgeryToken" type="hidden" value="<%: Model.AntiForgeryToken %>"/> 
   
    <div class="login-wrapper">
        <div class="login-header">
            <img src="../Images/logo.png" />
        </div>
        <div class="login-inner-wrap">
            <div class="logn-name">
                    <h3> Member Self Service Portal</h3> 
                    <hr />
                </div>
        <div class="login-wrap">
           
            <div class="login-box">
                <p>You have an active member account and a payee account with NDPERS. Please select the account that you wish to view:</p>
                <div class="radio-wrap">
                    <%: Html.RadioButtonFor(m => m.IsActiveAccountSelected, "ACTV", new { id="rdActiveAccount"})%>
                    <%: Html.LabelFor(m =>m.IsActiveAccountSelected,"Active Member Account") %>
                    <%: Html.RadioButtonFor(m => m.IsActiveAccountSelected, "RETR", new  { id="rdRetireeAccount"})%>
                    <%: Html.LabelFor(m =>m.IsActiveAccountSelected,"Payee Account") %>
                    <%: Html.HiddenFor(m=> m.LoginWindowName) %>
                </div>
                <div class="logerror">
                    <%= Model.Message%>
                    <%: Html.ValidationSummary(true)%>
                </div>
                <p>You can 'switch' accounts by using the 'Switch Accounts' Tab on upper right corner.</p>                
                 <input type="submit" class="go-btn" value="Go" />
                <div class="col-md-12" style="margin:15px;"></div>
                <div align="center" style ="text-decoration:underline;cursor:pointer; font-size:large;">
                    <span><a href="#" style="color:#252525;" onclick="javascript:localStorage.clear(); sessionStorage.clear(); return ns.logoutSesssion();">Logout</a></span>                            
                 </div>
            </div>
        </div>
    </div></div>
        </div>
    <% } %>
    <%--<script src="<%=Url.Action("GetEmbeddedResource", "Home", new { astrResourceName = "Lib.jquery.min.js"})%>" type="text/javascript"></script>--%>
    <script type="text/javascript">    
        localStorage.clear();
        sessionStorage.clear();
        nsCommon.SetWindowName();
        window.history.pushState(null, "", window.location.href);
        window.onpopstate = function () {
            window.history.pushState(null, "", window.location.href);
        };
    </script>
</body>
</html>
