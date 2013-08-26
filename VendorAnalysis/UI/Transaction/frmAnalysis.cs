using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.Data;
using System.Reflection;
using DevExpress.Utils.Paint;

namespace VendorAnalysis
{
    public partial class frmAnalysis : Form
    {
        VendorAnalysis.BusinessLayer.VendorCompareBL m_oVendor;
        int m_iQId = 0;
        string m_stype = "";

        public frmAnalysis()
        {
            InitializeComponent();
            m_oVendor = new BusinessLayer.VendorCompareBL();
        }

        public void Execute(int argQId,string argtype)
        {
            m_iQId = argQId;
            m_stype = argtype;
            if (BsfGlobal.g_bWorkFlow == true) { this.Show();}
            else { this.Show(); }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            clsStatics.DW2.Show();
            clsStatics.DW3.Hide();
            clsStatics.DW1.Hide();
            //this.Close();
        }

        private void frmAnalysis_Load(object sender, EventArgs e)
        {
            SetMyGraphics();
            barEditItem1.EditValue = 0;
          
            //repositoryItemTextEdit1.Mask.EditMask = "##.##";
            //repositoryItemTextEdit1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            //repositoryItemTextEdit1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            PopulateGrid();
        }
        private void SetMyGraphics()
        {
            FieldInfo fi = typeof(XPaint).GetField("graphics", BindingFlags.Static | BindingFlags.NonPublic);
            fi.SetValue(null, new MyXPaint());
        }

