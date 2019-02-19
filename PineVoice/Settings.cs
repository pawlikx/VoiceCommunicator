using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PineVoice
{
    public partial class Settings : Form
    {
        SqlConnection connection = new SqlConnection(@"Data source=den1.mssql3.gear.host;
                                                             database=tip1;
                                                             User id=tip1;
                                                             Password=On1Yt!RAN7-0;");
        int myID;
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
        static bool s_bFormOpened = false;
        public Settings(int ID)
        {
            myID = ID;
            InitializeComponent();
            this.CenterToScreen();
        }

        private void Settings_Load(object sender, EventArgs e)
        {//if one form was opened now, the current form will be closed
            if (!s_bFormOpened)
                s_bFormOpened = true;
            else
                this.Dispose();
        }



        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {//restored the state
            if (s_bFormOpened)
                s_bFormOpened = false;
        }

        private void textbox_login_TextChanged(object sender, EventArgs e)
        {

        }

        private void button_login_Click(object sender, EventArgs e)
        {
            if (textbox_password.Text == "" || textBox1.Text == "" || textbox_login.Text == "")
            {
                MessageBox.Show("Pole hasła nie może być puste.", "Błąd.", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string password = GetHash(md5, textbox_login.Text);
                    string npass = textbox_password.Text;
                    string rpass = textBox1.Text;
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "SELECT password FROM users WHERE userID = @0";
                    command.Parameters.AddWithValue("@0", myID);
                    string hashed = command.ExecuteScalar().ToString();
                    if(password != hashed)
                    {
                        MessageBox.Show("Stare hasło nie zostało wprowadzone poprawnie.", "Błąd.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (npass != rpass)
                        {
                            MessageBox.Show("Nowe hasło nie zostało wprowadzone poprawnie.", "Błąd.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            string hashednew = GetHash(md5, npass);
                            SqlCommand command1 = connection.CreateCommand();
                            command1.CommandText = "UPDATE users SET password = @0 WHERE userID = @1";
                            command1.Parameters.AddWithValue("@0", hashednew);
                            command1.Parameters.AddWithValue("@1", myID);
                            command1.ExecuteNonQuery();
                            textbox_login.Text = "";
                            textbox_password.Text = "";
                            textBox1.Text = "";
                            MessageBox.Show("Zmiana hasła powiodła się!", "Sukces!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            connection.Close();
                            this.Close();
                        }
                    }

                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Błąd.", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
    }
}
