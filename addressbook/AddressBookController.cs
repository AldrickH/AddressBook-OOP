using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace addressbook
{
    class AddressBookController
    {
        public List<People> listData { get; set; }

        public AddressBookController()
        {
            listData = new List<People>();
            try
            {
                if (File.Exists(Properties.Settings.Default.NamaFile))
                {
                    string[] fileContent = File.ReadAllLines(Properties.Settings.Default.NamaFile);
                    foreach (string item in fileContent)
                    {
                        string[] arrItem = item.Split(';');
                        listData.Add(new People { Nama = arrItem[0].Trim(), Alamat = arrItem[1].Trim(), Kota = arrItem[2].Trim(), NoHP = arrItem[3].Trim(), Tanggal = Convert.ToDateTime(arrItem[4].Trim()), Email = arrItem[5].Trim() });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void loadData(DataGridView dgv, Label lbl)
        {
            try
            {
                dgv.Rows.Clear();
                AddressBookController controller = new AddressBookController();
                List<People> listData = controller.listData;
                foreach (People ppl in listData)
                {
                    dgv.Rows.Add(new string[] { ppl.Nama, ppl.Alamat, ppl.Kota, ppl.NoHP, ppl.Tanggal.ToShortDateString(), ppl.Email });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "AddressBook", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                lbl.Text = $"{dgv.Rows.Count.ToString("n0")} Record data.";
            }
        }

        public void saveData(bool mode, People ppl, People temp)
        {
            try
            {
                if (mode)
                {
                    using (var fs = new FileStream(Properties.Settings.Default.NamaFile, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(fs))
                        {
                            writer.WriteLine($"{ppl.Nama};{ppl.Alamat};{ppl.Kota};{ppl.NoHP};{ppl.Tanggal.ToShortDateString()};{ppl.Email}"); // nama;alamat, ....
                        }
                    }
                }
                else // edit data
                {
                    string[] line = File.ReadAllLines(Properties.Settings.Default.NamaFile);
                    using (var fs = new FileStream("temporary.csv", FileMode.Create, FileAccess.ReadWrite))
                    {
                        using (StreamWriter writer = new StreamWriter(fs))
                        {
                            for (int i = 0; i < line.Length; i++)
                            {
                                if (line[i] == $"{temp.Nama};{temp.Alamat};{temp.Kota};{temp.NoHP};{temp.Tanggal.ToShortDateString()};{temp.Email}")
                                {
                                    writer.WriteLine($"{ppl.Nama};{ppl.Alamat};{ppl.Kota};{ppl.NoHP};{ppl.Tanggal.ToShortDateString()};{ppl.Email}");
                                }
                                else
                                {
                                    writer.WriteLine(line[i]);
                                }
                            }
                        }
                    }
                    File.Delete(Properties.Settings.Default.NamaFile);
                    File.Move("temporary.csv", Properties.Settings.Default.NamaFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "AddressBook", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void deleteData(People ppl)
        {
            if (MessageBox.Show("Hapus Baris Data Terpilih ? ", "Delete Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string[] line = File.ReadAllLines(Properties.Settings.Default.NamaFile);
                using (var fs = new FileStream("temporary.csv", FileMode.Create, FileAccess.ReadWrite))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        for (int i = 0; i < line.Length; i++)
                        {
                            if (line[i] != $"{ppl.Nama};{ppl.Alamat};{ppl.Kota};{ppl.NoHP};{ppl.Tanggal.ToShortDateString()};{ppl.Email}")
                            {
                                writer.WriteLine(line[i]);
                            }
                        }
                    }
                    File.Delete(Properties.Settings.Default.NamaFile);
                    File.Move("temporary.csv", Properties.Settings.Default.NamaFile);
                }
            }
        }

        public void filterData(DataGridView dgv, People ppl, Label lbl)
        {
            try
            {
                MessageBox.Show($"{ppl.Nama};{ppl.Alamat};{ppl.Kota};{ppl.NoHP};{ppl.Tanggal.ToShortDateString()};{ppl.Email}");
                dgv.Rows.Clear();
                string[] fileContent = File.ReadAllLines(Properties.Settings.Default.NamaFile);
                foreach (string line in fileContent)
                {
                    bool benar = false;
                    string[] arrItem = line.Split(';');
                    if ((ppl.Nama != "" && arrItem[0].ToLower().Contains(ppl.Nama.ToLower().Trim()))
                        || (ppl.Alamat != "" && arrItem[1].ToLower().Contains(ppl.Alamat.ToLower().Trim()))
                        || (ppl.Kota != "" && arrItem[2].ToLower().Contains(ppl.Kota.ToLower().Trim()))
                        || (ppl.NoHP != "" && arrItem[3].ToLower().Contains(ppl.NoHP.ToLower().Trim()))
                        || (ppl.Tanggal.ToShortDateString() != "" && arrItem[4].ToLower().Contains(ppl.Tanggal.ToShortDateString()))
                        || (ppl.Email != "" && arrItem[5].ToLower().Contains(ppl.Email.ToLower().Trim())))
                    {
                        DateTime tglDari, tglSampai;
                        if (ppl.Tanggal.ToShortDateString().Trim().Contains("-"))
                        {
                            string[] arrTanggal = ppl.Tanggal.ToShortDateString().Split('-');
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
                            if (!DateTime.TryParse(ppl.Tanggal.ToShortDateString(), out tglDari))
                            {
                                throw new Exception("Sorry, kriteria tanggal lahir tidak valid ...");
                            }
                            tglSampai = tglDari;
                        }
                        DateTime tglLahir = Convert.ToDateTime(arrItem[4]);
                        if (tglLahir.Date >= tglDari.Date && tglLahir.Date <= tglSampai.Date) benar = true;
                        benar = true;
                    }
                    if (benar)
                    {
                        dgv.Rows.Add(new string[] { arrItem[0], arrItem[1], arrItem[2], arrItem[3], arrItem[4], arrItem[5] });
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("", "Filter Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                lbl.Text = $"{dgv.Rows.Count.ToString("n0")} Record data.";
            }
        }


        public bool EmailIsValid(string emailAddr)
        {
            string emailPattern1 = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            Regex regex = new Regex(emailPattern1);
            Match match = regex.Match(emailAddr);
            return match.Success;
        }

    }
}

