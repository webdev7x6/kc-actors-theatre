using System;
using System.Web.Security;
using System.Web.UI;

namespace Web
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ProcessLogin(Object objSender, EventArgs objArgs)
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