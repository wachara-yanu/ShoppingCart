namespace Prosoft.Base
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for BaseReport.
    /// </summary>
    public partial class BaseReport : Telerik.Reporting.Report
    {

        //public BaseDTO dtoReport = new BaseDTO();
        
        public BaseReport()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

    }
}