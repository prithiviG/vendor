using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VendorAnalysis.BusinessLayer;
using VendorAnalysis.BusinessObjects;
using DevExpress.XtraEditors.Repository;
using System.Reflection;
using System.Collections;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils.Paint;
using Telerik.WinControls.UI;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraVerticalGrid.Rows;

namespace VendorAnalysis
{
    public partial class frmRequestEntry : Form
    {
        #region Variables
        int m_iRevId = 0;
        int m_iResourceId = 0;        
        string m_sSplit = "";
        
        EditorRow m_editorRow1;
        DataTable dtCC;
        DataTable dtIOWPop;
        DataTable dtRpop;
        DataTable dtRes;
        DataTable dtRTrans = new DataTable();
        DataSet dsRDet;
        DataSet dsAnalysisHead;
        DataTable dtAnal;
        DataTable grdAnal;
        DataTable dtAnalUpdate;
        DataTable dtReqSchedule;
        string sComps = "";
        string sRComps = "";
        string Qtype = "";
        string ProjDb = "";
        int m_sCompTypeId = 0;
        int ReqTransId = 0;
        public string m_sMode = "";
        int m_iCCId = 0;
        public int RequestId = 0;
        public RadPanel Radpanel { get; set; }
        int ResId = 0;
        Decimal VariantQty = 0;
        Decimal VarAmt = 0;
        Decimal castdecimal = 0;
        string m_sDescription = "";
        DataTable dtProjDetails;
        public bool m_bViewScreen = false;
        decimal m_VariantQty = 0;
        
        DataTable m_dtWOIOW = new DataTable();
        DataTable m_dtSWOIOW = new DataTable();

       
        #endregion

        #region Objects

        BsfGlobal.VoucherType oVType;
        BsfGlobal.VoucherType oVCCType;

        List<WOIOWTransUpdate> oWOIOWTrans = new List<WOIOWTransUpdate>();
        List<WOWBSTrans> m_lWOWBS = new List<WOWBSTrans>();

        frmIOW oIOW;
        frmIOWDet m_lIDet = new frmIOWDet();
       // BsfGlobal.VoucherType oVCCType;
       // BsfGlobal.VoucherType oVCompanyType;
        VendorAnalysis.BusinessLayer.QuotationReqestBL oQReqBL;
        VendorAnalysis.BusinessLayer.IOWBL oIowBL;
        VendorAnalysis.BusinessLayer.ComponentBL oCompBL;
        VendorAnalysis.BusinessLayer.RequestEntryBL oReqBL;
        VendorAnalysis.BusinessObjects.RequestReg oRReg;
        List<RequestTrans> oRTransCol;

        #endregion

        #region Constructor

        public frmRequestEntry()
        {
            InitializeComponent();
            
            oQReqBL = new BusinessLayer.QuotationReqestBL();
            oIowBL = new BusinessLayer.IOWBL();
            oCompBL = new BusinessLayer.ComponentBL();
            oReqBL = new RequestEntryBL();
            oVType = new BsfGlobal.VoucherType();
        }
        #endregion

