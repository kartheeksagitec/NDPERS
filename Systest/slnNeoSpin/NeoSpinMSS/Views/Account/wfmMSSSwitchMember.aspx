<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<NeoSpinMSS.Model.wfmMSSSwitchMember>" %>

<!DOCTYPE html>
<html>
<head>
    <title>Welcome to NDPERS Member Web Portal for Internal Users</title>
    <link href="https://fonts.googleapis.com/css?family=Open+Sans:400,300,600,700&subset=all" rel="stylesheet" type="text/css" />
    <link href="../assets/plugins/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="../CSS/login.css" type="text/css" rel="stylesheet" />
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMLibScript")%>
    <%: System.Web.Optimization.Scripts.Render("~/bundles/FMScript")%>
    <script src="<%=Url.Content("~/Scripts/App/UserDefinedFunctions.js")%>"></script>
    <script src="<%=Url.Content("~/Scripts/App/Notification.js")%>"></script>
    <script type="text/javascript">
        var historyLength = history.length;
        if (historyLength > 0) history.go(-historyLength);
        window.history.forward(1);
    </script>
    <style>
        html,
        body {
            height: 100%;
        }

        #page-loader {
            position: absolute;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
            z-index: 99999;
            width: 100%;
            height: 100%;
            padding-top: 25px;
            display: none;
            background: rgba(0,0,0,.3);
        }

        /* default style */
        .selectnav {
            display: none;
        }

        /* small screen */
        @media screen and (max-width: 900px) {
            .js #MenuUl {
                display: none;
            }

            .js .selectnav {
                display: block;
            }
        }

        .placeholder {
            opacity: 0.4;
            border: 1px dashed #a6a6a6;
        }
    </style>
</head>
<body>
    <% using (Html.BeginForm(new { astrReturnUrl = ViewBag.ReturnUrl }))
        { %>
    <%: Html.AntiForgeryToken()%>
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

    <div role="group" id="wfmSelectMemeberHolder">
        <div id="LookupHolder"></div>
        <div id="wfmSelectMemeber">
            <div class="login-header">
                <img src="../Images/logo-white.png" class="logo-white">
                <img src="../Images/bg-small.jpg" class="bg-small">
            </div>

            <div class="login-inner-wrap">
                <div class="logn-name">
                    <h3>SWITCH Member</h3>
                    <hr />
                </div>

                <div class="logn-box-info">
                    <div class="neospin-login">
                    </div>
                </div>

                <div class="login-container">

                    <div class="login-content">
                        <div class="form-group">
                            <%: Html.TextBoxFor(m => m.MemberID, new { @class = "form-control placeholder-no-fix", @autocomplete = "off", @placeholder = "Member ID", @Value= (@Model.MemberID != 0 ? Convert.ToString(@Model.MemberID) : "")})%>
                            <%: Html.HiddenFor(m=> m.LoginWindowName) %>
                            <img src="../Images/id.svg" class="login-icon" />
                        </div>
                        <div class="logerror">
                            <%= Model.Message%>
                            <%: Html.ValidationMessageFor(m => m.MemberID)%>
                        </div>
                        <input id="btnMSSSwitchMember" type="submit" class="login-btn" value="Switch Member" style="font-size: large; text-align: center;" />
                        <br />
                        <div class="col-md-12" style="margin: 10px;"></div>
                        <div align="center">
                            <span>Privacy Policy &amp; Disclaimer </span>
                            <span>| NDPERS &copy; <%= DateTime.Now.Year%></span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
   <script type="text/javascript">  
       $("#MemberID").trigger("focus");
       localStorage.clear();
       sessionStorage.clear();
       nsCommon.SetWindowName();
       sessionStorage.setItem("IsBrowserBackButtonPressed", "True");
       $(function () {
           $("#btnMSSSwitchMember").click(function () {
               if ($("#MemberID").val() != "") {
                   sessionStorage.setItem("IsBrowserBackButtonPressed", "False");
               }
               else {
                   sessionStorage.setItem("IsBrowserBackButtonPressed", "True");
               }
           });
       });
   </script>
    <% } %>
</body>
</html>
