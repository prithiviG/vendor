using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VendorAnalysis.BusinessLayer;
using VendorAnalysis.BusinessObjects;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.Utils;

namespace VendorAnalysis
{
    public partial class frmCompare : Form
    {
        #region Variables
        DataTable dtBandCol;
        DataTable dtGrid;         

        #endregion
        
        #region Objects


        VendorCompareBL oVCompBL;
        #endregion

        #region Constructor

        public frmCompare()
        {
            InitializeComponent();

            oVCompBL = new VendorCompareBL();
        }
        #endregion

        #region Form Event
        private void frmCompare_Load(object sender, EventArgs e)
        {
            PopulateColumn();
            //pulateGrid2();
            //Populate();
            //NewFunction();
            //PopulateGrid();
        }
        #endregion

        #region Functions

        private void PopulateColumn()
        {
            dtBandCol = new DataTable();
            dtBandCol.Columns.Add("General");
            DataTable dtVname = new DataTable();
            DataRow dr;

            DataTable dtVCBind = new DataTable();            
            dtVCBind = oVCompBL.getQuotationResorce(1);

            dtVname = oVCompBL.getQSubDetails(1);

            dtGrid = new DataTable();

            dtGrid.Columns.Add("ID");
            dtGrid.Columns.Add("Code");
            dtGrid.Columns.Add("Description");
            dtGrid.Columns.Add("Unit");
            dtGrid.Columns.Add("Quantity");


            DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn Bcolumn;// = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand;

            if (dtVname.Rows.Count <= 0)
            {
                return;
            }
            else
            {
                for (int b = 0; b < dtVname.Rows.Count; b++)
                {
                    dtBandCol.Columns.Add(dtVname.Rows[b]["VendorName"].ToString());

                    for (int t = 0; t <4;t++)
                    {
                        if (t == 0)
                        {
                            dtGrid.Columns.Add(dtVname.Rows[b]["VendorName"].ToString() + "VendorId", typeof(int)).DefaultValue = dtVname.Rows[b]["VendorId"].ToString();
                        }
                        if (t == 1)
                        {
                            dtGrid.Columns.Add(dtVname.Rows[b]["VendorName"].ToString() + "Rate", typeof(decimal)).DefaultValue = 0;
                        }
                        if (t == 2)
                        {
                            dtGrid.Columns.Add(dtVname.Rows[b]["VendorName"].ToString() + "QRate", typeof(decimal)).DefaultValue = 0;
                        }
                        if (t == 3)
                        {
                            dtGrid.Columns.Add(dtVname.Rows[b]["VendorName"].ToString() + "Amount", typeof(decimal)).DefaultValue = 0;
                        }                        
                    }        

                }
            }
            if (dtVCBind.Rows.Count > 0)
            {
                for (int u = 0; u < dtVCBind.Rows.Count; u++)
                {
                    dr = dtGrid.NewRow();

                    dr["ID"] = dtVCBind.Rows[u]["ID"].ToString();
                    dr["Code"] = dtVCBind.Rows[u]["Code"].ToString();
                    dr["Description"] = dtVCBind.Rows[u]["Description"].ToString();
                    dr["Unit"] = dtVCBind.Rows[u]["Unit"].ToString();
                    dr["Quantity"] = dtVCBind.Rows[u]["Quantity"].ToString();
                    
                    dtGrid.Rows.Add(dr);           
                }
            }

            gridView1.Bands.Clear();
            gridView1.Columns.Clear();
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.OptionsView.ShowGroupPanel = false;

            gridControl1.DataSource = dtGrid;         

            //For Column Design
            if (dtBandCol.Columns.Count <= 0)
            {
                return;
            }
            else
            {
                for (int i = 0; i < dtBandCol.Columns.Count; i++)
                {
                    if (i == 0)
                    {
                        gridBand = new GridBand();
                        gridBand.Name = "";
                        gridBand.Caption = "";
                        gridBand.OptionsBand.AllowMove = false;
                        gridBand.OptionsBand.AllowPress = false;
                        gridView1.Bands.Add(gridBand);
                    }
                    else
                    {
                        gridBand = new GridBand();
                        gridBand.Name = dtBandCol.Columns[i].ToString();
                        gridBand.Caption = dtBandCol.Columns[i].ToString();
                        gridBand.OptionsBand.AllowMove = false;
                        gridBand.OptionsBand.AllowPress = false;
                        gridView1.Bands.Add(gridBand);
                    }
                }

                DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit qualiButEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
                qualiButEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                //qualiButEdit.Click += new EventHandler(qualiButEdit_Click);

                DevExpress.XtraEditors.Repository.RepositoryItemTextEdit RatetxtEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
                RatetxtEdit.Validating += new CancelEventHandler(RatetxtEdit_Validating);

                DevExpress.XtraEditors.Repository.RepositoryItemTextEdit amttxtEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
                //amttxtEdit.Validating += new CancelEventHandler(amttxtEdit_Validating);

                int colCount = (gridView1.Bands.Count - 1) * 4;
                int balColCount = dtGrid.Columns.Count - colCount;
                int finColCunt = (dtGrid.Columns.Count - balColCount) / (gridView1.Bands.Count - 1);
                int m;
                for (int j = 0; j < gridView1.Bands.Count; j++)
                {
                    gridView1.Bands[j].AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                    if (j == 0)
                    {
                        for (int k = 0; k < dtGrid.Columns.Count - colCount; k++)
                        {
                            if (dtGrid.Columns[k].ToString() == "ID")
                            {
                                Bcolumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                                Bcolumn.OptionsColumn.AllowMove = false;
                                Bcolumn.Caption = dtGrid.Columns[k].ToString();
                                Bcolumn.Name = dtGrid.Columns[k].ToString();
                                Bcolumn.FieldName = dtGrid.Columns[k].ToString();
                                Bcolumn.Visible = false;                              
                                gridView1.Bands[j].Columns.Add(Bcolumn);
                            }
                            else
                            {
                                Bcolumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                                Bcolumn.OptionsColumn.AllowMove = false;
                                Bcolumn.Caption = dtGrid.Columns[k].ToString();
                                Bcolumn.Name = dtGrid.Columns[k].ToString();
                                Bcolumn.FieldName = dtGrid.Columns[k].ToString();
                                Bcolumn.Visible = true;
                                if (dtGrid.Columns[k].ToString() == "Code" || dtGrid.Columns[k].ToString() == "Unit")
                                {
                                    Bcolumn.Width = 50;
                                }
                                if (dtGrid.Columns[k].ToString() == "Quantity")
                                {
                                    Bcolumn.Width = 50;
                                }
                                if (dtGrid.Columns[k].ToString() == "Description")
                                {
                                    Bcolumn.Width = 150;
                                }
                                Bcolumn.OptionsColumn.AllowEdit = false; 
                                gridView1.Bands[j].Columns.Add(Bcolumn);

                            }
                        }
                    }
                    else
                    {
                        for (m=5; m < dtGrid.Columns.Count; m++)
                        {
                            if (dtGrid.Columns[m].ToString().Contains(gridView1.Bands[j] + "QRate"))
                            {
                                Bcolumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                                Bcolumn.OptionsColumn.AllowMove = false;
                                Bcolumn.Caption = "QRate";
                                Bcolumn.Name = dtGrid.Columns[m].ToString();
                                Bcolumn.FieldName = dtGrid.Columns[m].ToString();
                                Bcolumn.Visible = true;
                                Bcolumn.OptionsColumn.AllowEdit = true;
                                Bcolumn.ColumnEdit = qualiButEdit;
                                Bcolumn.Width = 90;
                                gridView1.Bands[j].Columns.Add(Bcolumn);
                            }
                            if (dtGrid.Columns[m].ToString().Contains(gridView1.Bands[j] + "VendorId"))
                            {
                                Bcolumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                                Bcolumn.OptionsColumn.AllowMove = false;
                                Bcolumn.Caption = "VendorId";
                                Bcolumn.Name = dtGrid.Columns[m].ToString();
                                Bcolumn.FieldName = dtGrid.Columns[m].ToString();
                                Bcolumn.Visible = false;
                                Bcolumn.OptionsColumn.AllowEdit = false;                                
                                gridView1.Bands[j].Columns.Add(Bcolumn);
                            }
                            if (dtGrid.Columns[m].ToString().Contains(gridView1.Bands[j] + "Rate") || dtGrid.Columns[m].ToString().Contains(gridView1.Bands[j] + "Amount"))
                            {
                                if (dtGrid.Columns[m].ToString().Contains(gridView1.Bands[j] + "Rate"))
                                {
                                    Bcolumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                                    Bcolumn.OptionsColumn.AllowMove = false;
                                    Bcolumn.Caption = "Rate";
                                    Bcolumn.Name = dtGrid.Columns[m].ToString();
                                    Bcolumn.FieldName = dtGrid.Columns[m].ToString();
                                    Bcolumn.Visible = true;
                                    Bcolumn.OptionsColumn.AllowEdit = true;
                                    Bcolumn.ColumnEdit = RatetxtEdit;
                                    Bcolumn.Width = 40;
                                    gridView1.Bands[j].Columns.Add(Bcolumn);
                                }
                                else
                                {
                                    Bcolumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                                    Bcolumn.OptionsColumn.AllowMove = false;
                                    Bcolumn.Caption = "Amount";
                                    Bcolumn.Name =dtGrid.Columns[m].ToString();
                                    Bcolumn.FieldName = dtGrid.Columns[m].ToString();
                                    Bcolumn.Visible = true;
                                    Bcolumn.OptionsColumn.AllowEdit = true;
                                    Bcolumn.ColumnEdit = RatetxtEdit;
                                    Bcolumn.Width = 90;
                                    gridView1.Bands[j].Columns.Add(Bcolumn);
                                }
                            }
                        }
                        m += 1;
                        
                    }
                }                                
            }

        }