        public class MyXPaint : XPaint
        {
            public override void DrawFocusRectangle(Graphics g, Rectangle r, Color foreColor, Color backColor)
            {
                if (!CanDraw(r)) return;
                Brush hb = Brushes.Red;
                g.FillRectangle(hb, new Rectangle(r.X, r.Y, 2, r.Height - 2)); // left
                g.FillRectangle(hb, new Rectangle(r.X, r.Y, r.Width - 2, 2)); // top
                g.FillRectangle(hb, new Rectangle(r.Right - 2, r.Y, 2, r.Height - 2)); // right
                g.FillRectangle(hb, new Rectangle(r.X, r.Bottom - 2, r.Width, 2)); // bottom
            }
        }
        private void PopulateGrid()
        {
            barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barEditItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barEditItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            DataSet ds = new DataSet();
            DataTable dtV = new DataTable();
            DataTable dt = new DataTable();
            ds = m_oVendor.GetVendorAnalysis(m_iQId);
            dtV = ds.Tables[0];
            dt = ds.Tables[1];
            gridControl1.DataSource = dt;
            advBandedGridView1.PopulateColumns();


            DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit txtSpec = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            txtSpec.AutoHeight = true;
            advBandedGridView1.OptionsView.RowAutoHeight = true;
            advBandedGridView1.Columns["Spec"].Width = 250;
            advBandedGridView1.Columns["Spec"].ColumnEdit = txtSpec;
            advBandedGridView1.Bands.Clear();

            GridBand dBand = new GridBand();
            BandedGridColumn dBandC = new BandedGridColumn();
            dBand.Name = "General";
            advBandedGridView1.Bands.Add(dBand);
            dBandC = advBandedGridView1.Columns[1];
            dBandC.Caption = "Serial No";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);


            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[2];
            dBandC.Caption = "Specification";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);

            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[3];
            dBandC.Caption = "Unit";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);

            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[4];
            dBandC.Caption = "Qty";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);          
            dBand.Columns.Add(dBandC);

            
            dBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBand.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;

            dBand = new GridBand();
            dBand.Name = "Estimate";
            advBandedGridView1.Bands.Add(dBand);

            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[5];
            dBandC.Caption = "Rate";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);

            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[6];
            dBandC.Caption = "Amount";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);

            dBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBand.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);

            int j = 7;
            for (int i = 0; i <= dtV.Rows.Count - 1; i++)
            {
                dBand = new GridBand();
                dBand.Name = dtV.Rows[i]["Vendor"].ToString();
                dBand.Caption = dtV.Rows[i]["VendorName"].ToString();
                advBandedGridView1.Bands.Add(dBand);

                dBandC = new BandedGridColumn();
                dBandC = advBandedGridView1.Columns[j];
                dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
                dBandC.Width = 100;
                advBandedGridView1.Columns[j].Caption = "Rate";
                advBandedGridView1.Columns[j].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                advBandedGridView1.Columns[j].DisplayFormat.FormatString = "{0:N2}";
                dBand.Columns.Add(dBandC);

                dBandC = new BandedGridColumn();
                dBandC = advBandedGridView1.Columns[j + 1];
                dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
                dBandC.Width = 100;
                advBandedGridView1.Columns[j + 1].Caption = "QRate";
                advBandedGridView1.Columns[j].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                advBandedGridView1.Columns[j].DisplayFormat.FormatString = "{0:N2}";
                dBand.Columns.Add(dBandC);

                dBandC = new BandedGridColumn();
                dBandC = advBandedGridView1.Columns[j + 2];
                dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
                dBandC.Width = 100;
                advBandedGridView1.Columns[j + 2].Caption = "Amount";
                advBandedGridView1.Columns[j+2].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                //advBandedGridView1.Columns[j+2].DisplayFormat.FormatString = "{0:N2}";
                advBandedGridView1.Columns[j + 2].SummaryItem.SummaryType = SummaryItemType.Sum;
                advBandedGridView1.Columns[j + 2].SummaryItem.DisplayFormat = "N2";


                dBand.Columns.Add(dBandC);

                dBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBand.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
                
                j = j + 3;
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PopulateGrid();
            
        }

        private void PopulateGridVar()
        {

            decimal dVar =Convert.ToDecimal (clsStatics.IsNullCheck(barEditItem1.EditValue.ToString(),clsStatics.datatypes.vartypenumeric));


            barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            barEditItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barEditItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            DataSet ds = new DataSet();
            DataTable dtV = new DataTable();
            DataTable dt = new DataTable();
            ds = m_oVendor.GetVendorAnalysisVariance(m_iQId);
            dtV = ds.Tables[0];
            string sStr="";
            int m=9;

            if (dVar !=0)
            {
                sStr = "";

                DataView dv= new DataView(ds.Tables[1]);
                while (m < ds.Tables[1].Columns.Count-1)
                {
                    
                	sStr =  sStr + " " + m.ToString () + "  = " + dVar + " and";
  
                    m=m+3;
                }
                
                if (sStr != "") 
                {
                    sStr = sStr.Substring(0, sStr.ToString().Length - 4);
                    dv.RowFilter = sStr;
                    dt = dv.ToTable();
                }

            }
            else {dt = ds.Tables[1];}

            gridControl1.DataSource = dt;
            advBandedGridView1.PopulateColumns();

            DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit txtSpec = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            txtSpec.AutoHeight = true;
            advBandedGridView1.OptionsView.RowAutoHeight = true;
            advBandedGridView1.Columns["Spec"].Width = 250;
            advBandedGridView1.Columns["Spec"].ColumnEdit = txtSpec;
            advBandedGridView1.Bands.Clear();

            GridBand dBand = new GridBand();
            BandedGridColumn dBandC = new BandedGridColumn();
            dBand.Name = "General";
            advBandedGridView1.Bands.Add(dBand);
            dBandC = advBandedGridView1.Columns[1];
            dBandC.Caption = "Serial No";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);

            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[2];
            dBandC.Caption = "Specification";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);

            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[3];
            dBandC.Caption = "Unit";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);

            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[4];
            dBandC.Caption = "Qty";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);


            dBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBand.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;

            dBand = new GridBand();
            dBand.Name = "Estimate";
            advBandedGridView1.Bands.Add(dBand);

            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[5];
            dBandC.Caption = "Rate";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);

            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[6];
            dBandC.Caption = "Amount";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);

            dBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBand.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);

            int j = 7;
            for (int i = 0; i <= dtV.Rows.Count - 1; i++)
            {
                dBand = new GridBand();
                dBand.Name = dtV.Rows[i]["VendorName"].ToString();
                dBand.Caption = dtV.Rows[i]["VendorName"].ToString();
                advBandedGridView1.Bands.Add(dBand);

                dBandC = new BandedGridColumn();
                dBandC = advBandedGridView1.Columns[j];
                dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
                dBandC.Width = 100;
                advBandedGridView1.Columns[j].Caption = "Rate";
                advBandedGridView1.Columns[j].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                advBandedGridView1.Columns[j].DisplayFormat.FormatString = "{0:N2}";

                dBand.Columns.Add(dBandC);

                dBandC = new BandedGridColumn();
                dBandC = advBandedGridView1.Columns[j + 1];
                dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
                dBandC.Width = 100;
                advBandedGridView1.Columns[j + 1].Caption = "Amt";
                advBandedGridView1.Columns[j].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                advBandedGridView1.Columns[j].DisplayFormat.FormatString = "{0:N2}";

                dBand.Columns.Add(dBandC);

                dBandC = new BandedGridColumn();
                dBandC = advBandedGridView1.Columns[j + 2];
                dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
                dBandC.Width = 100;
                advBandedGridView1.Columns[j + 2].Caption = "Variance";
                advBandedGridView1.Columns[j].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                advBandedGridView1.Columns[j].DisplayFormat.FormatString = "{0:N2}";


                dBand.Columns.Add(dBandC);

                dBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBand.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);

                j = j + 3;
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PopulateGridVar();
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            advBandedGridView1.ShowPrintPreview();
        }

        private void PopulateGridItemWise()
        {
            barStaticItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barEditItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            barEditItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            DataSet ds = new DataSet();
            DataTable dtV = new DataTable();
            DataTable dt = new DataTable();
            ds = m_oVendor.GetVendorItemAnalysis(m_iQId);
            dtV = ds.Tables[0];
            dt = ds.Tables[1];
            gridControl1.DataSource = dt;
            advBandedGridView1.PopulateColumns();


            DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit txtSpec = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
            txtSpec.AutoHeight = true;
            advBandedGridView1.OptionsView.RowAutoHeight = true;
            advBandedGridView1.Columns["Spec"].Width = 250;
            advBandedGridView1.Columns["Spec"].ColumnEdit = txtSpec;
            advBandedGridView1.Bands.Clear();

            GridBand dBand = new GridBand();
            BandedGridColumn dBandC = new BandedGridColumn();
            dBand.Name = "General";
            advBandedGridView1.Bands.Add(dBand);
            dBandC = advBandedGridView1.Columns[1];
            dBandC.Caption = "Serial No";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);
            
            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[2];
            dBandC.Caption = "Specification";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);

            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[3];
            dBandC.Caption = "Unit";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);

            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[4];
            dBandC.Caption = "Qty";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);

            dBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBand.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;

            dBand = new GridBand();
            dBand.Name = "Estimate";
            advBandedGridView1.Bands.Add(dBand);

            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[5];
            dBandC.Caption = "Rate";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);

            dBandC = new BandedGridColumn();
            dBandC = advBandedGridView1.Columns[6];
            dBandC.Caption = "Amount";
            dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            dBand.Columns.Add(dBandC);

            dBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            dBand.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);

            int j = 7;
            for (int i = 0; i <= dtV.Rows.Count - 1; i++)
            {
                dBand = new GridBand();
                dBand.Name = dtV.Rows[i]["VendorName"].ToString();
                dBand.Caption = dtV.Rows[i]["VendorName"].ToString();
                advBandedGridView1.Bands.Add(dBand);

                dBandC = new BandedGridColumn();
                dBandC = advBandedGridView1.Columns[j];
                dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
                dBandC.Width = 100;
                advBandedGridView1.Columns[j].Caption = "Rate";
                advBandedGridView1.Columns[j + 1].SummaryItem.SummaryType = SummaryItemType.Sum;
                advBandedGridView1.Columns[j + 1].SummaryItem.DisplayFormat = "{0:N2}";
                dBand.Columns.Add(dBandC);

                dBandC = new BandedGridColumn();
                dBandC = advBandedGridView1.Columns[j + 1];
                dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
                dBandC.Width = 100;
                advBandedGridView1.Columns[j + 1].Caption = "QRate";
                advBandedGridView1.Columns[j + 1].SummaryItem.SummaryType = SummaryItemType.Sum;
                advBandedGridView1.Columns[j + 1].SummaryItem.DisplayFormat = "{0:N2}";
                dBand.Columns.Add(dBandC);

                dBandC = new BandedGridColumn();
                dBandC = advBandedGridView1.Columns[j + 2];
                dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
                dBandC.Width = 100;
                advBandedGridView1.Columns[j + 2].Caption = "Amount";
                advBandedGridView1.Columns[j + 2].SummaryItem.SummaryType = SummaryItemType.Sum;
                advBandedGridView1.Columns[j + 2].SummaryItem.DisplayFormat = "N2";

                dBand.Columns.Add(dBandC);

                dBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBand.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);

                j = j + 3;
            }

          
                dBand = new GridBand();
                dBand.Name ="Lowest";
                dBand.Caption = "Lowest Rate";
                advBandedGridView1.Bands.Add(dBand);

                dBandC = new BandedGridColumn();
                dBandC = advBandedGridView1.Columns[j];
                dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
                dBandC.Width = 100;
                advBandedGridView1.Columns[j].Caption = "Rate";
                dBand.Columns.Add(dBandC);

                dBandC = new BandedGridColumn();
                dBandC = advBandedGridView1.Columns[j + 1];
                dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
                dBandC.Width = 100;
                advBandedGridView1.Columns[j + 1].Caption = "Amount";
                dBand.Columns.Add(dBandC);

                dBandC = new BandedGridColumn();
                dBandC = advBandedGridView1.Columns[j + 2];
                dBandC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBandC.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
                dBandC.Width = 100;
                advBandedGridView1.Columns[j + 2].Caption = "Vendor";
                advBandedGridView1.Columns[j + 2].SummaryItem.SummaryType = SummaryItemType.Sum;
                advBandedGridView1.Columns[j + 2].SummaryItem.DisplayFormat = "N2";

                dBand.Columns.Add(dBandC);

                dBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                dBand.AppearanceHeader.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);

        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PopulateGridItemWise();
        }

        private void advBandedGridView1_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            if (e.RowHandle > advBandedGridView1.RowCount - 4)
            {
                e.Appearance.Font = new Font(e.Appearance.Font.Name,8.5F,FontStyle.Bold);
            }

        }

        private void repositoryItemTextEdit1_EditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit cboType = (DevExpress.XtraEditors.TextEdit)sender;
            barEditItem1.EditValue = cboType.EditValue;
            PopulateGridVar();
        }

        private void frmAnalysis_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (BsfGlobal.g_bWorkFlow == true)
            //{
            //    if (BsfGlobal.g_bTrans == true)
            //    {
            //        BsfGlobal.g_oWindowTrans.Hide();
            //        BsfGlobal.g_oWindow.Show();
            //        BsfGlobal.g_bTrans = false;
            //    }
            //    else
            //    {
            //        BsfGlobal.g_oPanel.Hide();
            //    }
            //}
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmCompareList frm = new frmCompareList();
            frm.Execute(m_iQId, m_stype);
        }

       
    }
}
