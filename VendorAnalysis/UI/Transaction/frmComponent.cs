using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using VendorAnalysis.BusinessLayer;
using VendorAnalysis.BusinessObjects;
using DevExpress.XtraEditors.Repository;


namespace VendorAnalysis
{
    public partial class frmComponent : Form
    {

        #region Variables
        string m_woTypeList = "";
        DataView m_dvBindview;
        DataTable m_dtBindata;
        DataSet m_dtGetComponent = new DataSet();
        DataTable m_dtgetTypeDetails =  new DataTable();
        DataTable m_dtReturn;
        DataTable m_dtCompEntry;
        DataRow m_row;      
        DataTable m_dtGetIOW;
        DataTable m_dtGetSubIOW;
        
        DataTable m_dtPutIOWDetails;
        double m_dIOWRate = 0;
        int m_iContractId = 0;
        int m_iCoscenreId = 0;        
        int m_iIOWId = 0;
        int m_iAnlysId = 0;       
        double m_Dqtty = 0;
        string m_sWhereForm = "";
        int m_iRevId = 0;
        int m_iWoRegId = 0;

        string m_sResId = "";
        //string m_sServiceId = "";
        //string m_sIOWId = "";
        string m_sAHId = "";
        string m_sRowId = "";

        public string m_sMUpdate="";             

        
     

        #endregion

        #region Objects
        //WorkOrderBL oWorkOrderBL;
        ComponentBL oComponentBL;
        List<WorkOrderComponentLists> oWorkOrderComponentListsCol;
        #endregion

        #region Constructor
     
        public frmComponent()
        {
            InitializeComponent();
            oWorkOrderComponentListsCol = new List<WorkOrderComponentLists>();

            oComponentBL = new ComponentBL();
        }

        #endregion

        #region Form Event
        private void frmComponent_Load(object sender, EventArgs e)
        {            
            this.SuspendLayout();

            m_dtGetComponent = oComponentBL.GetComponentDetails(m_woTypeList, m_iRevId, m_iWoRegId);         

            DefaultTabHide();            
            BindGrid();
            this.ResumeLayout();
        }      
        #endregion

        #region Button Event


        private void btnOK_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_dtCompEntry = new DataTable();
            //Labour
            if (grdviewLabour.RowCount > 0)
            {
                grdviewLabour.FocusedRowHandle = grdviewLabour.FocusedRowHandle + 1;

                m_dtReturn = new DataTable();
                DataView dvData = new DataView(grdLabour.DataSource as DataTable);
                dvData.RowFilter = "Select = '" + true + "'";
                m_dtReturn = dvData.ToTable();
                for (int i = 0; i < m_dtReturn.Rows.Count; i++)
                {
                    if (m_sWhereForm == "D")
                    {
                        m_Dqtty = Convert.ToDouble(oComponentBL.getCumQty(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()), m_iContractId, m_iAnlysId, "L"));
                        m_dIOWRate = oComponentBL.GetWOCompRate(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()), m_iContractId, "L", m_sWhereForm, m_iWoRegId);

                        if (m_Dqtty!=0)
                        {
                            m_Dqtty = 0;
                        }
                        else
                        {
                            m_Dqtty = 0;
                        }
                        if (m_dIOWRate != 0)
                        {
                        }
                        else
                        {
                            m_dIOWRate = Convert.ToDouble(m_dtReturn.Rows[i]["Qualified"].ToString());
                        }
                        //dIOWRate = 0;
                    }
                    else
                    {
                        if (m_sWhereForm == "B" || m_sWhereForm == "BDPEList")
                        {
                            m_Dqtty = 0;
                        }

                        m_Dqtty = oComponentBL.getWOCompQty(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()), "L");

                        m_dIOWRate = oComponentBL.GetWOCompRate(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()), m_iContractId, "L", m_sWhereForm, m_iWoRegId);

                        if (Convert.ToDouble(m_dtReturn.Rows[i]["Qty"].ToString()) > m_Dqtty)
                        {
                            m_Dqtty = Convert.ToDouble(m_dtReturn.Rows[i]["Qty"].ToString()) - m_Dqtty;
                        }
                        else
                        {
                            m_Dqtty = 0;
                        }
                   
