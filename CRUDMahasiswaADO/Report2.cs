using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class Report2 : Form
    {
        DAL dbLogic = new DAL();


        public Report2()
        {
            InitializeComponent();

        }

        private void Report2_Load(object sender, EventArgs e)
        {
            dtpTanggalMasuk.Format = DateTimePickerFormat.Custom;
            dtpTanggalMasuk.CustomFormat = "yyyy";
            dtpTanggalMasuk.ShowUpDown = true;
            dtpTanggalMasuk.MinDate = new DateTime(2000, 1, 1);
            dtpTanggalMasuk.MaxDate = DateTime.Now;

            cmbProdi.DropDownStyle = ComboBoxStyle.DropDownList;

            btnRekapData.Enabled = false;

            try
            {
                cmbProdi.DataSource = dbLogic.getProdi();
                cmbProdi.DisplayMember = "namaprodi";
                cmbProdi.ValueMember = "namaprodi";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtMahasiswa = dbLogic.getDataRekap(cmbProdi.SelectedValue.ToString(), dtpTanggalMasuk.Value);
                dataGridView1.DataSource = dtMahasiswa;


                if (dtMahasiswa.Rows.Count > 0)
                {
                    btnRekapData.Enabled = true;
                }
                else
                {
                    btnRekapData.Enabled = false;
                    MessageBox.Show("Data tidak ditemukan");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void btnRekapData_Click(object sender, EventArgs e)
        {
            FormReports frm2 = new FormReports(cmbProdi.SelectedValue.ToString(), dtpTanggalMasuk.Value);
            frm2.Show();
            this.Hide();
        }
    }
}
