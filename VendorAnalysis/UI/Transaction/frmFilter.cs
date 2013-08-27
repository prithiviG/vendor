using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VendorAnalysis.BusinessLayer;
using VendorAnalysis.BusinessObjects;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using CrystalDecisions.CrystalReports.Engine;

namespace VendorAnalysis
{
    public partial class frmFilter: Form
    {

        #region Variables
        public int m_iId = 0;
        string m_ssName = "";
        public string m_sName = "";
        DataSet m_dsCostcentre;     
        #endregion

        #region Object
               
        #endregion

        #region Constructor

        public frmFilter()
        {
            InitializeComponent();           
        }
        #endregion

        #region Form Load

        private void frmFilter_Load(object sender, EventArgs e)
        {           
            m_dsCostcentre = new DataSet();
            m_dsCostcentre = clsStatics.PopulateCostCentreVendor();
            if (m_ssName == "C")
            {
                labelControl1.Text = "Cost centre wise";
                this.Text = "CostCentre  ";
                PopulateCostCentre();
            }
            
            cboAcc.EditValue = 0;
        }
       

        #endregion

        #region Button Event
        private void btnOK_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {          
            this.Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
            this.Close();
        }
        #endregion

        #region Functions

        public void Execute(string  argfrom)
        {
            m_ssName = argfrom;
            this.ShowDialog();          
           
        }

        private void PopulateCostCentre()
        {
            cboAcc.Properties.DataSource = null;
            cboAcc.Properties.Columns.Clear();
            DataRow dr;
            DataView dv = new DataView(m_dsCostcentre.Tables[0]);
            dv.RowFilter = "CostCentreId=0";
            if (dv.ToTable().Rows.Count > 0) { }
            else
            {
                dr = m_dsCostcentre.Tables[0].NewRow();
                dr["CostCentreName"] = "All";
                dr["CostCentreId"] = 0;
                m_dsCostcentre.Tables[0].Rows.InsertAt(dr, 0);
            }

            cboAcc.Properties.DataSource = m_dsCostcentre.Tables[0];
            cboAcc.Properties.PopulateColumns();
            cboAcc.Properties.DisplayMember = "CostCentreName";
            cboAcc.Properties.ValueMember = "CostCentreId";
            cboAcc.Properties.Columns["CostCentreId"].Visible = false;
            cboAcc.Properties.ShowFooter = false;
            cboAcc.Properties.ShowHeader = false;
        }

        private void PopulateContractor()
        {
            cboAcc.Properties.DataSource = null;
            cboAcc.Properties.Columns.Clear();

            DataTable dt = new DataTable();
            DataRow dr;
            DataView dv = new DataView(m_dsCostcentre.Tables[1]);
            dv.RowFilter = "VendorId=0";
            if (dv.ToTable().Rows.Count > 0) { }
            else
            {
                dr = m_dsCostcentre.Tables[1].NewRow();
                dr["VendorName"] = "All";
                dr["VendorId"] = 0;
                m_dsCostcentre.Tables[1].Rows.InsertAt(dr, 0);
            }

            cboAcc.Properties.DataSource = m_dsCostcentre.Tables[1];

            cboAcc.Properties.PopulateColumns();
            cboAcc.Properties.DisplayMember = "VendorName";
            cboAcc.Properties.ValueMember = "VendorId";
            cboAcc.Properties.Columns["VendorId"].Visible = false;
            cboAcc.Properties.ShowFooter = false;
            cboAcc.Properties.ShowHeader = false;
        }

        void cboAcc_EditValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cboAcc.EditValue) != 0)
            {
                DevExpress.XtraEditors.LookUpEdit editor = (DevExpress.XtraEditors.LookUpEdit)sender;
                DataRowView row = editor.Properties.GetDataSourceRowByKeyValue(editor.EditValue) as DataRowView;
                if (m_ssName == "C")
                {
                    m_iId = Convert.ToInt32(row["CostCentreId"].ToString());
                    m_sName = row["CostCentreName"].ToString();
                }
                else
                {
                    m_iId = Convert.ToInt32(row["VendorId"].ToString());
                    m_sName = row["VendorName"].ToString();
                }
                editor.EditValue = m_iId;
            }
        }
        #endregions

       

       

      

    }
}
