using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Net;

namespace PineVoice
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
            this.CenterToScreen();
        }

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
            else if (nameBox.Text == "" || nameBox.Text == login)
            {
                MessageBox.Show("Nazwa wyświetlana nie może być pusta, ani taka sama jak login.", "Błąd,", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    SqlCommand komendaSQL = connection.CreateCommand();
                    komendaSQL.CommandText = "SELECT login FROM users";
                    SqlDataReader czytnik = komendaSQL.ExecuteReader();
                    int marking = 0;
                    while (czytnik.Read())
                    {
                        if (czytnik["login"].ToString() == login)
                        {
                            marking += 1;
                        }
                    }
                    czytnik.Close();
                    //connection.Close();
                    if (password == "")
                    {
                        MessageBox.Show("Pole hasło nie może być puste.", "Błąd.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (marking > 0)
                    {
                        MessageBox.Show("Login jest już zajęty.", "Błąd.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    /*else if (marking1 > 0)
                    {
                        MessageBox.Show("Nazwa wyświetlana jest już zajęta.", "Błąd.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }*/
                    else if (password != rpassword)
                    {
                        MessageBox.Show("Hasła nie są identyczne.", "Błąd.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                        SqlCommand komendaSQL1 = connection.CreateCommand();
                        komendaSQL1.CommandText = "SELECT name FROM users";
                        SqlDataReader czytnik1 = komendaSQL1.ExecuteReader();
                        int marking1 = 0;
                        while (czytnik1.Read())
                        {
                            if (czytnik1["name"].ToString() == nameBox.Text)
                            {
                                marking1 += 1;
                            }
                        }
                        czytnik1.Close();
                        if (marking1 > 0)
                        {
                            MessageBox.Show("Nazwa wyświetlana jest już zajęta.", "Błąd.", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                                command.CommandText = "INSERT INTO users (login, password, ip, online, name) VALUES(@0, @1, @2, 'offline', @3)";
                                command.Parameters.AddWithValue("@0", login);
                                command.Parameters.AddWithValue("@1", hash);
                                command.Parameters.AddWithValue("@2", ipa);
                                command.Parameters.AddWithValue("@3", nameBox.Text);
                                command.ExecuteNonQuery();
                                connection.Close();
                                textbox_login.Text = "";
                                textbox_password.Text = "";
                                textBox1.Text = "";
                                nameBox.Text = "";
                                MessageBox.Show("Rejestracja powiodła się!", "Sukces!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.Hide();
                                Login login_form = new Login();
                                login_form.Show();

                            }
                            catch (Exception exs)
                            {
                                MessageBox.Show(exs.Message.ToString(), "Błąd połączenia z bazą danych.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message.ToString(), "Błąd połączenia z bazą danych.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login login_form = new Login();
            login_form.Show();
        }

        private void Register_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void textbox_login_TextChanged(object sender, EventArgs e)
        {

        }

        private void textbox_password_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void nameBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
