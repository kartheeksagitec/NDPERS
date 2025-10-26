<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="wfmReportClient.aspx.cs" Inherits="Neo.AspxPages.wfmReportClient" %>

<%--<%string lstrRprt = NeoSpinCommon.ApplicationSettings.Instance.ReportTool; %>
<% if (!string.IsNullOrEmpty(lstrRprt))
    {
        switch (lstrRprt.ToUpper())
        {
            case NeoSpinCommon.ReportConstants.SSRS_REPORT: %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%break;
    case NeoSpinCommon.ReportConstants.CRYSTAL_REPORT: %>--%>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<%--<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>--%>
<%--<% break;
        }
    }%>--%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body onload="DisplayMessage()" style="overflow-y:hidden;">
    <form id="form1" runat="server">
       <%-- <% string lstrRprt = ApplicationSettings.Instance.ReportTool;
            if (!string.IsNullOrEmpty(lstrRprt))
            {
                switch (lstrRprt.ToUpper())
                {
                    case NeoSpin.Common.ReportConstants.SSRS_REPORT: %>
        <asp:ScriptManager ID="Scriptmanager1" runat="server"></asp:ScriptManager>
        <div>
            <rsweb:ReportViewer ID="rvViewer" runat="server" Width="100%" SizeToReportContent="true" Height="100%" Visible="false" Enabled="False" />
        </div>
        <% break;
            case NeoSpin.Common.ReportConstants.CRYSTAL_REPORT: %>
        <div>
        </div>--%>

         <asp:ScriptManager ID="Scriptmanager1" runat="server"></asp:ScriptManager>
<%--       <rsweb:ReportViewer ID="rvViewer" runat="server" Width="100%" SizeToReportContent="true" Height="100%"  />--%>
        
        <CR:CrystalReportViewer ID="CRViewer" runat="server" AutoDataBind="true" />
      <%--  <% break;--%>
               <%-- }--%>
          <%--  } %>--%>
    </form>
    <script>
        function DisplayMessage() {
            if (window.parent.ns.viewModel.currentModel.startsWith("wfmReportClient"))
                window.parent.nsCommon.DispalyMessage("[ Report Successfully Generated ]", window.parent.ns.viewModel.currentModel);
        }
    </script> 
</body>
</html>
