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
using System.Security.Cryptography;

namespace _4Pd_Sotkevicius_v1
{
    public partial class NewPassword : Form
    {
        public NewPassword()
        {
            InitializeComponent();
            informationLabel.Hide();
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            informationLabel.Hide();
            // generate random password in passwordBox
            // information label displays "new password has been generated"

            // generating random password
            int lenght = 12;
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < lenght--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            passwordBox.Text = res.ToString();
            informationLabel.Text = "New password has been generated.";
            informationLabel.Show();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            informationLabel.Hide();
            string Username = getUsername();

            if (nameBox.TextLength != 0)
            {
                if (passwordBox.TextLength != 0)
                {
                    if (urlBox.TextLength != 0)
                    {
                        if (descriptionBox.TextLength != 0)
                        {
                            // checking if name exists
                            bool foundName = false;
                            string line;
                            StreamReader file = new StreamReader("users.txt");
                            while ((line = file.ReadLine()) != null)
                            {
                                if (nameBox.Text == line.Split(' ')[0])
                                {
                                    foundName = true;
                                }
                            }
                            file.Close();

                            if (foundName) // check if name is already in use
                            {
                                informationLabel.Text = "There is already a password with this name";
                                informationLabel.Show();
                            }
                            else
                            {
                                // Save account information in users database
                                TextWriter writer = new StreamWriter(@".\database\" + Username + ".txt", true);

                                // encrypt password using RSA
                                List<string> list = EncryptAesManaged(passwordBox.Text);
                                // list[0] is aes.Key, list[1] is aes.IV, list[2] is encrypted password
                                // save account information to user database file
                                writer.WriteLine(nameBox.Text + " " + list[0] + " " + list[1] + " " + list[2] + " " + urlBox.Text + " " + descriptionBox.Text);
                                informationLabel.Text = "Password saved under the name: " + nameBox.Text;
                                informationLabel.Show();
                                writer.Close();

                                nameBox.Clear();
                                passwordBox.Clear();
                                urlBox.Clear();
                                descriptionBox.Clear();
                            }
                        }
                        else
                        {
                            informationLabel.Text = "Please enter a description.";
                            informationLabel.Show();
                        }
                    }
                    else
                    {
                        informationLabel.Text = "Please enter Application name or URL.";
                        informationLabel.Show();
                    }
                }
                else
                {
                    informationLabel.Text = "Please enter a password.";
                    informationLabel.Show();
                }
            }
            else
            {
                informationLabel.Text = "Please enter a name.";
                informationLabel.Show();
            }
            // if name doesnt exist
                // cipher password
                // save name, password, url, description
                // information label displays "your password has been saved"
            // else
                // information label displays "There is already a password with this name"
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            Application.OpenForms["Login"].Show();
            this.Close();
        }

        private string getUsername()
        {
            Account getUsername = (Account)Application.OpenForms["Account"];
            string Username = getUsername.getUsername;
            return Username;
        }
        static List<string> EncryptAesManaged(string raw)
        {
            // Create Aes that generates a new key and initialization vector (IV).    
            // Same key must be used in encryption and decryption    
            using (AesManaged aes = new AesManaged())
            {
                var aesKey = aes.Key;
                var aesIV = aes.IV;
                // Encrypt string    
                byte[] encrypted = Encrypt(raw, aesKey, aesIV);
                // Put the text, key and IV into one array
                List<string> list = new List<string>();
                list.Clear();

                list.Add(Convert.ToBase64String(aesKey));
                list.Add(Convert.ToBase64String(aesIV));
                list.Add(Convert.ToBase64String(encrypted));

                // Return encrypted text, key and IV
                return list;
            }
        }
        static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            // Create a new AesManaged.    
            using (AesManaged aes = new AesManaged())
            {
                // Create encryptor    
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                // Create MemoryStream    
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream    
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }
    }
}
