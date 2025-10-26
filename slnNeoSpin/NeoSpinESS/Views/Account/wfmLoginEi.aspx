<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Sagitec.MVVMClient.LoginModel>" %>

<!DOCTYPE html>
<html>
<head>
    <title>PERSLink Web Self Service - Employer Portal Internal User Login</title>
    <link href="../Styles/Login.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css?family=Open+Sans" rel="stylesheet">
    <%--<script src="<%=Url.Action("GetEmbeddedResource", "Home", new { astrResourceName = "CommonFunctions.js" })%>" type="text/javascript"></script>
    <script src="<%=Url.Action("GetEmbeddedResource", "Home", new { astrResourceName = "DataContext.js" })%>" type="text/javascript"></script>--%>
    <script type="text/javascript">
        if (navigator.appVersion.indexOf("Mac") == -1) {
            var historyLength = history.length;
            if (historyLength > 0) history.go(-historyLength);
        }
    </script>
</head>
<body class="LoginBg">
    <% using (Html.BeginForm(new { astrReturnUrl = ViewBag.ReturnUrl }))
       { %>
    <%: Html.AntiForgeryToken() %>
    <input id="antiForgeryToken" type="hidden" value="<%: Model.AntiForgeryToken %>"/>
    <div class="login-wrapper">
         <div class="login-header">
            <img src="../Images/logo.png" />
        </div>
        <div class="login-inner-wrap">
            <div class="logn-name">
                    <h3>Login</h3> 
                    <hr />
                </div>
             <div class="logn-box-info">                    
                    <h3>Use your NDPERS User Id and Password to log in to the Employer Self Service portal.
                                                                 Provide the Contact ID that you want to log in as.
                                                                     Once you logged in, you can switch to a different Contact without logout.</h3>
               
                </div>
               <div class="login-container">
               
                <div class="login-content">

                    <div class="login-row">                        
                        <%: Html.TextBoxFor(m => m.UserName,new { @autocomplete = "off",@maxlength="70", @placeholder = "Enter Username" ,@class="logintxt"}) %>
                    
                        <%: Html.HiddenFor(m=> m.LoginWindowName) %>
                        <img src="../Images/user.svg" class="login-icon"/>
                    </div>
               
                    <div class="login-row">
                        <%: Html.PasswordFor(m => m.Password,new { @autocomplete = "off",@maxlength="70", @placeholder = "Enter Password",@class="logintxt" }) %>
                        <img src="../Images/password.svg" class="login-icon"/>
                    </div>
                    <div class="login-row">
                        <%: Html.TextBoxFor(m => m.ReferenceID,new { @autocomplete = "off",@maxlength="10", @placeholder = "Enter Contact ID",@class="logintxt",@Value = (@Model.ReferenceID == 0) ? "" : Convert.ToString(@Model.ReferenceID) }) %>
                        <img src="../Images/id.svg" class="login-icon"/>
                    </div>
                    <div class="logerror">
                    
                        <%--<%: Html.ValidationMessageFor(m => m.UserName) %>
                        <%: Html.ValidationMessageFor(m => m.Password) %>
                        <%: Html.ValidationMessageFor(m => m.ReferenceID) %>--%>
                        <%= Model.Message %>
                    </div>
                    <div class="login-caption"></div>
                    <div class="">
                       <%-- <div class="remember-me">
                            <span class="login-box-options">
                                <input type="checkbox" name="1" value="1">
                                Remember Me </span>
                        </div>--%>
                        <input id="Submit1" type="submit" class="login-btn" value="Login" />
                    </div>
                </div>
            </div>
        </div>
    </div>
     <%: System.Web.Optimization.Scripts.Render("~/bundles/FMLibScript")%>
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMScript")%>
    <script type="text/javascript">
        var username = document.getElementById("UserName").value;
        if (username == "") {
            document.getElementById("UserName").focus();
        }
        else {
            document.getElementById("Password").focus();
        }
        sessionStorage.clear();
        nsCommon.SetWindowName();
        $(function () {
            //var isSafari = !!navigator.userAgent.match(/Version\/[\d\.]+.*Safari/);
                window.history.pushState(null, "", window.location.href);
                window.onpopstate = function () {
                    window.history.pushState(null, "", window.location.href);
                };
        });
    </script>

    <% } %>

    <%--<script src="<%=Url.Content("~/js/jquery.min.js")%>"></script>--%>

</body>

</html>
