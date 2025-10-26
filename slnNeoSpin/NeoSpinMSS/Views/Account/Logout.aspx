<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<NeoSpinMSS.Model.wfmMSSSwitchMember>" %>

<!DOCTYPE html>
<html>
<head>
    <title>Neospin - Member Self Service Portal</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" />
    <link href="../CSS/bootstrap.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/bootstrap-responsive.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/login.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/font-awesome.css" type="text/css" rel="stylesheet" />
    <link href="../CSS/css.css" type="text/css" rel="stylesheet" />
    <link href="http://fonts.googleapis.com/css?family=Open+Sans:400,300,600,700&subset=all" rel="stylesheet" type="text/css" />
     <%: System.Web.Optimization.Scripts.Render("~/bundles/FMLibScript")%>
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMScript")%>   
    <style>
    .close {
    cursor: pointer;
    }
    </style>

</head>
<body>
    <% using (Html.BeginForm(new { astrReturnUrl = "" }))
       { %>
    <%: Html.AntiForgeryToken() %>
    <div class="login-wrapper">

       <div class="logn-box-info">
            <h3>You have successfully logged out of the system.</h3>
            <div class="neospin-login">
            </div>
        </div>

            <div class="login-container">

             <%--   <div class="login-header">
                <img src="../Images/neospin-login-logo.png">
            </div>--%>

                <div class="login-content" >
                    <%--<div class="form-group">
               <%: Html.TextBoxFor(m => m.UserName, new { @class = "form-control placeholder-no-fix", @autocomplete = "off", @placeholder = "Username" })%>
                    </div>--%>
                
                    <%--<div class="form-group">
              <%: Html.PasswordFor(m => m.Password, new { @class = "form-control placeholder-no-fix", @autocomplete = "off", @placeholder = "Password" })%>
                
                        </div>--%>
                <%--<div class="login-error">
                    <%= Model.Message %>
                    <%: Html.ValidationMessageFor(m => m.UserName) %>
                    <%: Html.ValidationMessageFor(m => m.Password) %>
                </div>--%>
                <%--<input id="btnLogout" type="submit" class="login-btn" value="Login" style="cursor : pointer"/>--%>
                    <%: Html.HiddenFor(m=> m.LoginWindowName) %>
                </div>
            </div>
        
    </div>
     <input id="antiForgeryToken" type="hidden" value="<%: Model.AntiForgeryToken %>"/> 
    <script type="text/javascript">
           
        nsCommon.SetWindowName();
        $(function () {
            var ImageLinkToNavigate = sessionStorage.getItem("ImageLinkToNavigate");
            if (ImageLinkToNavigate != undefined && ImageLinkToNavigate != null && ImageLinkToNavigate != "") {
                sessionStorage.setItem("ImageLinkToNavigate", "");
                sessionStorage.clear();
                localStorage.clear(); 
                window.open(ImageLinkToNavigate, "_self");
            }            
        });
        //$('#btnLogout').off('touchstart touchend');
        //$('#btnLogout').trigger('click');

    </script>
    <% } %>
</body>
</html>