        //private void Populate()
        //{ 
        //    // creating new bands
        //    DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
        //    DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn Bcolumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();

        //    //Dispose of the old view 
        //    gridControl2.MainView.Dispose();
        //    //Create a Banded Grid View 
        //    BandedGridView bandedView = new BandedGridView(gridControl2);
        //    gridControl2.MainView = bandedView;

        //    //bandedGridView1.Columns.Clear();
        //    //bandedGridView1.Bands.Clear();

        //    for (int i = 1; i <=2; i++)
        //    {
        //        //gridBand = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
        //        //gridBand.Name = "Vendor" + i;
        //        //gridBand.Caption = "Vendor" + i;
        //        //gridBand.OptionsBand.AllowMove = false;
        //        //gridBand.OptionsBand.AllowPress = false;
        //        //bandedGridView1.Bands.Add(gridBand);

        //        gridBand = bandedView.Bands.AddBand("Vendor" + i);
        //    }
        //    for (int j = 1; j <= 2;j++)
        //    {
        //        Bcolumn = (BandedGridColumn)bandedView.Columns.AddField("Column" + j);
        //        Bcolumn.OwnerBand = gridBand;
        //        Bcolumn.Visible = true;

        //        //Bcolumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
        //        //Bcolumn.OptionsColumn.AllowMove = false;
        //        //Bcolumn.Caption = "column" + j;
        //        //Bcolumn.Name = "Column";
        //        //Bcolumn.FieldName = "Column" + j;
        //        //Bcolumn.Visible = true;
        //        //gridBand.Columns.Add(Bcolumn); 
        //        //bandedGridView1.Bands[j].Columns.Add(Bcolumn);