        #region Form Events 
        private void frmRequestEntry_Load(object sender, EventArgs e)
        {
            clsStatics.SetMyGraphics();
            if (m_bViewScreen == true)
            {
                btnOk.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnExit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
            else
            {
                btnOk.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                btnExit.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            
            try
            {
                cboReqType.Enabled = true;
                cboCC.Enabled = true;
                dtpQDate.EditValue = DateTime.Now;
                PopulateCostcentre();
                //CreateColumnsR();
                cboCC.EditValue = 0;
                cboReqType.Text = "None";
               
                ReqScheduleColumns();
                if (RequestId == 0)
                {
                    cboCC.EditValue = RequestEntryBL.GetDefaultCCId();
                }
                if (RequestId > 0){ PopulateEditData(); }
                if (BsfGlobal.g_sUnPermissionMode == "H" || BsfGlobal.g_sUnPermissionMode == "D")
                    CheckPermission();
                if (BsfGlobal.FindPermission("Request-Modify") == false && RequestId != 0)
                {
                    MessageBox.Show("No Rights to Modify the Request", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnOk.Enabled = false;
                    btnCancel.Enabled = false;
                }
                else
                {
                    btnOk.Enabled = true;
                    btnCancel.Enabled = true;
                }

                if (BsfGlobal.FindPermissionVariant("Allow-Request-Qty-Greater-than-Estimate-Qty", ref VariantQty) == false)
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmRequestEntry_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (BsfGlobal.g_bWorkFlow == true && BsfGlobal.g_bWorkFlowDialog == false)
            {
                if (RequestId!=0)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    try
                    {
                        this.Parent.Controls.Owner.Hide();
                    }
                    catch
                    {
                    }
                    ChangeGridValue(RequestId);                                        
                    frmRequestRegister.m_oDW.Show();
                    Cursor.Current = Cursors.Default;
                }
                else
                    this.Parent.Controls.Owner.Hide();
            }
            else
            {
                if (RequestId != 0)
                {
                    CommFun.DW1.Show();
                    CommFun.RP1.Controls.Clear();
                    //RequestListShow();
                }
                CommFun.DW1.Hide();
                CommFun.RP1.Controls.Clear();    
                
            }

        }

        #endregion

        #region Functions

        public void Execute(string argMode)
        {
            m_sMode = argMode;
            this.Show();
        }

        private void ChangeGridValue(int argRegId)
        {
            DataView dv;
            DataTable dt = new DataTable();
            dt = oReqBL.GetChangeValues();

            dv = new DataView(dt);
            dv.RowFilter = "RequestId=" + argRegId;
            dt = dv.ToTable();
            int iRowId = frmRequestRegister.m_oGridView.FocusedRowHandle;
            if (dt.Rows.Count > 0)
            {
                frmRequestRegister.m_oGridView.SetRowCellValue(iRowId, "RequestDate", Convert.ToDateTime(dt.Rows[0]["RequestDate"]));
                frmRequestRegister.m_oGridView.SetRowCellValue(iRowId, "RequestNo", dt.Rows[0]["RequestNo"].ToString());
                frmRequestRegister.m_oGridView.SetRowCellValue(iRowId, "CCReqNo", dt.Rows[0]["CCReqNo"].ToString());
                frmRequestRegister.m_oGridView.SetRowCellValue(iRowId, "RequestType", dt.Rows[0]["RequestType"].ToString());
                frmRequestRegister.m_oGridView.SetRowCellValue(iRowId, "CostCentre", dt.Rows[0]["CostCentre"].ToString());
                frmRequestRegister.m_oGridView.SetRowCellValue(iRowId, "Approve", dt.Rows[0]["Approve"].ToString());
            }
            //frmRequestRegister.m_oGridView.Columns["CostCentreId"].Visible = false;
           // frmRequestRegister.m_oGridView.Columns["RequestId"].Visible = false;
            dt.Dispose();
        }

        private void CheckPermission()
        {
            if (BsfGlobal.g_sUnPermissionMode == "H")
            {
                //if (BsfGlobal.FindPermission("Indent-Create") == false)

            }
            else if (BsfGlobal.g_sUnPermissionMode == "D")
            {
                if (BsfGlobal.FindPermission("Request-Modify") == false)
                    btnOk.Enabled = false;
                btnCancel.Enabled = false;
            }
        }

        private void PopulateCostcentre()
        {
            dtCC = new DataTable();
            DataView dv;
            DataRow dr;
            dtCC = QuotationReqestBL.GetOperationalCC();

            dv = new DataView(dtCC);
            dv.RowFilter = "CostCentreId = 0";
            if (dv.ToTable().Rows.Count > 0) { }
            else
            {
                dr = dtCC.NewRow();
                dr["CostCentreName"] = "None";       
                dr["CostCentreId"] = 0;
                dtCC.Rows.InsertAt(dr, 0);
            }     
            if (dtCC.Rows.Count > 0)
            {
                cboCC.Properties.DataSource = dtCC;
                cboCC.Properties.PopulateColumns();
                cboCC.Properties.DisplayMember = "CostCentreName";
                cboCC.Properties.ValueMember = "CostCentreId";
                cboCC.Properties.Columns["ProjectDB"].Visible = false;
                cboCC.Properties.Columns["CostCentreId"].Visible = false;
                cboCC.Properties.Columns["WBSReqdMMS"].Visible = false;
                cboCC.Properties.ShowFooter = false;
                cboCC.Properties.ShowHeader = false;
            }
            cboCC.EditValue = 0;
        }

       
        private void PoupateGrdDetail()
        {
            RowCreated();

            grdDetail.Rows["EstimateQty"].Properties.Value = 0;
            //grdDetail.Rows["EstimateRate"].Properties.Value = 0;
            //grdDetail.Rows["QuotationRate"].Properties.Value = 0;
            //grdDetail.Rows["ApprovedRate"].Properties.Value = 0;
            grdDetail.Rows["RequestQty"].Properties.Value = 0;

            if (RequestView.RowCount == 0) { return; }

            if (Qtype == "L" || Qtype == "A" || Qtype == "H")
            {

                DataTable dtB = new DataTable();

                dtB = clsStatics.GrdDetailLA(Qtype, m_iResourceId, m_iCCId, 0, 0, m_iRevId, 0, 0);

                if (dtB.Rows.Count > 0)
                {
                    grdDetail.Rows["EstimateQty"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtB.Rows[0]["EstQty"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    //grdDetail.Rows["EstimateRate"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtB.Rows[0]["EstRate"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    //grdDetail.Rows["QuotationRate"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtB.Rows[0]["QRate"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    //grdDetail.Rows["ApprovedRate"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtB.Rows[0]["AppRate"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    grdDetail.Rows["RequestQty"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtB.Rows[0]["WOQty"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    grdDetail.Rows["BalanceQty"].Properties.Value = clsStatics.FormatNum1((Convert.ToDecimal(clsStatics.IsNullCheck(grdDetail.Rows["EstimateQty"].Properties.Value, clsStatics.datatypes.vartypenumeric)) - Convert.ToDecimal(clsStatics.IsNullCheck(grdDetail.Rows["RequestQty"].Properties.Value, clsStatics.datatypes.vartypenumeric))).ToString(), clsStatics.g_iCurrencyDigit);
                }

            }
            //WoType S
            if (Qtype == "S")
            {
                DataTable dtS = new DataTable();
                dtS = clsStatics.GrdDetailS(Qtype, m_iResourceId, m_iCCId, 0, 0, m_iRevId);

                if (dtS.Rows.Count > 0)
                {
                    grdDetail.Rows["EstimateQty"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtS.Rows[0]["EstQty"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    //grdDetail.Rows["EstimateRate"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtS.Rows[0]["EstRate"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    //grdDetail.Rows["QuotationRate"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtS.Rows[0]["QRate"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    //grdDetail.Rows["ApprovedRate"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtS.Rows[0]["AppRate"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    grdDetail.Rows["RequestQty"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtS.Rows[0]["WOQty"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    grdDetail.Rows["BalanceQty"].Properties.Value = clsStatics.FormatNum1((Convert.ToDecimal(clsStatics.IsNullCheck(grdDetail.Rows["EstimateQty"].Properties.Value, clsStatics.datatypes.vartypenumeric)) - Convert.ToDecimal(clsStatics.IsNullCheck(grdDetail.Rows["RequestQty"].Properties.Value, clsStatics.datatypes.vartypenumeric))).ToString(), clsStatics.g_iCurrencyDigit);
                }
            }

            //WoType I
            if (Qtype == "I")
            {
                DataTable dtI = new DataTable();

                dtI = clsStatics.GrdDetailI(Qtype, m_iResourceId, m_iCCId, 0, m_sSplit, Convert.ToInt32(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "AnalysisHeadId").ToString()), 0, m_iRevId);

                if (dtI.Rows.Count > 0)
                {
                    grdDetail.Rows["EstimateQty"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtI.Rows[0]["EstQty"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    //grdDetail.Rows["EstimateRate"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtI.Rows[0]["EstRate"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    //grdDetail.Rows["QuotationRate"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtI.Rows[0]["QRate"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    //grdDetail.Rows["ApprovedRate"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtI.Rows[0]["AppRate"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    grdDetail.Rows["RequestQty"].Properties.Value = clsStatics.FormatNum1(Convert.ToDecimal(clsStatics.IsNullCheck(dtI.Rows[0]["WOQty"], clsStatics.datatypes.vartypenumeric)).ToString(), clsStatics.g_iCurrencyDigit);
                    grdDetail.Rows["BalanceQty"].Properties.Value = clsStatics.FormatNum1((Convert.ToDecimal(clsStatics.IsNullCheck(grdDetail.Rows["EstimateQty"].Properties.Value, clsStatics.datatypes.vartypenumeric)) - Convert.ToDecimal(clsStatics.IsNullCheck(grdDetail.Rows["RequestQty"].Properties.Value, clsStatics.datatypes.vartypenumeric))).ToString(), clsStatics.g_iCurrencyDigit);
                }

            }

        }     

        public void PopulateIOW(string argDBNAMe, string argIow)
        {
            try
            {
                dtIOWPop = new DataTable();
                if (string.IsNullOrEmpty(argIow))
                {
                    return;
                }
                //dtIOWPop = oIowBL.PopulateIOW(argDBNAMe, argIow);
                if (dtIOWPop.Rows.Count > 0)
                {
                    DataRow dr;
                    for (int k = 0; k < dtIOWPop.Rows.Count; k++)
                    {
                        dr = dtRes.NewRow();

                        dr["ID"] = dtIOWPop.Rows[k]["Project_IOW_ID"].ToString();
                        dr["Code"] = dtIOWPop.Rows[k]["Serial_No"].ToString();
                        dr["Description"] = dtIOWPop.Rows[k]["Specification"].ToString();
                        dr["Unit"] = dtIOWPop.Rows[k]["Unit_Name"].ToString();
                        dr["UnitId"] = dtIOWPop.Rows[k]["Unit_Id"].ToString();
                        dr["Quantity"] = 0;

                        dtRes.Rows.Add(dr);
                    }
                }


            }
            catch (Exception Except)
            {
                MessageBox.Show("Error: " + Except.Message);
            }
        }

        public void PopulateComp(string argComps)
        {
            dtRpop = new DataTable();
            if (string.IsNullOrEmpty(argComps))
            {
                return;
            }
            dtRpop = oCompBL.PopulateComponent(argComps);
            if (dtRpop.Rows.Count > 0)
            {
                DataRow row;
                for (int j = 0; j < dtRpop.Rows.Count; j++)
                {
                    row = dtRes.NewRow();
                    row["RowId"] = (dtRes.Rows.Count + 1);
                    row["ID"] = dtRpop.Rows[j]["Resource_Id"].ToString();
                    row["Code"] = dtRpop.Rows[j]["Resource_Code"].ToString();
                    row["Description"] = dtRpop.Rows[j]["Resource_Name"].ToString();
                    row["Unit"] = dtRpop.Rows[j]["Unit_Name"].ToString();
                    row["UnitId"] = dtRpop.Rows[j]["Unit_Id"].ToString();
                    row["Quantity"] = 0;

                    dtRes.Rows.Add(row);
                }
            }
        }

        private void CreateColumnsR()
        {
            dtRes = new DataTable();
            if (dtRes.Columns.Count == 0)
            {
                dtRes.Columns.Add("RowId", typeof(int));
                dtRes.Columns.Add("ID", typeof(Int32));
                dtRes.Columns.Add("IOWID", typeof(Int32));
                dtRes.Columns.Add("Code", typeof(string));
                dtRes.Columns.Add("Description", typeof(string));
                dtRes.Columns.Add("Unit", typeof(string));
                dtRes.Columns.Add("UnitId", typeof(int));
                dtRes.Columns.Add("Quantity", typeof(decimal));
                dtRes.Columns.Add("Req Date", typeof(DateTime));
                dtRes.Columns.Add("Remarks", typeof(string));
                dtRes.Columns.Add("HiddenQty", typeof(decimal));
                dtRes.Columns.Add("AnalysisHeadId", typeof(decimal));
            }           

            grdReqEntry.DataSource = dtRes;

            RequestView.Columns["HiddenQty"].Visible = false;
            RequestView.Columns["AnalysisHeadId"].Visible = false;            
            RequestView.Columns["RowId"].Visible = false;
            RequestView.Columns["UnitId"].Visible = false;
            RequestView.Columns["IOWID"].Visible = false;
            RequestView.Columns["ID"].Visible = false;
            RequestView.Columns["Code"].Width = 80;
            RequestView.Columns["Description"].Width = 270;
            RequestView.Columns["Unit"].Width = 50;
            RequestView.Columns["Quantity"].Width = 80;
            RequestView.Columns["Req Date"].Width = 80;
            RequestView.Columns["Remarks"].Width = 150;
            RequestView.Columns["Req Date"].ColumnEdit=deReqDate;

            RequestView.Columns["ID"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            RequestView.Columns["Code"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            RequestView.Columns["Description"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            RequestView.Columns["Unit"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            RequestView.Columns["Quantity"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            RequestView.Columns["Req Date"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            RequestView.Columns["Remarks"].SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;

            RequestView.Columns["ID"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            RequestView.Columns["Code"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            RequestView.Columns["Description"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            RequestView.Columns["Unit"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            RequestView.Columns["Quantity"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            RequestView.Columns["Quantity"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            RequestView.Columns["Req Date"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            RequestView.Columns["Remarks"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            RequestView.Columns["ID"].AppearanceHeader.Font = new Font(RequestView.Columns[0].AppearanceHeader.Font, FontStyle.Bold);
            RequestView.Columns["Code"].AppearanceHeader.Font = new Font(RequestView.Columns[1].AppearanceHeader.Font, FontStyle.Bold);
            RequestView.Columns["Description"].AppearanceHeader.Font = new Font(RequestView.Columns[2].AppearanceHeader.Font, FontStyle.Bold);
            RequestView.Columns["Unit"].AppearanceHeader.Font = new Font(RequestView.Columns[3].AppearanceHeader.Font, FontStyle.Bold);
            RequestView.Columns["Quantity"].AppearanceHeader.Font = new Font(RequestView.Columns[4].AppearanceHeader.Font, FontStyle.Bold);
            RequestView.Columns["Req Date"].AppearanceHeader.Font = new Font(RequestView.Columns[4].AppearanceHeader.Font, FontStyle.Bold);
            RequestView.Columns["Remarks"].AppearanceHeader.Font = new Font(RequestView.Columns[4].AppearanceHeader.Font, FontStyle.Bold);

            RequestView.Columns["Quantity"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            RequestView.Columns["Quantity"].DisplayFormat.FormatString = "##0.000";

            if (Qtype != "M" && Qtype != "H")
            {
                DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit txtQty = new RepositoryItemButtonEdit();
                txtQty.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                txtQty.Mask.EditMask = "N3";
                RequestView.Columns["Quantity"].ColumnEdit = txtQty;
                txtQty.EditValueChanged += new EventHandler(txtQty_EditValueChanged);
                txtQty.KeyDown += new KeyEventHandler(txtQty_KeyDown);
                txtQty.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(txtQty_Spin);
                txtQty.DoubleClick += new EventHandler(txtQty_DoubleClick);
                txtQty.KeyPress += new KeyPressEventHandler(txtQty_KeyPress);

                txtQty.Validating += new CancelEventHandler(txtQty_Validating);
            }
            else
            {
                DevExpress.XtraEditors.Repository.RepositoryItemTextEdit txtQty = new RepositoryItemTextEdit();
                txtQty.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                txtQty.Mask.EditMask = "N3";
                RequestView.Columns["Quantity"].ColumnEdit = txtQty;
                txtQty.EditValueChanged += new EventHandler(txtQty_EditValueChanged);
                txtQty.KeyDown += new KeyEventHandler(txtQty_KeyDown);
                txtQty.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(txtQty_Spin);
                //txtQty.DoubleClick += new EventHandler(txtQty_DoubleClick);
                //txtQty.KeyPress += new KeyPressEventHandler(txtQty_KeyPress);
                //txtQty.Validating += new CancelEventHandler(txtQty_Validating);
            }


            RequestView.Columns["Quantity"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            RequestView.Columns["Quantity"].SummaryItem.DisplayFormat = "{0:N3}";
        }

        void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            ButtonEdit editor = (ButtonEdit)sender;

            if (Qtype != "M")
            {

                //DataTable m_dtargPass = new DataTable();
                if (RequestView.FocusedColumn.FieldName == "Quantity")
                {

                    if (Qtype == "L" || Qtype == "A" || Qtype == "H" || Qtype == "R")
                    {
                        m_iResourceId = Convert.ToInt32(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "ID").ToString());
                    }
                    else
                    {
                        m_iResourceId = Convert.ToInt32(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "IOWID").ToString());
                    }

                    int iRowId = Convert.ToInt32(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "RowId").ToString());
                    m_sDescription = clsStatics.IsNullCheck(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "Description"), clsStatics.datatypes.vartypestring).ToString();

                    string w_sUnitName = RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "Unit").ToString();

                    if (Qtype == "L" || Qtype == "A" || Qtype == "H")
                    {
                        editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                        editor.Properties.ReadOnly = true;
                        ReqShowIOW(iRowId);
                    }

                    if (Qtype == "R")
                    {
                        editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                        return;
                    }
                    if ((Qtype == "I") && (m_sSplit == "Y"))
                    {
                        if (clsStatics.getWBSCheck(m_iResourceId, m_iRevId) == true)
                        {
                            editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                            editor.Properties.ReadOnly = true;
                            ReqShowWOWBSTrans(iRowId);
                        }
                        else
                        {
                            editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                            editor.Properties.ReadOnly = false;
                        }
                    }
                }
            }
            else
            {
                if (RequestView.FocusedColumn.FieldName == "Quantity")
                {
                    int CCId = Convert.ToInt32(cboCC.EditValue);
                    ResId = Convert.ToInt32(clsStatics.IsNullCheck(RequestView.GetFocusedRowCellValue("ID").ToString(), clsStatics.datatypes.vartypenumeric));
                    if (Qtype != "")
                    {
                        if (CCId > 0 && ResId > 0)
                        {
                            GetStockDetails(CCId, ResId, Qtype);

                        }
                        if (dsAnalysisHead.Tables["Analysis"].Rows.Count > 0 && Convert.ToBoolean(((DataRowView)(cboCC.GetSelectedDataRow())).Row.ItemArray[3].ToString()) == true)
                        {
                            editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                            editor.Properties.ReadOnly = true;

                            DataView dv = new DataView(dtAnal);
                            dv.RowFilter = String.Format("REsource_ID={0} ", RequestView.GetFocusedRowCellValue("ID"));
                            if (dv.ToTable().Rows.Count == 0)
                            {
                                DataRow dr1;
                                foreach (DataRow dr in dsAnalysisHead.Tables["Analysis"].Rows)
                                {
                                    dr1 = dtAnal.NewRow();
                                    dr1["WBS"] = dr["AnalysisHeadName"];
                                    dr1["Qty"] = "0.00000";
                                    dr1["REsource_ID"] = RequestView.GetFocusedRowCellValue("ID").ToString();
                                    dr1["BrandID"] = 0;
                                    dr1["Analysis_ID"] = dr["AnalysisID"];
                                    dr1["CCID"] = dr["CCID"];
                                    dr1["HiddenQty"] = "0.00000";
                                    dtAnal.Rows.Add(dr1);
                                }
                                if (RequestId > 0)
                                {
                                    if (dtAnalUpdate != null)
                                        if (dtAnalUpdate.Rows.Count > 0)
                                        {
                                            DataRow[] SelectU = null; ;

                                            SelectU = dtAnal.Select("REsource_ID =' " + RequestView.GetFocusedRowCellValue("ID").ToString() + "' ");
                                            foreach (DataRow r in SelectU)
                                            {
                                                foreach (DataRow drow in dtAnalUpdate.Rows)
                                                {
                                                    if (Convert.ToInt32(r["Analysis_ID"]) == Convert.ToInt32(drow["AnalysisId"]) && Convert.ToInt32(r["REsource_ID"]) == Convert.ToInt32(drow["ResourceId"]))
                                                    {

                                                        r["Qty"] = drow["ReqQty"];
                                                        r["HiddenQty"] = drow["ReqQty"];
                                                    }
                                                }
                                            }
                                        }
                                }

                                dv.RowFilter = "REsource_ID=" + RequestView.GetFocusedRowCellValue("ID").ToString() + " ";
                                grdAnal = dv.ToTable();
                                grdAnalysis.DataSource = grdAnal;
                            }
                            else
                            {
                                DataView dvA = new DataView(dtAnal);
                                dvA.RowFilter = "REsource_ID=" + RequestView.GetFocusedRowCellValue("ID").ToString() + " ";
                                grdAnal = dvA.ToTable();
                                grdAnalysis.DataSource = grdAnal;
                            }
                            HideAnalColumns();
                            AnalView.Columns["Qty"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                            AnalView.Columns["Qty"].SummaryItem.DisplayFormat = "{0:N5}";
                            DevExpress.XtraEditors.Repository.RepositoryItemTextEdit txtAnalQty = new RepositoryItemTextEdit();
                            txtAnalQty.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                            txtAnalQty.KeyDown += new KeyEventHandler(txtAnalQty_KeyDown);
                            txtAnalQty.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(txtAnalQty_Spin);
                            txtAnalQty.Mask.EditMask = "N5";
                            AnalView.Columns["Qty"].ColumnEdit = txtAnalQty;
                            AnalView.Columns["WBS"].Width = 77;
                            AnalView.Columns["Qty"].Width = 23;
                            AnalView.Columns["Qty"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                        }
                        else
                        {
                            editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                            editor.Properties.ReadOnly = false;

                        }
                    }
                }
            }
        }

        void txtQty_Validating(object sender, CancelEventArgs e)
        {
            TextEdit editor = (TextEdit)sender;
            if (Qtype != "M" && Qtype != "H")
            {
                if (RequestView.FocusedRowHandle >= 0)
                {
                    decimal preQty = Convert.ToDecimal(Convert.ToDecimal(clsStatics.IsNullCheck(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "Quantity"), clsStatics.datatypes.vartypenumeric)));

                    decimal dQty = Convert.ToDecimal(clsStatics.IsNullCheck(grdDetail.Rows["EstimateQty"].Properties.Value, clsStatics.datatypes.vartypenumeric)) * (1 + m_VariantQty / 100);
                    dQty = dQty - Convert.ToDecimal(clsStatics.IsNullCheck(grdDetail.Rows["RequestQty"].Properties.Value, clsStatics.datatypes.vartypenumeric));

                    if (dQty < Convert.ToDecimal(editor.EditValue))
                    {
                        if (BsfGlobal.g_bPowerUser == false)
                        {
                            e.Cancel = true;
                            RequestView.SetRowCellValue(RequestView.FocusedRowHandle, "Quantity", 0);
                        }
                        else
                        {
                            RequestView.SetRowCellValue(RequestView.FocusedRowHandle, "Quantity", preQty);
                        }
                        MessageBox.Show("Request Qty Greater than Estimate Qty");
                        editor.Focus();
                        return;
                    }

                    if ((Qtype != "") && (Qtype == "I"))
                    {
                        RequestView.SetRowCellValue(RequestView.FocusedRowHandle, "Quantity", editor.EditValue);
                    }
                }
            }
        }

        private void ReqShowIOW(int argRowId)
        {
          
            if (RequestView.FocusedColumn.FieldName == "Quantity")
            {
                oIOW = new frmIOW();               

                oIOW.m_iIOWRowId = RequestView.FocusedRowHandle;
                oIOW.m_iBRowId = RequestView.FocusedRowHandle;
                oIOW.m_iWORegId = RequestId;


                if (Convert.ToDouble(clsStatics.IsNullCheck(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "Quantity"), clsStatics.datatypes.vartypenumeric)) == 0)
                {
                    List<WOIOWTransUpdate> ocheckUpdate = oWOIOWTrans.FindAll(
                   delegate(WOIOWTransUpdate sel)
                   {
                       return (sel.IOW_Trans_ID == m_iResourceId && sel.WOTrnsRowId == argRowId);

                   });

                    if (ocheckUpdate.Count > 0)
                    {
                        oIOW.Execute2(ocheckUpdate, "W", m_iRevId, m_sMode, m_iResourceId, m_iCCId, 0, null, 0, RequestView.FocusedColumn.FieldName, true, null);
                    }
                    else
                    {
                        oIOW.Execute2(oWOIOWTrans, "W", m_iRevId, m_sMode, m_iResourceId, m_iCCId, 0, null, 0, RequestView.FocusedColumn.FieldName, true, null);
                    }
                }                
                else
                {
                    List<WOIOWTransUpdate> ocheckUpdate = oWOIOWTrans.FindAll(
                     delegate(WOIOWTransUpdate sel)
                     {
                         return (sel.IOW_Trans_ID == m_iResourceId && sel.WOTrnsRowId == argRowId);

                     });
                    if (ocheckUpdate.Count > 0)
                    {
                        oIOW.Execute2(ocheckUpdate, "W", m_iRevId, m_sMode, m_iResourceId, m_iCCId, 0, null, 0, RequestView.FocusedColumn.FieldName, true, null);
                    }
                    else
                    {
                        oIOW.Execute2(oWOIOWTrans, "W", m_iRevId, m_sMode, m_iResourceId, m_iCCId, 0, null, 0, RequestView.FocusedColumn.FieldName, true, null);
                    }

                }
                if (oIOW.m_dRetruntQty != 0)
                {
                    RequestView.SetRowCellValue(RequestView.FocusedRowHandle, "Quantity", oIOW.m_dRetruntQty);
                    RequestView.SetRowCellValue(RequestView.FocusedRowHandle, "AnalysisHeadId", oIOW.m_iAnalysisId);

                }
                else
                {
                    RequestView.SetRowCellValue(RequestView.FocusedRowHandle, "Quantity", oIOW.m_dRetruntQty);
                    RequestView.SetRowCellValue(RequestView.FocusedRowHandle, "AnalysisHeadId", oIOW.m_iAnalysisId);
                }

                if (oIOW.m_sClkOption == "OK")
                    ReqWOIOWUpdateNew(m_iResourceId, argRowId);    
                
            }

        }

        private void ReqWOIOWUpdateNew(int ResourceId, int argWOTransRowId)
        {
            if (oIOW.grdviewIOWTrans.RowCount > 0)
            {
                List<WOIOWTransUpdate> ocheckUpdate = oWOIOWTrans.FindAll(
                    delegate(WOIOWTransUpdate sel)
                    {
                        return (sel.IOW_Trans_ID == ResourceId && sel.WOTrnsRowId == argWOTransRowId);

                    });
                if (ocheckUpdate.Count > 0)
                {
                    oWOIOWTrans.RemoveAll(delegate(WOIOWTransUpdate del)
                    {
                        return (del.IOW_Trans_ID == ResourceId && del.WOTrnsRowId == argWOTransRowId);
                    });

                    for (int et = 0; et < oIOW.grdviewIOWTrans.RowCount; et++)
                    {
                        oWOIOWTrans.Add(new WOIOWTransUpdate()
                        {
                            BillType = Qtype,
                            WOTrnsRowId = argWOTransRowId,
                            IOWRowId = Convert.ToInt32(clsStatics.IsNullCheck(oIOW.grdviewIOWTrans.GetRowCellValue(et, "RowId"), clsStatics.datatypes.vartypenumeric)),
                            IOW_Trans_ID = ResourceId,
                            WOTransId = 0,
                            Specification = oIOW.grdviewIOWTrans.GetRowCellValue(et, "Specification").ToString(),
                            IOW_ID = Convert.ToInt32(clsStatics.IsNullCheck(oIOW.grdviewIOWTrans.GetRowCellValue(et, "IOW_ID"), clsStatics.datatypes.vartypenumeric)),
                            Qty = Convert.ToDouble(clsStatics.IsNullCheck(oIOW.grdviewIOWTrans.GetRowCellValue(et, "Qty"), clsStatics.datatypes.vartypenumeric)),
                            AnalysisHeadID = Convert.ToInt32(clsStatics.IsNullCheck(oIOW.grdviewIOWTrans.GetRowCellValue(et, "AnalysisHeadID"), clsStatics.datatypes.vartypenumeric)),
                            CNType = oIOW.grdviewIOWTrans.GetRowCellValue(et, "CNType").ToString(),
                            UOM_ID = oIOW.grdviewIOWTrans.GetRowCellValue(et, "Unit").ToString(),
                            Serial_No = oIOW.grdviewIOWTrans.GetRowCellValue(et, "Serial_No").ToString()

                        });
                    }
                }

            }
        }

        private void ReqWOWBSUpdate(int argItemTransId, int argRowId)
        {
            try
            {
                if (m_lIDet.grdViewIDet.RowCount > 0)
                {
                    //   List<WOWBSTrans> m_lWOWBS;
                    if (m_lWOWBS.Count > 0)
                    {
                        m_lWOWBS.RemoveAll(
                            delegate(WOWBSTrans del)
                            {
                                return ((del.ItemTransId == argItemTransId) && (del.TransRowId == argRowId));

                            });
                        for (int q = 0; q < m_lIDet.grdViewIDet.RowCount; q++)
                        {
                            m_lWOWBS.Add(new WOWBSTrans()
                            {
                                WBSRowId = Convert.ToInt32(clsStatics.IsNullCheck(m_lIDet.grdViewIDet.GetRowCellValue(q, "RowId"), clsStatics.datatypes.vartypenumeric)),
                                ItemTransId = argItemTransId,
                                WOTransId = 0,
                                AnalysisHeadId = Convert.ToInt32(clsStatics.IsNullCheck(m_lIDet.grdViewIDet.GetRowCellValue(q, "AnalysisHeadId"), clsStatics.datatypes.vartypenumeric)),
                                TransRowId = argRowId,
                                ResourceCode = clsStatics.IsNullCheck(m_lIDet.grdViewIDet.GetRowCellValue(q, "ResourceCode"), clsStatics.datatypes.vartypestring).ToString(),
                                Description = clsStatics.IsNullCheck(m_lIDet.grdViewIDet.GetRowCellValue(q, "AnalysisHeadName"), clsStatics.datatypes.vartypestring).ToString(),
                                Qty = Convert.ToDecimal(clsStatics.IsNullCheck(m_lIDet.grdViewIDet.GetRowCellValue(q, "Qty"), clsStatics.datatypes.vartypenumeric))

                            });
                        }
                    }
                    else
                    {
                        for (int q = 0; q < m_lIDet.grdViewIDet.RowCount; q++)
                        {
                            m_lWOWBS.Add(new WOWBSTrans()
                            {
                                WBSRowId = Convert.ToInt32(clsStatics.IsNullCheck(m_lIDet.grdViewIDet.GetRowCellValue(q, "RowId"), clsStatics.datatypes.vartypenumeric)),
                                ItemTransId = argItemTransId,
                                WOTransId = 0,
                                AnalysisHeadId = Convert.ToInt32(clsStatics.IsNullCheck(m_lIDet.grdViewIDet.GetRowCellValue(q, "AnalysisHeadId"), clsStatics.datatypes.vartypenumeric)),
                                TransRowId = argRowId,
                                ResourceCode = clsStatics.IsNullCheck(m_lIDet.grdViewIDet.GetRowCellValue(q, "ResourceCode"), clsStatics.datatypes.vartypestring).ToString(),
                                Description = clsStatics.IsNullCheck(m_lIDet.grdViewIDet.GetRowCellValue(q, "AnalysisHeadName"), clsStatics.datatypes.vartypestring).ToString(),
                                Qty = Convert.ToDecimal(clsStatics.IsNullCheck(m_lIDet.grdViewIDet.GetRowCellValue(q, "Qty"), clsStatics.datatypes.vartypenumeric))


                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ReqShowWOWBSTrans(int argRowId)
        {

            if (RequestView.FocusedColumn.FieldName == "Quantity")
            {
                m_lIDet = new frmIOWDet();

                List<WOWBSTrans> ocheckUpdate = m_lWOWBS.FindAll(
                     delegate(WOWBSTrans sel)
                     {
                         return (sel.ItemTransId == m_iResourceId && sel.TransRowId == argRowId);

                     });
                if (ocheckUpdate.Count > 0)
                {
                    m_lIDet.Execute("W", m_iCCId, 0, m_iRevId, m_iResourceId, clsStatics.GenericListToDataTable(ocheckUpdate), null, m_sDescription, m_sSplit);
                }
                else
                {
                    m_lIDet.Execute("W", m_iCCId, 0, m_iRevId, m_iResourceId, clsStatics.GenericListToDataTable(ocheckUpdate), null, m_sDescription, m_sSplit);
                }

                if (m_lIDet.m_dRetruntQty != 0)
                {
                    RequestView.SetRowCellValue(RequestView.FocusedRowHandle, "Quantity", m_lIDet.m_dRetruntQty);
                }
                else
                {
                    RequestView.SetRowCellValue(RequestView.FocusedRowHandle, "Quantity", m_lIDet.m_dRetruntQty);
                }

                //WOWBS Update
                if (m_lIDet.m_sClkOption == "OK")
                    ReqWOWBSUpdate(m_iResourceId, argRowId);              
            }
        }

        private void ReqInsertIOW(string argWOType, int argRowId, int argCompId, string split, int argWoTrnsId)
        {
            DataTable dtIOWWo = new DataTable();
            double dtIOWModQty = 0;
            dtIOWWo = clsStatics.InsertIOW(argCompId, split, m_iRevId);

            string serialNo = "";
            if (dtIOWWo != null)
            {
                for (int m = 0; m < dtIOWWo.Rows.Count; m++)
                {
                    if (dtIOWWo.Rows[m]["RefSerialNo"].ToString() != "")
                        serialNo = dtIOWWo.Rows[m]["RefSerialNo"].ToString();
                    else
                        serialNo = dtIOWWo.Rows[m]["Serial_No"].ToString();

                    dtIOWModQty = clsStatics.getWOIOWTransQtyE(Convert.ToInt32(dtIOWWo.Rows[m]["IOW_ID"].ToString()), argWoTrnsId, Convert.ToInt32(dtIOWWo.Rows[m]["Analysis_Head_ID"].ToString()));
                    if (dtIOWModQty != 0)
                    {
                        DataRow[] customerRow = dtIOWWo.Select("IOW_ID=" + Convert.ToInt32(dtIOWWo.Rows[m]["IOW_ID"].ToString()) + " and Analysis_Head_ID=" + Convert.ToInt32(dtIOWWo.Rows[m]["Analysis_Head_ID"].ToString()) + "");
                        customerRow[0]["Qty"] = dtIOWModQty;
                    }
                }
            }
            for (int m = 0; m < dtIOWWo.Rows.Count; m++)
            {
                if (dtIOWWo.Rows[m]["RefSerialNo"].ToString() != "")
                    serialNo = dtIOWWo.Rows[m]["RefSerialNo"].ToString();
                else
                    serialNo = dtIOWWo.Rows[m]["Serial_No"].ToString();

                oWOIOWTrans.Add(new WOIOWTransUpdate()
                {
                    WOTrnsRowId = argRowId,
                    IOWRowId = Convert.ToInt32(dtIOWWo.Rows[m]["RowId"].ToString()),
                    WOTransId = argWoTrnsId,
                    Serial_No = serialNo,
                    BillType = argWOType,
                    IOW_Trans_ID = argCompId,
                    UOM_ID = dtIOWWo.Rows[m]["UOM_ID"].ToString(),
                    IOW_ID = Convert.ToInt32(dtIOWWo.Rows[m]["IOW_ID"].ToString()),
                    Specification = dtIOWWo.Rows[m]["Specification"].ToString(),
                    AnalysisHeadID = Convert.ToInt32(dtIOWWo.Rows[m]["Analysis_Head_Id"].ToString()),
                    Qty = Convert.ToDouble(dtIOWWo.Rows[m]["Qty"].ToString())

                });
            }

        }

        private void ReqInsertSubIOW(string argWOType, int argRowId, int argCompId, string argMode, int argWoTrnsId, string argSplit)
        {
            double dtIOWModQty = 0;
            DataTable dtRetIOW = new DataTable();

            dtRetIOW = clsStatics.InsertSubIOW(m_iRevId, argCompId, argSplit);

            List<DataRow> drlist = new List<DataRow>();


            string serialNo = "";

            if (dtRetIOW != null)
            {
                for (int m = 0; m < dtRetIOW.Rows.Count; m++)
                {
                    if (dtRetIOW.Rows[m]["New_Serial_No"].ToString() != "")
                        serialNo = dtRetIOW.Rows[m]["New_Serial_No"].ToString();
                    else
                        serialNo = dtRetIOW.Rows[m]["Serial_No"].ToString();


                    dtIOWModQty = clsStatics.getWOIOWTransQtyE(Convert.ToInt32(dtRetIOW.Rows[m]["IOW_ID"].ToString()), argWoTrnsId, Convert.ToInt32(dtRetIOW.Rows[m]["Analysis_Head_ID"].ToString()));
                    if (dtIOWModQty != 0)
                    {
                        DataRow[] customerRow = dtRetIOW.Select("IOW_ID=" + Convert.ToInt32(dtRetIOW.Rows[m]["IOW_ID"].ToString()) + " and Analysis_Head_ID=" + Convert.ToInt32(dtRetIOW.Rows[m]["Analysis_Head_ID"].ToString()) + "");
                        customerRow[0]["Qty"] = dtIOWModQty;
                    }
                }

                for (int m = 0; m < dtRetIOW.Rows.Count; m++)
                {
                    if (dtRetIOW.Rows[m]["New_Serial_No"].ToString() != "")
                        serialNo = dtRetIOW.Rows[m]["New_Serial_No"].ToString();
                    else
                        serialNo = dtRetIOW.Rows[m]["Serial_No"].ToString();

                    oWOIOWTrans.Add(new WOIOWTransUpdate()
                    {
                        WOTrnsRowId = argRowId,
                        IOWRowId = Convert.ToInt32(dtRetIOW.Rows[m]["RowId"].ToString()),
                        WOTransId = argWoTrnsId,
                        Serial_No = serialNo,
                        BillType = argWOType,
                        IOW_Trans_ID = argCompId,
                        UOM_ID = dtRetIOW.Rows[m]["UOM_ID"].ToString(),
                        IOW_ID = Convert.ToInt32(dtRetIOW.Rows[m]["IOW_ID"].ToString()),
                        Specification = dtRetIOW.Rows[m]["Specification"].ToString(),
                        AnalysisHeadID = Convert.ToInt32(dtRetIOW.Rows[m]["Analysis_Head_Id"].ToString()),
                        Qty = Convert.ToDouble(dtRetIOW.Rows[m]["Qty"].ToString())

                    });
                }
            }
        }

        private void ReqWOWBSE(string argWOType, int argRowId, int argCompId, string split, int argWoTrnsId)
        {
            DataTable dtWOWBS = new DataTable();

            dtWOWBS = clsStatics.WOWBSE(argWoTrnsId, split, m_iRevId, argCompId);


            for (int m = 0; m < dtWOWBS.Rows.Count; m++)
            {
                m_lWOWBS.Add(new WOWBSTrans()
                {
                    WBSRowId = Convert.ToInt32(dtWOWBS.Rows[m]["RowId"].ToString()),
                    ItemTransId = argCompId,
                    WOTransId = argWoTrnsId,
                    AnalysisHeadId = Convert.ToInt32(dtWOWBS.Rows[m]["AnalysisId"].ToString()),
                    TransRowId = argRowId,
                    ResourceCode = dtWOWBS.Rows[m]["New_Serial_No"].ToString(),
                    Description = dtWOWBS.Rows[m]["AnalysisHeadName"].ToString(),
                    Qty = Convert.ToDecimal(dtWOWBS.Rows[m]["Qty"].ToString())


                });
            }

        }

        void txtQty_DoubleClick(object sender, EventArgs e)
        {
            ButtonEdit editor = (ButtonEdit)sender;

            if (Qtype != "")
            {                            
                //DataTable m_dtargPass = new DataTable();
                if (Qtype != "M")
                {
                    if (RequestView.FocusedColumn.FieldName == "Quantity")
                    {

                        if (Qtype == "L" || Qtype == "A" || Qtype == "H" || Qtype == "R")
                        {
                            m_iResourceId = Convert.ToInt32(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "ID").ToString());
                        }
                        else
                        {
                            m_iResourceId = Convert.ToInt32(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "IOWID").ToString());
                        }

                        int iRowId = Convert.ToInt32(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "RowId").ToString());
                        m_sDescription = clsStatics.IsNullCheck(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "Description"), clsStatics.datatypes.vartypestring).ToString();

                        string w_sUnitName = RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "Unit").ToString();

                        if (Qtype == "L" || Qtype == "A" || Qtype == "H")
                        {
                            editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                            editor.Properties.ReadOnly = true;
                            ReqShowIOW(iRowId);
                        }

                        if (Qtype == "R")
                        {
                            editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                            return;
                        }
                        if ((Qtype == "I") && (m_sSplit == "Y"))
                        {
                            if (clsStatics.getWBSCheck(m_iResourceId, m_iRevId) == true)
                            {
                                editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                                editor.Properties.ReadOnly = true;
                                ReqShowWOWBSTrans(iRowId);
                            }
                            else
                            {
                                editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                                editor.Properties.ReadOnly = false;
                            }
                        }
                    }
                }
                else if (Qtype == "M" || Qtype == "H")
                {
                    if (RequestView.FocusedColumn.FieldName == "Quantity")
                    {
                        int CCId = Convert.ToInt32(cboCC.EditValue);
                        ResId = Convert.ToInt32(clsStatics.IsNullCheck(RequestView.GetFocusedRowCellValue("ID").ToString(), clsStatics.datatypes.vartypenumeric));
                        if (Qtype != "")
                        {
                            if (CCId > 0 && ResId > 0)
                            {
                                GetStockDetails(CCId, ResId, Qtype);

                            }
                            if (dsAnalysisHead.Tables["Analysis"].Rows.Count > 0 && Convert.ToBoolean(((DataRowView)(cboCC.GetSelectedDataRow())).Row.ItemArray[3].ToString()) == true)
                            {
                                editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                                editor.Properties.ReadOnly = true;

                                DataView dv = new DataView(dtAnal);
                                dv.RowFilter = String.Format("REsource_ID={0} ", RequestView.GetFocusedRowCellValue("ID"));
                                if (dv.ToTable().Rows.Count == 0)
                                {
                                    DataRow dr1;
                                    foreach (DataRow dr in dsAnalysisHead.Tables["Analysis"].Rows)
                                    {
                                        dr1 = dtAnal.NewRow();
                                        dr1["WBS"] = dr["AnalysisHeadName"];
                                        dr1["Qty"] = "0.00000";
                                        dr1["REsource_ID"] = RequestView.GetFocusedRowCellValue("ID").ToString();
                                        dr1["BrandID"] = 0;
                                        dr1["Analysis_ID"] = dr["AnalysisID"];
                                        dr1["CCID"] = dr["CCID"];
                                        dr1["HiddenQty"] = "0.00000";
                                        dtAnal.Rows.Add(dr1);
                                    }
                                    if (RequestId > 0)
                                    {
                                        if (dtAnalUpdate != null)
                                            if (dtAnalUpdate.Rows.Count > 0)
                                            {
                                                DataRow[] SelectU = null; ;

                                                SelectU = dtAnal.Select("REsource_ID =' " + RequestView.GetFocusedRowCellValue("ID").ToString() + "' ");
                                                foreach (DataRow r in SelectU)
                                                {
                                                    foreach (DataRow drow in dtAnalUpdate.Rows)
                                                    {
                                                        if (Convert.ToInt32(r["Analysis_ID"]) == Convert.ToInt32(drow["AnalysisId"]) && Convert.ToInt32(r["REsource_ID"]) == Convert.ToInt32(drow["ResourceId"]))
                                                        {

                                                            r["Qty"] = drow["ReqQty"];
                                                            r["HiddenQty"] = drow["ReqQty"];
                                                        }
                                                    }
                                                }
                                            }
                                    }

                                    dv.RowFilter = "REsource_ID=" + RequestView.GetFocusedRowCellValue("ID").ToString() + " ";
                                    grdAnal = dv.ToTable();
                                    grdAnalysis.DataSource = grdAnal;
                                }
                                else
                                {
                                    DataView dvA = new DataView(dtAnal);
                                    dvA.RowFilter = "REsource_ID=" + RequestView.GetFocusedRowCellValue("ID").ToString() + " ";
                                    grdAnal = dvA.ToTable();
                                    grdAnalysis.DataSource = grdAnal;
                                }
                                HideAnalColumns();
                                AnalView.Columns["Qty"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                                AnalView.Columns["Qty"].SummaryItem.DisplayFormat = "{0:N5}";
                                DevExpress.XtraEditors.Repository.RepositoryItemTextEdit txtAnalQty = new RepositoryItemTextEdit();
                                txtAnalQty.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                                txtAnalQty.KeyDown += new KeyEventHandler(txtAnalQty_KeyDown);
                                txtAnalQty.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(txtAnalQty_Spin);
                                txtAnalQty.Mask.EditMask = "N5";
                                AnalView.Columns["Qty"].ColumnEdit = txtAnalQty;
                                AnalView.Columns["WBS"].Width = 77;
                                AnalView.Columns["Qty"].Width = 23;
                                AnalView.Columns["Qty"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                            }
                            else
                            {
                                editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                                editor.Properties.ReadOnly = false;

                            }
                        }
                    }
                }
            }
            
        }

        void txtQty_Spin(object sender, DevExpress.XtraEditors.Controls.SpinEventArgs e)
        {
            e.Handled = true;
        }

        void txtQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up | e.KeyCode == Keys.Down)
            {
                e.Handled = true;
            }
        }

        public void UpdateData()
        {
            try
            {
                //string sType = "";
                int iOwId = 0;
                int resrceId = 0;
                string RDate = string.Format("{0:dd/MMM/yyyy}", Convert.ToDateTime(dtpQDate.EditValue));

                oRReg = new BusinessObjects.RequestReg();
                oRReg.RequestDate = RDate;
                oRReg.RequestID = RequestId;
                oRReg.RequestType = cboReqType.EditValue.ToString();
                oRReg.RequestNo = txtReqNo.Text;
                oRReg.CCId = Convert.ToInt32(cboCC.EditValue);
                oRReg.CCReqNo = txtCCReqNo.Text;
                oRReg.RefNo = txtRefNo.Text;
                oRReg.Narration = rTxtNarration.Text;
                oRTransCol = new List<RequestTrans>();


                if (RequestView.RowCount > 0)
                {
                    oRTransCol.Clear();
                    for (int m = 0; m < RequestView.RowCount; m++)
                    {
                        if (cboReqType.EditValue.ToString().Trim() == "IOW")
                        {
                            iOwId = Convert.ToInt32(RequestView.GetRowCellValue(m, "ID").ToString());
                        }
                        else
                        {
                            resrceId = Convert.ToInt32(RequestView.GetRowCellValue(m, "ID").ToString());
                        }
                        oRTransCol.Add(new RequestTrans()
                        {
                            RequestID = RequestId,
                            ReqTransID = ReqTransId,
                            ResourceID = resrceId,
                            IOWID = iOwId,
                            Code = RequestView.GetRowCellValue(m, "Code").ToString(),
                            Description = RequestView.GetRowCellValue(m, "Description").ToString(),
                            Unit = RequestView.GetRowCellValue(m, "Unit").ToString(),
                            AnalysisHeadId = Convert.ToInt32(clsStatics.IsNullCheck(RequestView.GetRowCellValue(m, "AnalysisHeadId").ToString(), clsStatics.datatypes.vartypenumeric)),
                            Quantity = Convert.ToDecimal(RequestView.GetRowCellValue(m, "Quantity").ToString()),
                            ReqDate = Convert.ToDateTime(clsStatics.IsNullCheck(RequestView.GetRowCellValue(m, "Req Date").ToString(),clsStatics.datatypes.VarTypeDate)),
                            Remarks=Convert.ToString(clsStatics.IsNullCheck(RequestView.GetRowCellValue(m, "Remarks").ToString(),clsStatics.datatypes.vartypestring)),
                            UnitId=Convert.ToInt32(clsStatics.IsNullCheck(RequestView.GetRowCellValue(m,"UnitId").ToString(),clsStatics.datatypes.vartypenumeric))
                        });
                    }
                    if (oRTransCol.Count > 0)
                    {
                        dtRTrans =clsStatics.GenericListToDataTable(oRTransCol);
                    }
                }

            }
            catch (Exception Except)
            {
                MessageBox.Show("Error: " + Except.Message);
            }
        }

        public DataTable GenericListToDataTable(object list)
        {
            DataTable dt = null;
            Type listType = list.GetType();
            if (listType.IsGenericType)
            {
                //determine the underlying type the List<> contains
                Type elementType = listType.GetGenericArguments()[0];

                //create empty table -- give it a name in case
                //it needs to be serialized
                dt = new DataTable(elementType.Name + "List");

                //define the table -- add a column for each public
                //property or field
                MemberInfo[] miArray = elementType.GetMembers(
                    BindingFlags.Public | BindingFlags.Instance);
                foreach (MemberInfo mi in miArray)
                {
                    if (mi.MemberType == MemberTypes.Property)
                    {
                        PropertyInfo pi = mi as PropertyInfo;
                        dt.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else if (mi.MemberType == MemberTypes.Field)
                    {
                        FieldInfo fi = mi as FieldInfo;
                        dt.Columns.Add(fi.Name, fi.FieldType);
                    }
                }

                //populate the table
                IList il = list as IList;
                foreach (object record in il)
                {
                    int i = 0;
                    object[] fieldValues = new object[dt.Columns.Count];
                    foreach (DataColumn c in dt.Columns)
                    {
                        MemberInfo mi = elementType.GetMember(c.ColumnName)[0];
                        if (mi.MemberType == MemberTypes.Property)
                        {
                            PropertyInfo pi = mi as PropertyInfo;
                            fieldValues[i] = pi.GetValue(record, null);
                        }
                        else if (mi.MemberType == MemberTypes.Field)
                        {
                            FieldInfo fi = mi as FieldInfo;
                            fieldValues[i] = fi.GetValue(record);
                        }
                        i++;
                    }
                    dt.Rows.Add(fieldValues);
                }
            }
            return dt;
        }

        private void clearEntris()
        {
            txtReqNo.Text = "";
            cboCC.EditValue = -1;
            cboReqType.Text = "None";
            dtRes.Rows.Clear();
            cboReqType.Enabled = true;
            cboCC.Enabled = true;
            dtAnal = null;
            dtAnalUpdate = null;
            dsAnalysisHead = null;
            grdAnal = null;
            grdAnalysis.DataSource = null;
            GetVoucherNo();
            grdStock.DataSource = null;
            grdWBSQty.DataSource = null;
            tvAnal.DataSource = null;
            txtCCReqNo.Text = "";
            rTxtNarration.Text = "";         
            txtRefNo.Text = "";
            grdDetail.Rows.Clear();

        }

        private void PopulateEditData()
        {
            dsRDet = new DataSet();

            dsRDet = oReqBL.getRequestDetails(RequestId);
            cboReqType.Enabled = false;
            cboCC.Enabled = false;
            txtCCReqNo.Enabled = false;
            if (dsRDet.Tables[0].Rows[0]["Approve"].ToString() == "Y")
                btnOk.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            else
                btnOk.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            if (dsRDet.Tables.Count > 0)
            {
                m_sMode = "E";

                if (dsRDet.Tables["ReqRegister"].Rows.Count > 0)
                {
                    dtpQDate.EditValue = Convert.ToDateTime(dsRDet.Tables["ReqRegister"].Rows[0]["RequestDate"].ToString());
                    txtReqNo.Text = dsRDet.Tables["ReqRegister"].Rows[0]["RequestNo"].ToString();
                    txtRefNo.Text = dsRDet.Tables["ReqRegister"].Rows[0]["RefNo"].ToString();
                    rTxtNarration.Text = dsRDet.Tables["ReqRegister"].Rows[0]["Narration"].ToString();
                    oVType = new BsfGlobal.VoucherType();
                    oVType = BsfGlobal.GetVoucherNo(24, Convert.ToDateTime(dtpQDate.EditValue), 0, 0);
                    if (oVType.GenType == true)
                    {
                        txtReqNo.Visible = false;
                        lblReqNo.Visible = false;
                        lblAutoNo.Caption = dsRDet.Tables["ReqRegister"].Rows[0]["RequestNo"].ToString();
                    }
                    else
                    {
                        lblAutoNo.Caption = "";
                        txtReqNo.Text = dsRDet.Tables["ReqRegister"].Rows[0]["RequestNo"].ToString();
                        txtReqNo.Visible = true;
                        lblReqNo.Visible = true;
                    }
                    cboCC.EditValue = Convert.ToInt32(dsRDet.Tables["ReqRegister"].Rows[0]["CostCentreId"].ToString());

                    cboReqType.Text = dsRDet.Tables["ReqRegister"].Rows[0]["RequestType"].ToString();

                    cboCC.EditValue = Convert.ToInt32(dsRDet.Tables["ReqRegister"].Rows[0]["CostCentreId"].ToString());
                    txtCCReqNo.Text = dsRDet.Tables["ReqRegister"].Rows[0]["CCReqNo"].ToString();
                    dtAnalUpdate = RequestEntryBL.GetAnalUpdate(Convert.ToInt32(cboCC.EditValue), RequestId);

                    if (dsRDet.Tables["ReqRegister"].Rows[0]["RequestType"].ToString() == "Material" || dsRDet.Tables["ReqRegister"].Rows[0]["RequestType"].ToString()=="Asset")
                    {
                        grdAnalysis.Enabled = true;
                        docWPM.Hide();
                        documentWindow1.Show();
                        documentWindow2.Show();
                        documentWindow3.Show();
                        documentWindow4.Show();

                        if (dsAnalysisHead.Tables["Analysis"].Rows.Count > 0)
                        {
                            if (dtAnalUpdate.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dtAnalUpdate.Rows)
                                {
                                    foreach (DataRow dr1 in dsAnalysisHead.Tables["Analysis"].Rows)
                                    {
                                        if (dr["AnalysisId"].ToString() == dr1["AnalysisID"].ToString())
                                        {
                                            dr["WBS"] = dr1["AnalysisHeadName"];
                                        }
                                    }
                                }

                                DataRow dr2;
                                foreach (DataRow dr in dtAnalUpdate.Rows)
                                {
                                    dr2 = dtAnal.NewRow();
                                    dr2["WBS"] = dr["WBS"];
                                    dr2["Qty"] = dr["ReqQty"];
                                    dr2["REsource_ID"] = dr["ResourceId"];
                                    dr2["Analysis_ID"] = dr["AnalysisId"];
                                    dr2["CCID"] = cboCC.EditValue;
                                    dtAnal.Rows.Add(dr2);
                                }
                            }
                        }

                        if (dsRDet.Tables["ReqTrans"].Rows.Count > 0)
                        {
                            dtRpop = dsRDet.Tables["ReqTrans"];

                            DataRow row;
                            for (int j = 0; j < dtRpop.Rows.Count; j++)
                            {
                                row = dtRes.NewRow();
                                if (cboReqType.Text.ToString().Trim() == "IOW")
                                {
                                    row["ID"] = dtRpop.Rows[j]["Project_IOW_ID"].ToString();
                                }
                                else
                                {
                                    row["ID"] = dtRpop.Rows[j]["ResourceId"].ToString();
                                }
                                row["Code"] = dtRpop.Rows[j]["Resource_Code"].ToString();
                                row["Description"] = dtRpop.Rows[j]["Resource_Name"].ToString();
                                row["Unit"] = dtRpop.Rows[j]["Unit_Name"].ToString();
                                row["Quantity"] = Convert.ToDecimal(clsStatics.IsNullCheck(dtRpop.Rows[j]["Quantity"].ToString(), clsStatics.datatypes.vartypenumeric));
                                row["Req Date"] = dtRpop.Rows[j]["ReqDate"].ToString();
                                row["Remarks"] = dtRpop.Rows[j]["Remarks"].ToString();
                                row["HiddenQty"] = Convert.ToDecimal(clsStatics.IsNullCheck(dtRpop.Rows[j]["HiddenQty"].ToString(), clsStatics.datatypes.vartypenumeric));
                                row["UnitId"] = dtRpop.Rows[j]["Unit_Id"].ToString();
                                dtRes.Rows.Add(row);
                            }
                        }
                        dtReqSchedule = RequestEntryBL.GetReqSchedule(RequestId);
                    }
                    else
                    {
                        documentWindow1.Hide();
                        documentWindow2.Hide();
                        documentWindow3.Hide();
                        documentWindow4.Show();
                        docWPM.Show();
                        
                        PopulateLabourEditData(dsRDet.Tables["ReqRegister"].Rows[0]["RequestType"].ToString(), RequestId);
                    }
                }
            }            

        }       

        private void PopulateLabourEditData(string argqType,int argRequestId)
        {
            try
            {
                int eRowId = 0;
                DataTable dtpoplbr = new DataTable();
                DataTable dtSevice = new DataTable();

                dtRes.Rows.Clear();   
  
                if (argqType == "Labour" || argqType == "Activity" || argqType == "Asset")
                {
                    dtpoplbr = clsStatics.PopulateLabour(argRequestId);

                    for (int trns = 0; trns < dtpoplbr.Rows.Count; trns++)
                    {
                        DataRow dr = dtRes.NewRow();

                        dr["RowId"] = (dtRes.Rows.Count + 1);
                        dr["IOWID"] = 0;
                        dr["ID"] = Convert.ToInt32(dtpoplbr.Rows[trns]["ResourceId"].ToString());
                        dr["Description"] = dtpoplbr.Rows[trns]["Description"].ToString();
                        dr["Code"] = dtpoplbr.Rows[trns]["Code"].ToString();
                        dr["UnitId"] = dtpoplbr.Rows[trns]["UnitId"].ToString();
                        dr["Unit"] = dtpoplbr.Rows[trns]["Unit"].ToString();
                        dr["Quantity"] = Convert.ToDouble(dtpoplbr.Rows[trns]["Quantity"].ToString());
                        dr["AnalysisHeadId"] = dtpoplbr.Rows[trns]["AnalysisHeadId"].ToString();
                        dr["Req Date"] = dtpoplbr.Rows[trns]["ReqDate"].ToString();
                        dr["Remarks"] = dtpoplbr.Rows[trns]["Remarks"].ToString();


                        eRowId = (dtRes.Rows.Count + 1);
                        dtRes.Rows.Add(dr);

                        ReqInsertIOW(argqType, eRowId, Convert.ToInt32(dtpoplbr.Rows[trns]["ResourceId"].ToString()), m_sSplit, Convert.ToInt32(dtpoplbr.Rows[trns]["RequestTransId"].ToString()));
                    }
                }
                else if (argqType == "IOW" || argqType == "Sub-IOW")
                {
                    DataTable dtIWOTran = new DataTable();

                    dtIWOTran = clsStatics.PopulateIOW(argRequestId);

                    for (int trns = 0; trns < dtIWOTran.Rows.Count; trns++)
                    {
                        DataRow dr = dtRes.NewRow();

                        dr["RowId"] = (dtRes.Rows.Count + 1);
                        dr["IOWID"] = Convert.ToInt32(dtIWOTran.Rows[trns]["IOW_ID"].ToString());
                        dr["ID"] = 0;
                        dr["Description"] = dtIWOTran.Rows[trns]["Description"].ToString();
                        dr["Code"] = dtIWOTran.Rows[trns]["Code"].ToString();
                        dr["UnitId"] = dtIWOTran.Rows[trns]["UnitId"].ToString();
                        dr["Unit"] = dtIWOTran.Rows[trns]["Unit"].ToString();
                        dr["Quantity"] = Convert.ToDouble(dtIWOTran.Rows[trns]["Quantity"].ToString());
                        dr["AnalysisHeadId"] = dtIWOTran.Rows[trns]["AnalysisHeadId"].ToString();
                        dr["Req Date"] = dtIWOTran.Rows[trns]["ReqDate"].ToString();
                        dr["Remarks"] = dtIWOTran.Rows[trns]["Remarks"].ToString();
                        

                        eRowId = (dtRes.Rows.Count + 1);

                        dtRes.Rows.Add(dr);

                        if (argqType == "IOW")
                        {
                            ReqWOWBSE("I", eRowId, Convert.ToInt32(dtIWOTran.Rows[trns]["IOW_ID"].ToString()), m_sSplit, Convert.ToInt32(dtIWOTran.Rows[trns]["RequestTransId"].ToString()));
                        }

                        if (argqType == "Sub-IOW")
                        {
                            ReqInsertSubIOW(argqType, eRowId, Convert.ToInt32(dtIWOTran.Rows[trns]["IOW_ID"].ToString()), m_sMode, Convert.ToInt32(dtIWOTran.Rows[trns]["RequestTransId"].ToString()), m_sSplit);
                        }
                    }
                }
                else if (argqType == "Service")
                {
                    dtSevice = clsStatics.PopulateService(argRequestId);

                    for (int trns = 0; trns < dtSevice.Rows.Count; trns++)
                    {
                        DataRow dr = dtRes.NewRow();

                        dr["RowId"] = (dtRes.Rows.Count + 1);
                        dr["IOWID"] = 0;
                        dr["ResourceId"] = Convert.ToInt32(dtSevice.Rows[trns]["ResourceId"].ToString());
                        dr["Description"] = dtSevice.Rows[trns]["Description"].ToString();
                        dr["ResourceCode"] = dtSevice.Rows[trns]["Code"].ToString();
                        dr["UnitId"] = dtSevice.Rows[trns]["UnitId"].ToString();
                        dr["Unit"] = dtSevice.Rows[trns]["Unit"].ToString();                        
                        dr["Qty"] = Convert.ToDouble(dtSevice.Rows[trns]["Quantity"].ToString());
                        dr["AnalysisHeadId"] = dtSevice.Rows[trns]["AnalysisHeadId"].ToString();
                        dr["Req Date"] = dtSevice.Rows[trns]["ReqDate"].ToString();
                        dr["Remarks"] = dtSevice.Rows[trns]["Remarks"].ToString();

                        dtRes.Rows.Add(dr);
                    }
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void HideAnalColumns()
        {
            AnalView.Columns["REsource_ID"].Visible = false;
            AnalView.Columns["BrandID"].Visible = false;
            AnalView.Columns["Analysis_ID"].Visible = false;
            AnalView.Columns["CCID"].Visible = false;
            AnalView.Columns["HiddenQty"].Visible = false;
        }

        private void GetAnalysisHead()
        {
            dsAnalysisHead = RequestEntryBL.GetAnalysisHead(Convert.ToInt32(cboCC.EditValue));
        }

        private void GetVoucherNo()
        {
            oVType = new BsfGlobal.VoucherType();
            oVType = BsfGlobal.GetVoucherNo(24, Convert.ToDateTime(dtpQDate.EditValue), 0, 0);
            if (oVType.GenType == true)
            {
                txtReqNo.Visible = false;
                txtReqNo.Text = oVType.VoucherNo;
                lblAutoNo.Caption = oVType.VoucherNo;
                lblReqNo.Visible = false;
            }
            else
            {
                txtReqNo.Visible = true;
                txtReqNo.Enabled = true;
                lblAutoNo.Caption = "";
                lblReqNo.Visible = true;
                txtReqNo.Text = "";
            }
        }

        private void PopulateLevelWBSDetails(int argAnalId, string argProjDb)
        {
            DataTable dtT = new DataTable();
            dtT = oReqBL.GetAnalTree(argAnalId, argProjDb);
            tvAnal.RootValue = "WBS Analysis";
            tvAnal.ParentFieldName = "ParentID";
            tvAnal.KeyFieldName = "AnalysisID";
            tvAnal.DataSource = dtT;
            tvAnal.Columns["AnalysisHeadName"].Visible = true;
            tvAnal.Columns["LevelNo"].Visible = false;
            tvAnal.Columns["LastLevel"].Visible = false;
        }

        private void ReqScheduleColumns()
        {
            if (dtReqSchedule == null)
            {
                dtReqSchedule = new DataTable();
                dtReqSchedule.Columns.Add("ResourceId", typeof(int));
                dtReqSchedule.Columns.Add("Qty", typeof(decimal));
                dtReqSchedule.Columns.Add("ReqDate", typeof(DateTime));
            }
        }

        private void RequestListShow()
        {
            frmRequestRegister IList = new frmRequestRegister() { TopLevel = false, Dock = DockStyle.Fill };
            if (BsfGlobal.g_bWorkFlow == true)
            {
                Cursor.Current = Cursors.WaitCursor;
                Parent.Controls.Owner.Hide();
                frmRequestRegister.m_oDW.Show();
                Cursor.Current = Cursors.Default;
            }
            else
            {
               // Radpanel.Controls.Clear();
                //Radpanel.Controls.Add(IList);
            }

            //if (BsfGlobal.g_bWorkFlow == false)
            //    IList.Radpanel = Radpanel;
            //IList.Show();
        }

        private void GetStockDetails(int argCCId,int argResId,string Qtype)
        {
            DataTable DtStock;
            if (argCCId > 0 && argResId > 0)
            {
                grdStock.DataSource = null;
                DtStock = RequestEntryBL.GetStockDetails(argCCId.ToString(), argResId, Qtype);
                grdStock.DataSource = DtStock;
                grdStock.ForceInitialize();
                StockView.PopulateColumns();
                StockView.Columns["BalReqQty"].Visible = false;
                StockView.Columns["BalIndQty"].Visible = false;
                StockView.Columns["BalWOQty"].Visible = false;
                StockView.Columns["BalPOQty"].Visible = false;
                StockView.Columns["TotPurchase"].Visible = false;
                StockView.Columns["WOQty"].Visible = false;
                //StockView.Columns["DCQty"].Visible = false;
                StockView.Columns["CostCentreId"].Visible = false;
                StockView.Columns["OpeningStock"].Visible = false;
                StockView.Columns["MinStock"].Visible = false;
                StockView.Columns["ClosingStock"].Visible = false;
                //StockView.Columns["IndentQty"].Visible = false;
                //StockView.Columns["POQty"].Visible = false;
               // StockView.Columns["BillQty"].Visible = false;
                StockView.Columns["Resource_Id"].Visible = false;
                StockView.Columns["QualifiedRate"].Visible = false;
                StockView.Columns["TotDCQty"].Visible = false;
                StockView.Columns["TotBillQty"].Visible = false;
                StockView.Columns["TotRetQty"].Visible = false;
                StockView.Columns["TotTranQty"].Visible = false;


                StockView.OptionsCustomization.AllowSort = false;
                StockView.OptionsBehavior.AllowIncrementalSearch = true;
                StockView.OptionsView.ShowAutoFilterRow = false;
                StockView.OptionsView.ShowViewCaption = false;
                StockView.OptionsCustomization.AllowFilter = false;
                StockView.OptionsView.ShowFooter = true;
                StockView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
                StockView.OptionsSelection.EnableAppearanceFocusedCell = true;
                StockView.OptionsSelection.EnableAppearanceFocusedRow = false;
                StockView.OptionsSelection.InvertSelection = false;
                StockView.Appearance.HeaderPanel.Font = new Font(StockView.Appearance.HeaderPanel.Font, FontStyle.Bold);
                StockView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                StockView.OptionsMenu.EnableColumnMenu = false;
                StockView.OptionsCustomization.AllowColumnMoving = false;
                grdStock.Controls.Clear();
            }
        }

        private void PopulateWBSQty(int argCCId, int argResId,int argAnalId)
        {
            DataTable dtWBS = new DataTable();
            using (DataTable dt = new DataTable())
            {
            }

            dtWBS = RequestEntryBL.GetRequestWBSQty(argCCId.ToString(), argResId, argAnalId,Qtype);
            grdWBSQty.DataSource = dtWBS;
            WBSQtyView.PopulateColumns();
            WBSQtyView.Columns["HiddenQty"].Visible = false;
            WBSQtyView.Columns["CostCentreId"].Visible = false;
            WBSQtyView.Columns["ResourceId"].Visible = false;
            WBSQtyView.Columns["AnalysisId"].Visible = false;
            WBSQtyView.Columns["BalReqQty"].Visible = false;
            WBSQtyView.Columns["BalIndQty"].Visible = false;
            WBSQtyView.Columns["BalPOQty"].Visible = false;
            WBSQtyView.Columns["TotPurchase"].Visible = false;
            WBSQtyView.Columns["TotDCQty"].Visible = false;
            WBSQtyView.Columns["TotBillQty"].Visible = false;
            WBSQtyView.Columns["TotRetQty"].Visible = false;
            WBSQtyView.Columns["TotTranQty"].Visible = false;

            WBSQtyView.Columns["AnalysisId"].Group();
            WBSQtyView.Columns["AnalysisHeadName"].Group();
            //WBSQtyView.Columns["AnalysisHeadName"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            WBSQtyView.ExpandAllGroups();

            WBSQtyView.OptionsBehavior.AllowIncrementalSearch = true;
            WBSQtyView.OptionsView.ShowAutoFilterRow = false;
            WBSQtyView.OptionsView.ShowViewCaption = false;
            WBSQtyView.OptionsCustomization.AllowFilter = false;
            WBSQtyView.OptionsView.ShowFooter = true;
            WBSQtyView.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
            WBSQtyView.OptionsSelection.EnableAppearanceFocusedCell = true;
            WBSQtyView.OptionsSelection.EnableAppearanceFocusedRow = false;
            WBSQtyView.OptionsSelection.InvertSelection = false;
            WBSQtyView.Appearance.HeaderPanel.Font = new Font(WBSQtyView.Appearance.HeaderPanel.Font, FontStyle.Bold);
            WBSQtyView.OptionsMenu.EnableColumnMenu = false;
            WBSQtyView.OptionsCustomization.AllowColumnMoving = false;
        }
        public bool ValidRequest()
        {
            bool valid = true;
            StringBuilder sb = new StringBuilder();
            if (cboCC.Text == "None")
            {
                valid = false;
                sb.Append(" * Select Pro" + Environment.NewLine);
                errorProvider1.SetError(cboCC, "Select Project");
            }
            else
            {
                errorProvider1.SetError(cboCC, "");
            }
            if (cboReqType.Text == "None")
            {
                valid = false;
                sb.Append(" * Select Request Type " + Environment.NewLine);
                errorProvider1.SetError(cboReqType, "Select Request Type ");
            }
            else
            {
                errorProvider1.SetError(cboReqType, "");
            }
           
            return valid;
        }
       
        #endregion

        #region ButtonEvents

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (ValidRequest() == false) return;
                sComps = "";
                sRComps = "";
                frmComponent frmComp = new frmComponent();
                     
                if (RequestView.RowCount > 0)
                {
                    cboReqType.Enabled = false;

                    for (int i = 0; i < RequestView.RowCount; i++)
                    {
                        sComps = sComps + RequestView.GetRowCellValue(i, "ID").ToString() + ",";
                    }
                    if (sComps != "")
                    {
                        sComps = sComps.Substring(0, sComps.Length - 1);
                    }
                }
                           
                if (cboReqType.EditValue.ToString() != "None")
                {
                    m_sCompTypeId = 0;
                    
                    if (cboReqType.EditValue.ToString() == "Material")
                    {
                        m_sCompTypeId = 2;
                    }
                    else if (cboReqType.EditValue.ToString() == "Asset")
                    {
                        m_sCompTypeId = 3;
                    }
                    if (Qtype == "M" || Qtype == "H")
                    {
                        grdAnalysis.Enabled = true;
                                              
                        docWPM.Hide();
                        documentWindow1.Show();
                        documentWindow2.Show();
                        documentWindow3.Show();
                        documentWindow4.Show();

                        if (m_sCompTypeId != 0)
                        {
                            if (Convert.ToInt32(cboCC.EditValue) <= 0) return;
                            frmComponentPickList frm1 = new frmComponentPickList();
                            sRComps = frm1.Execute(sComps, m_sCompTypeId, false, "C", "", ProjDb, m_iRevId);
                            PopulateComp(sRComps);
                            if (RequestView.RowCount > 0)
                            {
                                cboReqType.Enabled = false;
                                cboCC.Enabled = false;
                            }
                        }
                    }
                    else
                    {
                        documentWindow1.Hide();
                        documentWindow2.Hide();
                        documentWindow3.Hide();
                     
                        docWPM.Show();
                        documentWindow4.Show();

                        int iRowId=0;
                        DataTable m_dt = new DataTable();
                        m_dt = frmComp.Execute("W", Qtype, sComps, m_iCCId, 0, m_iRevId, 0, "", "");
                        if (m_dt != null)
                        {
                            DataRow row;
                            for (int j = 0; j < m_dt.Rows.Count; j++)
                            {
                                row = dtRes.NewRow();
                                iRowId=(dtRes.Rows.Count + 1);
                                row["RowId"] = iRowId;                                
                                row["ID"] = m_dt.Rows[j]["ResourceId"].ToString();
                                row["Code"] = m_dt.Rows[j]["ResourceCode"].ToString();
                                row["Description"] = m_dt.Rows[j]["Description"].ToString();
                                row["Unit"] = m_dt.Rows[j]["Unit"].ToString();
                                row["UnitId"] = m_dt.Rows[j]["UnitId"].ToString();
                                row["IOWID"] = m_dt.Rows[j]["IOW_ID"].ToString();
                                row["AnalysisHeadId"] = m_dt.Rows[j]["AnalysisHeadId"].ToString();
                                row["Quantity"] = 0;

                                dtRes.Rows.Add(row);

                                if (Qtype != "I")
                                {                                   
                                    if (Qtype == "S" || Qtype == "I")
                                    {
                                        m_dtWOIOW.Rows.Clear();
                                        m_dtSWOIOW = clsStatics.getSubIOWDetails(Convert.ToInt32(m_dt.Rows[j]["IOW_ID"].ToString()), m_sSplit, "W", m_iRevId, 0);
                                        PopulateIOWDetailsWO(iRowId);
                                    }
                                    else
                                    {
                                        m_dtSWOIOW.Rows.Clear();
                                        m_dtWOIOW = clsStatics.getIOWDetails(Convert.ToInt32(m_dt.Rows[j]["ResourceId"].ToString()), m_sSplit, "W", m_iRevId, 0);
                                        PopulateIOWDetailsWO(iRowId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Except)
            {
                MessageBox.Show("Error: " + Except.Message);
            }
        }

        private void PopulateIOWDetailsWO(int argRowId)
        {
            try
            {
                // List<WOIOWTransUpdate> oWOIOWTrans
                if ((Qtype == "L") || (Qtype == "A") || (Qtype == "H"))
                {
                    string Serial_No = "";
                    double getQty = 0;
                   

                    if (m_dtWOIOW.Rows.Count > 0)
                    {

                        if (oWOIOWTrans.Count > 0)
                        {
                            oWOIOWTrans.RemoveAll(delegate(WOIOWTransUpdate del)
                            {
                                return (del.WOTrnsRowId == argRowId);
                            });
                        }

                        for (int k = 0; k < m_dtWOIOW.Rows.Count; k++)
                        {
                            

                            if (m_dtWOIOW.Rows[k]["RefSerialNo"].ToString() != "")
                            {
                                Serial_No = m_dtWOIOW.Rows[k]["RefSerialNo"].ToString();
                            }
                            else
                            {
                                Serial_No = m_dtWOIOW.Rows[k]["RefSerialNo"].ToString();
                            }
                            if (getQty < 0) { getQty = 0; }

                            oWOIOWTrans.Add(new WOIOWTransUpdate()
                            {
                                WOTrnsRowId = argRowId,
                                IOWRowId = Convert.ToInt32(m_dtWOIOW.Rows[k]["RowId"].ToString()),                                                               
                                IOW_Trans_ID = Convert.ToInt32(clsStatics.IsNullCheck(m_dtWOIOW.Rows[k]["Resource_Id"].ToString(), clsStatics.datatypes.vartypenumeric)),
                                Specification = m_dtWOIOW.Rows[k]["Specification"].ToString(),
                                UOM_ID = m_dtWOIOW.Rows[k]["UOM_ID"].ToString(),
                                IOW_ID = Convert.ToInt32(m_dtWOIOW.Rows[k]["IOW_ID"].ToString()),
                                AnalysisHeadID = Convert.ToInt32(clsStatics.IsNullCheck(m_dtWOIOW.Rows[k]["Analysis_Head_ID"].ToString(), clsStatics.datatypes.vartypenumeric)),
                                Serial_No = Serial_No,
                                BillType = Qtype,
                                Qty = getQty,
                                CNType = "N",
                            });
                        }
                    }
                }
                if (Qtype == "S")
                {
                    string Serial_No = "";
                    double getQty = 0;
                                        

                    if (m_dtSWOIOW.Rows.Count > 0)
                    {
                        for (int k = 0; k < m_dtSWOIOW.Rows.Count; k++)
                        {                           
                            if (oWOIOWTrans.Count > 0)
                            {
                                oWOIOWTrans.RemoveAll(delegate(WOIOWTransUpdate del)
                                {
                                    return (del.WOTrnsRowId == argRowId);
                                });
                            }
                            if (m_dtSWOIOW.Rows[k]["RefSerialNo"].ToString() != "")
                            {
                                Serial_No = m_dtSWOIOW.Rows[k]["RefSerialNo"].ToString();
                            }
                            else
                            {
                                Serial_No = m_dtSWOIOW.Rows[k]["Serial_No"].ToString();
                            }
                            if (getQty < 0) { getQty = 0; }
                            oWOIOWTrans.Add(new WOIOWTransUpdate()
                            {
                                WOTrnsRowId = argRowId,
                                IOWRowId = Convert.ToInt32(m_dtSWOIOW.Rows[k]["RowId"].ToString()),                                                                
                                IOW_Trans_ID = Convert.ToInt32(m_dtSWOIOW.Rows[k]["IOW_ID"].ToString()),
                                Specification = m_dtSWOIOW.Rows[k]["Specification"].ToString(),
                                UOM_ID = m_dtSWOIOW.Rows[k]["UOM_ID"].ToString(),
                                IOW_ID = Convert.ToInt32(m_dtSWOIOW.Rows[k]["IOW_ID"].ToString()),
                                AnalysisHeadID = Convert.ToInt32(m_dtSWOIOW.Rows[k]["Analysis_Head_ID"].ToString()),
                                Serial_No = Serial_No,
                                BillType = Qtype,
                                Qty = getQty,
                                CNType = "N",
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void txtQty_EditValueChanged(object sender, EventArgs e)
        {
            //DevExpress.XtraEditors.TextEdit cboType = (DevExpress.XtraEditors.TextEdit)sender;
            //RequestView.SetRowCellValue(RequestView.FocusedRowHandle, "Quantity", cboType.EditValue);
        }

        private void btnExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void btnOk_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
            if (BsfGlobal.FindPermission("Request-Create") == false && RequestId == 0)
            {
                MessageBox.Show("No Rights to Add New Request", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (BsfGlobal.FindPermission("Request-Modify") == false && RequestId > 0)
            {
                MessageBox.Show("No Rights to Modify Request", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (ValidRequest() == false) return;
            if (RequestView.RowCount == 0)
            {
                MessageBox.Show("Add Material's!", "Information ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //Zero Qty Validation
            decimal Qty = 0;
            for (int i = 0; i < RequestView.RowCount; i++)
            {
                Qty = Convert.ToDecimal(clsStatics.IsNullCheck(RequestView.GetRowCellValue(i, "Quantity"), clsStatics.datatypes.vartypenumeric));

                if (Qty == 0)
                {
                    MessageBox.Show("Zero Quantity Exists!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RequestView.FocusedRowHandle = i;
                    return;
                }

            }
            bool bUpdate = false;
            string sRefNo = "";

            UpdateData();

            oVType = new BsfGlobal.VoucherType();
            oVCCType = new BsfGlobal.VoucherType();


             if (Qtype == "M")
             {
                 if (RequestId > 0)
                 {
                     if (oRReg.RequestDate != Convert.ToDateTime(dtpQDate.EditValue).ToShortDateString())
                     {
                         if (oVType.PeriodWise == true)
                         {
                             if (BsfGlobal.CheckPeriodChange(Convert.ToDateTime(oRReg.RequestDate), Convert.ToDateTime(dtpQDate.EditValue)) == true)
                             {
                                 oVType = BsfGlobal.GetVoucherNo(24, Convert.ToDateTime(dtpQDate.EditValue), 0, 0);
                                 txtReqNo.Text = oVType.VoucherNo;
                                 oRReg.RequestNo = oVType.VoucherNo;


                                 oVCCType = BsfGlobal.GetVoucherNo(24, Convert.ToDateTime(dtpQDate.EditValue), 0, oRReg.CCId);
                                 if (oVCCType.GenType == true)
                                 {
                                     txtCCReqNo.Text = oVCCType.VoucherNo;
                                     oRReg.CCReqNo = oVCCType.VoucherNo;
                                 }
                                 BsfGlobal.UpdateMaxNo(24, oVType, 0, 0);
                                 BsfGlobal.UpdateMaxNo(24, oVCCType, 0, oRReg.CCId);
                             }
                         }
                     }
                     oReqBL.UpdateReqReg(oRReg, dtRTrans, dtAnal, dtReqSchedule);
                     BsfGlobal.InsertLog(DateTime.Now, "Request-Modify", "E", "Indent", RequestId, Convert.ToInt32(clsStatics.IsNullCheck(cboCC.EditValue, clsStatics.datatypes.vartypenumeric)), 0, BsfGlobal.g_sVendorDBName, txtReqNo.Text, BsfGlobal.g_lUserId);
                     
                     clearEntris();                    
                 }
                 else
                 {
                     RequestId = oReqBL.InsertReqReg(oRReg, dtRTrans, dtAnal, dtReqSchedule, ref bUpdate,ref sRefNo);
                     if (bUpdate == true)
                     {
                         BsfGlobal.InsertLog(DateTime.Now, "Request-Create", "N", "Request", RequestId, m_iCCId, 0, BsfGlobal.g_sVendorDBName, sRefNo, BsfGlobal.g_lUserId);
                     }                     
                     clearEntris();
                 }
             }
             else
             {
                 //Insert
                 if (RequestId == 0)
                 {
                     RequestId = oReqBL.InsertLbrRegister(oRReg, dtRTrans, oWOIOWTrans, m_lWOWBS, ref bUpdate, ref sRefNo, Qtype);
                     if (bUpdate == true)
                     {
                         BsfGlobal.InsertLog(DateTime.Now, "Request-Create", "N", "Request", RequestId, m_iCCId, 0, BsfGlobal.g_sVendorDBName, sRefNo, BsfGlobal.g_lUserId);
                     }
                     clearEntris();
                 }
                 else //Update
                 {
                     if (oRReg.RequestDate != Convert.ToDateTime(dtpQDate.EditValue).ToShortDateString())
                     {
                         if (oVType.PeriodWise == true)
                         {
                             if (BsfGlobal.CheckPeriodChange(Convert.ToDateTime(oRReg.RequestDate), Convert.ToDateTime(dtpQDate.EditValue)) == true)
                             {
                                 oVType = BsfGlobal.GetVoucherNo(24, Convert.ToDateTime(dtpQDate.EditValue), 0, 0);
                                 txtReqNo.Text = oVType.VoucherNo;
                                 oRReg.RequestNo = oVType.VoucherNo;


                                 oVCCType = BsfGlobal.GetVoucherNo(24, Convert.ToDateTime(dtpQDate.EditValue), 0, oRReg.CCId);
                                 if (oVCCType.GenType == true)
                                 {
                                     txtCCReqNo.Text = oVCCType.VoucherNo;
                                     oRReg.CCReqNo = oVCCType.VoucherNo;
                                 }                                
                                 BsfGlobal.UpdateMaxNo(24, oVType, 0, 0);
                                 BsfGlobal.UpdateMaxNo(24, oVCCType, 0, oRReg.CCId);                                
                             }
                         }
                     }
                     oReqBL.UpdateLbrRegister(oRReg, dtRTrans, oWOIOWTrans, m_lWOWBS,Qtype);
                     BsfGlobal.InsertLog(DateTime.Now, "Request-Modify", "E", "Indent", RequestId, Convert.ToInt32(clsStatics.IsNullCheck(cboCC.EditValue, clsStatics.datatypes.vartypenumeric)), 0, BsfGlobal.g_sVendorDBName, txtReqNo.Text, BsfGlobal.g_lUserId);
                 }
             }
             if (RequestId == 0)
             {
                 clearEntris();
             }
             else
             {
                 this.Close();
             }
             
           
        }
        
        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (RequestId == 0)
            {
                clearEntris();
            }
            else
            {
                this.Close();
            }
        }

        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int m_iAnalsHId = 0;
            DataRow[] dr;
            if (Qtype != "M")
            {
                if (Qtype == "L" || Qtype == "A" || Qtype == "H" || Qtype == "R")
                {
                    m_iResourceId = Convert.ToInt32(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "ID").ToString());

                    if (dtRes.Rows.Count > 0)
                    {
                        dr = dtRes.Select("ID=" + m_iResourceId + "");
                        if (dr.Length != 0)
                        {
                            dr[0].Delete();
                            dtRes.AcceptChanges();
                        }
                    }
                }
                else if (Qtype == "S" || Qtype == "I")
                {
                    m_iResourceId = Convert.ToInt32(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "IOWID").ToString());

                    m_iAnalsHId = Convert.ToInt32(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "AnalysisHeadId").ToString());

                    if (dtRes.Rows.Count > 0)
                    {
                        if (m_iAnalsHId != 0)
                            dr = dtRes.Select("IOWID=" + m_iResourceId + " And AnalysisHeadId=" + m_iAnalsHId + "");
                        else
                            dr = dtRes.Select("IOWID=" + m_iResourceId + "");

                        if (dr.Length != 0)
                        {
                            dr[0].Delete();
                            dtRes.AcceptChanges();
                        }
                    }
                }

                if (oWOIOWTrans.Count > 0)
                {
                    List<WOIOWTransUpdate> oSelect = oWOIOWTrans.FindAll(
                        delegate(WOIOWTransUpdate dl)
                        {
                            if (m_iAnalsHId == 0)
                                return dl.IOW_Trans_ID == m_iResourceId;
                            else
                                return dl.IOW_Trans_ID == m_iResourceId && dl.AnalysisHeadID == m_iAnalsHId;
                        });
                    if (oSelect.Count > 0)
                    {
                        oWOIOWTrans.RemoveAll(
                        delegate(WOIOWTransUpdate dl)
                        {
                            if (m_iAnalsHId == 0)
                                return dl.IOW_Trans_ID == m_iResourceId;
                            else
                                return dl.IOW_Trans_ID == m_iResourceId && dl.AnalysisHeadID == m_iAnalsHId;

                        });
                    }
                }
            }
            else
            {
                if (RequestId == 0)
                {
                    if (RequestView.RowCount > 0)
                    {
                        DialogResult reply = MessageBox.Show("Do you want Delete?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (reply == DialogResult.Yes)
                        {
                            RequestView.DeleteRow(RequestView.FocusedRowHandle);
                        }
                    }
                }
                else
                {
                    DialogResult reply = MessageBox.Show("Do you want Delete?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (reply == DialogResult.Yes)
                    {
                        oReqBL.DeleteReqResource(RequestId, Convert.ToInt32(RequestView.GetFocusedRowCellValue("ID")));
                        RequestView.DeleteRow(RequestView.FocusedRowHandle);
                    }

                }
            }
        }
            
        private void dtpQDate_Validated(object sender, EventArgs e)
        {
            if (RequestId == 0)
            {
                if (oVType.PeriodWise == true)
                {
                    oVType = new BsfGlobal.VoucherType();
                    oVType = BsfGlobal.GetVoucherNo(24, Convert.ToDateTime(dtpQDate.EditValue), 0, 0);
                    if (oVType.GenType == true)
                    {
                        txtReqNo.Text = oVType.VoucherNo;
                        txtReqNo.Visible = false;
                        lblReqNo.Visible = false;
                        lblAutoNo.Caption = oVType.VoucherNo;
                    }
                    else
                    {
                        txtReqNo.Visible = true;
                        lblReqNo.Visible = true;
                        txtReqNo.Text = "";
                        lblAutoNo.Caption = "";
                    }
                }
            }
        }

        private void dtpQDate_EditValueChanged(object sender, EventArgs e)
        {
            GetVoucherNo();
        }

        private void cboReqType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboReqType.Text != "None")
            {
                if (cboReqType.Text == "Material")
                    Qtype = "M";
                else if (cboReqType.Text == "IOW")
                    Qtype = "I";
                else if (cboReqType.Text == "Labour")
                    Qtype = "L";
                else if (cboReqType.Text == "Asset")
                    Qtype = "H";                
                else if (cboReqType.Text == "Sub-IOW")
                    Qtype = "S";
                else if (cboReqType.Text == "Activity")
                    Qtype = "A";
                else if (cboReqType.Text == "Service")
                    Qtype = "R";

                //if(Qtype=="M")
                //    CreateColumnsR();
                //else
                CreateColumnsR();

                dtProjDetails = RequestEntryBL.GetProjDetails(cboCC.EditValue.ToString(), Qtype);
            }
        }
     

        private void cboCC_EditValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cboCC.EditValue) == 0 || Convert.ToInt32(cboCC.EditValue) == -1) { return; }
            if (Convert.ToInt32(cboCC.EditValue)!=0)
            {
                m_iCCId = Convert.ToInt32(cboCC.EditValue);
                GetAnalysisHead();                
                
                //DevExpress.XtraEditors.LookUpEdit editor = (DevExpress.XtraEditors.LookUpEdit)sender;
                //DataRowView row = editor.Properties.GetDataSourceRowByKeyValue(editor.EditValue) as DataRowView;
                //if (row != null)
                //{
                //    ProjDb = row["ProjectDB"].ToString();
                //    m_iRevId = oReqBL.GetRevisionId(ProjDb);
                //}
                
                if (Convert.ToBoolean(clsStatics.GetProjectDB(m_iCCId)) == true)
                {
                    m_iRevId = clsStatics.GetRevisionId();
                    ProjDb = clsStatics.g_sProjWPMDBName;                    
                }
                else
                {
                    //MessageBox.Show("Project DataBase Not Linked in this CostCentre...");
                    //return;
                }

                if (ProjDb == "")
                {
                    MessageBox.Show("ProjectDB Not Found!", "Request", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cboCC.EditValue = -1;
                    return;
                }
                m_sSplit = clsStatics.GetWBSReqd(m_iCCId);
                RowCreated();

                if(dsAnalysisHead.Tables.Count > 0)
                    dtAnal = dsAnalysisHead.Tables["Anal_Resource"].Clone();                

                oVType = new BsfGlobal.VoucherType();
                oVType = BsfGlobal.GetVoucherNo(24, Convert.ToDateTime(dtpQDate.EditValue), 0, Convert.ToInt32(cboCC.EditValue));
                if (oVType.GenType == true)
                {
                    txtCCReqNo.Text = oVType.VoucherNo;
                    txtCCReqNo.Enabled = false;
                }
                else
                {
                    txtCCReqNo.Enabled = true;
                    txtCCReqNo.Text = "";
                }
            }
        }

        private void RowCreated()
        {
            grdDetail.Rows.Clear();

            m_editorRow1 = new EditorRow();
            m_editorRow1.Name = "EstimateQty";
            m_editorRow1.Properties.Caption = "Estimate Qty";
            m_editorRow1.Properties.Value = "";
            m_editorRow1.Appearance.Options.UseTextOptions = true;
            m_editorRow1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            m_editorRow1.Properties.ReadOnly = false;
            m_editorRow1.Enabled = false;
            grdDetail.Rows.Add(m_editorRow1);

            //m_editorRow1 = new EditorRow();
            //m_editorRow1.Name = "EstimateRate";
            //m_editorRow1.Properties.Caption = "Estimate Rate";
            //m_editorRow1.Properties.Value = "";
            //m_editorRow1.Appearance.Options.UseTextOptions = true;
            //m_editorRow1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            //m_editorRow1.Properties.ReadOnly = false;
            //m_editorRow1.Enabled = false;
            //grdDetail.Rows.Add(m_editorRow1);

            //m_editorRow1 = new EditorRow();
            //m_editorRow1.Name = "QuotationRate";
            //m_editorRow1.Properties.Caption = "Quotation Rate";
            //m_editorRow1.Appearance.Options.UseTextOptions = true;
            //m_editorRow1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            //m_editorRow1.Properties.Value = "";
            //m_editorRow1.Properties.ReadOnly = false;
            //m_editorRow1.Enabled = false;
            //grdDetail.Rows.Add(m_editorRow1);

            //m_editorRow1 = new EditorRow();
            //m_editorRow1.Name = "ApprovedRate";
            //m_editorRow1.Properties.Caption = "Approved Rate";
            //m_editorRow1.Properties.Value = "";
            //m_editorRow1.Appearance.Options.UseTextOptions = true;
            //m_editorRow1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            //m_editorRow1.Properties.ReadOnly = false;
            //m_editorRow1.Enabled = false;
            //grdDetail.Rows.Add(m_editorRow1);

            m_editorRow1 = new EditorRow();
            m_editorRow1.Name = "RequestQty";
            m_editorRow1.Properties.Caption = "Request Qty";
            m_editorRow1.Properties.Value = "";
            m_editorRow1.Appearance.Options.UseTextOptions = true;
            m_editorRow1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            m_editorRow1.Properties.ReadOnly = false;
            m_editorRow1.Enabled = false;
            grdDetail.Rows.Add(m_editorRow1);

            m_editorRow1 = new EditorRow();
            m_editorRow1.Name = "BalanceQty";
            m_editorRow1.Properties.Caption = "Balance Qty";
            m_editorRow1.Properties.Value = "";
            m_editorRow1.Appearance.Options.UseTextOptions = true;
            m_editorRow1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            m_editorRow1.Properties.ReadOnly = false;
            m_editorRow1.Enabled = false;
            grdDetail.Rows.Add(m_editorRow1);
        }

        #endregion

        #region GridEvents
        private void AnalView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            AnalView.RefreshData();
            Decimal VarCalQty = 0;
            Decimal EstimateQty = 0;
            Decimal qty = 0;
            //Decimal ReqQty = 0;
            if (dtProjDetails != null)
            {
                DataView dvProj = new DataView(dtProjDetails);
                if (dvProj.ToTable().Columns.Count > 3)
                {
                    dvProj.RowFilter = "Resource_Id = " + Convert.ToInt32(AnalView.GetFocusedRowCellValue("REsource_ID")) + " AND Analysis_Id=" + Convert.ToInt32(AnalView.GetFocusedRowCellValue("Analysis_ID")) + " ";
                }
                else
                {
                    dvProj.RowFilter = "Resource_Id = " + Convert.ToInt32(AnalView.GetFocusedRowCellValue("REsource_ID")) + " ";
                }
                Decimal VarReqQty = 0;
                if (Qtype == "M")
                {
                    if (WBSQtyView.RowCount == 4)
                    {
                        //VarReqQty = Convert.ToDecimal(clsStatics.IsNullCheck(WBSQtyView.GetRowCellValue(0, "HiddenQty"), clsStatics.datatypes.vartypenumeric));
                        VarReqQty = Convert.ToDecimal(clsStatics.IsNullCheck(WBSQtyView.GetRowCellValue(1, "BalReqQty"), clsStatics.datatypes.vartypenumeric)) + Convert.ToDecimal(clsStatics.IsNullCheck(WBSQtyView.GetRowCellValue(1, "BalIndQty"), clsStatics.datatypes.vartypenumeric)) + Convert.ToDecimal(clsStatics.IsNullCheck(WBSQtyView.GetRowCellValue(1, "BalPOQty"), clsStatics.datatypes.vartypenumeric)) + Convert.ToDecimal(clsStatics.IsNullCheck(WBSQtyView.GetRowCellValue(1, "TotPurchase"), clsStatics.datatypes.vartypenumeric));
                    }
                    else
                    {
                        VarReqQty = Convert.ToDecimal(clsStatics.IsNullCheck(WBSQtyView.GetRowCellValue(0, "BalReqQty"), clsStatics.datatypes.vartypenumeric)) + Convert.ToDecimal(clsStatics.IsNullCheck(WBSQtyView.GetRowCellValue(0, "BalIndQty"), clsStatics.datatypes.vartypenumeric)) + Convert.ToDecimal(clsStatics.IsNullCheck(WBSQtyView.GetRowCellValue(0, "BalPOQty"), clsStatics.datatypes.vartypenumeric)) + Convert.ToDecimal(clsStatics.IsNullCheck(WBSQtyView.GetRowCellValue(0, "TotPurchase"), clsStatics.datatypes.vartypenumeric));
                    }
                }
                else
                {
                    if (WBSQtyView.RowCount == 4)
                    {                        
                        VarReqQty = Convert.ToDecimal(clsStatics.IsNullCheck(WBSQtyView.GetRowCellValue(1, "BalReqQty"), clsStatics.datatypes.vartypenumeric)) + Convert.ToDecimal(clsStatics.IsNullCheck(WBSQtyView.GetRowCellValue(1, "WOQty"), clsStatics.datatypes.vartypenumeric));
                    }
                    else
                    {
                        VarReqQty = Convert.ToDecimal(clsStatics.IsNullCheck(WBSQtyView.GetRowCellValue(0, "BalReqQty"), clsStatics.datatypes.vartypenumeric)) + Convert.ToDecimal(clsStatics.IsNullCheck(WBSQtyView.GetRowCellValue(0, "WOQty"), clsStatics.datatypes.vartypenumeric)) ;
                    }
                }

                VarCalQty = 0;

                if (dvProj.ToTable().Rows.Count > 0) EstimateQty = Convert.ToDecimal(dvProj.ToTable().Rows[0]["Qtty"]); else EstimateQty = 0;
                VarCalQty = EstimateQty * (1 + VariantQty / 100);

                if (RequestId == 0)
                {
                    if (((Convert.ToDecimal(clsStatics.IsNullCheck(AnalView.GetFocusedRowCellValue("Qty"), clsStatics.datatypes.vartypenumeric)) + VarReqQty) > VarCalQty))
                    {
                        MessageBox.Show("CurrentQty is greater than EstimateQty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        AnalView.SetRowCellValue(AnalView.FocusedRowHandle, "Qty", "0.00000");

                        return;
                    }
                }
                else
                {
                    if (((Convert.ToDecimal(clsStatics.IsNullCheck(AnalView.GetFocusedRowCellValue("Qty"), clsStatics.datatypes.vartypenumeric)) + VarReqQty) - Convert.ToDecimal(clsStatics.IsNullCheck(AnalView.GetFocusedRowCellValue("HiddenQty"), clsStatics.datatypes.vartypenumeric))) > VarCalQty)
                    {
                        MessageBox.Show("CurrentQty is greater than EstimateQty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        AnalView.SetRowCellValue(AnalView.FocusedRowHandle, "Qty", Convert.ToDecimal(clsStatics.IsNullCheck(AnalView.GetFocusedRowCellValue("HiddenQty"), clsStatics.datatypes.vartypenumeric)));

                        return;
                    }
                }
                if (((Convert.ToDecimal(clsStatics.IsNullCheck(AnalView.GetFocusedRowCellValue("Qty"), clsStatics.datatypes.vartypenumeric)) + VarReqQty - Convert.ToDecimal(clsStatics.IsNullCheck(AnalView.GetFocusedRowCellValue("HiddenQty"), clsStatics.datatypes.vartypenumeric))) > VarCalQty))
                {
                    MessageBox.Show("CurrentQty is greater than EstimateQty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AnalView.SetRowCellValue(AnalView.FocusedRowHandle, "Qty", Convert.ToDecimal(clsStatics.IsNullCheck(AnalView.GetFocusedRowCellValue("HiddenQty"), clsStatics.datatypes.vartypenumeric)));

                    return;
                }
                //if (ReqQty > 0)
                //{
                //    decimal Qty = Convert.ToDecimal(AnalView.GetFocusedRowCellValue("Quantity"));
                //    decimal HQty = Convert.ToDecimal(AnalView.GetFocusedRowCellValue("HiddenQty"));

                //    decimal BalQty = Convert.ToDecimal(AnalView.GetFocusedRowCellValue("BalanceQty"));
                //    BalQty = BalQty + HQty;
                //    if (Qty > BalQty)
                //    {
                //        MessageBox.Show("CurrentQty is greater than BalanceQty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //        AnalView.SetRowCellValue(AnalView.FocusedRowHandle, "Qty", Convert.ToDecimal(clsStatics.IsNullCheck(AnalView.GetFocusedRowCellValue("HiddenQty"), clsStatics.datatypes.vartypenumeric)));

                //        return;

                //    }
                //}

            }
            if (AnalView.GetFocusedRowCellValue("Qty") != DBNull.Value)
            {
                qty = Convert.ToDecimal(AnalView.GetRowCellValue(AnalView.FocusedRowHandle, "Qty"));
                dtAnal.AcceptChanges();
            }

            ////////////
            DataRow[] SelectU = null; ;

            SelectU = dtAnal.Select("REsource_ID =' " + RequestView.GetFocusedRowCellValue("ID").ToString() + "' ");
            foreach (DataRow r in SelectU)
            {
                foreach (DataRow drow in grdAnal.Rows)
                {
                    if (Convert.ToInt32(r["Analysis_ID"]) == Convert.ToInt32(drow["Analysis_ID"]))
                    {

                        r["Qty"] = drow["Qty"];
                    }
                }
            }
            grdAnalysis.RefreshDataSource();
            RequestView.SetRowCellValue(RequestView.FocusedRowHandle, "Quantity", AnalView.Columns["Qty"].SummaryText);
        }


        private void AnalView_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (AnalView.FocusedColumn.FieldName == "WBS") e.Cancel = true;
            int AnalId = Convert.ToInt32(AnalView.GetFocusedRowCellValue("Analysis_ID"));
            PopulateLevelWBSDetails(AnalId, ProjDb);
            int CCId = Convert.ToInt32(cboCC.EditValue);          
            PopulateWBSQty(CCId, ResId,AnalId);
        }

        private void RequestView_ShowingEditor(object sender, CancelEventArgs e)
        {            
            if (RequestView.FocusedColumn.FieldName == "Code") { e.Cancel = true; }
            if (RequestView.FocusedColumn.FieldName == "Description") { e.Cancel = true; }
            if (RequestView.FocusedColumn.FieldName == "Unit") { e.Cancel = true; }

            if (Qtype == "M" || Qtype == "H")
            {
                if (RequestView.FocusedColumn.FieldName == "Quantity")
                {
                    int CCId = Convert.ToInt32(cboCC.EditValue);
                    ResId = Convert.ToInt32(clsStatics.IsNullCheck(RequestView.GetFocusedRowCellValue("ID").ToString(), clsStatics.datatypes.vartypenumeric));
                    if (Qtype != "")
                    {
                        if (CCId > 0 && ResId > 0)
                        {
                            GetStockDetails(CCId, ResId,Qtype);

                        }
                        if (dsAnalysisHead.Tables["Analysis"].Rows.Count > 0 && Convert.ToBoolean(((DataRowView)(cboCC.GetSelectedDataRow())).Row.ItemArray[3].ToString()) == true)
                        {
                            e.Cancel = true;

                            DataView dv = new DataView(dtAnal);
                            dv.RowFilter = String.Format("REsource_ID={0} ", RequestView.GetFocusedRowCellValue("ID"));
                            if (dv.ToTable().Rows.Count == 0)
                            {
                                DataRow dr1;
                                foreach (DataRow dr in dsAnalysisHead.Tables["Analysis"].Rows)
                                {
                                    dr1 = dtAnal.NewRow();
                                    dr1["WBS"] = dr["AnalysisHeadName"];
                                    dr1["Qty"] = "0.00000";
                                    dr1["REsource_ID"] = RequestView.GetFocusedRowCellValue("ID").ToString();
                                    dr1["BrandID"] = 0;
                                    dr1["Analysis_ID"] = dr["AnalysisID"];
                                    dr1["CCID"] = dr["CCID"];
                                    dr1["HiddenQty"] = "0.00000";
                                    dtAnal.Rows.Add(dr1);
                                }
                                if (RequestId > 0)
                                {
                                    if (dtAnalUpdate != null)
                                        if (dtAnalUpdate.Rows.Count > 0)
                                        {
                                            DataRow[] SelectU = null; ;

                                            SelectU = dtAnal.Select("REsource_ID =' " + RequestView.GetFocusedRowCellValue("ID").ToString() + "' ");
                                            foreach (DataRow r in SelectU)
                                            {
                                                foreach (DataRow drow in dtAnalUpdate.Rows)
                                                {
                                                    if (Convert.ToInt32(r["Analysis_ID"]) == Convert.ToInt32(drow["AnalysisId"]) && Convert.ToInt32(r["REsource_ID"]) == Convert.ToInt32(drow["ResourceId"]))
                                                    {

                                                        r["Qty"] = drow["ReqQty"];
                                                        r["HiddenQty"] = drow["ReqQty"];
                                                    }
                                                }
                                            }
                                        }
                                }

                                dv.RowFilter = "REsource_ID=" + RequestView.GetFocusedRowCellValue("ID").ToString() + " ";
                                grdAnal = dv.ToTable();
                                grdAnalysis.DataSource = grdAnal;
                            }
                            else
                            {
                                DataView dvA = new DataView(dtAnal);
                                dvA.RowFilter = "REsource_ID=" + RequestView.GetFocusedRowCellValue("ID").ToString() + " ";
                                grdAnal = dvA.ToTable();
                                grdAnalysis.DataSource = grdAnal;
                            }
                            HideAnalColumns();
                            AnalView.Columns["Qty"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                            AnalView.Columns["Qty"].SummaryItem.DisplayFormat = "{0:N5}";
                            DevExpress.XtraEditors.Repository.RepositoryItemTextEdit txtAnalQty = new RepositoryItemTextEdit();
                            txtAnalQty.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                            txtAnalQty.KeyDown += new KeyEventHandler(txtAnalQty_KeyDown);
                            txtAnalQty.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(txtAnalQty_Spin);
                            txtAnalQty.Mask.EditMask = "N5";
                            AnalView.Columns["Qty"].ColumnEdit = txtAnalQty;
                            AnalView.Columns["WBS"].Width = 77;
                            AnalView.Columns["Qty"].Width = 23;
                            AnalView.Columns["Qty"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                        }
                        else
                        {
                            e.Cancel = false;
                        }
                    }
                    else //WBS Qty for Labour,Activity,IOW,Sub-IOW,Asset
                    {

                    }
                }
            }

        }

        void txtAnalQty_Spin(object sender, DevExpress.XtraEditors.Controls.SpinEventArgs e)
        {
            e.Handled = true;
        }

        void txtAnalQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up | e.KeyCode == Keys.Down)
            {
                e.Handled = true;
            }
        }
           

        private void RequestView_DoubleClick(object sender, EventArgs e)
        {
            if (Qtype == "M")
            {
                if (RequestView.FocusedRowHandle >= 0)
                {
                    frmReqSchedule frm = new frmReqSchedule();
                    if (dsAnalysisHead.Tables["Analysis"].Rows.Count > 0)
                    {
                        if (Convert.ToDecimal(RequestView.GetFocusedRowCellValue("Quantity")) > 0)
                        {
                            dtReqSchedule = frm.Execute(Convert.ToInt32(RequestView.GetFocusedRowCellValue("ID")), RequestView.GetFocusedRowCellValue("Description").ToString(), Convert.ToDecimal(RequestView.GetFocusedRowCellValue("Quantity").ToString()), dtReqSchedule);
                        }
                    }
                    else
                    {
                        if (Convert.ToDecimal(RequestView.GetFocusedRowCellValue("Quantity")) > 0)
                        {
                            dtReqSchedule = frm.Execute(Convert.ToInt32(RequestView.GetFocusedRowCellValue("ID")), RequestView.GetFocusedRowCellValue("Description").ToString(), Convert.ToDecimal(RequestView.GetFocusedRowCellValue("Quantity")), dtReqSchedule);
                        }
                    }
                }
            }
        }

        private void WBSQtyView_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            int AnalId = 0;
            string AnalGrpName = "";
            GridGroupRowInfo info = e.Info as GridGroupRowInfo;
            if (info.Column.Name == "colAnalysisId")
            {
                AnalId = Convert.ToInt32(info.GroupValueText);
                if (AnalId > 0)
                {
                    AnalGrpName = RequestEntryBL.GetWBSGrpName(cboCC.EditValue.ToString(), AnalId);
                    info.GroupText = AnalGrpName;
                    e.Appearance.ForeColor = Color.Blue;
                    e.Appearance.Font = new Font("Verdana", 10, FontStyle.Bold);
                }
            }

            if (info.Column.FieldName == "AnalysisHeadName")
            {
                info.GroupText = info.GroupValueText;
                e.Appearance.ForeColor = Color.Maroon;
                e.Appearance.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }

        private void RequestView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (RequestView.RowCount <= 0) { return; }

            if (Qtype == "M") { return; }

            if (Qtype == "L" || Qtype == "A" || Qtype == "H" || Qtype == "R")
            {
                m_iResourceId = Convert.ToInt32(clsStatics.IsNullCheck(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "ID").ToString(), clsStatics.datatypes.vartypenumeric));
            }
            else
            {
                m_iResourceId = Convert.ToInt32(clsStatics.IsNullCheck(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "IOWID").ToString(), clsStatics.datatypes.vartypenumeric));
            }
            PoupateGrdDetail();
        }

        private void RequestView_RowClick(object sender, RowClickEventArgs e)
        {
            if (RequestView.RowCount <= 0) { return; }
            if (Qtype == "M") { return; }
            if (Qtype == "L" || Qtype == "A" || Qtype == "H" || Qtype == "R")
            {
                m_iResourceId = Convert.ToInt32(clsStatics.IsNullCheck(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "ID").ToString(), clsStatics.datatypes.vartypenumeric));
            }
            else
            {
                m_iResourceId = Convert.ToInt32(clsStatics.IsNullCheck(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "IOWID").ToString(), clsStatics.datatypes.vartypenumeric));
            }
            PoupateGrdDetail();

        }

        #endregion                

        private void RequestView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (Qtype == "M" || Qtype == "H")
            {
                if (RequestView.FocusedRowHandle >= 0)
                {
                    decimal TotQty = 0;
                    if (RequestView.FocusedColumn.FieldName == "Quantity")
                    {
                        RequestView.RefreshData();
                        if (dtAnal.Rows.Count == 0)
                        {
                            // dvProj = new DataView(oProjLink.DtProjLinkDetails) { RowFilter = String.Format("Resource_Id = {0}  ", ResId) };
                            VarAmt = 0;
                            VarAmt = Convert.ToDecimal(StockView.GetRowCellValue(0, "EstimateQty")) * (1 + VariantQty / 100);
                            if (StockView.RowCount > 0)
                            {
                                TotQty = Convert.ToDecimal(clsStatics.IsNullCheck(StockView.GetRowCellValue(0, "BalReqQty"), clsStatics.datatypes.vartypenumeric)) + Convert.ToDecimal(clsStatics.IsNullCheck(StockView.GetRowCellValue(0, "BalIndQty"), clsStatics.datatypes.vartypenumeric)) + Convert.ToDecimal(clsStatics.IsNullCheck(StockView.GetRowCellValue(0, "BalPOQty"), clsStatics.datatypes.vartypenumeric)) + Convert.ToDecimal(clsStatics.IsNullCheck(StockView.GetRowCellValue(0, "TotPurchase"), clsStatics.datatypes.vartypenumeric));
                                //TotQty = Convert.ToDecimal(clsStatics.IsNullCheck(StockView.GetRowCellValue(0, "RequestQty"), clsStatics.datatypes.vartypenumeric));
                            }
                            if (RequestId == 0)
                            {
                                if ((TotQty + Convert.ToDecimal(clsStatics.IsNullCheck(RequestView.GetFocusedRowCellValue("Quantity"), clsStatics.datatypes.vartypenumeric)) > VarAmt))
                                {
                                    MessageBox.Show("Request Qty is greater than EstimateQty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    RequestView.SetRowCellValue(RequestView.FocusedRowHandle, "Quantity", "0.00000");

                                    return;
                                }
                            }
                            else
                            {
                                if (((TotQty + Convert.ToDecimal(clsStatics.IsNullCheck(RequestView.GetFocusedRowCellValue("Quantity"), clsStatics.datatypes.vartypenumeric))) - Convert.ToDecimal(clsStatics.IsNullCheck(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "HiddenQty"), clsStatics.datatypes.vartypenumeric))) > VarAmt)
                                {
                                    MessageBox.Show("Request Qty is greater than EstimateQty!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    RequestView.SetRowCellValue(RequestView.FocusedRowHandle, "Quantity", Convert.ToDecimal(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "HiddenQty")));
                                    return;
                                }
                            }
                        }
                    }
                }

                if (RequestView.GetFocusedRowCellValue("Quantity") != DBNull.Value)
                {
                    castdecimal = Convert.ToDecimal(RequestView.GetRowCellValue(RequestView.FocusedRowHandle, "Quantity"));
                    if (dtRes.Rows.Count>0)
                        dtRes.Rows[RequestView.FocusedRowHandle]["Quantity"] = castdecimal.ToString("N5");
                }
            }
        }  
      
    }

}
