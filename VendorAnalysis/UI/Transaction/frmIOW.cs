using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using VendorAnalysis.BusinessLayer;
using VendorAnalysis.BusinessObjects;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraVerticalGrid.Rows;
using System.Data.SqlClient;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Data;
using DevExpress.XtraEditors;

namespace VendorAnalysis
{
    public partial class frmIOW : Form
    {
        #region Variables

        public int m_ibillPartRate = 0;
        int m_iRevId = 0;
      
        bool m_bDPEItemLbr = false;
        public int m_iWORegId = 0;
        public int m_idpeTrnsRowId = 0;
        public int m_iBillRegId = 0;
        public int m_iDPERegId = 0;
        public double m_dRate = 0;   
      
        string m_sType = "";
        double m_dAmt = 0;
        public DataTable m_dtIOW;
        DataTable m_dtSubIOW;
        DataTable m_dt;
        
      
        DataTable m_dtNewLbr;
       
        DataTable m_dtGetDPEIow;
        DataTable m_dtGetWOIOW;
      
        int m_iResourceId = 0;
        int m_iCCId = 0;
        public int m_iAbsTypeId = 0;
        public decimal m_dRetruntQty = 0;
        public  int m_iAnalysisId = 0;
        string m_sWhereform = "";     
        string m_sfldName = "";
        
        string m_sMode = "";
        double m_dfldValue = 0;
        string m_sSplit = "";
        public string m_sPartSign = "";
        
        int m_iContId = 0;
    
        
        public int m_iMType=0 ;
        
      
        public int m_iIOWRowId = 0;
        public int m_iBRowId = 0;
        public string m_sRetnIOWID = "";
        DataTable m_dtGetWOIOWfrm=new DataTable();
        DataTable m_dtGetDPEIOWfrm;
        DataTable m_dtGetBillIOWfrm;
        public string m_sUpdate = "";
        public int m_iedit = 0;

    
        
        public DataTable m_dtDPEMsrmnt;
        public DataTable m_dtBillMsrmnt;

        int m_iAmendWORegID = 0;      
        

        public DataTable m_dtRtnIOWDPEMsrmnt;
        public DataTable m_dtRtnIOWDPELbr;
        public DataTable m_dtRtnIOWDPE;
        public string m_sBclktype = "";

        public DataTable m_dtfrmMsrmtDPE;

       
        public string m_sClkOption = "";
      
        string m_sLSTypeId = ""; 

      

        public int m_ibEntryLbrCount = 0;

        public DataTable m_dtRtnIOWLbr = new DataTable();
        public DataTable m_dtEModeIOWLbr = new DataTable();

        public bool m_bWorkFlow = false;
        public DevExpress.XtraEditors.PanelControl m_oPanel;

        
        public int m_iMsrType=0;

      

        #endregion

        #region Objects
        //frmLabourPickList oLbrPList;
        ComponentBL oComponentBL;
        
        
        WorkOrderBO oWorkOrderBO;

        //public List<DPEIOWTrans> oDPEIOWTransCol;
        public List<WOIOWTransUpdate> oWOIOWTransCol;       
        //public List<BillIOWTrans> oBillIOWTransCol=new List<BillIOWTrans>();
        //public List<BillIOWTransDPEList> oBillIOWTransDPECol;
        
      

        //public List<BillIOWLabour> oBIOWL = new List<BillIOWLabour>();

        public List<DPEIOWMeasurementBO> oDPEIOWMsrmentBO = new List<DPEIOWMeasurementBO>();
        
        public List<BillIOWMeasurementBO> oBillIOWMsr = new List<BillIOWMeasurementBO>();
     

      
      
        
        IOWBL oIOWBL;

        

        #endregion

        #region Constructor
        public frmIOW()
        {
            InitializeComponent();

            oWOIOWTransCol = new List<WOIOWTransUpdate>();
            
            oWorkOrderBO = new WorkOrderBO();
            

            
            oIOWBL = new IOWBL();


            oComponentBL = new ComponentBL();
            
            
        }
        #endregion
        
        #region Form Event
        
        private void frmIOW_Load(object sender, EventArgs e)
        {
            this.SuspendLayout();
            clsStatics.SetMyGraphics();
         
            cmdIOWQtyClear.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;

            if (oIOWBL.GetProjectDB(m_iCCId) == true)
            {                
                m_sSplit = clsStatics.GetWBSReqd(m_iCCId);
            }
            //RowCreated();
            if (m_sWhereform == "W")
            {
                cmdIOWQtyClear.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                
            }

            if ((m_sWhereform == "D")&&(m_sfldName == "Amount"))
                cmdIOWLbr.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

            if ((m_sWhereform == "B")||(m_sWhereform == "BDPEList"))
                cmdIOWLbr.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;        

            GetDetails();


            this.ResumeLayout();
        }

        #endregion

        #region Functions

        public void Execute2(List<WOIOWTransUpdate> argIOWTransCol, string argWhere, int argRevID, string argMode, int argResourceId, int argCCId, int argContrctId,DataTable dtDPEIOW,double argfldValue,string argfldName,bool argItemLbrCount,DataTable dtBillIOW)
        {
            oWOIOWTransCol = argIOWTransCol;
            m_sWhereform = argWhere;
            m_iRevId = argRevID;
            m_sMode = argMode;
            m_iResourceId = argResourceId;
            m_iCCId = argCCId;
            m_iContId = argContrctId;
            m_dtGetDPEIOWfrm = dtDPEIOW;
            m_sfldName = argfldName;
            m_dfldValue = argfldValue;
            m_bDPEItemLbr = argItemLbrCount;
            m_dtGetBillIOWfrm = dtBillIOW;           

            this.ShowDialog();
        }

        