                        if (m_dIOWRate != 0)
                        {
                            
                        }
                        else
                        {
                            m_dIOWRate = Convert.ToDouble(m_dtReturn.Rows[i]["Qualified_Rate"].ToString());
                        }   
                    }
                    
                    
                    oWorkOrderComponentListsCol.Add(new WorkOrderComponentLists()
                    {
                        RowId = Convert.ToInt32(m_dtReturn.Rows[i]["RowId"].ToString()),
                        ResourceCode = m_dtReturn.Rows[i]["Resource_Code"].ToString(),
                        ResourceId = Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()),
                        IOW_ID = 0,                        
                        ResourceName = m_dtReturn.Rows[i]["Resource_Name"].ToString(),
                        Unit = m_dtReturn.Rows[i]["Unit_Name"].ToString(),
                        UnitId = Convert.ToInt32(m_dtReturn.Rows[i]["Unit_Id"].ToString()),
                        Qty = m_Dqtty,
                        NRate = m_dIOWRate,
                        DCRate=m_dIOWRate,
                        Type = "L",
                        TypeId = Convert.ToInt32(m_dtReturn.Rows[i]["TypeId"].ToString()),
                        AnalysisHeadId = 0,
                        MType = 0,
                        CumQty = 0,
                        MUnitID = "",
                        MUpdate = "",
                        UFactor = 0,
                        PartRate=0

                    });
                }
            }
            //Activiy
            if (grdviewActivity.RowCount > 0)
            {
                grdviewActivity.FocusedRowHandle = grdviewActivity.FocusedRowHandle + 1;
                m_dtReturn = new DataTable();

                DataView dvData = new DataView(grdActivity.DataSource as DataTable);
                dvData.RowFilter = "Select = '" + true + "'";
                m_dtReturn = dvData.ToTable();
                for (int i = 0; i < m_dtReturn.Rows.Count; i++)
                {
                    if (m_sWhereForm == "D")
                    {
                        m_Dqtty = Convert.ToDouble(oComponentBL.getCumQty(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()), m_iContractId, m_iAnlysId, "A"));
                        m_dIOWRate = oComponentBL.GetWOCompRate(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()), m_iContractId, "A", m_sWhereForm, m_iWoRegId);
                        if (m_dIOWRate != 0)
                        {
                        }
                        else
                        {
                            //dIOWRate = Convert.ToDouble(m_tReturn.Rows[i]["Qualified_Rate"].ToString());
                        }
                       // dIOWRate = 0;
                    }
                    else
                    {
                        if (m_sWhereForm == "B" || m_sWhereForm == "BDPEList")
                        {
                            m_Dqtty = 0;
                        }
                        m_Dqtty = oComponentBL.getWOCompQty(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()), "A");

                        m_dIOWRate = oComponentBL.GetWOCompRate(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()), m_iContractId, "A", m_sWhereForm, m_iWoRegId);

                        if (Convert.ToDouble(m_dtReturn.Rows[i]["Qty"].ToString()) > m_Dqtty)
                        {
                            m_Dqtty = Convert.ToDouble(m_dtReturn.Rows[i]["Qty"].ToString()) - m_Dqtty;
                        }
                        else
                        {
                            m_Dqtty = 0;
                        }                        
                        if (m_dIOWRate != 0)
                        {
                            
                        }
                        else
                        {
                            m_dIOWRate = Convert.ToDouble(m_dtReturn.Rows[i]["Qualified_Rate"].ToString());
                        }
                    }
                    oWorkOrderComponentListsCol.Add(new WorkOrderComponentLists()
                    {
                        RowId = Convert.ToInt32(m_dtReturn.Rows[i]["RowId"].ToString()),
                        ResourceCode = m_dtReturn.Rows[i]["Resource_Code"].ToString(),
                        ResourceId = Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()),
                        IOW_ID = 0,
                        ResourceName = m_dtReturn.Rows[i]["Resource_Name"].ToString(),
                        Unit = m_dtReturn.Rows[i]["Unit_Name"].ToString(),
                        UnitId = Convert.ToInt32(m_dtReturn.Rows[i]["Unit_Id"].ToString()),
                        Qty = m_Dqtty,
                        NRate = m_dIOWRate,
                        DCRate = m_dIOWRate,
                        Type = "A",
                        TypeId = Convert.ToInt32(m_dtReturn.Rows[i]["TypeId"].ToString()),
                        AnalysisHeadId = 0,
                        MType = 0,
                        CumQty = m_Dqtty,
                        MUnitID = "",
                        MUpdate = "",
                        UFactor = 0,
                        PartRate = 0
                    });
                }
            }
            //Asset
            if (grdviewHire.RowCount > 0)
            {
                grdviewHire.FocusedRowHandle = grdviewHire.FocusedRowHandle + 1;
                m_dtReturn = new DataTable();

                DataView dvData = new DataView(grdHire.DataSource as DataTable);
                dvData.RowFilter = "Select = '" + true + "'";
                m_dtReturn = dvData.ToTable();
                for (int i = 0; i < m_dtReturn.Rows.Count; i++)
                {
                    if (m_sWhereForm == "D")
                    {
                        m_Dqtty = Convert.ToDouble(oComponentBL.getCumQty(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()), m_iContractId, m_iAnlysId, "A"));
                        m_dIOWRate = oComponentBL.GetWOCompRate(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()), m_iContractId, "A", m_sWhereForm, m_iWoRegId);
                        if (m_dIOWRate != 0)
                        {
                        }
                        else
                        {
                            //dIOWRate = Convert.ToDouble(m_tReturn.Rows[i]["Qualified_Rate"].ToString());
                        }
                        //dIOWRate = 0;
                    }
                    else
                    {
                        if (m_sWhereForm == "B" || m_sWhereForm == "BDPEList")
                        {
                            m_Dqtty = 0;
                        }
                        m_Dqtty = oComponentBL.getWOCompQty(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()), "A");

                        m_dIOWRate = oComponentBL.GetWOCompRate(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()), m_iContractId, "A", m_sWhereForm, m_iWoRegId);

                        if (Convert.ToDouble(m_dtReturn.Rows[i]["Qty"].ToString()) > m_Dqtty)
                        {
                            m_Dqtty = Convert.ToDouble(m_dtReturn.Rows[i]["Qty"].ToString()) - m_Dqtty;
                        }
                        else
                        {
                            m_Dqtty = 0;
                        }
                        if (m_dIOWRate != 0)
                        {
                            
                        }
                        else
                        {
                            m_dIOWRate = Convert.ToDouble(m_dtReturn.Rows[i]["Qualified_Rate"].ToString());
                        }
                    }
                    oWorkOrderComponentListsCol.Add(new WorkOrderComponentLists()
                    {
                        RowId = Convert.ToInt32(m_dtReturn.Rows[i]["RowId"].ToString()),
                        ResourceCode = m_dtReturn.Rows[i]["Resource_Code"].ToString(),
                        ResourceId = Convert.ToInt32(m_dtReturn.Rows[i]["Resource_Id"].ToString()),
                        IOW_ID = 0,
                        ResourceName = m_dtReturn.Rows[i]["Resource_Name"].ToString(),
                        Unit = m_dtReturn.Rows[i]["Unit_Name"].ToString(),
                        UnitId = Convert.ToInt32(m_dtReturn.Rows[i]["Unit_Id"].ToString()),
                        Qty = m_Dqtty,
                        NRate = m_dIOWRate,
                        DCRate = m_dIOWRate,
                        Type = "H",
                        TypeId = Convert.ToInt32(m_dtReturn.Rows[i]["TypeId"].ToString()),
                        AnalysisHeadId = 0,
                        MType = 0,
                        CumQty = m_Dqtty,
                        MUnitID = "",
                        MUpdate = "",
                        UFactor = 0,
                        PartRate = 0
                    });
                }
            }
            //Service
            if (grdviewService.RowCount > 0)
            {
                grdviewService.FocusedRowHandle = grdviewService.FocusedRowHandle + 1;
                m_dtReturn = new DataTable();

                DataView dvData = new DataView(grdService.DataSource as DataTable);
                dvData.RowFilter = "Select = '" + true + "'";
                m_dtReturn = dvData.ToTable();
                for (int i = 0; i < m_dtReturn.Rows.Count; i++)
                {
                    //Dqtty = oComponentBL.getWOCompQty(c_iCoscenreId, Convert.ToInt32(m_tReturn.Rows[i]["Resource_Id"].ToString()), "A");
                    //if (Convert.ToDouble(m_tReturn.Rows[i]["Qtty"].ToString()) > Dqtty)
                    //{
                    //    Dqtty = Convert.ToDouble(m_tReturn.Rows[i]["Qtty"].ToString()) - Dqtty;
                    //}
                    //else
                    //{
                    //    Dqtty = 0;
                    //}
                    oWorkOrderComponentListsCol.Add(new WorkOrderComponentLists()
                    {
                        RowId = Convert.ToInt32(m_dtReturn.Rows[i]["RowId"].ToString()),
                        ResourceCode = m_dtReturn.Rows[i]["ServiceCode"].ToString(),
                        ResourceId = Convert.ToInt32(m_dtReturn.Rows[i]["ServiceId"].ToString()),
                        IOW_ID = 0,
                        ResourceName = m_dtReturn.Rows[i]["ServiceName"].ToString(),
                        Unit = m_dtReturn.Rows[i]["Unit"].ToString(),
                        UnitId = Convert.ToInt32(m_dtReturn.Rows[i]["Unit_Id"].ToString()),
                        Qty = 0,
                        NRate = 0,
                        DCRate = m_dIOWRate,
                        Type = "R",
                        TypeId = 0,
                        AnalysisHeadId = 0,
                        MType = 0,
                        CumQty = 0,
                        MUnitID = "",
                        MUpdate = "",
                        UFactor = 0,
                        PartRate = 0
                    });
                }
            }

            //IOW-I
            if (grdviewSubCntIow.RowCount > 0)
            {
                grdviewSubCntIow.FocusedRowHandle = grdviewSubCntIow.FocusedRowHandle + 1;
                m_dtReturn = new DataTable();

                DataView dvData = new DataView(grdSubCntIow.DataSource as DataTable);
                dvData.RowFilter = "Select = '" + true + "'";
                m_dtReturn = dvData.ToTable();

                if ((m_sWhereForm == "W") || (m_sWhereForm == "B") || (m_sWhereForm == "BDPEList"))
                {
                    if (m_sWhereForm == "B" || m_sWhereForm == "BDPEList")
                    {
                        m_Dqtty = 0;
                    }
                    for (int i = 0; i < m_dtReturn.Rows.Count; i++)
                    {
                        m_dIOWRate = oComponentBL.GetWOIOWRate(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["PIOWID"].ToString()), m_iContractId,m_iWoRegId, m_sWhereForm);
                        if (m_dIOWRate != 0)
                        {
                            
                        }
                        else
                        {
                            m_dIOWRate = Convert.ToDouble(m_dtReturn.Rows[i]["Unit_Rate"].ToString());
                        }
                        if (m_sWhereForm == "W") 
                        {
                            m_Dqtty = Convert.ToDouble(oComponentBL.getWOIOWQty(Convert.ToInt32(m_dtReturn.Rows[i]["PIOWID"].ToString()), Convert.ToInt32(m_dtReturn.Rows[i]["AnalysisID"].ToString()), "I", m_iCoscenreId));
                            if (Convert.ToDouble(m_dtReturn.Rows[i]["Qty"].ToString()) > m_Dqtty)
                            {
                                m_Dqtty = Convert.ToDouble(m_dtReturn.Rows[i]["Qty"].ToString()) - m_Dqtty;
                            }
                            else
                            {
                                m_Dqtty = 0;
                            }
                        }

                        oWorkOrderComponentListsCol.Add(new WorkOrderComponentLists()
                        {
                            RowId = Convert.ToInt32(m_dtReturn.Rows[i]["RowId"].ToString()),
                            ResourceCode = m_dtReturn.Rows[i]["RefSerialNo"].ToString(),
                            ResourceId = 0,
                            IOW_ID = Convert.ToInt32(m_dtReturn.Rows[i]["PIOWID"].ToString()),
                            ResourceName = m_dtReturn.Rows[i]["Specification"].ToString(),
                            Unit = m_dtReturn.Rows[i]["Unit"].ToString(),
                            UnitId = Convert.ToInt32(m_dtReturn.Rows[i]["Unit_Id"].ToString()),
                            Qty = m_Dqtty,
                            NRate = m_dIOWRate,                           
                            Type = "I",
                            TypeId = 0,
                            AnalysisHeadId = Convert.ToInt32(clsStatics.IsNullCheck(m_dtReturn.Rows[i]["AnalysisId"].ToString(),clsStatics.datatypes.vartypenumeric)),
                            MType = 0,
                            CumQty = 0,
                            MUnitID = "",
                            MUpdate = "",
                            UFactor = 0,
                            PartRate = 0
                        });
                    }
                }
                if (m_sWhereForm == "D")
                {
                    for (int i = 0; i < m_dtReturn.Rows.Count; i++)
                    {           

                        m_Dqtty = Convert.ToDouble(oComponentBL.getCumQty(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["PIOWID"].ToString()), m_iContractId, m_iAnlysId, "I"));
                        //dIOWRate = oComponentBL.GetWOIOWRateDPE(c_iCoscenreId, Convert.ToInt32(m_tReturn.Rows[i]["PIOWID"].ToString()), c_iContractId,"I");
                        m_dIOWRate = oComponentBL.GetWOIOWRate(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["PIOWID"].ToString()), m_iContractId, m_iWoRegId, m_sWhereForm);
                        if (m_dIOWRate != 0)
                        {
                            m_dIOWRate = Convert.ToDouble(m_dtReturn.Rows[i]["Unit_Rate"].ToString());
                        }
                        else
                        {
                            
                        }
                        m_dIOWRate = 0;
                        oWorkOrderComponentListsCol.Add(new WorkOrderComponentLists()
                        {
                            RowId = Convert.ToInt32(m_dtReturn.Rows[i]["RowId"].ToString()),
                            ResourceCode = m_dtReturn.Rows[i]["RefSerialNo"].ToString(),
                            ResourceId = 0,
                            IOW_ID = Convert.ToInt32(m_dtReturn.Rows[i]["PIOWID"].ToString()),
                            ResourceName = m_dtReturn.Rows[i]["Specification"].ToString(),
                            Unit = m_dtReturn.Rows[i]["Unit"].ToString(),
                            UnitId = Convert.ToInt32(m_dtReturn.Rows[i]["Unit_Id"].ToString()),
                            Qty = m_Dqtty,
                            NRate =m_dIOWRate,
                            DCRate = m_dIOWRate,
                            Type = "I",
                            TypeId = 0,
                            AnalysisHeadId = Convert.ToInt32(clsStatics.IsNullCheck(m_dtReturn.Rows[i]["AnalysisId"].ToString(), clsStatics.datatypes.vartypenumeric)),
                            MType = 0,
                            CumQty = m_Dqtty,
                            MUnitID = m_dtReturn.Rows[i]["Unit"].ToString(),
                            MUpdate = "C",
                            UFactor = 0,
                            PartRate = 0
                        });
                    }
                }

            }
            //SubIOW-S
            if (grdviewSubCntSubIow.RowCount > 0)
            {
                grdviewSubCntSubIow.FocusedRowHandle = grdviewSubCntSubIow.FocusedRowHandle + 1;
                m_dtReturn = new DataTable();

                DataView dvData = new DataView(grdSubCntSubIow.DataSource as DataTable);
                dvData.RowFilter = "Select = '" + true + "'";
                m_dtReturn = dvData.ToTable();

                if ((m_sWhereForm == "W") || (m_sWhereForm == "B") || (m_sWhereForm == "BDPEList"))
                {
                    if (m_sWhereForm == "B" || m_sWhereForm == "BDPEList")
                    {
                        m_Dqtty = 0;
                    }
                    for (int i = 0; i < m_dtReturn.Rows.Count; i++)
                    {
                        m_dIOWRate = oComponentBL.GetWOIOWRate(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["PIOWID"].ToString()), m_iContractId, m_iWoRegId, m_sWhereForm);
                        if (m_dIOWRate != 0)
                        {
                            
                        }
                        else
                        {
                            m_dIOWRate = Convert.ToDouble(m_dtReturn.Rows[i]["Unit_Rate"].ToString());
                        }
                        if (m_sWhereForm == "W")
                        {
                            m_Dqtty = Convert.ToDouble(oComponentBL.getWOIOWQty(Convert.ToInt32(m_dtReturn.Rows[i]["PIOWID"].ToString()), m_iAnlysId, "S", m_iCoscenreId));
                            if (Convert.ToDouble(m_dtReturn.Rows[i]["Qty"].ToString()) > m_Dqtty)
                            {
                                m_Dqtty = Convert.ToDouble(m_dtReturn.Rows[i]["Qty"].ToString()) - m_Dqtty;
                            }
                            else
                            {
                                m_Dqtty = 0;
                            }
                            
                        }
                        oWorkOrderComponentListsCol.Add(new WorkOrderComponentLists()
                        {
                            RowId = Convert.ToInt32(m_dtReturn.Rows[i]["RowId"].ToString()),
                            ResourceCode = m_dtReturn.Rows[i]["RefSerialNo"].ToString(),
                            ResourceId = 0,
                            IOW_ID = Convert.ToInt32(m_dtReturn.Rows[i]["PIOWID"].ToString()),
                            ResourceName = m_dtReturn.Rows[i]["Specification"].ToString(),
                            Unit = m_dtReturn.Rows[i]["Unit"].ToString(),
                            UnitId = Convert.ToInt32(m_dtReturn.Rows[i]["Unit_Id"].ToString()),
                            Qty = m_Dqtty,
                            NRate = m_dIOWRate,
                            Type = "S",
                            TypeId = 0,
                            AnalysisHeadId = 0,
                            MType = 0,
                            CumQty = 0,
                            MUnitID = "",
                            MUpdate = "",
                            UFactor = 0,
                            PartRate = 0
                        });
                    }
                }
                if (m_sWhereForm == "D")
                {
                    for (int i = 0; i < m_dtReturn.Rows.Count; i++)
                    {
                        m_Dqtty = Convert.ToDouble(oComponentBL.getCumQty(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["PIOWID"].ToString()), m_iContractId, m_iAnlysId, "S"));
                        //dIOWRate = oComponentBL.GetWOIOWRateDPE(c_iCoscenreId, Convert.ToInt32(m_tReturn.Rows[i]["PIOWID"].ToString()), c_iContractId,"S");
                        m_dIOWRate = oComponentBL.GetWOIOWRate(m_iCoscenreId, Convert.ToInt32(m_dtReturn.Rows[i]["PIOWID"].ToString()), m_iContractId, m_iWoRegId, m_sWhereForm);
                        if (m_dIOWRate != 0)
                        {
                        }
                        else
                        {
                           // dIOWRate = Convert.ToDouble(m_tReturn.Rows[i]["Unit_Rate"].ToString());
                        }
                        //dIOWRate = 0;
                        oWorkOrderComponentListsCol.Add(new WorkOrderComponentLists()
                        {
                            RowId = Convert.ToInt32(m_dtReturn.Rows[i]["RowId"].ToString()),
                            ResourceCode = m_dtReturn.Rows[i]["RefSerialNo"].ToString(),
                            ResourceId = 0,
                            IOW_ID = Convert.ToInt32(m_dtReturn.Rows[i]["PIOWID"].ToString()),
                            ResourceName = m_dtReturn.Rows[i]["Specification"].ToString(),
                            Unit = m_dtReturn.Rows[i]["Unit"].ToString(),
                            UnitId = Convert.ToInt32(m_dtReturn.Rows[i]["Unit_Id"].ToString()),
                            Qty = m_Dqtty,
                            NRate = m_dIOWRate,
                            DCRate = m_dIOWRate,
                            Type = "S",
                            TypeId = 0,
                            AnalysisHeadId = 0,
                            MType = 0,
                            CumQty = m_Dqtty,
                            MUnitID = m_dtReturn.Rows[i]["Unit"].ToString(),
                            MUpdate = "",
                            UFactor = 0,
                            PartRate = 0
                        });
                    }
                }
            }

            AddNewEntry();
            if (oWorkOrderComponentListsCol.Count > 0)
            {
                foreach (WorkOrderComponentLists obj in oWorkOrderComponentListsCol)
                {
                    m_row = m_dtCompEntry.NewRow();

                    m_row["RowId"] = Convert.ToInt32(m_dtCompEntry.Rows.Count+1);

                    m_row["ResourceCode"] = obj.ResourceCode;
                    m_row["ResourceId"] = obj.ResourceId;
                    m_row["IOW_ID"] = obj.IOW_ID;
                    m_row["Description"] = obj.ResourceName;
                    m_row["Unit"] = obj.Unit;
                    m_row["Qty"] = obj.Qty;
                    m_row["UnitId"] = obj.UnitId;
                    if (m_sWhereForm == "D")
                    {
                        m_row["Rate"] = obj.Qty;
                        m_row["DCRate"] = obj.DCRate;
                        m_row["Amount"] = 0;
                    }
                    else
                    {
                        m_row["Rate"] = obj.NRate;
                        m_row["Amount"] = obj.Qty * obj.NRate;
                    }
                    m_row["PartRate"] = obj.PartRate;
                    m_row["Type"] = obj.Type;
                    m_row["AnalysisHeadId"] = obj.AnalysisHeadId;
                    m_row["MType"] = obj.MType;
                    m_row["CumQty"] = obj.CumQty;
                    m_row["MUpdate"] = obj.MUpdate;
                    m_row["MUnitID"] = obj.MUnitID;
                    m_row["UFactor"] = obj.UFactor;

                    m_dtCompEntry.Rows.Add(m_row);
                }
            }
            this.Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            m_dtReturn = null;
            this.Close();
        }     
        #endregion

        #region Functions

        public DataTable Execute(string argwhere, string argWoType, string argResId, int argCostCentrId, int argContractId, int argRevId, int argWoRegId, string argAHId,string argRowId)
        {
            m_sWhereForm = argwhere;
            m_sResId = argResId;       
            m_sAHId = argAHId;
            m_sRowId = argRowId;
            m_iCoscenreId = argCostCentrId;
            m_iContractId = argContractId;
            m_iRevId = argRevId;
            m_iWoRegId = argWoRegId;
            m_woTypeList = argWoType;
           
            this.ShowDialog();
            return m_dtCompEntry;
        }

        private void AddNewEntry()
        {
            m_dtCompEntry = new DataTable();
            m_dtCompEntry.Columns.Add("RowId");
            m_dtCompEntry.Columns.Add("ResourceId", typeof(int));
            m_dtCompEntry.Columns.Add("ResourceCode", typeof(string));
            m_dtCompEntry.Columns.Add("IOW_ID", typeof(int));
            m_dtCompEntry.Columns.Add("Description", typeof(string));
            m_dtCompEntry.Columns.Add("Unit", typeof(string));              
            m_dtCompEntry.Columns.Add("Qty", typeof(double));
            m_dtCompEntry.Columns.Add("Rate", typeof(double));
            m_dtCompEntry.Columns.Add("Amount", typeof(double));
            m_dtCompEntry.Columns.Add("Type", typeof(string));
            m_dtCompEntry.Columns.Add("AnalysisHeadId", typeof(int));
            m_dtCompEntry.Columns.Add("MType", typeof(int));
            m_dtCompEntry.Columns.Add("CumQty", typeof(double));
            m_dtCompEntry.Columns.Add("MUpdate", typeof(string));
            m_dtCompEntry.Columns.Add("MUnitID", typeof(string));
            m_dtCompEntry.Columns.Add("UFactor", typeof(double));
            m_dtCompEntry.Columns.Add("DCRate", typeof(double));
            m_dtCompEntry.Columns.Add("PartRate", typeof(int));
            m_dtCompEntry.Columns.Add("UnitId", typeof(int));      
            
            
            for (int i = 0; i < 1; i++)
            {
                string rowNumber = Convert.ToString(i + 1);

                m_dtCompEntry.Rows.Add(new object[] {0, 0, "", 0, "", "", 0.00, 0.00, 0.00, "",0,0.00,0.00,"","",0.00,0.00,0});
            }
            m_dtCompEntry.Rows.RemoveAt(0);

        }

        private void BindGrid()
        {
            try
            {
                m_dtBindata = new DataTable();  
                //Labour
                if (m_woTypeList == "L")
                {

                    m_dtBindata = null;
                    m_dvBindview = new DataView(m_dtGetComponent.Tables["L"]);
                    if (m_sResId != string.Empty)
                    {
                        m_dvBindview.RowFilter = "Resource_Id NOT IN (" + m_sResId.TrimEnd(',') + " )";
                        m_dtBindata = m_dvBindview.ToTable();
                        DataColumn dtcCheck = new DataColumn("Select");
                        dtcCheck.DataType = System.Type.GetType("System.Boolean");
                        dtcCheck.DefaultValue = false;
                        m_dtBindata.Columns.Add(dtcCheck);
                    }
                    else
                    {

                        //dvBindview.RowFilter = "TypeId=" + dr[0][0] + "";
                        m_dtBindata = m_dvBindview.ToTable();
                        DataColumn dtcCheck = new DataColumn("Select");
                        dtcCheck.DataType = System.Type.GetType("System.Boolean");
                        dtcCheck.DefaultValue = false;
                        m_dtBindata.Columns.Add(dtcCheck);
                    }

                    if (m_dtBindata.Rows.Count == 0) { }
                    else
                    {
                        dWLabour.Show();
                        grdLabour.DataSource = m_dtBindata;

                        grdviewLabour.Columns["RowId"].Visible = false;
                        grdviewLabour.Columns["Resource_Name"].Caption = "Component";
                        grdviewLabour.Columns["Unit_Name"].Caption = "Unit";
                        grdviewLabour.Columns["Unit_Name"].Width = 60;
                        grdviewLabour.Columns["Resource_Code"].Caption = "Code";
                        grdviewLabour.Columns["Resource_Code"].Width = 60;
                        grdviewLabour.Columns["Resource_Name"].Width = 250;

                        grdviewLabour.Columns["Resource_Id"].Visible = false;
                        grdviewLabour.Columns["Unit_Id"].Visible = false;
                        grdviewLabour.Columns["Qualified_Rate"].Visible = false;
                        grdviewLabour.Columns["Qty"].Visible = false;
                        grdviewLabour.Columns["TypeId"].Visible = false;
                        grdviewLabour.Columns["Select"].Width = 60;

                        grdviewLabour.Columns["Select"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        grdviewLabour.Columns["Select"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    }


                }
                //Activity
                if (m_woTypeList == "A")
                {
                    dWActiviy.Show();

                    m_dtBindata = null;
                    m_dvBindview = new DataView(m_dtGetComponent.Tables["A"]);
                    if (m_sResId != string.Empty)
                    {
                        m_dvBindview.RowFilter = "Resource_Id NOT IN (" + m_sResId.TrimEnd(',') + " )";
                        m_dtBindata = m_dvBindview.ToTable();
                        DataColumn dtcCheck = new DataColumn("Select");
                        dtcCheck.DataType = System.Type.GetType("System.Boolean");
                        dtcCheck.DefaultValue = false;
                        m_dtBindata.Columns.Add(dtcCheck);
                    }
                    else
                    {

                        //dvBindview.RowFilter = "TypeId=" + dr[0][0] + "";
                        m_dtBindata = m_dvBindview.ToTable();
                        DataColumn dtcCheck = new DataColumn("Select");
                        dtcCheck.DataType = System.Type.GetType("System.Boolean");
                        dtcCheck.DefaultValue = false;
                        m_dtBindata.Columns.Add(dtcCheck);
                    }

                    if (m_dtBindata.Rows.Count == 0) { }
                    else
                    {
                        dWActiviy.Show();
                        grdActivity.DataSource = m_dtBindata;

                        grdviewActivity.Columns["RowId"].Visible = false;
                        grdviewActivity.Columns["Resource_Name"].Caption = "Component";
                        grdviewActivity.Columns["Unit_Name"].Caption = "Unit";
                        grdviewActivity.Columns["Unit_Name"].Width = 60;
                        grdviewActivity.Columns["Resource_Code"].Caption = "Code";
                        grdviewActivity.Columns["Resource_Code"].Width = 60;
                        grdviewActivity.Columns["Resource_Name"].Width = 250;

                        grdviewActivity.Columns["Unit_Id"].Visible = false;
                        grdviewActivity.Columns["Resource_Id"].Visible = false;
                        grdviewActivity.Columns["Qualified_Rate"].Visible = false;
                        grdviewActivity.Columns["Qty"].Visible = false;
                        grdviewActivity.Columns["TypeId"].Visible = false;
                        grdviewActivity.Columns["Select"].Width = 60;

                        grdviewActivity.Columns["Select"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        grdviewActivity.Columns["Select"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    }

                }

                //Activity
                if (m_woTypeList == "H")
                {
                    dWActiviy.Show();

                    m_dtBindata = null;
                    m_dvBindview = new DataView(m_dtGetComponent.Tables["H"]);
                    if (m_sResId != string.Empty)
                    {
                        m_dvBindview.RowFilter = "Resource_Id NOT IN (" + m_sResId.TrimEnd(',') + " )";
                        m_dtBindata = m_dvBindview.ToTable();
                        DataColumn dtcCheck = new DataColumn("Select");
                        dtcCheck.DataType = System.Type.GetType("System.Boolean");
                        dtcCheck.DefaultValue = false;
                        m_dtBindata.Columns.Add(dtcCheck);
                    }
                    else
                    {

                        //dvBindview.RowFilter = "TypeId=" + dr[0][0] + "";
                        m_dtBindata = m_dvBindview.ToTable();
                        DataColumn dtcCheck = new DataColumn("Select");
                        dtcCheck.DataType = System.Type.GetType("System.Boolean");
                        dtcCheck.DefaultValue = false;
                        m_dtBindata.Columns.Add(dtcCheck);
                    }

                    if (m_dtBindata.Rows.Count == 0) { }
                    else
                    {
                        dWHire.Show();
                        grdHire.DataSource = m_dtBindata;
                        grdviewHire.Columns["Unit_Id"].Visible = false;
                        grdviewHire.Columns["RowId"].Visible = false;
                        grdviewHire.Columns["Resource_Name"].Caption = "Component";
                        grdviewHire.Columns["Unit_Name"].Caption = "Unit";
                        grdviewHire.Columns["Unit_Name"].Width = 60;
                        grdviewHire.Columns["Resource_Code"].Caption = "Code";
                        grdviewHire.Columns["Resource_Code"].Width = 60;
                        grdviewHire.Columns["Resource_Name"].Width = 250;

                        grdviewHire.Columns["Resource_Id"].Visible = false;
                        grdviewHire.Columns["Qualified_Rate"].Visible = false;
                        grdviewHire.Columns["Qty"].Visible = false;
                        grdviewHire.Columns["TypeId"].Visible = false;
                        grdviewHire.Columns["Select"].Width = 60;

                        grdviewHire.Columns["Select"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        grdviewHire.Columns["Select"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    }

                }

                //SubCon-IOW
                if (m_woTypeList == "I")
                {
                    dWSubContractIOW.Show();
                    PopulateIOW();

                }
                //Subcon-SubIOW
                if (m_woTypeList == "S")
                {
                    dWSubContractSubIOW.Show();
                    PopulateSubIOW();

                }
                //Service
                if (m_woTypeList == "R")
                {
                    dWService.Show();
                    PopulateService();
                }                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }      

        private void DefaultTabHide()
        {
            dWLabour.Hide();
            dWActiviy.Hide();
            dWSubContractIOW.Hide();
            dWSubContractSubIOW.Hide();            
            dWService.Hide();
            dWHire.Hide();
        }        

        private void PopulateIOW()
        {
            m_dtGetIOW = new DataTable();
            DataTable dtAn = new DataTable();
            DataRow dr;
            grdSubCntSubIow.DataSource = null;
            grdviewSubCntIow.Columns.Clear();
            rdBtnAll.Checked = true;
            string sWBSReq = "";

            sWBSReq = clsStatics.GetWBSReqd(m_iCoscenreId);           

            m_dtBindata = null;
          

            m_dvBindview = new DataView(m_dtGetComponent.Tables["IOW"]);

            if (m_sResId != string.Empty && m_sAHId != string.Empty)
            {               
                if (m_sAHId != "")
                {
                    m_dvBindview.RowFilter = "RowId Not In("+m_sRowId+")";
                }
                else
                {
                    m_dvBindview.RowFilter = "PIOWID NOT IN (" + m_sResId.TrimEnd(',') + " )";
                }               
                m_dtBindata = m_dvBindview.ToTable();
                DataColumn dtcCheck = new DataColumn("Select");
                dtcCheck.DataType = System.Type.GetType("System.Boolean");
                dtcCheck.DefaultValue = false;
                m_dtBindata.Columns.Add(dtcCheck);            
            }
            else
            {
                //if (m_sIOWId != string.Empty) { dvBindview.RowFilter = " PIOWID NOT IN  (" + m_sIOWId.TrimEnd(',') + " )"; }               
                m_dtBindata = m_dvBindview.ToTable();

                DataColumn dtcCheck = new DataColumn("Select");
                dtcCheck.DataType = System.Type.GetType("System.Boolean");
                dtcCheck.DefaultValue = false;
                m_dtBindata.Columns.Add(dtcCheck);
               
            }
            if (m_dtBindata.Rows.Count == 0) { }
            else
            {

                grdSubCntIow.DataSource = m_dtBindata;

                if (sWBSReq == "Y")
                {
                    RepositoryItemLookUpEdit AnalEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();

                    dtAn = oComponentBL.GetAnalysisTypeHead();
                    DataView dv = new DataView(dtAn);
                    dv.RowFilter = "AnalysisID=0";
                    if (dv.ToTable().Rows.Count > 0) { }
                    else
                    {
                        dr = dtAn.NewRow();
                        dr["AnalysisHeadName"] = "All";
                        dr["AnalysisId"] = 0;
                        dtAn.Rows.InsertAt(dr, 0);
                    }
                    if (dtAn.Rows.Count > 0)
                    {
                        AnalEdit.DataSource = dtAn;
                        AnalEdit.ForceInitialize();
                        AnalEdit.PopulateColumns();
                        AnalEdit.DisplayMember = "AnalysisHeadName";
                        AnalEdit.ValueMember = "AnalysisId";
                        AnalEdit.Columns["AnalysisId"].Visible = false;
                        AnalEdit.ShowFooter = false;
                        AnalEdit.ShowHeader = false;
                    }
                    grdviewSubCntIow.Columns["AnalysisId"].ColumnEdit = AnalEdit;
                    grdviewSubCntIow.Columns["AnalysisId"].OptionsColumn.AllowEdit = false;


                    grdviewSubCntIow.Columns["Specification"].Caption = "Component";
                    grdviewSubCntIow.Columns["AnalysisId"].Caption = "AnalysisHead";
                    grdviewSubCntIow.Columns["Unit"].Caption = "Unit";
                    grdviewSubCntIow.Columns["Unit"].Width = 40;
                    grdviewSubCntIow.Columns["RefSerialNo"].Caption = "Code";
                    grdviewSubCntIow.Columns["RefSerialNo"].Width = 60;
                    grdviewSubCntIow.Columns["Specification"].Width = 250;
                }
                else
                {
                    grdviewSubCntIow.Columns["AnalysisId"].Visible = false;

                    grdviewSubCntIow.Columns["Specification"].Caption = "Component";
                    grdviewSubCntIow.Columns["Unit"].Caption = "Unit";
                    grdviewSubCntIow.Columns["Unit"].Width = 40;
                    grdviewSubCntIow.Columns["RefSerialNo"].Caption = "Code";
                    grdviewSubCntIow.Columns["RefSerialNo"].Width = 60;
                    grdviewSubCntIow.Columns["Specification"].Width = 400;
                }


                grdviewSubCntIow.Columns["Unit_Id"].Visible = false;
                grdviewSubCntIow.Columns["Unit_Rate"].Visible = false;
                grdviewSubCntIow.Columns["RowId"].Visible = false;
                grdviewSubCntIow.Columns["PIOWID"].Visible = false;
                grdviewSubCntIow.Columns["Qty"].Visible = false;

                grdviewSubCntIow.Columns["Select"].Width = 60;

                grdviewSubCntIow.Columns["Select"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                grdviewSubCntIow.Columns["Select"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
        }      
        
        private void PopulateSubIOW()
        {
            m_dtGetSubIOW = new DataTable();
            //dtGetSubIOW = oComponentBL.GetWOSubIOWComponent(r_iresouceId);
            m_dtBindata = null;
            m_dvBindview = new DataView(m_dtGetComponent.Tables["SIOW"]);
            if (m_sResId != string.Empty)
            {
                m_dvBindview.RowFilter = "PIOWID NOT IN (" + m_sResId.TrimEnd(',') + " )";
                m_dtBindata = m_dvBindview.ToTable();
                DataColumn dtcCheck = new DataColumn("Select");
                dtcCheck.DataType = System.Type.GetType("System.Boolean");
                dtcCheck.DefaultValue = false;
                m_dtBindata.Columns.Add(dtcCheck);
            }
            else
            {
                m_dtBindata = m_dvBindview.ToTable();
                DataColumn dtcCheck = new DataColumn("Select");
                dtcCheck.DataType = System.Type.GetType("System.Boolean");
                dtcCheck.DefaultValue = false;
                m_dtBindata.Columns.Add(dtcCheck);
            }
            if (m_dtBindata.Rows.Count == 0) { }
            else
            {
                grdSubCntSubIow.DataSource = m_dtBindata;

                grdviewSubCntSubIow.Columns["Specification"].Caption = "Component";
                grdviewSubCntSubIow.Columns["Unit"].Width = 60;
                grdviewSubCntSubIow.Columns["RefSerialNo"].Caption = "Code";
                grdviewSubCntSubIow.Columns["RefSerialNo"].Width = 60;
                grdviewSubCntSubIow.Columns["Specification"].Width = 250;

                grdviewSubCntSubIow.Columns["Unit_Id"].Visible = false;
                grdviewSubCntSubIow.Columns["RowId"].Visible = false;
                grdviewSubCntSubIow.Columns["PIOWID"].Visible = false;
                grdviewSubCntSubIow.Columns["Unit_Rate"].Visible = false;
                grdviewSubCntSubIow.Columns["Qty"].Visible = false;
                grdviewSubCntSubIow.Columns["Select"].Width = 60;

                grdviewSubCntSubIow.Columns["Select"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                grdviewSubCntSubIow.Columns["Select"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }
        }

        private void PopulateService()
        {
            m_dtGetSubIOW = new DataTable();
            //dtGetSubIOW = oComponentBL.GetWOSubIOWComponent(r_iresouceId);
            m_dtBindata = null;
            m_dvBindview = new DataView(m_dtGetComponent.Tables["Service"]);
            if (m_sResId != string.Empty)
            {
                m_dvBindview.RowFilter = "ServiceId NOT IN (" + m_sResId.TrimEnd(',') + " )";
                m_dtBindata = m_dvBindview.ToTable();
                DataColumn dtcCheck = new DataColumn("Select");
                dtcCheck.DataType = System.Type.GetType("System.Boolean");
                dtcCheck.DefaultValue = false;
                m_dtBindata.Columns.Add(dtcCheck);
            }
            else
            {
                m_dtBindata = m_dvBindview.ToTable();
                DataColumn dtcCheck = new DataColumn("Select");
                dtcCheck.DataType = System.Type.GetType("System.Boolean");
                dtcCheck.DefaultValue = false;
                m_dtBindata.Columns.Add(dtcCheck);
            }
            if (m_dtBindata.Rows.Count == 0) { return; }
            grdService.DataSource = m_dtBindata;

            grdviewService.Columns["ServiceName"].Caption = "Component";
            grdviewService.Columns["Unit"].Width = 60;
            grdviewService.Columns["ServiceCode"].Caption = "Code";
            grdviewService.Columns["ServiceCode"].Width = 60;
            grdviewService.Columns["ServiceName"].Width = 250;
            grdviewService.Columns["Unit_Id"].Visible = false;
            grdviewService.Columns["RowId"].Visible = false;
            grdviewService.Columns["ServiceGroupId"].Visible = false;
            grdviewService.Columns["ServiceId"].Visible = false;  
            
            grdviewService.Columns["Select"].Width = 60;

            grdviewService.Columns["Select"].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            grdviewService.Columns["Select"].AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        }

        private void PopulatputIOWDetails()
        {
            m_dtPutIOWDetails = new DataTable();
            m_dtPutIOWDetails = oComponentBL.PutIOWDetails(m_iIOWId);
           
        }
        
        #endregion  
    
        #region Gridview Event

        private void grdviewSubCntIow_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            m_iIOWId = Convert.ToInt32(grdviewSubCntIow.GetRowCellValue(grdviewSubCntIow.FocusedRowHandle, "IOW_ID"));
            PopulatputIOWDetails();
        }

        private void grdviewLabour_ShowingEditor(object sender, CancelEventArgs e)
        {
             DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
             if (view.FocusedColumn.FieldName == "Resource_Code" || view.FocusedColumn.FieldName == "Resource_Name" || view.FocusedColumn.FieldName == "Unit_Name") 
             {
                 e.Cancel = true;
             }      
        }

        private void grdviewActivity_ShowingEditor(object sender, CancelEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view.FocusedColumn.FieldName == "Resource_Code" || view.FocusedColumn.FieldName == "Resource_Name" || view.FocusedColumn.FieldName == "Unit_Name")
            {
                e.Cancel = true;
            } 
        }

        private void grdviewSubCntIow_ShowingEditor(object sender, CancelEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view.FocusedColumn.FieldName == "RefSerialNo" || view.FocusedColumn.FieldName == "Specification" || view.FocusedColumn.FieldName == "UOM_ID")
            {
                e.Cancel = true;
            } 
        }

        private void grdviewSubCntSubIow_ShowingEditor(object sender, CancelEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view.FocusedColumn.FieldName == "RefSerialNo" || view.FocusedColumn.FieldName == "Specification" || view.FocusedColumn.FieldName == "Unit")
            {
                e.Cancel = true;
            } 
        }

       

        private void grdviewHire_ShowingEditor(object sender, CancelEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view.FocusedColumn.FieldName == "Resource_Code" || view.FocusedColumn.FieldName == "Resource_Name" || view.FocusedColumn.FieldName == "Unit_Name")
            {
                e.Cancel = true;
            }
        }

        


        #endregion

        #region DropDownEvent
        

   
        #endregion

        #region Option Button Event

        private void rdBtnAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnAll.Checked == true)
            {
                //w_iOptTypeId = "A";
                PopulateIOW();
            }
           
        }
        private void rdBtnAgrement_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnAgrement.Checked == true)
            {
                //w_iOptTypeId = "G";
                PopulateIOW();
            }
        }
        private void rdBtnNAgrement_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnNAgrement.Checked == true)
            {
                //w_iOptTypeId = "N";
                PopulateIOW();
            }            
        }
        private void rdBtnOthrs_CheckedChanged(object sender, EventArgs e)
        {
            if (rdBtnNAgrement.Checked == true)
            {
               // w_iOptTypeId = "O";
                PopulateIOW();
            }
        }

        #endregion              

     

    }
}

