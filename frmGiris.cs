using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Threading;

namespace yemekUygulamasi
{
    public partial class frmGiris : Form
    {
        public frmGiris()
        {
            InitializeComponent();
        }



        private void label8_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        Point İlkkonum; // Bu değişkenler Global olarak tanımlanmalı.
        bool durum = false;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            // Mouse a tıklandığı anda. Burada sol yada sağ tıklanması farketmeyecektir.
            durum = true;
            this.Cursor = Cursors.SizeAll; // Fareyi taşıma şeklinde seçim yapılmış ikon halini almasını sağladık.
            İlkkonum = e.Location; /* İlk konum olarak fareye tam basıldığında e parametresinin Location özelliğini
                                    * kullanarak konum aldık. X ve Y koordinatlarını almış olduk.*/

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            // Mouse'u hareket ettirdiğimizde çalışacak kodlar.
            if (durum)
            {
                this.Left = e.X + this.Left - (İlkkonum.X);
                this.Top = e.Y + this.Top - (İlkkonum.Y);
            }

        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            // Mouse'u bıraktığımızda çalışacak kodlar.
            durum = false;
            this.Cursor = Cursors.Default; // Fare işaretçisi Default halini aldı.

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Width = 321;
            MessageBox.Show("Başlamak için lütfen hareket eden görsele tıklayın, arka planda sistem ayarları yapıldığı için program doğrudan açılmamaktadır!", "Yemek Defteri", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\tmdbRoman");
            if (key != null)
            {
                txtUsername.Text = (string)key.GetValue("Username");
                txtPass.Text = (string)key.GetValue("Password");
                txtMail.Text = (string)key.GetValue("Mail");
                cRemember.Checked = true;
                key.Close();
            }

        }
        public static string databasePath = Path.Combine(Application.StartupPath, "db.accdb");
        public string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + databasePath;

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.BackColor != Color.FromArgb(238, 26, 74))
            {
                OleDbConnection conn = new OleDbConnection(connectionString);
                conn.Open();
                OleDbCommand sorgu = new OleDbCommand("SELECT id FROM users WHERE username=@p1 AND pass=@p2 AND mail=@p3", conn);
                sorgu.Parameters.AddWithValue("@p1", txtUsername.Text);
                sorgu.Parameters.AddWithValue("@p2", txtPass.Text);
                sorgu.Parameters.AddWithValue("@p3", txtMail.Text);
                int kayitSayisi = Convert.ToInt32(sorgu.ExecuteScalar());
                conn.Close();
                if (kayitSayisi > 0)
                {
                    Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\tmdbRoman");
                    key.SetValue("id", kayitSayisi.ToString());
                    if (cRemember.Checked)
                    {

                        key.SetValue("Username", txtUsername.Text);
                        key.SetValue("Password", txtPass.Text);
                        key.SetValue("Mail", txtMail.Text);
                        key.Close();
                    }
                    this.Hide();
                    if (kayitSayisi.ToString() == "1")
                    {
                        frmAdmin dash = new frmAdmin();
                        dash.Show();
                    }
                    else
                    {
                        frmUser user = new frmUser();
                        user.Show();
                    }

                }
                else
                {
                    MessageBox.Show("Kullanıcı adınız veya şifreniz hatalı.", "Hata oluştu!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtUsername.Focus();
                }


            }
            else { MessageBox.Show("Boş değer bulunmaktadır, lütfen tüm alanları eksiksiz doldurun!", "Hata oluştu!", MessageBoxButtons.OK, MessageBoxIcon.Stop); }
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            if (txtUsername.Text.Length > 1 && txtPass.Text.Length > 1 && txtMail.Text.Length > 3)
            {
                button1.BackColor = Color.Green;
            }
            else
            {
                button1.BackColor = Color.FromArgb(238, 26, 74);
            }
            //txtUsername.Text = txtUsername.Text.ToLower();
            //txtMail.Text = txtMail.Text.ToLower();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button1.BackColor != Color.FromArgb(238, 26, 74))
            {
                OleDbConnection conn = new OleDbConnection(connectionString);
                conn.Open();
                OleDbCommand sorgu = new OleDbCommand("SELECT * FROM users WHERE username=@p1 OR mail=@p3 ", conn);
                sorgu.Parameters.AddWithValue("@p1", txtUsername.Text);
                sorgu.Parameters.AddWithValue("@p3", txtMail.Text);
                int kayitSayisi = Convert.ToInt32(sorgu.ExecuteScalar());
                conn.Close();
                if (kayitSayisi <= 0)
                {
                    Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("SOFTWARE\\tmdbRoman");
                    key.SetValue("id", kayitSayisi.ToString());
                    if (cRemember.Checked)
                    {

                        key.SetValue("Username", txtUsername.Text);
                        key.SetValue("Password", txtPass.Text);
                        key.SetValue("Mail", txtMail.Text);
                        key.Close();
                    }

                    conn.Open();
                    OleDbCommand ekle = new OleDbCommand("INSERT INTO users (username,pass,mail) VALUES (@p1,@p2,@p3) ", conn);
                    ekle.Parameters.AddWithValue("@p1", txtUsername.Text);
                    ekle.Parameters.AddWithValue("@p2", txtPass.Text);
                    ekle.Parameters.AddWithValue("@p3", txtMail.Text);
                    ekle.ExecuteNonQuery();
                    conn.Close();

                    MessageBox.Show("Hesabınız başarıyla oluştu, giriş yapabilirsiniz!", "Tamamlandı!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtUsername.Focus();
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı ya da Mail zaten kullanılmaktadır!", "Hata oluştu!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtUsername.Focus();
                }


            }
            else { MessageBox.Show("Boş değer bulunmaktadır, lütfen tüm alanları eksiksiz doldurun!", "Hata oluştu!", MessageBoxButtons.OK, MessageBoxIcon.Stop); }
        }
        public void ShakeForm()
        {
            var originalPos = pictureBox1.Location;
            const int shakeAmt = 10;
            for (int i = 0; i < 4; i++)
            {
                pictureBox1.Location = new Point(originalPos.X + shakeAmt, originalPos.Y);
                Thread.Sleep(50);
                pictureBox1.Location = new Point(originalPos.X - shakeAmt, originalPos.Y);
                Thread.Sleep(50);
            }
            pictureBox1.Location = originalPos;

        }

        private int dx = 5;
        private int dy = 5;

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            titret.Stop();
            kaydir.Start();
        }