        private void BindGrid()
        {
            if (m_sWhereform == "D")
            {
                dWIOWLbr.Hide();
                dWIOWDet.Show();

                cmdIOWLbr.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                if (m_dtGetDPEIOWfrm.Rows.Count > 0)
                {
                    if (m_sfldName == "Amount")
                    {
                        cmdIOWLbr.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    }
                    grdIOWTrans.DataSource = null;
                    AddNewEntryIOW();

                    for (int d = 0; d < m_dtGetDPEIOWfrm.Rows.Count;d++ )
                    {
                        DataRow dr = m_dtIOW.NewRow();

                        dr["RowId"] = Convert.ToInt32(m_dtGetDPEIOWfrm.Rows[d]["IOWRowId"].ToString());
                        dr["BillType"] = m_dtGetDPEIOWfrm.Rows[d]["BillType"].ToString();
                        dr["DPEIOWTransId"] = m_dtGetDPEIOWfrm.Rows[d]["DPEIOWTransID"].ToString();
                        dr["DPETransId"] = m_dtGetDPEIOWfrm.Rows[d]["DPETransId"].ToString();
                        dr["IOW_Trans_ID"] = m_dtGetDPEIOWfrm.Rows[d]["IOW_Trans_ID"].ToString();
                        dr["Serial_No"] = m_dtGetDPEIOWfrm.Rows[d]["Serial_No"].ToString();
                        dr["Specification"] = m_dtGetDPEIOWfrm.Rows[d]["Specification"].ToString();
                        dr["Unit"] = m_dtGetDPEIOWfrm.Rows[d]["UOM_ID"].ToString();
                        dr["IOW_ID"] = m_dtGetDPEIOWfrm.Rows[d]["IOW_ID"].ToString();
                        dr["AnalysisHeadID"] = m_dtGetDPEIOWfrm.Rows[d]["AnalysisHeadID"].ToString();

                        if (m_sMode == "E")
                        {
                            if (m_sfldName == "Amount")
                            {
                                dr["Qty"] = Convert.ToDouble(m_dtGetDPEIOWfrm.Rows[d]["Qty"].ToString());// +Convert.ToDouble(dtGetDPEIOWfrm.Rows[d]["PrevQtty"].ToString());
                            }
                            else
                            {
                                dr["Qty"] = Convert.ToDouble(m_dtGetDPEIOWfrm.Rows[d]["Qty"].ToString());
                            }
                        }
                        else
                        {
                            dr["Qty"] = Convert.ToDouble(m_dtGetDPEIOWfrm.Rows[d]["Qty"].ToString());
                        }

                        dr["ClaimType"] = m_dtGetDPEIOWfrm.Rows[d]["ClaimType"].ToString();
                        dr["MUOM_ID"] =  m_dtGetDPEIOWfrm.Rows[d]["UOM_ID"].ToString();
                        dr["UFactor"] =  m_dtGetDPEIOWfrm.Rows[d]["UFactor"].ToString();
                        dr["CumUpdate"] =  m_dtGetDPEIOWfrm.Rows[d]["CumUpdate"].ToString();
                        dr["PrevQtty"] =  m_dtGetDPEIOWfrm.Rows[d]["PrevQtty"].ToString();
                        dr["MType"] =  m_dtGetDPEIOWfrm.Rows[d]["MType"].ToString();
                        dr["BillTransId"] = 0;
                        dr["BillIOWTransId"] = 0;
                        m_dtIOW.Rows.Add(dr);
                    }                

                }
            }
            if (m_sWhereform == "W")
            {
                cmdIOWLbr.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                dWIOWLbr.Hide();
                dWIOWDet.Show();
             
                if (oWOIOWTransCol == null) { return; }

                if (oWOIOWTransCol.Count > 0)
                {
                    grdIOWTrans.DataSource = null;
                    AddNewEntryIOW();

                    foreach (WOIOWTransUpdate obj in oWOIOWTransCol)
                    {
                        DataRow dr = m_dtIOW.NewRow();

                        dr["RowId"] = obj.IOWRowId;
                        dr["AWONo"] = obj.AWONo;
                        dr["AWORegId"] = obj.AWORegId;
                        dr["AmentMent"] = obj.AmentMent;
                        dr["WOTrnsRowId"] = obj.WOTrnsRowId;
                        dr["BillType"] = obj.BillType;
                        dr["IOW_Trans_ID"] = obj.IOW_Trans_ID;
                        dr["Serial_No"] = obj.Serial_No;
                        dr["Specification"] = obj.Specification;
                        dr["Unit"] = obj.UOM_ID;
                        dr["IOW_ID"] = obj.IOW_ID;
                        dr["AnalysisHeadID"] = obj.AnalysisHeadID;
                        dr["Qty"] = obj.Qty;                        
                        dr["CNType"] = obj.CNType;
                        m_dtIOW.Rows.Add(dr);
                    }
                }
               
            }
            if (m_sWhereform == "B" || m_sWhereform == "BDPEList")
            {
                dWIOWLbr.Hide();
                dWIOWDet.Show();
                grdIOWTrans.DataSource = null;
                AddNewEntryIOW();
                cmdIOWLbr.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                for (int d = 0; d < m_dtGetBillIOWfrm.Rows.Count; d++)
                {
                    DataRow dr = m_dtIOW.NewRow();

                    dr["RowId"] = Convert.ToInt32(m_dtGetBillIOWfrm.Rows[d]["IOWRowId"].ToString());
                    dr["DPEIOWTransId"] = 0;
                    dr["DPETransId"] = 0;
                    dr["BillType"] = m_dtGetBillIOWfrm.Rows[d]["BillType"].ToString();
                    dr["IOW_Trans_ID"] = m_dtGetBillIOWfrm.Rows[d]["ItemTransId"].ToString();
                    dr["Serial_No"] = m_dtGetBillIOWfrm.Rows[d]["SerialNo"].ToString();
                    dr["Specification"] = m_dtGetBillIOWfrm.Rows[d]["Specification"].ToString();
                    dr["Unit"] = m_dtGetBillIOWfrm.Rows[d]["MUnitId"].ToString();
                    dr["IOW_ID"] = m_dtGetBillIOWfrm.Rows[d]["IOW_ID"].ToString();
                    dr["AnalysisHeadID"] = m_dtGetBillIOWfrm.Rows[d]["AnalysisHeadID"].ToString();
                    dr["Qty"] = Math.Abs(Convert.ToDouble(m_dtGetBillIOWfrm.Rows[d]["Qty"].ToString()));
                    dr["ClaimType"] = m_dtGetBillIOWfrm.Rows[d]["ClaimType"].ToString();
                    dr["ClkType"] = m_dtGetBillIOWfrm.Rows[d]["ClkType"].ToString();
                    dr["MType"] = m_dtGetBillIOWfrm.Rows[d]["MType"].ToString();
                    dr["MUnitId"] = m_dtGetBillIOWfrm.Rows[d]["MUnitId"].ToString();
                    dr["UFactor"] = m_dtGetBillIOWfrm.Rows[d]["UFactor"].ToString();
                    dr["CumUpdate"] = "";// dtGetBillIOWfrm.Rows[d]["CumUpdate"].ToString();
                    dr["BillTransId"] = m_dtGetBillIOWfrm.Rows[d]["BillTransId"].ToString(); 
                    dr["BillIOWTransId"] = m_dtGetBillIOWfrm.Rows[d]["BillIOWTransId"].ToString();
                    dr["PartRate"] = m_ibillPartRate;
                    dr["Sign"] = m_dtGetBillIOWfrm.Rows[d]["Sign"].ToString();


                    m_dtIOW.Rows.Add(dr);               
                }                
            }
       

            if (m_dtIOW == null) { return; }

            grdIOWTrans.DataSource = m_dtIOW;
            
            RepositoryItemButtonEdit btnQty = new RepositoryItemButtonEdit();
            btnQty.LookAndFeel.SkinName = "Blue";            
            btnQty.LookAndFeel.UseDefaultLookAndFeel = false;
            btnQty.Mask.EditMask = "###############.000";
            btnQty.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            btnQty.Mask.UseMaskAsDisplayFormat = true;
            btnQty.Validating += new CancelEventHandler(btnQty_Validating);
            btnQty.DoubleClick += new EventHandler(btnQty_DoubleClick);
            btnQty.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(btnQty_Spin);
            btnQty.KeyDown += new KeyEventHandler(btnQty_KeyDown);
            

            grdviewIOWTrans.Columns["Qty"].ColumnEdit = btnQty;

            RepositoryItemMemoEdit txtSpecf = new RepositoryItemMemoEdit();            
            txtSpecf.LookAndFeel.UseDefaultLookAndFeel = false;            
            txtSpecf.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Custom;
            txtSpecf.Mask.UseMaskAsDisplayFormat = true;
            txtSpecf.Appearance.Options.UseTextOptions = true;
            txtSpecf.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            txtSpecf.AppearanceReadOnly.Options.UseTextOptions = true;
            txtSpecf.AppearanceReadOnly.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

            grdviewIOWTrans.Columns["Specification"].ColumnEdit = txtSpecf;            


            RepositoryItemLookUpEdit AnalEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();

            DataTable dtAn = new DataTable();

            if (m_sSplit == "Y")
            {
                dtAn = oIOWBL.GetAnalysisTypeHead();
                DataRow dr1;
                DataView dv = new DataView(dtAn);
                dv.RowFilter = "AnalysisID=0";
                if (dv.ToTable().Rows.Count > 0) { }
                else
                {
                    dr1 = dtAn.NewRow();
                    dr1["AnalysisHeadName"] = "All";
                    dr1["AnalysisID"] = 0;
                    dtAn.Rows.InsertAt(dr1, 0);
                }
                if (dtAn.Rows.Count > 0)
                {
                    AnalEdit.DataSource = dtAn;
                    AnalEdit.ForceInitialize();
                    AnalEdit.PopulateColumns();
                    AnalEdit.DisplayMember = "AnalysisHeadName";
                    AnalEdit.ValueMember = "AnalysisID";
                    AnalEdit.Columns["AnalysisID"].Visible = false;
                    AnalEdit.ShowFooter = false;
                    AnalEdit.ShowHeader = false;
                }

                grdviewIOWTrans.Columns["AnalysisHeadID"].Caption = "Analysis Head";
                grdviewIOWTrans.Columns["AnalysisHeadID"].ColumnEdit = AnalEdit;
                grdviewIOWTrans.Columns["AnalysisHeadID"].OptionsColumn.AllowEdit = false;
            }
            else
            {
                grdviewIOWTrans.Columns["AnalysisHeadID"].Visible = false;
            }

            if (m_sWhereform == "W")
            {
                grdviewIOWTrans.Columns["AWONo"].Visible = false;
                grdviewIOWTrans.Columns["AWORegId"].Visible = false;
                grdviewIOWTrans.Columns["AmentMent"].Visible = false;
                grdviewIOWTrans.Columns["WOTrnsRowId"].Visible = false;

                grdviewIOWTrans.Columns["WOTransId"].Visible = false;
                grdviewIOWTrans.Columns["IOW_Trans_ID"].Visible = false;              
                grdviewIOWTrans.Columns["IOW_ID"].Visible = false;
                grdviewIOWTrans.Columns["Unit"].Visible = false;
                grdviewIOWTrans.Columns["CNType"].Visible = false;
                grdviewIOWTrans.Columns["BillType"].Visible = false;
            }
            if (m_sWhereform == "D")
            {
                grdviewIOWTrans.Columns["DPEIOWTransId"].Visible = false;
                grdviewIOWTrans.Columns["DPETransId"].Visible = false;
                grdviewIOWTrans.Columns["IOW_Trans_ID"].Visible = false;               
                grdviewIOWTrans.Columns["IOW_ID"].Visible = false;
                grdviewIOWTrans.Columns["Unit"].Visible = false;
                grdviewIOWTrans.Columns["BillType"].Visible = false;


                grdviewIOWTrans.Columns["ClaimType"].Visible = false;
                grdviewIOWTrans.Columns["MUOM_ID"].Visible = false;
                grdviewIOWTrans.Columns["UFactor"].Visible = false;
                grdviewIOWTrans.Columns["CumUpdate"].Visible = false;
                grdviewIOWTrans.Columns["PrevQtty"].Visible = false;
                grdviewIOWTrans.Columns["CumQty"].Visible = false;
                grdviewIOWTrans.Columns["MType"].Visible = false;
                grdviewIOWTrans.Columns["BillTransId"].Visible = false;
                grdviewIOWTrans.Columns["BillIOWTransId"].Visible = false;
            }
            if ((m_sWhereform == "B") || (m_sWhereform == "BDPEList"))
            {
                grdviewIOWTrans.Columns["PartRate"].Visible = false;
                grdviewIOWTrans.Columns["DPEIOWTransId"].Visible = false;
                grdviewIOWTrans.Columns["DPETransId"].Visible = false;
                grdviewIOWTrans.Columns["IOW_Trans_ID"].Visible = false;                
                grdviewIOWTrans.Columns["IOW_ID"].Visible = false;           
                grdviewIOWTrans.Columns["ClkType"].Visible = false;

                grdviewIOWTrans.Columns["ClaimType"].Visible = false;
                grdviewIOWTrans.Columns["MUnitId"].Visible = false;
                grdviewIOWTrans.Columns["UFactor"].Visible = false;
                grdviewIOWTrans.Columns["CumUpdate"].Visible = false;
                grdviewIOWTrans.Columns["MType"].Visible = false;
                grdviewIOWTrans.Columns["BillTransId"].Visible = false;
                grdviewIOWTrans.Columns["BillIOWTransId"].Visible = false;
                grdviewIOWTrans.Columns["BillType"].Visible = false;

                if (m_sWhereform == "B")
                {
                    grdviewIOWTrans.Columns["Sign"].Width = 20;
                }
                else
                {
                    grdviewIOWTrans.Columns["Sign"].Visible=false;
                }
                
            }

            grdviewIOWTrans.Columns["RowId"].Visible = false;
            grdviewIOWTrans.Columns["Serial_No"].Caption = "Code";
            grdviewIOWTrans.Columns["Qty"].Width = 100;
            grdviewIOWTrans.Columns["Serial_No"].Width = 80;
            grdviewIOWTrans.Columns["Unit"].Width = 50;
        
            if (m_sSplit == "Y")
            {
                if ((m_sWhereform == "B") || (m_sWhereform == "BDPEList")) { grdviewIOWTrans.Columns["Specification"].Width = 245; }
                else { grdviewIOWTrans.Columns["Specification"].Width = 275; }
                grdviewIOWTrans.Columns["AnalysisHeadID"].Width = 100;
            }
            else
            {
                if ((m_sWhereform == "B") || (m_sWhereform == "BDPEList")) { grdviewIOWTrans.Columns["Specification"].Width = 345; }
                else { grdviewIOWTrans.Columns["Specification"].Width = 375; }
            }

            grdviewIOWTrans.Columns["Qty"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            grdviewIOWTrans.Columns["Qty"].DisplayFormat.FormatString = clsStatics.sFormte;

            grdviewIOWTrans.Columns["Qty"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Custom;         
            grdviewIOWTrans.Columns["Qty"].SummaryItem.DisplayFormat = clsStatics.sFormteS;

            if (m_dtIOW.Rows.Count > 0)
            {
                object obj = m_dtIOW.Compute("Sum(Qty)", "");
                m_dRetruntQty = Convert.ToDecimal(obj);
            }

            grdviewIOWTrans.UpdateTotalSummary();            

        }

        void btnQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up | e.KeyCode == Keys.Down)
            {
                e.Handled = true;
            }
        }

