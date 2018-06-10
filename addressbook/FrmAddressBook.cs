using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace addressbook
{
    public partial class FrmAddressBook : Form
    {
        People p = new People();
        AddressBookController controller = new AddressBookController();

        public FrmAddressBook()
        {
            InitializeComponent();
            this.dgvData.AutoGenerateColumns = false;
        }

        private void FrmAddressBook_Load(object sender, EventArgs e)
        {
            try
            {
                this.dgvData.DataSource = controller.listData;
                this.dgvData.Columns[0].DataPropertyName = "Nama";
                this.dgvData.Columns[1].DataPropertyName = "Alamat";
                this.dgvData.Columns[2].DataPropertyName = "Kota";
                this.dgvData.Columns[3].DataPropertyName = "NoHP";
                this.dgvData.Columns[4].DataPropertyName = "Tanggal";
                this.dgvData.Columns[4].DefaultCellStyle.Format = "dd/MM/yyyy";
                this.dgvData.Columns[5].DataPropertyName = "Email";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                this.lblBanyakRecordData.Text = $"{this.dgvData.Rows.Count.ToString("n0")} Record.";
            }
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            FrmTambahData frm = new FrmTambahData(controller, true);
            frm.Run();
            this.dgvData.DataSource = null;
            this.dgvData.DataSource = controller.listData;
            this.btnFilter_Click(null, null);

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                FrmTambahData frm = new FrmTambahData(controller, false, initial_people(p));
                frm.Run();
                this.dgvData.DataSource = null;
                this.dgvData.DataSource = controller.listData;
                this.btnFilter_Click(null, null);
            }
            catch (Exception)
            {
                MessageBox.Show("Data belum ada, tambah data terlebih dahulu", "Edit item", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.btnEdit_Click(null, null);
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (controller?.listData.Count > 0)
            {
                try
                {
                    this.dgvData.DataSource = null;
                    var listQuery = controller.filterData(this.txtNama.Text.Trim(), this.txtAlamat.Text.Trim(), this.txtKota.Text.Trim(), this.txtNoHP.Text.Trim(), this.txtTglLahir.Text.Trim(), this.txtEmail.Text.Trim());
                    this.dgvData.DataSource = listQuery;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    this.lblBanyakRecordData.Text = $"{this.dgvData.Rows.Count.ToString("n0")} Record.";
                }
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (this.dgvData.SelectedRows.Count > 0 && MessageBox.Show("Hapus data terpilih ? ", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                controller.deleteData(initial_people(p));
                this.btnFilter_Click(null, null);
            }
        }

        private void FrmAddressBook_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Save data ? ", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                controller.saveData();
            }
        }

        private People initial_people(People p)
        {
            p.Nama = dgvData.CurrentRow.Cells[0].Value.ToString();
            p.Alamat = dgvData.CurrentRow.Cells[1].Value.ToString();
            p.Kota = dgvData.CurrentRow.Cells[2].Value.ToString();
            p.NoHP = dgvData.CurrentRow.Cells[3].Value.ToString();
            p.Tanggal = Convert.ToDateTime(dgvData.CurrentRow.Cells[4].Value).Date;
            p.Email = dgvData.CurrentRow.Cells[5].Value.ToString();

            return p;
        }
    }
}
