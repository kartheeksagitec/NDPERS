<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Neo.Model.wfmEmployerSelection>" %>

<!DOCTYPE html>
<html>
<head>
    <title>Neospin - Employer Self Service Portal</title>
    <link href="../Styles/Login.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css?family=Open+Sans" rel="stylesheet">
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
            <h3>Employer Self Service</h3>
            <div class="neospin-login">
            </div>
        </div>

            <div class="login-container">

               <%-- <div class="login-header">
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
                <input id="btnLogout" type="submit" class="login-btn" value="Login" style="cursor : pointer"/>
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
