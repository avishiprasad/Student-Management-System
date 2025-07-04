using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Security.Principal;
using Microsoft.Reporting.WebForms;

namespace SSRSReportViewer
{
  public class WindowsAuthReportCredentials : IReportServerCredentials
    {
        public WindowsIdentity ImpersonationUser => null; // use default identity

        public ICredentials NetworkCredentials => CredentialCache.DefaultNetworkCredentials;

        public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
        {
            authCookie = null;
            user = password = authority = null;
            return false;
        }
  }

}