        //        //if (bandedGridView1.Bands.Count > 0)
        //        //{
        //        //    for (int k = j; k <=bandedGridView1.Bands.Count; k++)
        //        //    {
        //        //        bandedGridView1.Bands[k].Columns.Add(Bcolumn);
        //        //    }
        //        //}
                   
        //        //gridBand.Columns.Add(Bcolumn); 
        //    }       
  
       
        //    //bandedGridView1.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[]{gridBand});
           
           

        //    bandedGridView1.GridControl = gridControl2;
        //    bandedGridView1.EndInit();        

         
        //    //// creating first column

        //    //DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn column1 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
        //    //column1.OptionsColumn.AllowMove = false;
        //    //column1.Caption = "column1";
        //    //column1.Name = "Column1";
        //    //column1.FieldName = "Column1";
        //    //column1.Visible = true;

        //    //// creating second column

        //    //DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn column2 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
        //    //column2.OptionsColumn.AllowMove = false;
        //    //column2.Caption = "column2";
        //    //column2.Name = "Column2";
        //    //column2.FieldName = "Column2";
        //    //column2.Visible = true;

        //    // adding bands and columns

        //    //bandedGridView1.Columns.Clear();
        //    //bandedGridView1.Bands.Clear();
        //    //bandedGridView1.Bands.Add(gridBand);
        //    ////bandedGridView1.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[]{gridBand});
        //    //gridBand.Columns.Add( column1 );
        //    //gridBand.Columns.Add( column2 );

