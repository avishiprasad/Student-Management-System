using System;
using System.Net;
using System.Security.Principal;
using Microsoft.Reporting.WebForms;

namespace SSRSReportViewer
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                /*ReportViewer1.ProcessingMode = ProcessingMode.Remote;   

                // Use the correct Report Server URL (SOAP endpoint)
                ReportViewer1.ServerReport.ReportServerUrl = new Uri("http://172.24.7.208/DMS");

                // The exact path of your report on the report server
                ReportViewer1.ServerReport.ReportPath = "/Training/StudentReport";

                // Use Windows authentication credentials
                ReportViewer1.ServerReport.ReportServerCredentials = new WindowsAuthReportCredentials();

                ReportViewer1.ServerReport.Refresh();  */

                // Get report path from query parameter
                string reportPath = Request.QueryString["report"];

                if (string.IsNullOrWhiteSpace(reportPath))
                {
                    // Fallback/default report
                    reportPath = "/Training/StudentReport";
                }

                ReportViewer1.ProcessingMode = ProcessingMode.Remote;

                // SSRS Report Server SOAP endpoint
                ReportViewer1.ServerReport.ReportServerUrl = new Uri("http://172.24.7.208/DMS");

                // Use the dynamic report path
                ReportViewer1.ServerReport.ReportPath = reportPath;

                // Use Windows credentials
                ReportViewer1.ServerReport.ReportServerCredentials = new WindowsAuthReportCredentials();

                ReportViewer1.ServerReport.Refresh();
            }
        }
    }
}