        void btnQty_Spin(object sender, DevExpress.XtraEditors.Controls.SpinEventArgs e)
        {
            e.Handled = true;   
        }

        void btnQty_Validating(object sender, CancelEventArgs e)
        {    

            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;

            decimal getQty = 0;
            decimal getQty2 = 0;
            decimal getPreQty = 0;
            decimal previousQty = Convert.ToDecimal(grdviewIOWTrans.GetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Qty"));
            decimal CurQty = Convert.ToDecimal(clsStatics.IsNullCheck(grdviewIOWTrans.GetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Qty"), clsStatics.datatypes.vartypenumeric));

            if ((m_sWhereform == "D") && (m_sfldName == "Qty"))
            {
                getPreQty = Convert.ToDecimal(clsStatics.IsNullCheck(grdviewIOWTrans.GetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "PrevQty"), clsStatics.datatypes.vartypenumeric));
                if (CurQty < Convert.ToDecimal(editor.EditValue))
                {
                    if (BsfGlobal.g_bPowerUser == false)
                    {
                        e.Cancel = true;
                        grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Qty", 0);
                    }
                    else { grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Qty", previousQty); }
                    MessageBox.Show("Cumulative Bill Qty Less Than Previous Bill Qty,Invalid", "Information");
                    //grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Qty", previousQty);
                    editor.Focus();
                    return;
                }
            }

            if (m_sWhereform != "W")
            {
                getQty = Convert.ToDecimal(clsStatics.IsNullCheck(gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "WOQty"), clsStatics.datatypes.vartypenumeric)) - Convert.ToDecimal(clsStatics.IsNullCheck(gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "CBQty"), clsStatics.datatypes.vartypenumeric));

                getQty2 = Convert.ToDecimal(clsStatics.IsNullCheck(gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "EQty"), clsStatics.datatypes.vartypenumeric)) - Convert.ToDecimal(clsStatics.IsNullCheck(gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "TotBQty"), clsStatics.datatypes.vartypenumeric));

                if ((getQty < Convert.ToDecimal(editor.EditValue)) && (getQty2 < Convert.ToDecimal(editor.EditValue)))
                {

                    if (BsfGlobal.g_bPowerUser == false)
                    {
                        e.Cancel = true;
                        grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Qty", 0);
                    }
                    else { grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Qty", previousQty); }

                    if ((m_sWhereform == "D") && (m_sfldName == "Amount"))
                    {
                        MessageBox.Show("DPE Qty Greater than WO Qty");
                        grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Qty", 0);
                    }
                    else
                    {
                        MessageBox.Show("Bill Qty Greater than Current Qty");
                        grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Qty", 0);
                    }
                    editor.Focus();
                    return;
                }
            }
            else
            {
                getQty = Convert.ToDecimal(clsStatics.IsNullCheck(gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "EQty"), clsStatics.datatypes.vartypenumeric)) - Convert.ToDecimal(clsStatics.IsNullCheck(gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "WOQty"), clsStatics.datatypes.vartypenumeric));
                //getQty = Convert.ToDecimal(clsStatics.IsNullCheck(gridView3.GetRowCellValue(gridView3.FocusedRowHandle, "EQty"), clsStatics.datatypes.vartypenumeric));