        //    //bandedGridView1.GridControl = gridControl2;
        //    //bandedGridView1.EndInit();        

        //    }

        //private void NewFunction()
        //{
        //    //Dispose of the old view 
        //    gridControl2.MainView.Dispose();
        //    //Create a Banded Grid View 
        //    BandedGridView bandedView = new BandedGridView(gridControl2);
        //    gridControl2.MainView = bandedView;
        //    //Add one band and one column to the view 
        //    GridBand band = bandedView.Bands.AddBand("General");
        //    BandedGridColumn column = (BandedGridColumn)bandedView.Columns.AddField("CustomerID");
        //    column.OwnerBand = band;
        //    column.Visible = true;
        //}

        //private void PopulateGrid()
        //{
        //    gridControl2.MainView.Dispose();
        //    BandedGridView bandedView = new BandedGridView(gridControl2);
        //    gridControl2.MainView = bandedView;
        //    List<BandedGridColumn> Bcolumn2=new List<BandedGridColumn>();
        //    DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn Bcolumn;// = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
        //    //GridBand gridBand;

        //    for (int i = 0; i <2; i++)
        //    {
        //        //bandedView.Bands.Add(new GridBand() { Caption = "Band"+i });
        //        GridBand gridBand = bandedView.Bands.AddBand("Band" + i);
        //    }
        //    for (int j = 0; j <bandedView.Bands.Count; j++)
        //    {
        //        //Bcolumn2.Add(new BandedGridColumn()
        //        //{
        //        //    Caption = "Column" + j,
        //        //    FieldName = "Field" + j,
        //        //    VisibleIndex = 0
        //        //});

        //        //Bcolumn = (BandedGridColumn)bandedView.Columns.AddField("Column" + j);
        //        //Bcolumn.OwnerBand = gridBand;
        //        //Bcolumn.Visible = true;             

        //        for (int k = 0; k < 3; k++)
        //        {

        //            Bcolumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
        //            Bcolumn.OptionsColumn.AllowMove = false;
        //            Bcolumn.Caption = "column" + k;
        //            Bcolumn.Name = "Column";
        //            Bcolumn.FieldName = "Column" + k;
        //            Bcolumn.Visible = true;

        //            bandedView.Bands[j].Columns.Add(Bcolumn);
        //        }


        //        //bandedView.Bands[].Columns.Add(new BandedGridColumn() { Caption = "Column" + j, FieldName = "Field" + j, VisibleIndex = 0 });
        //    }
        //    //for (int k = 0; k < bandedView.Bands.Count; k++)
        //    //{
        //    //    //bandedView.Bands[].Columns.Add(new BandedGridColumn() { Caption = "Column" + j, FieldName = "Field" + j, VisibleIndex = 0 });

        //    //    Bcolumn = (BandedGridColumn)bandedView.Columns.AddField("Column" + k);
        //    //    Bcolumn.OwnerBand = bandedView.Bands[k];
        //    //    Bcolumn.Visible = true;
        //    //    bandedView.Bands[k].Columns.Add(Bcolumn);
                    
        //    //}
            

          

        //   // DataTable table = new DataTable();
        //   // table.Columns.Add(new DataColumn("Field", typeof(string)));

