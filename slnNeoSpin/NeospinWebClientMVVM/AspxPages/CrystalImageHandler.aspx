<%@ Page Language="C#" AutoEventWireup="true" %>

<script runat="server" language="c#">
    protected void Page_Load(object sender, EventArgs e)
    {
        CrystalDecisions.Web.CrystalImageHandler handler = new CrystalDecisions.Web.CrystalImageHandler();
        handler.ProcessRequest(this.Context);
    }
</script>
