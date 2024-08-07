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

namespace yemekUygulamasi
{
    public partial class frmUser : Form
    {
        public frmUser()
        {
            InitializeComponent();
        }

        public static string databasePath = Path.Combine(Application.StartupPath, "db.accdb");
        public string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + databasePath;

        string userid = "";
        private void frmUser_Load(object sender, EventArgs e)
        {

            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\tmdbRoman");
            if (key != null)
            {
                userid = (string)key.GetValue("id");
                key.Close();
            }

            kayitgetir();
            button3.Enabled = false;
            comboBox1.SelectedIndex = 0;
        }
        public void kayitgetir()
        {
            string kayit = "SELECT ad,id,aciklama,resim FROM yemekler ORDER BY ad ASC";
            if (comboBox1.Text == "Favoriler")
            {
                kayit = "SELECT yemekler.id, yemekler.ad, yemekler.aciklama, yemekler.resim FROM yemekler INNER JOIN favoriler ON yemekler.id = CInt(favoriler.yemek_id) WHERE favoriler.user_id ='" + userid + "'";
            }

            OleDbConnection conn = new OleDbConnection(connectionString);
            conn.Open();
            OleDbCommand komut = new OleDbCommand(kayit, conn);
            OleDbDataAdapter da = new OleDbDataAdapter(komut);
            DataTable dt = new DataTable();
            da.FillSchema(dt, SchemaType.Source);
            da.Fill(dt);
            dataGridView1.DataSource = dt;
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
                if (comboBox1.Text != "Favoriler")
                {
                    button1.Enabled = true;
                    button3.Enabled = false;
                }
                else
                {
                    button1.Enabled = false;
                    button3.Enabled = true;
                }
            }
            else
            {
                button1.Enabled = true;
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
            try
            {
                OleDbConnection conn = new OleDbConnection(connectionString);
                conn.Open();

                // Yemek_id'nin favorilerde olup olmadığını kontrol etmek için sorgu
                OleDbCommand checkCmd = new OleDbCommand("SELECT COUNT(*) FROM favoriler WHERE yemek_id = @yemek_id AND user_id = @user_id", conn);
                checkCmd.Parameters.AddWithValue("@yemek_id", textBox2.Text);
                checkCmd.Parameters.AddWithValue("@user_id", userid);
                int count = (int)checkCmd.ExecuteScalar();

                if (count == 0)
                {
                    // Yemek_id favorilere eklenmemiş, favorilere ekle
                    OleDbCommand insertCmd = new OleDbCommand("INSERT INTO favoriler (yemek_id, user_id) VALUES (@yemek_id, @user_id)", conn);
                    insertCmd.Parameters.AddWithValue("@yemek_id", textBox2.Text);
                    insertCmd.Parameters.AddWithValue("@user_id", userid);
                    insertCmd.ExecuteNonQuery();
                    MessageBox.Show("Favorilere başarıyla eklendi!", "Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Yemek_id zaten favorilerde, hata mesajı göster
                    MessageBox.Show("Bu yemek zaten favorilerinizde!");
                }

                conn.Close();
                kayitgetir();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu!" + ex.ToString());
            }

        }

    

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult kapat = new DialogResult();
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            kapat = MessageBox.Show(textBox4.Text + " tarifini favorilerden silmek isiyor musun?", "Emin misin?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (kapat == DialogResult.Yes)
            {



                try
                {
                    int selectedId = Convert.ToInt32(textBox2.Text);
                    OleDbConnection conn = new OleDbConnection(connectionString);
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand("DELETE FROM favoriler WHERE yemek_id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", selectedId);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata oluştu!" + ex.ToString());
                }

                MessageBox.Show(textBox4.Text + " tarifini favorilerden silme işlemi başarı ile gerçekleşti", "Tarif silindi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                
                kayitgetir();
            }
            if (kapat == DialogResult.No)
            {
                MessageBox.Show(textBox4.Text + " tarifini favorilerden silme işlemi iptal edildi", "Tarif silinmedi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
  
        private void frmUser_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            kayitgetir();
        }
    }
}
