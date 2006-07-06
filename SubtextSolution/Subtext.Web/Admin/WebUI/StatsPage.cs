using System;
using System.Web.UI.WebControls;
using Subtext.Web.Admin.Pages;

namespace Subtext.Web.Admin.WebUI
{
    public class StatsPage : AdminPage
    {
        public StatsPage() : base()
        {
            this.TabSectionId = "Stats";
        }

        protected virtual void BindLocalUI()
        {
            HyperLink lnkReferrals = Utilities.CreateHyperLink("Referrals", "Referrers.aspx");
            HyperLink lnkViews = Utilities.CreateHyperLink("Views", "StatsView.aspx");
            HyperLink lnkErrorLog = Utilities.CreateHyperLink("Error Log", "ErrorLog.aspx");

            // Add the buttons to the PageContainer.
            AdminMasterPage.AddToActions(lnkReferrals);
            AdminMasterPage.AddToActions(lnkViews);
            AdminMasterPage.AddToActions(lnkErrorLog);
        }

        protected override void OnLoad(EventArgs e)
        {
            BindLocalUI();
            base.OnLoad(e);
        }
    }
}