        //   // DataRow row = table.NewRow();
        //   // row["Field"] = "Apples";
        //   // table.Rows.Add(row);

        //   // row = table.NewRow();
        //   // row["Field"] = "Bananas";
        //   // table.Rows.Add(row);

        //   // row = table.NewRow();
        //   // row["Field"] = "Pears";
        //   // table.Rows.Add(row);

        //   //gridControl2.DataSource = table;
        //}

        private void PopulateGrid2()
        {

            DataTable dtCol = new DataTable();
            dtCol.Columns.Add("ID");
            dtCol.Columns.Add("Code");
            dtCol.Columns.Add("Description");
            dtCol.Columns.Add("Unit");
            dtCol.Columns.Add("Quantity");
            //dtCol.Columns.Add("VendorId");
            dtCol.Columns.Add("Rate", typeof(decimal));
            dtCol.Columns.Add("QRate", typeof(decimal));
            dtCol.Columns.Add("Amount", typeof(decimal));

            dtBandCol = new DataTable();
            dtBandCol.Columns.Add("General");
            DataTable dtVname = new DataTable();

            dtVname = oVCompBL.getQSubDetails(1);

            if (dtVname.Rows.Count <= 0)
            {
                return;
            }
            else
            {
                for (int b = 0; b < dtVname.Rows.Count; b++)
                {
                    dtBandCol.Columns.Add(dtVname.Rows[b]["VendorName"].ToString());
                }
            }

            DataTable dtVCBind = new DataTable();

            DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn Bcolumn;// = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand;
            dtVCBind = oVCompBL.getQuotationResorce(1);

            gridView1.Bands.Clear();
            gridView1.Columns.Clear();
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.OptionsView.ShowGroupPanel = false;

            gridControl1.DataSource = dtVCBind;
            gridView1.Columns["ID"].Visible = false;
           
                
            //For Column Design
            if (dtBandCol.Columns.Count <= 0)
            {
                return;
            }
            else
            {
                for (int i = 0; i < dtBandCol.Columns.Count; i++)
                {
                    gridBand = new GridBand();
                    gridBand.Name = dtBandCol.Columns[i].ToString();
                    gridBand.Caption = dtBandCol.Columns[i].ToString();
                    gridBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                    gridBand.OptionsBand.AllowMove = false;
                    gridBand.OptionsBand.AllowPress = false;
                    gridView1.Bands.Add(gridBand);
                }

                DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit qualiButEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
                qualiButEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                //qualiButEdit.Click += new EventHandler(qualiButEdit_Click);

                DevExpress.XtraEditors.Repository.RepositoryItemTextEdit RatetxtEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
                RatetxtEdit.Validating += new CancelEventHandler(RatetxtEdit_Validating);

                DevExpress.XtraEditors.Repository.RepositoryItemTextEdit amttxtEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
                //amttxtEdit.Validating += new CancelEventHandler(amttxtEdit_Validating);



                for (int j = 0; j < gridView1.Bands.Count; j++)
                {
                    if (j == 0)
                    {
                        for (int k = 0; k < dtCol.Columns.Count - 3; k++)
                        {
                            if (dtCol.Columns[k].ToString() == "ID")
                            {
                                Bcolumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                                Bcolumn.OptionsColumn.AllowMove = false;
                                Bcolumn.Caption = dtCol.Columns[k].ToString();
                                Bcolumn.Name = dtCol.Columns[k].ToString();
                                Bcolumn.FieldName = dtCol.Columns[k].ToString();
                                Bcolumn.Visible = false;                                
                                gridView1.Bands[j].Columns.Add(Bcolumn);
                            }
                            else
                            {
                                Bcolumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                                Bcolumn.OptionsColumn.AllowMove = false;
                                Bcolumn.Caption = dtCol.Columns[k].ToString();
                                Bcolumn.Name = dtCol.Columns[k].ToString();
                                Bcolumn.FieldName = dtCol.Columns[k].ToString();
                                Bcolumn.Visible = true;                                
                                gridView1.Bands[j].Columns.Add(Bcolumn);

                            }
                        }
                    }
                    else
                    {
                        for (int k = 5; k < dtCol.Columns.Count; k++)
                        {
                            if (dtCol.Columns[k].ToString() == "Rate" || dtCol.Columns[k].ToString() == "QRate" || dtCol.Columns[k].ToString() == "Amount")
                            {
                                if (dtCol.Columns[k].ToString() == "QRate")
                                {
                                    Bcolumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                                    Bcolumn.OptionsColumn.AllowMove = false;
                                    Bcolumn.Caption = dtCol.Columns[k].ToString();
                                    Bcolumn.Name = dtCol.Columns[k].ToString();
                                    Bcolumn.FieldName = gridView1.Bands[j].Name+dtCol.Columns[k].ToString();
                                    Bcolumn.Visible = true;
                                    Bcolumn.OptionsColumn.AllowEdit = true;
                                    Bcolumn.ColumnEdit = qualiButEdit;
                                    gridView1.Bands[j].Columns.Add(Bcolumn);
                                }
                                else
                                {
                                    Bcolumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
                                    Bcolumn.OptionsColumn.AllowMove = false;
                                    Bcolumn.Caption = dtCol.Columns[k].ToString();
                                    Bcolumn.Name = dtCol.Columns[k].ToString();
                                    Bcolumn.FieldName = gridView1.Bands[j].Name+dtCol.Columns[k].ToString();
                                    Bcolumn.Visible = true;
                                    Bcolumn.OptionsColumn.AllowEdit = true;
                                    Bcolumn.ColumnEdit = RatetxtEdit;
                                    gridView1.Bands[j].Columns.Add(Bcolumn);

                                }
                            }
                        }
                    }
                }               
            }
        }

