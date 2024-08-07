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
using System.Data.OleDb;
using System.Net;
using System.Net.Mail;
using app = Microsoft.Office.Interop.Outlook;

namespace yemekUygulamasi
{
    public partial class frmAdmin : Form
    {
        public frmAdmin()
        {
            InitializeComponent();
        }

        public static string databasePath = Path.Combine(Application.StartupPath, "db.accdb");
        public string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + databasePath;


        private void frmAdmin_Load(object sender, EventArgs e)
        {
            kayitgetir();
            button2.Enabled = false;
            button3.Enabled = false;
        }
        public void kayitgetir()
        {
            OleDbConnection conn = new OleDbConnection(connectionString);
            conn.Open();
            string kayit = "SELECT ad,id,aciklama,resim FROM yemekler ORDER BY ad ASC";
            OleDbCommand komut = new OleDbCommand(kayit, conn);
            //Sorgumuzu ve baglantimizi parametre olarak alan bir SqlCommand nesnesi oluşturuyoruz.
            OleDbDataAdapter da = new OleDbDataAdapter(komut);
            //SqlDataAdapter sınıfı verilerin databaseden aktarılması işlemini gerçekleştirir.
            DataTable dt = new DataTable();
            da.Fill(dt);
            //Bir DataTable oluşturarak DataAdapter ile getirilen verileri tablo içerisine dolduruyoruz.
            dataGridView1.DataSource = dt;
            //Formumuzdaki DataGridViewin veri kaynağını oluşturduğumuz tablo olarak gösteriyoruz.
            conn.Close();
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.Columns["aciklama"].Visible = false;
            dataGridView1.Columns["resim"].Visible = false;
            dataGridView1.ClearSelection();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox4.Text = "";
            pictureBox2.Image = Properties.Resources.logos;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = string.Format("ad LIKE '%{0}%'", textBox3.Text);
        }
        int yemekId = 0;
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                yemekId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value);
                string yemekAdi = dataGridView1.Rows[e.RowIndex].Cells["ad"].Value.ToString();
                string yemekAciklamasi = dataGridView1.Rows[e.RowIndex].Cells["aciklama"].Value.ToString();
                pictureBox2.Image = Properties.Resources.logos;
                if (dataGridView1.Rows[e.RowIndex].Cells["resim"].Value.ToString() != "")
                {
                    byte[] resim = (byte[])dataGridView1.Rows[e.RowIndex].Cells["resim"].Value;
                    pictureBox2.Image = Image.FromStream(new MemoryStream(resim));
                }

