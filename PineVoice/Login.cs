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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

        string GetLocalIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork" && ip.ToString()[0]=='2') // !!!!!!!!!!!!!!!!!!!!!!! ip.AddressFamily.ToString() == "InterNetwork" &&
                {
                    localIP = ip.ToString();
                }
            }

            return localIP;
        }

        int userID = 0;

        SqlConnection connection = new SqlConnection(@"Data source=den1.mssql3.gear.host;
                                                             database=tip1;
                                                             User id=tip1;
                                                             Password=On1Yt!RAN7-0;");
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

        /* private void link_register_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
         {
             this.Hide();
             //Register register_form = new Register();
             //register_form.Show();
         }*/

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            //You may decide to prompt to user
            //else just kill
            Process.GetCurrentProcess().Kill();
        }
        private void button_login_Click(object sender, EventArgs e)
        {
            if (textbox_login.Text == "") {
                MessageBox.Show("Login nieprawidłowy, pole jest puste lub zawiera niedozwolone znaki.", "Błąd,", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (textbox_password.Text == "")
            {
                MessageBox.Show("Pole hasło nie może być puste.", "Błąd.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    MD5 md5 = MD5.Create();
                    string password = GetHash(md5, textbox_password.Text);
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "SELECT userID FROM users WHERE login = @0 AND password = @1";
                    command.Parameters.AddWithValue("@0", textbox_login.Text);
                    command.Parameters.AddWithValue("@1", password);
                    userID = Convert.ToInt32(command.ExecuteScalar());
                    //Console.WriteLine(userID);
                    if (userID != 0)
                    {
                        string newIP = GetLocalIP();
                        SqlCommand updateIP = connection.CreateCommand();
                        updateIP.CommandText = "UPDATE users SET ip = @0, online='online' WHERE userID = @1";
                        updateIP.Parameters.AddWithValue("@0", newIP);
                        updateIP.Parameters.AddWithValue("@1", userID);
                        updateIP.ExecuteNonQuery();
                        MessageBox.Show("Zalogowano!", "Sukces!", MessageBoxButtons.OK, MessageBoxIcon.None);
                        connection.Close();
                        this.Hide();
                        Program.userLogin = textbox_login.Text;
                        //Program.status = true;
                        Home app_form = new Home(userID);
                        app_form.Show();
                    }
                    else
                    {
                        MessageBox.Show("Nazwa użytkownika lub hasło nieprawidłowe.", "Błąd.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Błąd połączenia z bazą danych.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Register register_form = new Register();
            register_form.Show();
        }

        private void textbox_login_TextChanged(object sender, EventArgs e)
        {

        }

        private void textbox_password_TextChanged(object sender, EventArgs e)
        {

        }

        private void link_forget_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            ForgotPass forgotPass_form = new ForgotPass();
            forgotPass_form.Show();
        }
    }
}