        void RatetxtEdit_Validating(object sender, CancelEventArgs e)
        {
            DevExpress.XtraEditors.TextEdit editer = sender as DevExpress.XtraEditors.TextEdit;
            for (int b = 1; b < dtBandCol.Columns.Count; b++)
            {
                if (gridView1.FocusedColumn.FieldName == dtBandCol.Columns[b] + "Rate")
                {
                    gridView1.SetFocusedRowCellValue(dtBandCol.Columns[b] + "Rate", editer.Text);
                }
            }     
            gridView1.UpdateCurrentRow();
        }
        #endregion

        #region Grid Event
        private void gridView1_ValidatingEditor(object sender, DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventArgs e)
        {
            if (gridView1.RowCount > 0)
            {
                decimal amt = 0;
                decimal Qty = 0;
                decimal Rate = 0;

                DataTable dt = new DataTable();
                dt = gridView1.DataSource as DataTable;

                for (int b = 1; b < dtBandCol.Columns.Count; b++)
                {
                    if (gridView1.FocusedColumn.Name == dtBandCol.Columns[b] + "Rate")
                    {
                        Qty = Convert.ToDecimal(gridView1.GetFocusedRowCellValue("Quantity"));
                        Rate = Convert.ToDecimal(gridView1.GetFocusedRowCellValue(dtBandCol.Columns[b] + "Rate"));
                        amt = Qty * Rate;
                        gridView1.SetFocusedRowCellValue(dtBandCol.Columns[b] + "Amount", amt);
                    }
                }

                if (gridView1.Columns.Count > 0)
                {
                    for (int b = 1; b < dtBandCol.Columns.Count; b++)
                    {
                        gridView1.Columns[dtBandCol.Columns[b] + "Rate"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                        gridView1.Columns[dtBandCol.Columns[b] + "Rate"].SummaryItem.DisplayFormat = "{0:N3}";

                        gridView1.Columns[dtBandCol.Columns[b] + "QRate"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                        gridView1.Columns[dtBandCol.Columns[b] + "QRate"].SummaryItem.DisplayFormat = "{0:N3}";

                        gridView1.Columns[dtBandCol.Columns[b] + "Amount"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                        gridView1.Columns[dtBandCol.Columns[b] + "Amount"].SummaryItem.DisplayFormat = "{0:N3}";                            
                           
                    }

                }
            }
        }
        #endregion
    }
}
