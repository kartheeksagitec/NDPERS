<%@ Page Title="" Language="C#"  AutoEventWireup="true" CodeBehind="wfmTermsAndConditions.aspx.cs" Inherits="LoginPages_wfmTermsAndConditions" %>
<!doctype html public "-//w3c//dtd xhtml 1.1//en" "http://www.w3.org/tr/xhtml11/dtd/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Kentucky Teacher’s Retirement System</title>
    <link href="Common.css" type="text/css" rel="stylesheet" />
    <link href="App_Themes/Green/accordion.css" type="text/css" rel="stylesheet" />
    <link href="App_Themes/Green/Green.css" type="text/css" rel="stylesheet" />
    <link href="App_Themes/Green/layout-default-latest.css" type="text/css" rel="stylesheet" />
</head>
<body id="loginbody">
<div id="wrap">
<div class="loginRibbon">
<div id="loginAuthenticationBoxBg" style="min-height:645px;">
    <div class="loginLeft">
        <img src="image/Login_Logo.png" alt="" class="loginLogo" />
    </div>
    <div class="loginAuthenticationRight">
        <img src="image/login_Ess_Terms_Text.png" alt="" class="loginEssTermsText" />
    <form id="wfmTermsAndConditions" runat="server">
    <div class="LoginInnerContent">        
        <swc:sfwLabel ID="lblMessageID" EnableTheming="false" runat="Server" Visible="false" />
        <swc:sfwLabel ID="lblMessage" EnableTheming="false" runat="Server" Visible="false" />
        <swc:sfwValidationSummary ID="vlsErrors" runat="Server" />
    </div>
    <%--
    <div>
        <asp:TextBox ID="txtTermsAndConditions" runat="server" TextMode="MultiLine" ReadOnly="true" Height="50" Width="400"></asp:TextBox><br />    
    </div>
    --%>
        <div style="overflow:auto;height:448px; width:440px;" class="naviHeadPara">
            <p>
                Please read the following Terms of Use carefully before creating your UCRS Login. 
                By registering for a UCRS Login, you agree to be bound by and comply with the Terms of Use governing the use of your UCRS Login.
            </p>
            <p>
                The information that UCRS provides on this website is made available for access and use of our members 
                and retirees. These UCRS Terms of Use create no obligation on the part of UCRS to provide access to this website. 
                The UCRS terms of use specifically govern the Electronic Access to your UCRS information, contain 
                your responsibilities regarding access to your UCRS information, and contains the terms and 
                conditions under which this service is provided.
            </p>
            <p>
                Electronic information privacy protection is crucial for the ongoing success of the Internet as a 
                convenient means to provide customer service.  UCRS is committed to responsible information handling practices.  
                When you access this website you agree to the terms and conditions published here. Please note that 
                the privacy policies or statements of other websites linked to the UCRS website may differ.  
                UCRS is not responsible for the privacy practices of other websites.
            </p>
            <p>
                Security measures have been integrated into the design, implementation and day-to-day operations of this website.  
                Additionally, making sure your confidential information is secure is also an important responsibility for you.
            </p>
            <p>
                You are responsible for ensuring that your personal password remains secure, so you should not reveal your password to anyone. 				    
            </p>
            <div style="float:left;">
            <ul>
                <li>
                    You agree that the password you use to access UCRS Online Services using your UCRS Login will be kept 
                    confidential by you. If you forget your password or believe that your password has been used without 
                    authorization from you, you may reset your password online. You understand that you have sole responsibility 
                    for the security of your password.
                </li>
                <li>
                    You agree that you are fully responsible for all activities and uses that occur under your UCRS Login and password. 
                    You agree that if you permit other persons to use the Website with your credentials, you are responsible 
                    for any and all activity they authorize from your access. You agree to immediately notify UCRS Login Support 
                    of any unauthorized use of your UCRS password or any other breach of security.
                </li>
                <li>
                    By selecting “I Agree” at the end of this document you are signing this Agreement electronically and you agree 
                    that doing so is the legal equivalent of you manually signing this Agreement and that you will be legally bound 
                    by its terms and conditions.
                </li>
                <li>
                    By selecting “I Agree” at the end of this document you acknowledge that you are able to use Electronic Access 
                    to retain a record by printing and/or downloading and saving this Agreement, and any other agreements, documents, 
                    or records that you sign using your Electronic signature.  You accept as reasonable and proper notice, for 
                    the purpose of any and all laws, rules and regulations, Member Communications via Electronic Access and 
                    agree that such electronic form fully satisfies any requirement that such communications be provided 
                    to you in writing or in a form that you may keep.  
                </li>
                <li>
                    You agree that your current mailing addresses and email address are required in order for you to access this website. 
                    It is your responsibility to use this website regularly to check for updates, and to keep UCRS informed of any changes 
                    in your mailing address and your e-mail address to ensure that you receive communications.  Regardless of whether 
                    UCRS is able to deliver an email notification to you, you agree that the information will be deemed transmitted 
                    and received as soon as UCRS makes the information available to you through the Website.                        
                </li>
                <li>
                    You agree that the website may be unavailable at times for the various reasons such as maintenance, outages and other reasons.   
                    You agree that while UCRS will make reasonable efforts to ensure the availability of this website, UCRS is in no way liable 
                    for system unavailability or any type of damages that may result.                        
                </li>
                <li>
                    UCRS is not responsible for any errors or failures due to any malfunction of your personal computer, internet service or the software, 
                    or any virus or any problems whatsoever associated with the use of the web.                        
                </li>
                <li>
                    You agree that you may terminate your access to this website at any time by providing written notice to us.  UCRS, at our sole discretion, 
                    may terminate your access to this website at any time.                        
                </li>
                <li>
                    By agreeing to these terms and conditions, you also agree to our privacy policy.                        
                </li>
            </ul>
            </div>
            <p>
                The use of this UCRS website is voluntary and if you do not wish to conduct electronic business with UCRS, 
                our staff is available to assist you by phone at 1-800 618-1687.
            </p>
        </div>  
    <div style="float:left; width:440px;">
        <asp:CheckBox ID="chkAgree" runat="server" Text ="I Agree the above Terms and conditions" CssClass="agreeTerms" />
    </div>
    <div style="float:left; width:440px;">
        <span>
            <asp:Button ID="btnContinue" runat="server" CssClass="loginContinueTermsButton" OnClick="btnContinue_Click"/>
        </span>
        <span>
            <asp:Button ID="btnCancel" runat="server" CssClass="loginCancelButton" OnClick="btnCancel_Click" />
        </span>
    </div>
 </form>
  </div>
  </div>
        <div class="loginAuthenticationTermsBoxBgBtm"></div>
    </div>
</div>
</body>
</html>
