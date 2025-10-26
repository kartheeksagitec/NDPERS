<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="sfuCRViewer.ascx.cs" Inherits="Neo.sfuCRViewer" %>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304"
    Namespace="CrystalDecisions.Web" TagPrefix="CR" %>
<CR:CrystalReportViewer ID="CRViewerInner" 
    Style="padding: 5px; height:1000px;width:1000px;" runat="server" 
    AutoDataBind="false" HasCrystalLogo="False" 
    HasToggleParameterPanelButton="false" EnableDrillDown="False" 
    GroupTreeStyle-ShowLines="False" HasDrilldownTabs="False" 
    HasDrillUpButton="False" HasSearchButton="False" 
    HasToggleGroupTreeButton="False" Height="50px" ToolPanelView="None" 
    Width="350px" />