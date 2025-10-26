<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Neo.Model.wfmEmployerSelection>" %>

<!DOCTYPE html>
<html>
<head>
    <title>PERSLink Web Self Service - Impersonate Contact</title>
    <link href="../Styles/Login.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css?family=Open+Sans" rel="stylesheet">
    <%--<script src="<%=Url.Action("GetEmbeddedResource", "Home", new { astrResourceName = "CommonFunctions.js" })%>" type="text/javascript"></script>
    <script src="<%=Url.Action("GetEmbeddedResource", "Home", new { astrResourceName = "DataContext.js" })%>" type="text/javascript"></script>--%>
    <script type="text/javascript">
            var historyLength=history.length;
            if (historyLength > 0) history.go(-historyLength);
            window.history.forward(1);
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
                    <h3>Switch Contact</h3> 
                    <hr />
                </div>
           <div class="login-container">
                
            <div class="login-content">
                    <%: Html.TextBoxFor(m => m.ContactID,new { @autocomplete = "off",@maxlength="10", @placeholder = "Enter Contact ID",@class="logintxt",@Value = (@Model.ContactID == 0) ? "" : Convert.ToString(@Model.ContactID) }) %>
                    <span id="requiredField" style="color:red; display:none;">Contact ID field required.</span>
                 <img src="../Images/id.svg" class="login-icon"/>
                </div>
                <div class="logerror">
                    <%= Model.Message %>
                    <%: Html.ValidationMessageFor(m => m.ContactID) %>
                </div>
                <div class="login-caption"></div>
                <div class="login-row">
                    <%--<div class="remember-me">
                        <span class="login-box-options">
                            <input type="checkbox" name="1" value="1">
                            Remember Me </span>
                    </div>--%>
                    <input id="btnImpersonateContact" type="submit" class="login-btn" value="Select" />
                </div>
            </div>
            </div>
        </div>
     <%: System.Web.Optimization.Scripts.Render("~/bundles/FMLibScript")%>
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMScript")%>
    <script type="text/javascript">       
        $("#ContactID").trigger("focus");
        sessionStorage.clear();
        $("#requiredField").hide();
        nsCommon.SetWindowName();
        sessionStorage.setItem("IsBrowserBackButtonPressed", "True");
        $(function () {
            $("#btnImpersonateContact").click(function () {
            if ($("#ContactID").val() != "") {
                $("#requiredField").hide();
                sessionStorage.setItem("IsBrowserBackButtonPressed", "False");
            }
            else {
                $("#requiredField").show();
                sessionStorage.setItem("IsBrowserBackButtonPressed", "True");
                return false;
            }
        });
        });

    </script>

    <% } %>

    <%--<script src="<%=Url.Content("~/js/jquery.min.js")%>"></script>--%>

</body>

</html>
