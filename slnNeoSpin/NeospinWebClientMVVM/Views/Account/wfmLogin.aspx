<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Sagitec.MVVMClient.LoginModel>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Login</title>
    <link href="../Styles/login.min.css" rel="stylesheet" />
    <%-- <script src="<%=Url.Action("GetEmbeddedResource", "Home", new { astrResourceName = "CommonFunctions.js" })%>" type="text/javascript"></script>
    <script src="<%=Url.Action("GetEmbeddedResource", "Home", new { astrResourceName = "DataContext.js" })%>" type="text/javascript"></script>--%>
    <link href="https://fonts.googleapis.com/css?family=Open+Sans" rel="stylesheet">
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMLibScript")%>
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMScript")%>
</head>
<body class="login-page-Bg">

    <% using (Html.BeginForm(new { astrReturnUrl = ViewBag.ReturnUrl }))
        { %>
    <%: Html.AntiForgeryToken() %>

   <div class="login-wrapper">
       <div class="login-header">
            <img src="../Images/PERSLink-black.png">
        </div>
       <div class="login-inner-wrap">
            <div class="logn-name">
                    <h3>Login</h3> 
                    <hr />
            </div>
             <div class="logn-box-info">                    
                    <h3>This application is the property of NDPERS and is intended for NDPERS use only. This application contains confidential information. Unauthorized use and disclosure of this information is in violation of North Dakota Century Code and will result in disciplinary action up to and including termination.</h3>
                </div>
               <div class="login-container">
               
                <div class="login-content">

                    <div class="login-row text-input">
                        <%: Html.TextBoxFor(m => m.UserName,new { @autocomplete = "off", @placeholder = "Username" }) %>
                        <%: Html.HiddenFor(m=> m.LoginWindowName) %>
                        <img src="../Images/user.svg" class="login-icon">
                    </div>
                <div class="login-row password-input">
                    <%: Html.PasswordFor(m => m.Password,new { @autocomplete = "off", @placeholder = "Password" }) %>
                    <img src="../Images/password.svg" class="login-icon">
                </div>
                <%:Html.HiddenFor(m=>m.IsCaptchaRequired) %>
                <div class="login-row" id="captchacontrol" style="display: none">
                   <%-- <img src="x" id="captchadispaly_img" />--%>                
                    <img src="../Images/refresh.jfif" id="refresh_img" style="cursor: pointer; vertical-align: top; margin-top: 6%" />
                    <img src="../Images/speaker.jfif" id="audio_img" style="cursor: pointer; position: relative; right: 6%" />
                    <%: Html.HiddenFor(m => m.SessionPreserveCaptcha) %>
                    <%: Html.HiddenFor(m => m.EncryptedCaptchaText) %>
                    <%: Html.HiddenFor(m => m.Formname) %>
                    <audio id="speak"></audio>
                    <p>
                        <%: Html.TextBoxFor(m => m.CaptchaTextByUser,new { @autocomplete = "off", @placeholder = "Captcha", oncut="return false", oncopy= "return false", onpaste="return false" ,  @title = "Password" , @name="CaptchatextByUser",@style="width: 35%;margin-right:5%" }) %>
                    </p>
                </div>
                <div class="login-errors">
                    <%= Model.Message %>
                    <%  if(TempData["ErrorMessage"] != null && TempData["ErrorMessage"] != "") %>
                    <%  { %>
                    <%= TempData["ErrorMessage"] %>
                    <%  } %>
                </div>

                <input id="Submit1" type="submit" class="login-btn" value="Login" />
                <br /><br />
                <div class="logn-box-info">
                    <h3>© 2020 Sagitec LLC</h3>
                </div>

               <!-- <p class="remember-text">
                    <label><%: Html.CheckBox("RememberMe", Model.CustomField1 == "true" ? true:false)%> Remember Me</label>
                </p>-->
                <span>
                    <%--<p class="remember-text">
                        <% if(Model.ReferenceID == 0)
                            {%>
                        <%=Html.ActionLink("Forgot Password", "ActivateUser", "Home", routeValues:new { p= "SagiResetPassword" }, htmlAttributes:null)%>
                        <%}
                        %>
                    </p>--%>
                </span>
            </div>
        </div>
    </div>
</div>

    <%--<div class="main_window">
          <div class="sub_window">
                 <div class="Login-Left">
                    <div class="Login-Logo-Wrapper">
                       <div class="login-logo"></div>
                   </div>
                 </div>
                 <div class="right_login ">


                     <div class="Usr">
                          <%: Html.TextBoxFor(m => m.UserName,new { @autocomplete = "off", @placeholder = "Username" }) %>
                          <%: Html.HiddenFor(m=> m.LoginWindowName) %> 
                     </div>
                     <div class="pwd">
                          <%: Html.PasswordFor(m => m.Password,new { @autocomplete = "off", @placeholder = "Password" }) %>    
                     </div>
                     <div class="err">
                       
                     </div>
                <div class="login-caption"></div>
                <div class="login-textboxes">
                    <div class="remember-me">
                        <span class="login-box-options">
                            <input type="checkbox" name="1" value="1">
                            Remember Me </span>
                </div>
                    <input id="Submit1" type="submit" class="btnLogin" value="Login" />
            </div>
                    </div>             
            </div>
          </div>--%>

    <input id="antiForgeryToken" type="hidden" value="<%: Model.AntiForgeryToken %>" />

    <script type="text/javascript">

        var IsCaptchaRequired = $("#IsCaptchaRequired").val() === "False" ? false : true;

        if (!IsCaptchaRequired) {
            $("#captchacontrol").remove();
        }

        var username = document.getElementById("UserName").value;
        if (username == "") {
            document.getElementById("UserName").focus();
        }
        else {
            document.getElementById("Password").focus();
        }
        var Language = "en-US";
        localStorage.clear();  
        sessionStorage.clear();
        nsCommon.SetWindowName();

        $('#Password').keypress(function (e) {
            if (event.keyCode == 60)
                return false;
            else
                return true;
        });

    </script>

    <% } %>

    <script src="<%=Url.Content("~/js/jquery.min.js")%>"></script>

</body>

</html>
