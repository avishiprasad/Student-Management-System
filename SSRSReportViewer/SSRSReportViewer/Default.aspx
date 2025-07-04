<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SSRSReportViewer.Default" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SSRS Report Viewer</title>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Required for ReportViewer -->
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <!-- ReportViewer control -->
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="800px" ProcessingMode="Remote" />
    </form>
</body>
</html>