                if (getQty < Convert.ToDecimal(editor.EditValue) )
                {

                    if (BsfGlobal.g_bPowerUser == false)
                    {
                        e.Cancel = true;
                        grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Qty", 0);
                    }
                    else { grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Qty", previousQty); }

                    MessageBox.Show("Work Order Qty Greater than Estimate Qty");
                    //grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Qty", previousQty);
                    editor.Focus();
                    return;
                }
            }
            grdviewIOWTrans.UpdateTotalSummary(); 
        }

        

        void btnQty_DoubleClick(object sender, EventArgs e)
        {
           
            int AnalysId = 0;
            m_sType = grdviewIOWTrans.GetFocusedRowCellValue("BillType").ToString();
            AnalysId = Convert.ToInt32(clsStatics.IsNullCheck(grdviewIOWTrans.GetFocusedRowCellValue("AnalysisHeadID"), clsStatics.datatypes.vartypenumeric));      
            grdviewIOWTrans.UpdateTotalSummary(); 
        }

        private void GetDetails()
        {
            m_dt = new DataTable();
            m_dtGetDPEIow = new DataTable();
            m_dtGetWOIOW = new DataTable();
            m_dtSubIOW = new DataTable();

            grdIOWTrans.DataSource = null;
            grdviewIOWTrans.Columns.Clear();

            //WorkOrder
            if (m_sWhereform == "W")
            {
                dWIOWLbr.Hide();
                dWIOWDet.Show();

                if (m_sMode == "A" || m_sMode == "E")
                {
                    BindGrid();
                }                
                dWIOWLbr.Hide();
            }
            //DPE
            else if (m_sWhereform == "D")
            {
                dWIOWLbr.Hide();
                dWIOWDet.Show();
                if (m_sMode == "A" || m_sMode == "E")
                {
                    BindGrid();
                }
                dWIOWLbr.Hide();
            }
            //dtBDPEListAIOW  Or Bill 
            else if (m_sWhereform == "B" || m_sWhereform == "BDPEList")
            {
                dWIOWLbr.Hide();
                dWIOWDet.Show();
                cmdIOWLbr.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                if (m_sMode == "A" || m_sMode == "E")
                {
                    BindGrid();
                }
               
                dWIOWLbr.Hide();
            }
                    
        }

        private void RetunnIOWDs()
        {
            for (int r = 0; r < grdviewIOWTrans.RowCount;r++)
            {
                m_sRetnIOWID = m_sRetnIOWID + grdviewIOWTrans.GetRowCellValue(r,"IOW_ID")+",";
            }
        }

        private void getLabourTypeId()
        {
            m_sLSTypeId = "";           
            if (grdViewIOWLbr.RowCount > 0)
            {
                for (int i = 0; i < grdViewIOWLbr.RowCount; i++)
                {
                    m_sLSTypeId = m_sLSTypeId + grdViewIOWLbr.GetRowCellValue(i, "LbrTypeId").ToString() + ",";
                }
            }           
        }

      

        private void AddNewEntryIOW()
        {
            m_dtIOW = new DataTable();

            if (m_sWhereform == "D")
            {
                m_dtIOW.Columns.Add("RowId", typeof(int));
                m_dtIOW.Columns.Add("DPEIOWTransId", typeof(int));
                m_dtIOW.Columns.Add("DPETransId", typeof(int));
                m_dtIOW.Columns.Add("IOW_Trans_ID", typeof(int));
                m_dtIOW.Columns.Add("Serial_No", typeof(string));
                m_dtIOW.Columns.Add("Specification", typeof(string));
                m_dtIOW.Columns.Add("Unit", typeof(string));
                m_dtIOW.Columns.Add("IOW_ID", typeof(string));
                m_dtIOW.Columns.Add("AnalysisHeadID", typeof(int));
                m_dtIOW.Columns.Add("Qty", typeof(double));
                m_dtIOW.Columns.Add("ClaimType", typeof(string));
                m_dtIOW.Columns.Add("MUOM_ID", typeof(string));
                m_dtIOW.Columns.Add("UFactor", typeof(double));
                m_dtIOW.Columns.Add("CumUpdate", typeof(string));
                m_dtIOW.Columns.Add("BillType", typeof(string));              
                m_dtIOW.Columns.Add("PrevQtty", typeof(double));
                m_dtIOW.Columns.Add("CumQty", typeof(double));
                m_dtIOW.Columns.Add("MType", typeof(int));
                m_dtIOW.Columns.Add("BillTransId", typeof(int));
                m_dtIOW.Columns.Add("BillIOWTransId", typeof(int));
            }
            if(m_sWhereform == "W")
            {
                m_dtIOW.Columns.Add("AWONo", typeof(string));
                m_dtIOW.Columns.Add("AWORegId", typeof(int));
                m_dtIOW.Columns.Add("AmentMent", typeof(int));
                m_dtIOW.Columns.Add("WOTrnsRowId", typeof(int));
                m_dtIOW.Columns.Add("RowId", typeof(int));
                m_dtIOW.Columns.Add("WOTransId", typeof(int));
                m_dtIOW.Columns.Add("IOW_Trans_ID", typeof(int));
                m_dtIOW.Columns.Add("Serial_No", typeof(string));
                m_dtIOW.Columns.Add("BillType", typeof(string));
                m_dtIOW.Columns.Add("Specification", typeof(string));
                m_dtIOW.Columns.Add("Unit", typeof(string));
                m_dtIOW.Columns.Add("IOW_ID", typeof(string));
                m_dtIOW.Columns.Add("AnalysisHeadID", typeof(int));
                m_dtIOW.Columns.Add("Qty", typeof(double));
                m_dtIOW.Columns.Add("CNType", typeof(string));             
            }
            if ((m_sWhereform == "B") || (m_sWhereform == "BDPEList"))
            {
                m_dtIOW.Columns.Add("RowId", typeof(int));
                m_dtIOW.Columns.Add("DPEIOWTransId", typeof(int));
                m_dtIOW.Columns.Add("DPETransId", typeof(int));
                m_dtIOW.Columns.Add("IOW_Trans_ID", typeof(int));
                m_dtIOW.Columns.Add("BillType", typeof(string));
                m_dtIOW.Columns.Add("Serial_No", typeof(string));
                m_dtIOW.Columns.Add("Specification", typeof(string));
                m_dtIOW.Columns.Add("Unit", typeof(string));
                m_dtIOW.Columns.Add("IOW_ID", typeof(string));
                m_dtIOW.Columns.Add("AnalysisHeadID", typeof(int));
                m_dtIOW.Columns.Add("Qty", typeof(double));
                m_dtIOW.Columns.Add("CumUpdate", typeof(string));      
                m_dtIOW.Columns.Add("ClaimType", typeof(string));
                m_dtIOW.Columns.Add("ClkType", typeof(string));       
                m_dtIOW.Columns.Add("MType", typeof(int));
                m_dtIOW.Columns.Add("MUnitId", typeof(string));
                m_dtIOW.Columns.Add("BillTransId", typeof(int));
                m_dtIOW.Columns.Add("BillIOWTransId", typeof(int));
                m_dtIOW.Columns.Add("PartRate", typeof(int));
                m_dtIOW.Columns.Add("UFactor", typeof(double));
                m_dtIOW.Columns.Add("Sign", typeof(string));
             
            }
        
        }

    

