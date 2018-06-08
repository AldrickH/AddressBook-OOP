using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; // file
using System.Text.RegularExpressions;

namespace addressbook
{
    public partial class FrmTambahData : Form
    {
        People temp = null;
        People p_New = new People();
        bool _addMode = false; // klo true = additem , false = edit item
        AddressBookController addr = new AddressBookController();

        public void Run()
        {
            this.ShowDialog();
        }

        public FrmTambahData(bool addMode, People ppl = null)
        {
            InitializeComponent();
            _addMode = addMode;

            if (ppl != null)
            {
                temp = ppl;
                this.txtNama.Text = ppl.Nama;
                this.txtAlamat.Text = ppl.Alamat;
                this.txtKota.Text = ppl.Kota;
                this.txtNoHP.Text = ppl.NoHP;
                this.dtpTglLahir.Value = ppl.Tanggal.Date;
                this.txtEmail.Text = ppl.Email;
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            // validasi
            if (this.txtNama.Text.Trim() == "")
            {
                MessageBox.Show("nama wajib diisi ... ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtNama.Focus();
            }
            else if (this.txtAlamat.Text.Trim() == "")
            {
                MessageBox.Show("Alamat wajib diisi ... ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtAlamat.Focus();
            }
            else if (this.txtKota.Text.Trim() == "")
            {
                MessageBox.Show("Kota wajib diisi ... ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtKota.Focus();
            }
            else if (this.txtNoHP.Text.Trim() == "")
            {
                MessageBox.Show("No HP wajib diisi ... ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtNoHP.Focus();
            }
            else if (this.txtEmail.Text.Trim() == "")
            {
                MessageBox.Show("Email wajib diisi ... ", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtEmail.Focus();
            }
            else
            { 
                addr.saveData(_addMode,initial_people(p_New), temp);
                this.Close();
            }
        }

        private void txtNama_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) SendKeys.Send("{tab}");
        }

        private void txtEmail_Leave(object sender, EventArgs e)
        {
            if (this.txtEmail.Text.Trim() != "")
            {
                if (!addr.EmailIsValid(this.txtEmail.Text))
                {
                    MessageBox.Show("Sorry, data email tidak valid ...", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.txtEmail.Clear();
                    this.txtEmail.Focus();
                }
            }
        }

        private void txtNoHp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar) || e.KeyChar == (char)Keys.Back || e.KeyChar == '.' || e.KeyChar == ' ' || e.KeyChar == '-' || e.KeyChar == '(' || e.KeyChar == ')')
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private People initial_people(People p)
        {
            p.Nama = this.txtNama.Text;
            p.Alamat = this.txtAlamat.Text;
            p.Kota = this.txtKota.Text;
            p.NoHP = this.txtNoHP.Text;
            p.Tanggal = this.dtpTglLahir.Value;
            p.Email = this.txtEmail.Text;

            return p;
        }
    }
}
