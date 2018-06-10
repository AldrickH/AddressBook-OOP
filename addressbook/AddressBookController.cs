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
    public class AddressBookController
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

        public void insertData(bool mode, People ppl, People temp)
        {
            try
            {
                if (mode)
                {
                    addData(ppl);
                }
                else // edit data
                {
                    editData(ppl, temp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "AddressBook", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void addData(People ppl)
        {
            try
            {
                if (itemExist(ppl)) throw new Exception("Sorry, you can't add a same data");
                listData.Add(ppl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void editData(People ppl, People temp)
        {
            try
            {
                if (itemExist(temp))
                {
                    for (int i = 0; i < listData.Count; i++)
                    {
                        People data = listData[i];
                        if (data.Nama.ToLower().Equals(temp.Nama.ToLower()) && data.Alamat.ToLower().Equals(temp.Alamat.ToLower()) &&
                            data.Kota.ToLower().Equals(temp.Kota.ToLower()) && data.NoHP.ToLower().Equals(temp.NoHP.ToLower()) &&
                            data.Tanggal.ToShortDateString().Equals(temp.Tanggal.ToShortDateString()) && data.Email.ToLower().Equals(temp.Email.ToLower()))
                        {
                            listData[i] = ppl;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "AddressBook", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void deleteData(People ppl)
        {
            try
            {
                if (itemExist(ppl))
                {
                    People dataToDelete = null;
                    for (int i = 0; i < listData.Count; i++)
                    {
                        dataToDelete = listData[i];
                        if (dataToDelete.Nama.ToLower().Equals(ppl.Nama.ToLower()) && dataToDelete.Alamat.ToLower().Equals(ppl.Alamat.ToLower()) &&
                            dataToDelete.Kota.ToLower().Equals(ppl.Kota.ToLower()) && dataToDelete.NoHP.ToLower().Equals(ppl.NoHP.ToLower()) &&
                            dataToDelete.Tanggal.ToShortDateString().Equals(ppl.Tanggal.ToShortDateString()) && dataToDelete.Email.ToLower().Equals(ppl.Email.ToLower()))
                        {
                            break;
                        }
                    }
                    if (dataToDelete != null) listData.Remove(dataToDelete);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void saveData()
        {
            try
            {
                using (FileStream fs = new FileStream("temporary.csv", FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        foreach (People ppl in listData)
                        {
                            writer.WriteLine($"{ppl.Nama};{ppl.Alamat};{ppl.Kota};{ppl.NoHP};{ppl.Tanggal.ToShortDateString()};{ppl.Email}");
                        }
                    }
                }
                if (File.Exists(Properties.Settings.Default.NamaFile)) File.Delete(Properties.Settings.Default.NamaFile);
                File.Move("temporary.csv", Properties.Settings.Default.NamaFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<People> filterData(string nama = "", string alamat = "", string kota = "", string noHp = "", string tglLahir = "", string email = "")
        {
            List<People> listQuery = null;
            try
            {
                if (nama != "" || alamat != "" || kota != "" || noHp != "" || tglLahir != "" || email != "")
                {
                    if (listData?.Count > 0)
                    {
                        listQuery = new List<People>();
                        foreach (People data in listData)
                        {
                            bool benar = false;
                            if ((nama != "" && data.Nama.ToLower().Contains(nama.ToLower().Trim()))
                        || (alamat != "" && data.Alamat.ToLower().Contains(alamat.ToLower().Trim()))
                        || (kota != "" && data.Kota.ToLower().Contains(kota.ToLower().Trim()))
                        || (noHp != "" && data.NoHP.ToLower().Contains(noHp.ToLower().Trim()))
                        || (email != "" && data.Email.ToLower().Contains(email.ToLower().Trim())))
                            {
                                if (tglLahir != "")
                                {
                                    DateTime tglDari, tglSampai;
                                    if (tglLahir.Contains("-"))
                                    {
                                        string[] arrTanggal = tglLahir.Split('-');
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
                                        if (!DateTime.TryParse(tglLahir, out tglDari))
                                        {
                                            throw new Exception("Sorry, kriteria tanggal lahir tidak valid ...");
                                        }
                                        tglSampai = tglDari;
                                    }
                                    if (data.Tanggal.Date >= tglDari.Date && data.Tanggal.Date <= tglSampai.Date) benar = true;
                                }
                                benar = true;
                            }
                            if (benar)
                            {
                                listQuery.Add(data);
                            }
                        }
                    }
                }
                else
                {
                    if (listData?.Count > 0) listQuery = listData;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listQuery;
        }

        private bool itemExist(People temp)
        {
            if (listData?.Count > 0)
            {
                foreach (People data in listData)
                {
                    if (data.Nama.ToLower().Equals(temp.Nama.ToLower().Trim()) &&
                        data.Alamat.ToLower().Equals(temp.Alamat.ToLower().Trim()) &&
                        data.Kota.ToLower().Equals(temp.Kota.ToLower().Trim()) &&
                        data.NoHP.ToLower().Equals(temp.NoHP.ToLower().Trim()) &&
                        data.Tanggal.ToShortDateString().Equals(temp.Tanggal.ToShortDateString()) &&
                        data.Email.ToLower().Equals(temp.Email.ToLower().Trim())
                        )
                        return true;
                }
            }
            return false;
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

