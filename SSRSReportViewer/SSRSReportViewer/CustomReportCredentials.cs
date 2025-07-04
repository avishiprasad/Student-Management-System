using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using Microsoft.Reporting.WebForms;
using System.Net; // ✅ For ICredentials


namespace SSRSReportViewer
{
    public class CustomReportCredentials : IReportServerCredentials
    {
        private readonly string _username;
        private readonly string _password;
        private readonly string _domain;

        public CustomReportCredentials(string username, string password, string domain)
        {
            _username = username;
            _password = password;
            _domain = domain;
        }

        public WindowsIdentity ImpersonationUser
        {
            get { return null; }  // Use default identity
        }

        public ICredentials NetworkCredentials
        {
            get { return new NetworkCredential(_username, _password, _domain); }
        }

        public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
        {
            // We are not using Forms authentication.
            authCookie = null;
            user = password = authority = null;
            return false;
        }
    }
}