        private void titret_Tick(object sender, EventArgs e)
        {
            // PictureBox'ı hareket ettir
            pictureBox1.Left += dx;
            pictureBox1.Top += dy;

            // Kenarlara çarpma kontrolü
            if (pictureBox1.Left <= 0 || pictureBox1.Right >= ClientSize.Width)
            {
                dx = -dx; // Yatay yönde yön değiştir
            }
            if (pictureBox1.Top <= 0 || pictureBox1.Bottom >= ClientSize.Height)
            {
                dy = -dy; // Dikey yönde yön değiştir
            }
        }

        private void kaydir_Tick(object sender, EventArgs e)
        {
            // PictureBox'ı büyüt
            if (pictureBox1.Width < 300) // Maksimum boyut kontrolü
            {
                pictureBox1.Width += 5;
                pictureBox1.Height += 5;
            }

            // PictureBox'ı formun ortasına hareket ettir
            int centerX = (this.ClientSize.Width - pictureBox1.Width) / 2;
            int centerY = (this.ClientSize.Height - pictureBox1.Height) / 2;

            if (pictureBox1.Left < centerX)
            {
                pictureBox1.Left += 1;
            }
            else if (pictureBox1.Left > centerX)
            {
                pictureBox1.Left -= 1;
            }

            if (pictureBox1.Top < centerY)
            {
                pictureBox1.Top += 1;
            }
            else if (pictureBox1.Top > centerY)
            {
                pictureBox1.Top -= 1;
            }

            // Eğer PictureBox merkeze ulaştıysa ve maksimum boyuta ulaştıysa, timer'ı durdur
            if (pictureBox1.Left == centerX && pictureBox1.Top == centerY && pictureBox1.Width >= 200)
            {
                kaydir.Stop();
                timer1.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Width += 1;
            if (this.Width == 666)
            {
                timer1.Stop();
            }
        }
    }
}
