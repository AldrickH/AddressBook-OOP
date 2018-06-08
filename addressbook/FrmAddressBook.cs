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
        AddressBookController addr = new AddressBookController();

        public FrmAddressBook()
        {
            InitializeComponent();
        }

        private void FrmAddressBook_Load(object sender, EventArgs e)
        {
            addr.loadData(this.dgvData, this.lblBanyakRecordData);
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            FrmTambahData frm = new FrmTambahData(true);
            frm.Run();

            addr.loadData(this.dgvData, this.lblBanyakRecordData);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                FrmTambahData frm = new FrmTambahData(false, initial_people(p, true));
                frm.Run();

                addr.loadData(this.dgvData, this.lblBanyakRecordData);
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
            if (this.txtNama.Text.Trim() != "" || this.txtAlamat.Text.Trim() != "" || this.txtKota.Text.Trim() != "" || this.txtNoHP.Text.Trim() != "" || this.txtTglLahir.Text.Trim() != "" || this.txtEmail.Text.Trim() != "")
            {
                addr.filterData(this.dgvData, initial_people(p, false), this.lblBanyakRecordData);
            }
            else
            {
                addr.loadData(this.dgvData, this.lblBanyakRecordData);
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            addr.deleteData(initial_people(p, true));
            addr.loadData(this.dgvData, this.lblBanyakRecordData);
        }

        private People initial_people(People p, bool mode)
        {
            if (mode)
            {
                p.Nama = dgvData.CurrentRow.Cells[0].Value.ToString();
                p.Alamat = dgvData.CurrentRow.Cells[1].Value.ToString();
                p.Kota = dgvData.CurrentRow.Cells[2].Value.ToString();
                p.NoHP = dgvData.CurrentRow.Cells[3].Value.ToString();
                p.Tanggal = Convert.ToDateTime(dgvData.CurrentRow.Cells[4].Value).Date;
                p.Email = dgvData.CurrentRow.Cells[5].Value.ToString();
            }
            else
            {
                p.Nama = this.txtNama.Text.Trim();
                p.Alamat = this.txtAlamat.Text.Trim();
                p.Kota = this.txtKota.Text.Trim();
                p.NoHP = this.txtNoHP.Text.Trim();
                if(this.txtTglLahir.Text.Trim() != "")
                p.Tanggal = Convert.ToDateTime(this.txtTglLahir.Text.Trim());
                p.Email = this.txtEmail.Text.Trim();
            }

            return p;
        }
    }
}
