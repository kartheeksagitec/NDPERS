<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Sagitec.MVVMClient.LoginModel>" %>

<!DOCTYPE html>
<html>
<head>
    <title>Welcome to NDPERS Member Web Portal for Internal Users</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <link href="../CSS/bootstrap.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/bootstrap-responsive.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/login.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/font-awesome.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/css.css" type="text/css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css?family=Open+Sans:400,300,600,700&subset=all" rel="stylesheet" type="text/css" />
     <%--<%: System.Web.Optimization.Scripts.Render("~/bundles/FMLibScript")%>
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMScript")%>--%>
    <script type="text/javascript"> 
        if (navigator.appVersion.indexOf("Mac") == -1) { 
            var historyLength=history.length;
            if (historyLength > 0) history.go(-historyLength);
        }
    </script>
</head>
<body>
    <% using (Html.BeginForm(new { astrReturnUrl = ViewBag.ReturnUrl}))
       { %>
    <%: Html.AntiForgeryToken() %>
    <input id="antiForgeryToken" type="hidden" value="<%: Model.AntiForgeryToken %>"/>
    <div class="login-wrapper">

        <div class="login-header">
                <img src="../Images/logo-white.png" class="logo-white">
                <img src="../Images/bg-small.jpg" class="bg-small">
            </div>

        <div class="login-inner-wrap">
            <div class="logn-name">
                    <h3>Login</h3> 
                    <hr />
                </div>
               <div class="logn-box-info">
                    <h3>Use your NDPERS User Id and Password to log in to the member web portal. Provide the Member ID that you want to log in as. Once you logged in, you can switch to a different member without logout.</h3>
                    <div class="neospin-login">
                    </div>
                </div>

                <div class="login-container">

               
                    <div class="login-content">
                        <div class="form-group">
                   <%: Html.TextBoxFor(m => m.UserName, new { @class = "form-control placeholder-no-fix", @autocomplete = "off", @placeholder = "NDPERS User ID" })%>
                
                    <%: Html.HiddenFor(m=> m.LoginWindowName) %>
                            <img src="../Images/user.svg" class="login-icon"/>
                    </div>
                
                        <div class="form-group">
                  <%: Html.PasswordFor(m => m.Password, new { @class = "form-control placeholder-no-fix", @autocomplete = "off", @placeholder = "Password" })%>
                            <img src="../Images/password.svg" class="login-icon"/>                
                            </div>
                    <div class="form-group">
                   <%: Html.TextBoxFor(m => m.ReferenceID, new { @class = "form-control placeholder-no-fix", @autocomplete = "off", @placeholder = "Member ID" , @Value = ""})%>
                        <img src="../Images/id.svg" class="login-icon"/>
                        </div>
                    <div class="login-error">
                        <%= Model.Message %>
                        <%--<%: Html.ValidationMessageFor(m => m.UserName) %>
                        <%: Html.ValidationMessageFor(m => m.Password) %>
                        <%: Html.ValidationMessageFor(m => m.ReferenceID) %>--%>
                    </div>
                    <input id="btnLogin" type="submit" class="login-btn" value="Login"  />
                        <div class="col-md-12"><br></div>
                        <div align="center" >
                            <span>Privacy Policy &amp; Disclaimer </span>
                            <span> | NDPERS &copy; <%= DateTime.Now.Year%></span>
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
                window.history.pushState(null, "", window.location.href);
                window.onpopstate = function () {
                    window.history.pushState(null, "", window.location.href);
                };
        });
       
    </script>
    <% } %>
</body>
</html>