        void txtEditNumeric_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Qty", editor.EditValue);
            grdviewIOWTrans.UpdateCurrentRow();
        }

        //private void PopulateLbrGrid()
        //{
        //    if (m_dtIOWLbr.Rows.Count > 0)
        //    {
        //        for (int j = 0; j < m_dtIOWLbr.Rows.Count; j++)
        //        {
        //            DataRow dr = m_dtNewLbr.NewRow();

        //            dr["RowId"] = m_iIOWTrnsRowId;
        //            dr["DPEIOWTransID"] = 0;
        //            dr["DPEItemTransID"] = Convert.ToInt32(clsStatics.IsNullCheck(grdviewIOWTrans.GetRowCellValue(m_iIOWTrnsRowId, "IOW_Trans_ID").ToString(), clsStatics.datatypes.vartypenumeric));
        //            dr["AnalsId"] = Convert.ToInt32(clsStatics.IsNullCheck(grdviewIOWTrans.GetRowCellValue(m_iIOWTrnsRowId, "AnalysisHeadID").ToString(), clsStatics.datatypes.vartypenumeric));
        //            dr["DPETransID"] = 0;
        //            dr["IOW_ID"] = Convert.ToInt32(clsStatics.IsNullCheck(grdviewIOWTrans.GetRowCellValue(m_iIOWTrnsRowId, "IOW_ID").ToString(), clsStatics.datatypes.vartypenumeric));
        //            dr["LbrTypeId"] = m_dtIOWLbr.Rows[j]["TypeId"].ToString();                   
        //            dr["LabourTypeName"] = m_dtIOWLbr.Rows[j]["TypeName"].ToString();
        //            dr["Nos"] = 0;
        //            dr["Rate"] = Convert.ToDouble(m_dtIOWLbr.Rows[j]["Rate"].ToString());
        //            dr["Amount"] = 0;
        //            dr["MinRate"] = Convert.ToDouble(m_dtIOWLbr.Rows[j]["MinWage"].ToString());
        //            dr["AppRate"] = Convert.ToDouble(m_dtIOWLbr.Rows[j]["Rate"].ToString());
        //            dr["ClkType"] =m_sWhereform.ToString();

        //            m_dtNewLbr.Rows.Add(dr);
        //        }
        //    }
        //}

        private void AddNewEntryLabourNew()
        {
            m_dtNewLbr = new DataTable();

            m_dtNewLbr.Columns.Add("RowId", typeof(int));
            m_dtNewLbr.Columns.Add("AnalsId", typeof(int));
            m_dtNewLbr.Columns.Add("DPEIOWTransID", typeof(int));
            m_dtNewLbr.Columns.Add("DPEItemTransID", typeof(int));
            m_dtNewLbr.Columns.Add("IOW_ID", typeof(int));
            m_dtNewLbr.Columns.Add("DPETransID", typeof(int));
            m_dtNewLbr.Columns.Add("LbrTypeId", typeof(string));
            m_dtNewLbr.Columns.Add("LbrTransID", typeof(int));
            m_dtNewLbr.Columns.Add("LabourTypeName", typeof(string));
            m_dtNewLbr.Columns.Add("Nos", typeof(int));
            m_dtNewLbr.Columns.Add("Rate", typeof(double));
            m_dtNewLbr.Columns.Add("Amount", typeof(double));
            m_dtNewLbr.Columns.Add("MinRate", typeof(double));
            m_dtNewLbr.Columns.Add("AppRate", typeof(double));
            m_dtNewLbr.Columns.Add("Qty", typeof(double));
            m_dtNewLbr.Columns.Add("ClkType", typeof(string));


            grdIOWLbr.DataSource = m_dtNewLbr;

            grdViewIOWLbr.Columns["Qty"].Visible = false;
            grdViewIOWLbr.Columns["IOW_ID"].Visible = false;
            grdViewIOWLbr.Columns["DPETransID"].Visible = false;
            grdViewIOWLbr.Columns["LbrTypeId"].Visible = false;
            grdViewIOWLbr.Columns["DPEIOWTransID"].Visible = false;
            grdViewIOWLbr.Columns["LbrTransID"].Visible = false;
            grdViewIOWLbr.Columns["RowId"].Visible = false;
            grdViewIOWLbr.Columns["DPEItemTransID"].Visible = false;         

            grdViewIOWLbr.Columns["MinRate"].Visible = false;
            grdViewIOWLbr.Columns["AppRate"].Visible = false;
            grdViewIOWLbr.Columns["ClkType"].Visible = false;
            grdViewIOWLbr.Columns["AnalsId"].Visible = false;
            


            RepositoryItemTextEdit txtEditNosN = new RepositoryItemTextEdit();
            txtEditNosN.LookAndFeel.UseDefaultLookAndFeel = false;
            txtEditNosN.Mask.EditMask = "##################.00";
            txtEditNosN.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtEditNosN.Mask.UseMaskAsDisplayFormat = true;  
            txtEditNosN.Validating += new CancelEventHandler(txtEditNosN_Validating);
            txtEditNosN.KeyDown += new KeyEventHandler(txtEditNosN_KeyDown);
            txtEditNosN.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(txtEditNosN_Spin);

            grdViewIOWLbr.Columns["Nos"].ColumnEdit = txtEditNosN;

            RepositoryItemTextEdit txtEditRateLbrN = new RepositoryItemTextEdit();
            txtEditRateLbrN.LookAndFeel.UseDefaultLookAndFeel = false;
            txtEditRateLbrN.Mask.EditMask = "##################.00";
            txtEditRateLbrN.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtEditRateLbrN.Mask.UseMaskAsDisplayFormat = true; 
            txtEditRateLbrN.Validating += new CancelEventHandler(txtEditRateLbrN_Validating);
            txtEditRateLbrN.KeyDown += new KeyEventHandler(txtEditRateLbrN_KeyDown);
            txtEditRateLbrN.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(txtEditRateLbrN_Spin);
            grdViewIOWLbr.Columns["Rate"].ColumnEdit = txtEditRateLbrN;

            RepositoryItemTextEdit txtEditAmtN = new RepositoryItemTextEdit();
            txtEditAmtN.LookAndFeel.UseDefaultLookAndFeel = false;
            txtEditAmtN.Mask.EditMask = "##################.00";
            txtEditAmtN.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            txtEditAmtN.Mask.UseMaskAsDisplayFormat = true; 
            txtEditAmtN.Validating += new CancelEventHandler(txtEditAmtN_Validating);
            txtEditAmtN.KeyDown += new KeyEventHandler(txtEditAmtN_KeyDown);
            txtEditAmtN.Spin += new DevExpress.XtraEditors.Controls.SpinEventHandler(txtEditAmtN_Spin);

            grdViewIOWLbr.Columns["Amount"].ColumnEdit = txtEditAmtN;


            grdViewIOWLbr.Columns["LabourTypeName"].Width = 400;
            grdViewIOWLbr.Columns["Nos"].Width = 60;
            grdViewIOWLbr.Columns["Rate"].Width = 70;
            grdViewIOWLbr.Columns["Amount"].Width = 90;

            grdViewIOWLbr.Columns["Nos"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            grdViewIOWLbr.Columns["Nos"].SummaryItem.DisplayFormat = clsStatics.sFormteS;

            grdViewIOWLbr.Columns["Rate"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            grdViewIOWLbr.Columns["Rate"].SummaryItem.DisplayFormat = clsStatics.sFormteS;

            grdViewIOWLbr.Columns["Amount"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            grdViewIOWLbr.Columns["Amount"].SummaryItem.DisplayFormat = clsStatics.sFormteS;


            grdViewIOWLbr.Columns["Nos"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            grdViewIOWLbr.Columns["Nos"].DisplayFormat.FormatString = clsStatics.sFormte;

            grdViewIOWLbr.Columns["Rate"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            grdViewIOWLbr.Columns["Rate"].DisplayFormat.FormatString = clsStatics.sFormte;

            grdViewIOWLbr.Columns["Amount"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            grdViewIOWLbr.Columns["Amount"].DisplayFormat.FormatString = clsStatics.sFormte;
        }

        void txtEditRateLbrN_Spin(object sender, DevExpress.XtraEditors.Controls.SpinEventArgs e)
        {
            e.Handled = true;
        }

        void txtEditAmtN_Spin(object sender, DevExpress.XtraEditors.Controls.SpinEventArgs e)
        {
            e.Handled = true;
        }

        void txtEditNosN_Spin(object sender, DevExpress.XtraEditors.Controls.SpinEventArgs e)
        {
            e.Handled = true;
        }

        void txtEditRateLbrN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up | e.KeyCode == Keys.Down)
            {
                e.Handled = true;
            }
        }

        void txtEditAmtN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up | e.KeyCode == Keys.Down)
            {
                e.Handled = true;
            }
        }

        void txtEditNosN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up | e.KeyCode == Keys.Down)
            {
                e.Handled = true;
            }
        }

        void txtEditNosN_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            grdViewIOWLbr.SetRowCellValue(grdViewIOWLbr.FocusedRowHandle, "Nos", editor.EditValue);
            grdViewIOWLbr.UpdateCurrentRow();
        }

        void txtEditAmtN_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            grdViewIOWLbr.SetRowCellValue(grdViewIOWLbr.FocusedRowHandle, "Amount", editor.EditValue);
            grdViewIOWLbr.UpdateCurrentRow();
        }

        void txtEditRateLbrN_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editor = (DevExpress.XtraEditors.TextEdit)sender;
            grdViewIOWLbr.SetRowCellValue(grdViewIOWLbr.FocusedRowHandle, "Rate", editor.EditValue);
            grdViewIOWLbr.UpdateCurrentRow();
            
        }

       

        private void GetAgtType()
        {
            string AgmntType = "";
            AgmntType = oIOWBL.GetAgtType(Convert.ToInt32(grdviewIOWTrans.GetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "IOW_ID").ToString()));
           
        }

        private void PopulateGrdDetail()
        {
            DataTable dtt = new DataTable();
            m_iAmendWORegID = Convert.ToInt32(grdviewIOWTrans.GetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "AWORegId"));
            m_sType = grdviewIOWTrans.GetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "BillType").ToString();            

            int iAheadId = Convert.ToInt32(clsStatics.IsNullCheck(grdviewIOWTrans.GetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "AnalysisHeadID"), clsStatics.datatypes.vartypenumeric));
            int iIOW_ID = Convert.ToInt32(clsStatics.IsNullCheck(grdviewIOWTrans.GetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "IOW_ID"),clsStatics.datatypes.vartypenumeric));

            dtt = oIOWBL.GrdDetailLA(m_sWhereform, m_sType, m_iResourceId, m_iCCId, m_iContId, iIOW_ID, iAheadId, m_iAmendWORegID, m_iRevId, m_sMode, m_ibillPartRate, m_dRate, m_iWORegId, m_iDPERegId, m_iBillRegId, m_sSplit);

            gridControl1.DataSource = dtt;
            gridView3.PopulateColumns();

            gridView3.Columns["EQty"].Caption = "Estimate Qty";
            gridView3.Columns["WOQty"].Caption = "Request Qty";
            gridView3.Columns["CBQty"].Caption = "Contractor Bill Qty";
            gridView3.Columns["TotBQty"].Caption = "Total Bill Qty";

            gridView3.Columns["CBQty"].Visible = false;
            gridView3.Columns["TotBQty"].Visible = false;
            gridView3.Columns["Tot5"].Visible = false;
            gridView3.Columns["Tot6"].Visible = false;

            gridView3.Columns["EQty"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView3.Columns["EQty"].DisplayFormat.FormatString = "f3";

            gridView3.Columns["WOQty"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView3.Columns["WOQty"].DisplayFormat.FormatString = "f3";

            gridView3.Columns["CBQty"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView3.Columns["CBQty"].DisplayFormat.FormatString = "f3";

            gridView3.Columns["TotBQty"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView3.Columns["TotBQty"].DisplayFormat.FormatString = "f3";

        }

        #endregion
       
        #region Button Event

        private void cmdOK_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (grdviewIOWTrans.RowCount > 0)
            {
                m_dRetruntQty = Convert.ToDecimal(clsStatics.IsNullCheck(grdviewIOWTrans.Columns["Qty"].SummaryText.ToString(),clsStatics.datatypes.vartypenumeric));
                if (oDPEIOWMsrmentBO.Count > 0)
                {
                    DataTable dtDPEMsr = new DataTable();
                    dtDPEMsr = clsStatics.GenericListToDataTable(oDPEIOWMsrmentBO);
                    m_iMsrType = Convert.ToInt32(clsStatics.IsNullCheck(dtDPEMsr.Rows[0]["MType"].ToString(), clsStatics.datatypes.vartypenumeric));
                }
                if (oBillIOWMsr.Count > 0)
                {
                    DataTable dtBillMsr = new DataTable();
                    dtBillMsr = clsStatics.GenericListToDataTable(oBillIOWMsr);
                    m_iMsrType = Convert.ToInt32(clsStatics.IsNullCheck(dtBillMsr.Rows[0]["MType"].ToString(), clsStatics.datatypes.vartypenumeric));                    
                }
                m_iAnalysisId = Convert.ToInt32(clsStatics.IsNullCheck(grdviewIOWTrans.GetFocusedRowCellValue("AnalysisHeadID"), clsStatics.datatypes.vartypenumeric));
               

                m_sClkOption = "OK";
            }
            this.Close();
        }

        private void cmdCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (grdviewIOWTrans.RowCount > 0)
            {
                //w_dRetruntQty = Convert.ToDecimal(clsStatics.IsNullCheck(grdviewIOWTrans.Columns["Qty"].SummaryText.ToString(), clsStatics.datatypes.vartypenumeric));
                if (oDPEIOWMsrmentBO.Count > 0)
                {
                    DataTable dtDPEMsr = new DataTable();
                    dtDPEMsr = clsStatics.GenericListToDataTable(oDPEIOWMsrmentBO);
                    m_iMsrType = Convert.ToInt32(clsStatics.IsNullCheck(dtDPEMsr.Rows[0]["MType"].ToString(), clsStatics.datatypes.vartypenumeric));
                }
                if (oBillIOWMsr.Count > 0)
                {
                    DataTable dtBillMsr = new DataTable();
                    
                    dtBillMsr = clsStatics.GenericListToDataTable(oBillIOWMsr);
                    m_iMsrType = Convert.ToInt32(clsStatics.IsNullCheck(dtBillMsr.Rows[0]["MType"].ToString(), clsStatics.datatypes.vartypenumeric));
                }
                m_iAnalysisId = Convert.ToInt32(clsStatics.IsNullCheck(grdviewIOWTrans.GetFocusedRowCellValue("AnalysisHeadID"), clsStatics.datatypes.vartypenumeric));                
                m_sClkOption = "Cancel";
            }
            this.Close();
        }

        private void cmdIOWQtyClear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult reply = MessageBox.Show("Do you want to Clear all Qty to Zero?", "Quantity to Zero", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (reply == DialogResult.Yes)
            {
                if (grdviewIOWTrans.RowCount > 0)
                {
                    for (int m = 0; m < grdviewIOWTrans.RowCount; m++)
                    {
                        grdviewIOWTrans.SetRowCellValue(m, "Qty", 0.000);
                    }
                    grdviewIOWTrans.Columns["Qty"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    grdviewIOWTrans.Columns["Qty"].SummaryItem.DisplayFormat = "{0:N3}";
                }
            }
        }     

        private void btnIOWLbrDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult reply = MessageBox.Show("Do you want Delete?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (reply == DialogResult.Yes)
            {
                if (grdViewIOWLbr.RowCount > 0)
                {
                    grdViewIOWLbr.DeleteRow(grdViewIOWLbr.FocusedRowHandle);
                }
            }
        }


        #region LbrButton Event

        private void cmdIOWLbr_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            

        }

       
        private void btnIOWLbrExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {                
            dWIOWLbr.Hide();
            dWIOWDet.Show();
        }
        private void btnIOWLbrAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {  
        }

        #endregion

        #endregion

        #region Gridview Event
        private void grdviewIOWTrans_ShowingEditor(object sender, CancelEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            m_sType = Convert.ToString(view.GetRowCellValue(view.FocusedRowHandle,"BillType"));
            if (m_sWhereform == "D")
            {
                if (m_sfldName.ToString() == "Qty")
                {
                    if (view.FocusedColumn.FieldName == "Specification" || view.FocusedColumn.FieldName == "Serial_No" || view.FocusedColumn.FieldName == "Unit")
                    {
                        e.Cancel = true;
                    }
                }
                else if (m_sfldName.ToString() == "Amount")
                {
                    if (view.FocusedColumn.FieldName == "Specification" || view.FocusedColumn.FieldName == "Serial_No" || view.FocusedColumn.FieldName=="Unit" )
                    {
                        e.Cancel = true;
                    }               

                }
            }          
            if (m_sWhereform == "W")
            {
                if(m_sfldName.ToString() == "Qty")
                {
                    if (view.FocusedColumn.FieldName == "Specification" || view.FocusedColumn.FieldName == "Serial_No" || view.FocusedColumn.FieldName == "Unit")
                    {
                        e.Cancel = true;
                    }
                }
            }
            if ((m_sWhereform == "B") || (m_sWhereform == "BDPEList"))
            {
                if(m_sfldName.ToString() == "Qty")
                {
                    if (view.FocusedColumn.FieldName == "Specification" || view.FocusedColumn.FieldName == "Serial_No" || view.FocusedColumn.FieldName == "Unit")
                    {
                        e.Cancel = true;
                    }
                }
            }           
        }

        private void grdviewIOWTrans_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            int lCount = 0;
            double Tot = 0;

            if (m_sWhereform == "B" || m_sWhereform == "BDPEList")
            {
                if (((DevExpress.XtraGrid.GridSummaryItem)e.Item).FieldName == "Qty")
                {
                    //double Tot1 = 0;
                    if (((DevExpress.XtraGrid.GridSummaryItem)e.Item).FieldName == "Qty")
                    {
                        if (grdviewIOWTrans.RowCount > 0)
                        {
                            for (lCount = 0; lCount < grdviewIOWTrans.RowCount; lCount++)
                            {
                                if (grdviewIOWTrans.GetRowCellValue(lCount, "Qty") != DBNull.Value)
                                {
                                    if (m_sWhereform == "B")
                                    {
                                        if (grdviewIOWTrans.GetRowCellValue(lCount, "Sign").ToString() == "+" && grdviewIOWTrans.GetRowCellValue(lCount, "Sign") != DBNull.Value)
                                            Tot = (Tot + Convert.ToDouble(grdviewIOWTrans.GetRowCellValue(lCount, "Qty")));
                                        else if (grdviewIOWTrans.GetRowCellValue(lCount, "Sign").ToString() == "-")
                                            Tot = (Tot - Convert.ToDouble(grdviewIOWTrans.GetRowCellValue(lCount, "Qty")));
                                    }
                                    else
                                    {
                                        Tot = (Tot + Convert.ToDouble(grdviewIOWTrans.GetRowCellValue(lCount, "Qty")));                                        
                                    }
                                }                                
                            }
                            if (Tot.ToString().Contains("-") && (m_ibillPartRate == 1))
                                m_sPartSign = Tot.ToString().Substring(0, 1);

                            e.TotalValue = Math.Abs(Tot).ToString();
                        }
                    }
                }
            }
            else
            {
                if (((DevExpress.XtraGrid.GridSummaryItem)e.Item).FieldName == "Qty")
                {
                    if (grdviewIOWTrans.RowCount > 0)
                    {
                        for (lCount = 0; lCount < grdviewIOWTrans.RowCount; lCount++)
                        {
                            if (grdviewIOWTrans.GetRowCellValue(lCount, "Qty") != DBNull.Value)
                            {
                                Tot = (Tot + Convert.ToDouble(grdviewIOWTrans.GetRowCellValue(lCount, "Qty")));

                            }
                        }
                        e.TotalValue = Tot;
                    }
                }
            }
            
        }        

        private void grdviewIOWTrans_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (grdviewIOWTrans.UpdateCurrentRow())
                grdviewIOWTrans.UpdateTotalSummary();
        }


        private double CalculateQty()
        {            
            int lCount = 0;
            double Tot = 0;

            if ((grdviewIOWTrans.FocusedColumn.FieldName == "Qty") && (m_sWhereform == "B" || m_sWhereform == "BDPEList"))
            {
                //double Tot1 = 0;
                if (grdviewIOWTrans.FocusedColumn.FieldName == "Qty")
                {
                    if (grdviewIOWTrans.RowCount > 0)
                    {
                        for (lCount = 0; lCount < grdviewIOWTrans.RowCount; lCount++)
                        {
                            if (grdviewIOWTrans.GetRowCellValue(lCount, "Qty") != DBNull.Value)
                            {
                                if (grdviewIOWTrans.GetRowCellValue(lCount, "Sign").ToString() == "+" && grdviewIOWTrans.GetRowCellValue(lCount, "Sign") != DBNull.Value)
                                    Tot = (Tot + Convert.ToDouble(grdviewIOWTrans.GetRowCellValue(lCount, "Qty")));
                                else if (grdviewIOWTrans.GetRowCellValue(lCount, "Sign").ToString() == "-")
                                    Tot = (Tot - Convert.ToDouble(grdviewIOWTrans.GetRowCellValue(lCount, "Qty")));

                            }                            
                        }
                    }
                }
            }
            else
            {
                if (grdviewIOWTrans.FocusedColumn.FieldName == "Qty")
                {
                    if (grdviewIOWTrans.RowCount > 0)
                    {
                        for (lCount = 0; lCount < grdviewIOWTrans.RowCount; lCount++)
                        {
                            if (grdviewIOWTrans.GetRowCellValue(lCount, "Qty") != DBNull.Value)
                            {
                                Tot = (Tot + Convert.ToDouble(grdviewIOWTrans.GetRowCellValue(lCount, "Qty")));

                            }
                        }                        
                    }
                }
            }
            return Tot;
        }

        private void grdviewIOWTrans_ShownEditor(object sender, EventArgs e)
        {
            if (m_sWhereform == "B" || m_sWhereform == "BDPEList")
            {
                decimal curQty = 0;
                decimal sumQty = 0;
                decimal cBQty= 0;
                decimal tot5 = 0;
                decimal tot6 = 0;

                if (grdviewIOWTrans.RowCount > 0)
                {

                    if (grdviewIOWTrans.FocusedColumn.FieldName == "Sign")
                    {
                        curQty = Convert.ToDecimal(clsStatics.IsNullCheck(grdviewIOWTrans.GetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Qty"), clsStatics.datatypes.vartypenumeric));
                        sumQty = Convert.ToDecimal(clsStatics.IsNullCheck(grdviewIOWTrans.Columns["Qty"].SummaryText.ToString(), clsStatics.datatypes.vartypenumeric));

                        for (int k = 0; k < gridView3.RowCount; k++)
                        {
                            cBQty = Convert.ToDecimal(clsStatics.IsNullCheck(gridView3.GetRowCellValue(k, "CBQty"), clsStatics.datatypes.vartypenumeric));
                            tot5 = Convert.ToDecimal(clsStatics.IsNullCheck(gridView3.GetRowCellValue(k, "Tot5"), clsStatics.datatypes.vartypenumeric));
                            tot6 = Convert.ToDecimal(clsStatics.IsNullCheck(gridView3.GetRowCellValue(k, "Tot6"), clsStatics.datatypes.vartypenumeric));
                        }

                        if (grdviewIOWTrans.GetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Sign").ToString() == "+")
                        {
                            if (m_sMode == "A")
                            {
                                if (m_ibillPartRate == 1)
                                {
                                    if (curQty > tot6)
                                    {
                                        MessageBox.Show("Qty Greater than Previous Qty,Invalid", "Information");
                                        return;
                                    }
                                    grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Sign", "-");
                                }
                                else
                                {
                                    if (curQty > cBQty)
                                    {
                                        MessageBox.Show("Qty Greater than Previous Qty,Invalid", "Information");
                                        return;
                                    }
                                    grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Sign", "-");
                                }
                            }
                            else
                            {
                                if (m_ibillPartRate == 1)
                                {
                                    if (curQty > (sumQty - tot5)) //sumQty-sumery grid 5 th column
                                    {
                                        MessageBox.Show("Qty Greater than Previous Qty,Invalid", "Information");
                                        return;
                                    }
                                    grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Sign", "-");
                                }
                                else
                                {
                                    if (curQty > (cBQty - tot5)) //cBQty-sumery grid 5 th column
                                    {
                                        MessageBox.Show("Qty Greater than Previous Qty,Invalid", "Information");
                                        return;
                                    }
                                    grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Sign", "-");
                                }
                            }
                        }
                        else
                        {
                            grdviewIOWTrans.SetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "Sign", "+");
                        }                      

                        grdviewIOWTrans.UpdateTotalSummary();
                    }
                    
                }
            }
           
        }

        

        private void grdviewIOWTrans_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view.RowCount > 0)
            {
                GetAgtType();
                DataTable dtPutDetails = new DataTable();

                m_sType = Convert.ToString(view.GetRowCellValue(view.FocusedRowHandle, "BillType"));                
                int AHeadId = Convert.ToInt32(clsStatics.IsNullCheck(view.GetRowCellValue(view.FocusedRowHandle, "AnalysisHeadID").ToString(), clsStatics.datatypes.vartypenumeric));
                PopulateLevelWBSDetails(AHeadId, clsStatics.g_sProjWPMDBName);

                PopulateGrdDetail();

                //dtPutDetails = oBillBL.PutIOWDetails(Convert.ToInt32(grdviewIOWTrans.GetRowCellValue(grdviewIOWTrans.FocusedRowHandle, "IOW_ID").ToString()));

            }
            //b_Ans = false;
        }
        private void PopulateLevelWBSDetails(int argAnalId, string argProjDb)
        {

            DataTable dtT = new DataTable();
            dtT = oIOWBL.GetAnalTree(argAnalId, argProjDb);
            if (dtT.Rows.Count == 0) { return; }
            tvLevel.RootValue = "WBS Analysis";
            tvLevel.ParentFieldName = "ParentID";
            tvLevel.KeyFieldName = "AnalysisID";
            tvLevel.DataSource = dtT;
            tvLevel.Columns["AnalysisHeadName"].Visible = true;
            tvLevel.Columns["LevelNo"].Visible = false;
            tvLevel.Columns["LastLevel"].Visible = false;
        }
        private void grdviewIOWTrans_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {           
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view.RowCount > 0)
            {
                GetAgtType();
                DataTable dtPutDetails = new DataTable();

               
                m_sType = Convert.ToString(view.GetRowCellValue(view.FocusedRowHandle, "BillType"));
                int AHeadId = Convert.ToInt32(clsStatics.IsNullCheck(view.GetRowCellValue(view.FocusedRowHandle, "AnalysisHeadID").ToString(), clsStatics.datatypes.vartypenumeric));
                PopulateLevelWBSDetails(AHeadId, clsStatics.g_sProjWPMDBName);

                PopulateGrdDetail();      

            }
        }


        #region Lbr Gridview Event

        private void grdViewIOWLbr_ShowingEditor(object sender, CancelEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if((view.FocusedColumn.FieldName == "LabourTypeName") || (view.FocusedColumn.FieldName == "TypeName"))
            {
                e.Cancel = true;
            }
        }

        private void grdViewIOWLbr_ShownEditor(object sender, EventArgs e)
        {
            //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            //if((view.FocusedColumn.FieldName == "Nos") || (view.FocusedColumn.FieldName == "Qty")) 
            //{
            //    if (view.GetFocusedValue() != null && view.GetFocusedValue() != DBNull.Value)
            //    {
            //        if(i_sMode=="E")
            //            l_dAmt = Convert.ToInt32(view.GetRowCellValue(view.FocusedRowHandle, "Qty").ToString()) * Convert.ToDouble(view.GetRowCellValue(view.FocusedRowHandle, "Rate").ToString());
            //        else
            //            l_dAmt = Convert.ToInt32(view.GetRowCellValue(view.FocusedRowHandle, "Nos").ToString()) * Convert.ToDouble(view.GetRowCellValue(view.FocusedRowHandle, "Rate").ToString());

            //        view.SetRowCellValue(view.FocusedRowHandle, "Amount", l_dAmt);
            //    }
            //}
            //if (view.FocusedColumn.FieldName == "Amount")
            //{
            //    if (view.GetFocusedValue() != null && view.GetFocusedValue() != DBNull.Value)
            //    {
            //        if (i_sMode == "E")
            //            l_dAmt = Convert.ToInt32(view.GetRowCellValue(view.FocusedRowHandle, "Amount").ToString()) / Convert.ToDouble(view.GetRowCellValue(view.FocusedRowHandle, "Qty").ToString());
            //        else
            //            l_dAmt = Convert.ToInt32(view.GetRowCellValue(view.FocusedRowHandle, "Amount").ToString()) / Convert.ToDouble(view.GetRowCellValue(view.FocusedRowHandle, "Nos").ToString());                   

            //        view.SetRowCellValue(view.FocusedRowHandle, "Rate", l_dAmt);
            //    }
            //}
        }     

        private void grdViewIOWLbr_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view.FocusedColumn.FieldName == "Nos")
            {
                if (view.GetFocusedValue() != null && view.GetFocusedValue() != DBNull.Value)
                {
                    m_dAmt = Convert.ToInt32(view.GetRowCellValue(view.FocusedRowHandle, "Nos").ToString()) * Convert.ToDouble(view.GetRowCellValue(view.FocusedRowHandle, "Rate").ToString());    

                    view.SetRowCellValue(view.FocusedRowHandle, "Amount", m_dAmt);
                }
            }
            if (view.FocusedColumn.FieldName == "Rate")
            {
                if (view.GetFocusedValue() != null && view.GetFocusedValue() != DBNull.Value)
                {
                    m_dAmt = Convert.ToInt32(view.GetRowCellValue(view.FocusedRowHandle, "Nos").ToString()) * Convert.ToDouble(view.GetRowCellValue(view.FocusedRowHandle, "Rate").ToString());

                    view.SetRowCellValue(view.FocusedRowHandle, "Amount", m_dAmt);
                }
            }
            if (view.FocusedColumn.FieldName == "Amount")
            {
                if (view.GetFocusedValue() != null && view.GetFocusedValue() != DBNull.Value)
                {
                    m_dAmt = Convert.ToInt32(view.GetRowCellValue(view.FocusedRowHandle, "Amount").ToString()) / Convert.ToDouble(view.GetRowCellValue(view.FocusedRowHandle, "Nos").ToString());

                    view.SetRowCellValue(view.FocusedRowHandle, "Rate", m_dAmt);
                }
            }
        }

        #endregion

        #endregion

        #region Edit Function
      

    

        #endregion

      
    }
}
