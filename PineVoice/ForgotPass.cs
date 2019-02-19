using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PineVoice
{
    public partial class ForgotPass : Form
    {
        SqlConnection connection = new SqlConnection(@"Data source=den1.mssql3.gear.host;
                                                             database=tip1;
                                                             User id=tip1;
                                                             Password=On1Yt!RAN7-0;");
        string GetLocalIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }

            return localIP;
        }

        string GetHash(MD5 hash, string input)
        {

            byte[] tab = hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < tab.Length; i++)
            {
                builder.Append(tab[i].ToString("x2"));
            }

            return builder.ToString();
        }
        public ForgotPass()
        {
            InitializeComponent();
            this.CenterToScreen();
        }
        private void ForgotPass_FormClosing(object sender, FormClosingEventArgs e)
        {
            //You may decide to prompt to user
            //else just kill
            Process.GetCurrentProcess().Kill();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login login_form = new Login();
            login_form.Show();
        }

        private void button_register_Click(object sender, EventArgs e)
        {
            string login = textbox_login.Text;
            string password = textbox_password.Text;
            string rpassword = textBox1.Text;
            var newstr = String.Join("", login.Where(char.IsLetterOrDigit));
            if (newstr == "")
            {
                MessageBox.Show("Login nieprawidłowy, pole jest puste lub zawiera niedozwolone znaki.", "Błąd,", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (password == "")
            {
                MessageBox.Show("Pole hasło nie może być puste.", "Błąd.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (password != rpassword)
            {
                MessageBox.Show("Hasła nie są identyczne.", "Błąd.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    SqlCommand command = connection.CreateCommand();
                    MD5 md5 = MD5.Create();
                    string hash = GetHash(md5, password);
                    string ipa = GetLocalIP();
                    command.CommandText = "UPDATE users SET password = @0 WHERE login = @1";
                    command.Parameters.AddWithValue("@0", hash);
                    command.Parameters.AddWithValue("@1", login);
                    command.ExecuteNonQuery();
                    connection.Close();
                    textbox_login.Text = "";
                    textbox_password.Text = "";
                    textBox1.Text = "";
                    MessageBox.Show("Zmiana hasła powiodła się!", "Sukces!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();
                    Login login_form = new Login();
                    login_form.Show();
                } catch (Exception ex) { }

            }
        }
    }
}
