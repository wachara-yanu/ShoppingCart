<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Register assembly="Telerik.ReportViewer.WebForms, Version=6.0.12.215, Culture=neutral, PublicKeyToken=a9d7983dfcc261be" namespace="Telerik.ReportViewer.WebForms" tagprefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body style="height: 574px">
    <form id="form1" runat="server">
    <div>
        <telerik:ReportViewer ID="rpViewer" runat="server" Height="600px" 
            Width="1137px">
        </telerik:ReportViewer>
    </div>
    <script runat = "server">
       
        protected override void OnLoad(EventArgs e)
        {
            
            var objectDataSource = new Telerik.Reporting.ObjectDataSource();
            Ouikum.Web.MyB2B.SupplierReport reportview = new Ouikum.Web.MyB2B.SupplierReport();

            reportview.ReportParameters["AssignLeadCode"].Value = ViewBag.AssignLeadCode;
            reportview.ReportParameters["ToContactName"].Value = ViewBag.ContactName;
            reportview.ReportParameters["ToCompName"].Value = ViewBag.ContactCompName;
            reportview.ReportParameters["ToContactTel"].Value = ViewBag.ContactEmail;
            reportview.ReportParameters["ToContactEmail"].Value = ViewBag.ContactTel;
            reportview.ReportParameters["AssignLeadName"].Value = ViewBag.AssignLeadName;
            reportview.ReportParameters["CreatedDate"].Value = (DateTime)ViewBag.CreatedDate;
           
            
            reportview.DataSource = objectDataSource;
            
            rpViewer.Report = reportview;
            objectDataSource.DataSource = ViewBag.Company;
            rpViewer.DataBind();
            rpViewer.RefreshReport();
            base.OnLoad(e);
            
        }

    </script>
    

    </form>
    </body>
</html>
