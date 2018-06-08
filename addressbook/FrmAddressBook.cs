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
            FrmTambahData frm = new FrmTambahData(false, initial_people(p,true));
            frm.Run();

            addr.loadData(this.dgvData, this.lblBanyakRecordData);
        }

        private void dgvData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            this.btnEdit_Click(null, null);
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (this.txtNama.Text.Trim() != "" || this.txtAlamat.Text.Trim() != "" || this.txtKota.Text.Trim() != "" || this.txtNoHP.Text.Trim() != "" || this.txtTglLahir.Text.Trim() != "" || this.txtEmail.Text.Trim() != "")
            {
                try
                {
                    this.dgvData.Rows.Clear();
                    string[] fileContent = File.ReadAllLines("addressbook.csv");
                    foreach (string line in fileContent)
                    {
                        bool benar = false;
                        string[] arrItem = line.Split(';');
                        if (arrItem[0].ToLower().Contains(this.txtNama.Text.ToLower())) benar = true;
                        if (arrItem[1].ToLower().Contains(this.txtAlamat.Text.ToLower())) benar = true;
                        if (arrItem[2].ToLower().Contains(this.txtKota.Text.ToLower())) benar = true;
                        if (arrItem[3].ToLower().Contains(this.txtNoHP.Text.ToLower())) benar = true;
                        if (arrItem[5].ToLower().Contains(this.txtEmail.Text.ToLower())) benar = true;
                        if (this.txtTglLahir.Text != "")
                        {
                            DateTime tglDari, tglSampai;
                            if (this.txtTglLahir.Text.Trim().Contains("-"))
                            {
                                string[] arrTanggal = this.txtTglLahir.Text.Split('-');
                                if (!DateTime.TryParse(arrTanggal[0], out tglDari))
                                {
                                    throw new Exception("Sorry, kriteria tanggal lahir tidak valid ...");
                                }
                                if (!DateTime.TryParse(arrTanggal[1], out tglSampai))
                                {
                                    throw new Exception("Sorry, kriteria tanggal lahir tidak valid ...");
                                }
                            }
                            else
                            {
                                if (!DateTime.TryParse(this.txtTglLahir.Text, out tglDari))
                                {
                                    throw new Exception("Sorry, kriteria tanggal lahir tidak valid ...");
                                }
                                tglSampai = tglDari;
                            }
                            DateTime tglLahir = Convert.ToDateTime(arrItem[4]);
                            if (tglLahir.Date >= tglDari.Date && tglLahir.Date <= tglSampai.Date) benar = true;
                        }
                        if (benar)
                        {
                            this.dgvData.Rows.Add(new string[] { arrItem[0], arrItem[1], arrItem[2], arrItem[3], arrItem[4], arrItem[5] });
                        }
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                addr.loadData(this.dgvData, this.lblBanyakRecordData);
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            addr.deleteData(initial_people(p,true));
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
                p.Nama = this.txtNama.Text;
                p.Alamat = this.txtNama.Text;
                p.Kota = this.txtKota.Text;
                p.NoHP = this.txtNoHP.Text;
                p.Tanggal = Convert.ToDateTime(this.txtTglLahir.Text);
                p.Email = this.txtEmail.Text;
            }

            return p;
        }
    }
}
