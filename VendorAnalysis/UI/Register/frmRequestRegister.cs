using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using DevExpress.Utils.Paint;
using CrystalDecisions.CrystalReports.Engine;
using VendorAnalysis.BusinessLayer;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace VendorAnalysis
{
    public partial class frmRequestRegister : Form
    {

       void 
        

        #region Variabless
        VendorAnalysis.BusinessLayer.RequestEntryBL oReqBL;
        public static Telerik.WinControls.UI.Docking.DocumentWindow m_oDW = new Telerik.WinControls.UI.Docking.DocumentWindow();
        public Telerik.WinControls.UI.RadPanel Radpanel { get; set; }

        public static GridView m_oGridView = new GridView();
        string m_sSelectionFormula = "";
        int m_iCCId = 0;
        int RequestId = 0;
        DataTable dtRQC = null;

        #endregion


        #region Constructor

        public frmRequestRegister()
        {
            InitializeComponent();
            oReqBL = new BusinessLayer.RequestEntryBL();
        }

#endregion

        #region Form Load
       constant 
        //private void frmRequestRegister_Load(object sender, EventArgs e)
        //{
        //    clsStatics.SetMyGraphics();
        //    dtpFrmDate.EditValue = RequestEntryBL.GetMinDate();
        //    dtpTodate.EditValue = DateTime.Now;
        //    dwOptions.Hide();
        //    dwReqCancel.Hide();
        //    dwRegister.Show();
        //    PopulateGrid();     
        //}
        


        private void frmRequestRegister_Load(object sender, EventArgs e)
        {
            clsStatics.SetMyGraphics();
            dtpFrmDate.EditValue = RequestEntryBL.GetMinDate();
            dtpTodate.EditValue = DateTime.Now;
            dwOptions.Hide();
            dwReqCancel.Hide();
            dwRegister.Show();
            PopulateGrid();     
        }

        private void frmRequestRegister_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (BsfGlobal.g_bWorkFlow == true)
            //{
            //    try { Parent.Controls.Owner.Hide(); }
            //    catch { }
            //}
            //else
            //{
            //    CommFun.DW1.Hide();
            //    CommFun.RP1.Controls.Clear();
            //}
        }
        #endregion

        #region Functions

        private void PopulateGrid()
        {
            DataTable dt = new DataTable();
            DataView dv;
            DataTable dtReqReg = new DataTable();
            dtReqReg = oReqBL.GetRequestRegDetails(Convert.ToDateTime(dtpFrmDate.EditValue), Convert.ToDateTime(dtpTodate.EditValue));

            if (m_iCCId != 0)
            {
                dv = new DataView(dtReqReg);
                dv.RowFilter = "CostCentreId=" + m_iCCId + "";
                dt = dv.ToTable();
                grdReqList.DataSource = dt;
                ReqListView.Columns["CostCentreId"].Visible = false;
                ReqListView.Columns["RequestId"].Visible = false;
            }
            else
            {
                grdReqList.DataSource = dtReqReg;
                ReqListView.Columns["CostCentreId"].Visible = false;
                ReqListView.Columns["RequestId"].Visible = false;
            }
            ReqListView.Columns["Approved"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            ReqListView.Columns["Approve"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        }


        #endregion

        #region GridEvent
        private void ReqListView_DoubleClick(object sender, EventArgs e)
        {
            //int ReqId = Convert.ToInt32(ReqListView.GetFocusedRowCellValue("RequestId"));
            //clsStatics.DW1.Controls.Clear();
            //frmRequestEntry frm = new frmRequestEntry();
            //frm.RequestId = ReqId;
            //frm.Dock = DockStyle.Fill;
            //frm.TopLevel = false;
            //clsStatics.DW1.Controls.Add(frm);
            //frm.Show();


            if (grdReqList.DataSource != null)
            {

                int ReqId = Convert.ToInt32(ReqListView.GetFocusedRowCellValue("RequestId"));
                if (BsfGlobal.g_bWorkFlow == true)
                {
                    BsfGlobal.g_bTrans = true;
                    //try { Parent.Controls.Owner.Hide(); } catch { }
                    frmRequestEntry frm = new frmRequestEntry() { RequestId = Convert.ToInt32(ReqListView.GetFocusedRowCellValue("RequestId").ToString()) };
                    m_oDW = (Telerik.WinControls.UI.Docking.DocumentWindow)BsfGlobal.g_oDock.ActiveWindow;
                    m_oDW.Hide();
                    BsfGlobal.g_bTrans = false;
                    m_oGridView = ReqListView;
                    Cursor.Current = Cursors.WaitCursor;
                    DevExpress.XtraEditors.PanelControl oPanel = new DevExpress.XtraEditors.PanelControl();
                    oPanel = BsfGlobal.GetPanel(frm, "Request Entry");
                    if ((oPanel == null))
                        return;
                    oPanel.Controls.Clear();
                    frm.TopLevel = false;
                    frm.FormBorderStyle = FormBorderStyle.None;
                    frm.Dock = DockStyle.Fill;
                    oPanel.Controls.Add(frm);
                    frm.Radpanel = Radpanel;
                    frm.RequestId = ReqId;
                    frm.Execute("E");
                    oPanel.Visible = true;
                    Cursor.Current = Cursors.Default;
                }
                else
                {
                    frmRequestEntry frm = new frmRequestEntry() { RequestId = Convert.ToInt32(ReqListView.GetFocusedRowCellValue("RequestId").ToString()) };
                    Cursor.Current = Cursors.WaitCursor;
                    CommFun.RP1.Controls.Clear();
                    frm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    frm.TopLevel = false;
                    CommFun.RP1.Controls.Add(frm);
                    frm.Dock = DockStyle.Fill;
                    frm.Execute("E");

                }
            }
        }      
        #endregion
        
        #region Button Event

        private void btnExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }

        private void btnRegister_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dwRegister.Show();
            dwOptions.Hide();
            dwReqCancel.Hide();
            grdReqList.DataSource = null;
            ReqListView.Columns.Clear();
            ReqListView.GroupSummary.Clear();
            PopulateGrid();
        }

        private void btnCCWise_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dwRegister.Show();
            dwOptions.Hide();
            dwReqCancel.Hide();
            grdReqList.DataSource = null;
            ReqListView.Columns.Clear();
            ReqListView.GroupSummary.Clear();
            PopulateGrid();
            ReqListView.Columns["CostCentre"].Group();
            ReqListView.ExpandAllGroups();
        }


        private void btnPenRequest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime dtFDate = Convert.ToDateTime(dtpFrmDate.EditValue);
            DateTime dtTDate = Convert.ToDateTime(dtpTodate.EditValue);
            dwRegister.Hide();
            dwOptions.Show();
            dwReqCancel.Hide();
            frmRequestRPT frm = new frmRequestRPT();
            dwOptions.Text = "Request Options";
            frm.TopLevel = false;
            dwOptions.Controls.Clear();
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            dwOptions.Controls.Add(frm);
            dwOptions.Show();
            frm.Execute("Pending", Convert.ToDateTime(clsStatics.IsNullCheck(dtFDate, clsStatics.datatypes.VarTypeDate)), Convert.ToDateTime(clsStatics.IsNullCheck(dtTDate, clsStatics.datatypes.VarTypeDate)));

        }

        private void btnReqStatus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (ReqListView.GetFocusedRow() == null) { return; }
            int iRegId = Convert.ToInt32(clsStatics.IsNullCheck(ReqListView.GetRowCellValue(ReqListView.FocusedRowHandle, "RequestId"), clsStatics.datatypes.vartypenumeric));
            string sRefNo = clsStatics.IsNullCheck(ReqListView.GetRowCellValue(ReqListView.FocusedRowHandle, "RequestNo"), clsStatics.datatypes.vartypestring).ToString();
            BsfForm.frmLogHistory frm = new BsfForm.frmLogHistory();
            frm.Execute(iRegId, "Request", "Request-Approve", sRefNo, BsfGlobal.g_sVendorDBName);
        }

        private void btnReqVoucher_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable dt = new DataTable();
            dt = clsStatics.GetProjDetails(Convert.ToInt32(clsStatics.IsNullCheck(ReqListView.GetFocusedRowCellValue("CostCentreId"), clsStatics.datatypes.vartypenumeric)));
            frmReport objReport = new frmReport();
            string strReportPath = Application.StartupPath + "\\ReqVoucher.Rpt";
            ReportDocument cryRpt = new ReportDocument();
            cryRpt.Load(strReportPath);
            m_sSelectionFormula = "{RequestRegister.RequestId} = " + ReqListView.GetFocusedRowCellValue("RequestId") + " ";
            string[] DataFiles = new string[] 
            {
                BsfGlobal.g_sVendorDBName,
                BsfGlobal.g_sVendorDBName,
                BsfGlobal.g_sRateAnalDBName,
                BsfGlobal.g_sRateAnalDBName,
                BsfGlobal.g_sRateAnalDBName,
                BsfGlobal.g_sWorkFlowDBName,
            };
            objReport.ReportConvert(cryRpt, DataFiles);
            if (m_sSelectionFormula.Length > 0)
                cryRpt.RecordSelectionFormula = m_sSelectionFormula;
            objReport.rptViewer.ReportSource = null;
            objReport.rptViewer.ReportSource = cryRpt;
            if (dt.Rows.Count > 0)
            {
                cryRpt.DataDefinition.FormulaFields["@CCName"].Text = " '" + dt.Rows[0]["CompanyName"].ToString() + "'";
                cryRpt.DataDefinition.FormulaFields["@CAddress"].Text = " '" + dt.Rows[0]["Address"].ToString() + "'";
                cryRpt.DataDefinition.FormulaFields["@CCity"].Text = " '" + dt.Rows[0]["CityName"].ToString() + "'";
                cryRpt.DataDefinition.FormulaFields["@CState"].Text = " '" + dt.Rows[0]["StateName"].ToString() + "'";
                cryRpt.DataDefinition.FormulaFields["@CCountry"].Text = " '" + dt.Rows[0]["CountryName"].ToString() + "'";
                cryRpt.DataDefinition.FormulaFields["@CPinCode"].Text = " '" + dt.Rows[0]["Pincode"].ToString() + "'";
                cryRpt.DataDefinition.FormulaFields["@CPhone"].Text = " '" + dt.Rows[0]["Phone"].ToString() + "'";
                cryRpt.DataDefinition.FormulaFields["@CFax"].Text = " '" + dt.Rows[0]["Fax"].ToString() + "'";
            }
            objReport.rptViewer.Refresh();
            objReport.Show();
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (BsfGlobal.FindPermission("Request-Delete") == false)
            {
                MessageBox.Show("No Rights to Delete Request", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int RequestId = Convert.ToInt32(clsStatics.IsNullCheck(ReqListView.GetFocusedRowCellValue("RequestId").ToString(),clsStatics.datatypes.vartypenumeric));
            int argCCId = Convert.ToInt32(clsStatics.IsNullCheck(ReqListView.GetFocusedRowCellValue("CostCentreId"), clsStatics.datatypes.vartypenumeric));
            if (RequestEntryBL.ValidDelete(RequestId) == true)
            {
                if (MessageBox.Show("Are you sure want to Delete this Register", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question).ToString() == "Yes")
                {
                    RequestEntryBL.DeleteRegister(RequestId, argCCId);
                    ReqListView.DeleteRow(ReqListView.FocusedRowHandle);
                }
            }
            else
            {
                MessageBox.Show("Already Used. Do Not Delete!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private void btnReqHistory_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime dtFDate = Convert.ToDateTime(dtpFrmDate.EditValue);
            DateTime dtTDate = Convert.ToDateTime(dtpTodate.EditValue);
            dwRegister.Hide();
            dwOptions.Show();
            dwReqCancel.Hide();
            frmRequestRPT frm = new frmRequestRPT();
            dwOptions.Text = "Request Options";
            frm.TopLevel = false;
            dwOptions.Controls.Clear();
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            dwOptions.Controls.Add(frm);
            dwOptions.Show();
            frm.Execute("History", Convert.ToDateTime(clsStatics.IsNullCheck(dtFDate, clsStatics.datatypes.VarTypeDate)), Convert.ToDateTime(clsStatics.IsNullCheck(dtTDate, clsStatics.datatypes.VarTypeDate)));
        }

        private void btnVocherP_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = clsStatics.GetProjDetails(Convert.ToInt32(clsStatics.IsNullCheck(ReqListView.GetFocusedRowCellValue("CostCentreId"), clsStatics.datatypes.vartypenumeric)));
                frmReport objReport = new frmReport();
                string strReportPath = Application.StartupPath + "\\ReqVoucher.Rpt";
                ReportDocument cryRpt = new ReportDocument();
                cryRpt.Load(strReportPath);
                m_sSelectionFormula = "{RequestRegister.RequestId} = " + ReqListView.GetFocusedRowCellValue("RequestId") + " ";

                string[] DataFiles = new string[] 
                {
                    BsfGlobal.g_sVendorDBName,
                    BsfGlobal.g_sVendorDBName,
                    BsfGlobal.g_sRateAnalDBName,
                    BsfGlobal.g_sRateAnalDBName,
                    BsfGlobal.g_sRateAnalDBName,
                    BsfGlobal.g_sWorkFlowDBName
                };
                objReport.ReportConvert(cryRpt, DataFiles);
                if (m_sSelectionFormula.Length > 0)
                    cryRpt.RecordSelectionFormula = m_sSelectionFormula;
                objReport.rptViewer.ReportSource = null;
                objReport.rptViewer.ReportSource = cryRpt;
                if (dt.Rows.Count > 0)
                {
                    cryRpt.DataDefinition.FormulaFields["@CCName"].Text = " '" + dt.Rows[0]["CompanyName"].ToString() + "'";
                    cryRpt.DataDefinition.FormulaFields["@CAddress"].Text = " '" + dt.Rows[0]["Address"].ToString() + "'";
                    cryRpt.DataDefinition.FormulaFields["@CCity"].Text = " '" + dt.Rows[0]["CityName"].ToString() + "'";
                    cryRpt.DataDefinition.FormulaFields["@CState"].Text = " '" + dt.Rows[0]["StateName"].ToString() + "'";
                    cryRpt.DataDefinition.FormulaFields["@CCountry"].Text = " '" + dt.Rows[0]["CountryName"].ToString() + "'";
                    cryRpt.DataDefinition.FormulaFields["@CPinCode"].Text = " '" + dt.Rows[0]["Pincode"].ToString() + "'";
                    cryRpt.DataDefinition.FormulaFields["@CPhone"].Text = " '" + dt.Rows[0]["Phone"].ToString() + "'";
                    cryRpt.DataDefinition.FormulaFields["@CFax"].Text = " '" + dt.Rows[0]["Fax"].ToString() + "'";
                }
                objReport.rptViewer.Refresh();
                objReport.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnReqReg_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = clsStatics.GetProjDetails(Convert.ToInt32(clsStatics.IsNullCheck(ReqListView.GetFocusedRowCellValue("CostCentreId"), clsStatics.datatypes.vartypenumeric)));

                string strReportPath = "";

                frmReport objReport = new frmReport();
                if (m_iCCId != 0)
                    strReportPath = Application.StartupPath + "\\VAReqRegisterCC.Rpt";
                else
                    strReportPath = Application.StartupPath + "\\VAReqRegister.Rpt";
                ReportDocument cryRpt = new ReportDocument();
                string fstring = "";

                //string frmdat = string.Format("{0:dd/MMM/yyyy}", Convert.ToDateTime(dxFromDate.EditValue));
                //string tdat = string.Format("{0:dd/MMM/yyyy}", Convert.ToDateTime(dxToDate.EditValue));
                //fstring = "Detailed PO Register From " + frmdat + " To " + tdat;

                cryRpt.Load(strReportPath);


                string[] DataFiles = new string[] 
                {
                    BsfGlobal.g_sMMSDBName,
                    BsfGlobal.g_sMMSDBName,
                    BsfGlobal.g_sMMSDBName,
                    BsfGlobal.g_sRateAnalDBName,
                    BsfGlobal.g_sMMSDBName,
                    BsfGlobal.g_sWorkFlowDBName,
                    BsfGlobal.g_sMMSDBName,
                    BsfGlobal.g_sMMSDBName 
                };

                objReport.ReportConvert(cryRpt, DataFiles);


                objReport.rptViewer.ReportSource = null;
                objReport.rptViewer.ReportSource = cryRpt;

                if (m_iCCId != 0)
                    cryRpt.SetParameterValue("CCId", m_iCCId);

                cryRpt.SetParameterValue("VMDB", BsfGlobal.g_sVendorDBName);
                cryRpt.SetParameterValue("MMSDB", BsfGlobal.g_sMMSDBName);
                cryRpt.SetParameterValue("WFDB", BsfGlobal.g_sWorkFlowDBName);

                if (dt.Rows.Count > 0)
                {
                    cryRpt.DataDefinition.FormulaFields["CompCaption"].Text = " '" + dt.Rows[0]["CompanyName"].ToString() + "'";
                }
                cryRpt.DataDefinition.FormulaFields["FString"].Text = " '" + fstring + "'";



                objReport.rptViewer.Refresh();
                objReport.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmFilter frm = new frmFilter();
            frm.Execute("C");
            m_iCCId = frm.m_iId;
            PopulateGrid();
        }

        private void ReqListView_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (info.Column.Name == "colCostCentre")
            {
                info.GroupText = info.GroupValueText;
                e.Appearance.ForeColor = Color.Blue;
                e.Appearance.Font = new Font("Verdana", 9, FontStyle.Bold);
            }
        }

        private void btnRQC_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dwRegister.Show();
            dwReqCancel.Show();
            dwOptions.Hide();
            radDock1.ActiveWindow = dwReqCancel;
            RequestId = Convert.ToInt32(ReqListView.GetFocusedRowCellValue("RequestId"));
            dtRQC = RequestEntryBL.GetRQC(RequestId);
            grdReqCancel.DataSource = dtRQC;
            grdReqCancel.ForceInitialize();
            ReqCancelView.PopulateColumns();
            ReqCancelView.Columns["RequestId"].Visible = false;
            ReqCancelView.Columns["RequestTransId"].Visible = false;
            ReqCancelView.Columns["UnitId"].Visible = false;
            ReqCancelView.Columns["ResourceId"].Visible = false;
            ReqCancelView.Columns["HiddenQty"].Visible = false;
            DevExpress.XtraEditors.Repository.RepositoryItemTextEdit txtCQty = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
            ReqCancelView.Columns["CancelQty"].ColumnEdit = txtCQty;
            txtCQty.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtCQty.Mask.EditMask = "#########################.#####";

        }

        private void btnOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ReqCancelView.FocusedRowHandle = ReqCancelView.FocusedRowHandle + 1;
            RequestEntryBL.RQCTransaction(dtRQC);
            grdReqCancel.DataSource = null;
            grdReqCancel.RefreshDataSource();
        }

        private void ReqCancelView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            decimal dBQty = Convert.ToDecimal(clsStatics.IsNullCheck(ReqCancelView.GetFocusedRowCellValue("HiddenQty"), clsStatics.datatypes.vartypenumeric));
            decimal dCQty = Convert.ToDecimal(clsStatics.IsNullCheck(ReqCancelView.GetFocusedRowCellValue("CancelQty"), clsStatics.datatypes.vartypenumeric));
            if (dCQty > 0)
            {
                if (dBQty != dCQty)
                {
                    MessageBox.Show("Enter Valid Quantity!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ReqCancelView.SetRowCellValue(ReqCancelView.FocusedRowHandle, "CancelQty", "0.00000");
                    return;
                }
            }
        }
        
        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            dwRegister.Show();
            dwOptions.Hide();
            dwReqCancel.Hide();
        }

        #endregion

        #region Date Event

        private void dtpFrmDate_EditValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToDateTime(dtpFrmDate.EditValue) < Convert.ToDateTime(dtpFrmDate.EditValue))
            {
                MessageBox.Show("From Date Greater than to Date..", "Information");
                dtpFrmDate.EditValue = "01/04/2011";
                return;
            }
            PopulateGrid();          
        }

        private void dtpFrmDate_ItemPress(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (Convert.ToDateTime(dtpFrmDate.EditValue) < Convert.ToDateTime(dtpFrmDate.EditValue))
            {
                MessageBox.Show("From Date Greater than to Date..", "Information");
                dtpFrmDate.EditValue = "01/04/2011";
                return;
            }
            PopulateGrid();          
        }

        private void dtpTodate_ItemPress(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (Convert.ToDateTime(dtpTodate.EditValue) > Convert.ToDateTime(dtpTodate.EditValue))
            {
                MessageBox.Show("To Date Less than From Date..", "Information");
                dtpTodate.EditValue = DateTime.Now;
                return;
            }
            PopulateGrid();          
        }

        private void dtpTodate_EditValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToDateTime(dtpTodate.EditValue) > Convert.ToDateTime(dtpTodate.EditValue))
            {
                MessageBox.Show("To Date Less than From Date..", "Information");
                dtpTodate.EditValue = DateTime.Now;
                return;
            }
            PopulateGrid();
        }

        #endregion

        private void btnCancelRequest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime dtFDate = Convert.ToDateTime(dtpFrmDate.EditValue);
            DateTime dtTDate = Convert.ToDateTime(dtpTodate.EditValue);
            dwRegister.Hide();
            dwOptions.Show();
            dwReqCancel.Hide();
            frmRequestRPT frm = new frmRequestRPT();
            dwOptions.Text = "Cancel Request";
            frm.TopLevel = false;
            dwOptions.Controls.Clear();
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            dwOptions.Controls.Add(frm);
            dwOptions.Show();
            frm.Execute("Cancel", Convert.ToDateTime(clsStatics.IsNullCheck(dtFDate, clsStatics.datatypes.VarTypeDate)), Convert.ToDateTime(clsStatics.IsNullCheck(dtTDate, clsStatics.datatypes.VarTypeDate)));
        }
    }
}
