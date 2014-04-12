using System;
using System.Web.Security;
using System.Web.UI;
using Clickfarm.AppFramework.Logging;
using System.Web.Mvc;

namespace KCActorsTheatre.Web
{
    public partial class LogIn : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Url.Host == "www.wachter.com" || Request.Url.Host == "wachter.com")
            {
                DependencyResolver.Current.GetService<ILogger>().Error("The /login.aspx page on wachter.com has loaded.");
                Response.StatusCode = 404;
                try
                {
                    Response.End();
                }
                catch { }
            }
        }

        protected void ProcessLogin(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                if (FormsAuthentication.Authenticate(txtUsername.Text, txtPassword.Text))
                {
                    FormsAuthentication.RedirectFromLoginPage(txtUsername.Text, chkPersistLogin.Checked);
                }
                else
                {
                    divErr.InnerHtml = "<b>Incorrect username or password.</b> Please re-enter your credentials.";
                }
            }
        }
    }
}