                textBox4.Text = yemekAdi;
                textBox2.Text = yemekId.ToString();
                textBox1.Text = yemekAciklamasi;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                button1.Enabled = false;
                button2.Enabled = true;
                button3.Enabled = true;
            }
            else
            {
                button1.Enabled = true;
                button2.Enabled = false;
                button3.Enabled = false;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            textBox2.Text = "";
            textBox1.Text = "";
            textBox4.Text = "";
            pictureBox2.Image = Properties.Resources.logos;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Boş veri ekleyemezsiniz!", "Hata oluştu!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            try
            {
                OleDbConnection conn = new OleDbConnection(connectionString);
                conn.Open();
                OleDbCommand cmd = new OleDbCommand("INSERT INTO yemekler (ad, aciklama, resim) VALUES (@ad, @aciklama, @resim)", conn);
                cmd.Parameters.AddWithValue("@ad", textBox4.Text);
                cmd.Parameters.AddWithValue("@aciklama", textBox1.Text);
                cmd.Parameters.Add("@resim", OleDbType.VarBinary).Value = (object)imageBytes ?? DBNull.Value;
                cmd.ExecuteNonQuery();
                conn.Close();

                //bütün users tablosundaki kayıtlı olan mailleri çağır ve
                conn.Open();
                OleDbCommand cmd2 = new OleDbCommand("SELECT mail FROM users", conn);

                using (OleDbDataReader dr = cmd2.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        mail(dr[0].ToString());
                    }
                }

                conn.Close();
                MessageBox.Show("Tarif başarıyla eklendi ve mailler gönderildi!", "Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                kayitgetir();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu!" + ex.ToString());
            }

        }


        public void mail(string adres)
        {
            try
            {
                app.Application outlookApp = new app.Application();
                app.MailItem mailItem = (app.MailItem)outlookApp.CreateItem(app.OlItemType.olMailItem);
                mailItem.Subject = "🌟 Yeni Tarif Eklendi! 🌟 -> " + textBox4.Text;
                mailItem.To = adres;
                mailItem.HTMLBody = "<!DOCTYPE html> <html>   <head>     <meta charset='UTF-8'>     <title>Yeni Tarif Eklendi</title>     <style>       .container {         max-width: 500px;         margin: auto;         padding: 20px;         font-family: sans-serif;         text-align: center;         color: #333;         border: 1px solid #ddd;       }       h1 {         margin-bottom: 10px;       }       p {         margin: 10px 0;       }       .logo {         width: 100px;         margin-bottom: 20px;       }       .emoji {         font-size: 30px;         margin: 0 10px;       }     </style>   </head>   <body>     <div class='container'>       <img class='logo' src='https://i.hizliresim.com/c7ty4xv.png' alt='Yemek Defteri'>       <h1>Mutfakta ŞEF SİZSİNİZ</h1>       <p>🎉🎉🎉 Yaşasın! Yeni Tarif Eklendi 🎉🎉🎉</p>       <p>Tarif Adı: " + textBox4.Text + @"</p>
    <p>Açıklama: " + textBox1.Text.Substring(0, Math.Min(textBox1.Text.Length, 300)) + @"...</p>       <p>Teşekkür ederiz, Yemek Defteri Ekibi</p>       <p>         <span class='emoji'>&#x1F60B;</span>         <span class='emoji'>&#x1F354;</span>         <span class='emoji'>&#x1F373;</span>         <span class='emoji'>&#x1F60A;</span>       </p>     </div>   </body> </html>";
                mailItem.Importance = app.OlImportance.olImportanceHigh;
                mailItem.Send();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Boş veri ekleyemezsiniz!", "Hata oluştu!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            try
            {
                OleDbConnection conn = new OleDbConnection(connectionString);
                conn.Open();
                OleDbCommand cmd = new OleDbCommand("UPDATE yemekler SET ad = @ad, aciklama = @aciklama, resim = @resim WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@ad", textBox4.Text);
                cmd.Parameters.AddWithValue("@aciklama", textBox1.Text);
                cmd.Parameters.Add("@resim", OleDbType.VarBinary).Value = (object)imageBytes ?? DBNull.Value;
                cmd.Parameters.AddWithValue("@id", yemekId);
                cmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show("Yemek başarıyla güncellendi!", "Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                kayitgetir();

            }
            catch (Exception)
            {
                MessageBox.Show("Hata oluştu!");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult kapat = new DialogResult();
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            kapat = MessageBox.Show(dataGridView1.Rows[secilen].Cells[0].Value.ToString() + " tarifini silmek isiyor musun?", "Emin misin?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (kapat == DialogResult.Yes)
            {



                OleDbConnection conn = new OleDbConnection(connectionString);
                conn.Open();
                OleDbCommand cmd = new OleDbCommand("DELETE FROM yemekler WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", yemekId);
                cmd.ExecuteNonQuery();
                conn.Close();


                MessageBox.Show(dataGridView1.Rows[secilen].Cells[0].Value.ToString() + " tarifini silme işlemi başarı ile gerçekleşti", "Tarif silindi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //silme
                kayitgetir();
            }
            if (kapat == DialogResult.No)
            {
                MessageBox.Show(dataGridView1.Rows[secilen].Cells[0].Value.ToString() + " tarifini silme işlemi iptal edildi", "Tarif silinmedi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        byte[] imageBytes;
        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image Files(*.BMP;*.JPG;*.JPEG;*.PNG)|*.BMP;*.JPG;*.JPEG;*.PNG";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (FileStream fs = new FileStream(fileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        imageBytes = new byte[fs.Length];
                        fs.Read(imageBytes, 0, (int)fs.Length);
                    }
                    pictureBox2.Image = Image.FromFile(fileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
        }

        private void frmAdmin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
