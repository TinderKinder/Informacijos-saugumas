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
    public partial class DeletePassword : Form
    {
        public DeletePassword()
        {
            InitializeComponent();
            informationLabel.Hide();
            displayBox.Items.Clear();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            informationLabel.Hide();
            // if password box not empty
            // search for password
            // if found
            // display box is filled with encrypted password
            // else
            // information label displays "No password has been saved under this name"
            // else
            // information label displays "enter the password name in order to search"

            string Username = getUsername();

            if (passwordBox.TextLength != 0)
            {
                // checking if name exists
                bool foundName = false;
                string line;
                string foundPasswordValue = "";
                string foundPasswordUrl = "";
                string foundPasswordDescription = "";

                StreamReader file = new StreamReader(@".\database\" + Username + ".txt");
                while ((line = file.ReadLine()) != null) // read all file lines
                {
                    if (passwordBox.Text == line.Split(' ')[0]) // if a password saved under the name input exists
                    {
                        foundName = true; // check that the password exists
                        foundPasswordValue = line.Split(' ')[3]; // set the password value
                        foundPasswordUrl = line.Split(' ')[4]; // set the URL/App value
                        for (int i = 5; i < line.Split(' ').Length; i++) // get the left over string from the file
                        {
                            foundPasswordDescription = foundPasswordDescription + " " + line.Split(' ')[i]; // set the description value
                        }
                    }
                }
                file.Close();
                displayBox.Items.Clear();
                if (foundName) // if password saved under the name input exists
                {
                    displayBox.Items.Add("Password: " + foundPasswordValue);
                    displayBox.Items.Add("App or URL: " + foundPasswordUrl);
                    displayBox.Items.Add("Description:" + foundPasswordDescription);
                }
                else
                {
                    informationLabel.Text = "Password not found.";
                    informationLabel.Show();
                }
            }
            else
            {
                informationLabel.Text = "Enter password Name.";
                informationLabel.Show();
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            string Username = getUsername();
            informationLabel.Hide();
            // if displayBox not empty
                // clear displayBox
                // information label displays "the password record has been deleted"
                // find the password in account database file
                // delete the password whole information record from file
            // else
                // information label displays "there is no password to delete"

            if (displayBox.Items.Count != 0)
            {
                displayBox.Items.Clear();
                string line;
                string name = "";
                string Key = "";
                string IV = "";
                string password = "";
                string url = "";
                string description = "";

                StreamReader file = new StreamReader(@".\database\" + Username + ".txt");
                while ((line = file.ReadLine()) != null) // read all file lines
                {
                    if (passwordBox.Text == line.Split(' ')[0]) // get the required password line
                    {
                        name = line.Split(' ')[0]; // set the password value
                        Key = line.Split(' ')[1];
                        IV = line.Split(' ')[2];
                        password = line.Split(' ')[3];
                        url = line.Split(' ')[4]; // set the URL/App value
                        for (int i = 5; i < line.Split(' ').Length; i++) // get the left over string from the file
                        {
                            description = description + " " + line.Split(' ')[i]; // set the description value
                        }
                    }
                }
                file.Close();

                ////////////////// Delete old Password //////////////////

                // Get all the lines from file and delete the specified one
                string tempFile = Path.GetTempFileName();

                using (var sr = new StreamReader(@".\database\" + Username + ".txt"))
                using (var sw = new StreamWriter(tempFile))
                {
                    string line2;

                    while ((line2 = sr.ReadLine()) != null)
                    {
                        string lineCompare = name + " " + Key + " " + IV + " " + password + " " + url + description;
                        string fileLine = line2.ToString();
                        if (fileLine != lineCompare)
                            sw.WriteLine(line2);
                    }
                }

                File.Delete(@".\database\" + Username + ".txt");
                File.Move(tempFile, @".\database\" + Username + ".txt");

                informationLabel.Text = "Password Deleted.";
                informationLabel.Show();
            }
            else
            {
                informationLabel.Text = "Enter Password First.";
                informationLabel.Show();
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            Application.OpenForms["Login"].Show();
            this.Close();
        }

        private void decryptButton_Click(object sender, EventArgs e)
        {
            // if display box filled
            // if decrypted=false
            // decrypt the message and display again and set decrypted=true
            // else
            // information label displays "password has been already decrypted"
            // else
            // information label displays "there is no password to decrypt"

            informationLabel.Hide();
            string Username = getUsername();
            if (displayBox.Items.Count != 0)
            {
                string line;
                string encrypted = "";
                string Key = "";
                string IV = "";
                string foundPasswordUrl = "";
                string foundPasswordDescription = "";

                StreamReader file = new StreamReader(@".\database\" + Username + ".txt");
                while ((line = file.ReadLine()) != null) // read all file lines
                {
                    if (passwordBox.Text == line.Split(' ')[0]) // if a password saved under the name input exists
                    {
                        encrypted = line.Split(' ')[3]; // set the password byte value
                        Key = line.Split(' ')[1]; // set the password Key value
                        IV = line.Split(' ')[2]; // set the password IV value
                        foundPasswordUrl = line.Split(' ')[4]; // set the URL/App value
                        for (int i = 5; i < line.Split(' ').Length; i++) // get the left over string from the file
                        {
                            foundPasswordDescription = foundPasswordDescription + " " + line.Split(' ')[i]; // set the description value
                        }
                    }
                }
                file.Close();
                string passwordValue = EncryptAesManaged(Key, IV, encrypted);

                displayBox.Items.Clear();
                displayBox.Items.Add("Password: " + passwordValue);
                displayBox.Items.Add("App or URL: " + foundPasswordUrl);
                displayBox.Items.Add("Description:" + foundPasswordDescription);
            }
            else
            {
                informationLabel.Text = "Enter Password First.";
                informationLabel.Show();
            }
        }

        private string getUsername()
        {
            Account getUsername = (Account)Application.OpenForms["Account"];
            string Username = getUsername.getUsername;
            return Username;
        }
        static string EncryptAesManaged(string Key, string IV, string raw)
        {
            byte[] aesKey = Convert.FromBase64String(Key);
            byte[] aesIV = Convert.FromBase64String(IV);
            byte[] encrypted = Convert.FromBase64String(raw);
            // Create Aes that generates a new key and initialization vector (IV).    
            // Same key must be used in encryption and decryption    
            using (AesManaged aes = new AesManaged())
            {
                // Decrypt the bytes to a string.    
                string decrypted = Decrypt(encrypted, aesKey, aesIV);
                return decrypted;
            }
        }
        static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create AesManaged    
            using (AesManaged aes = new AesManaged())
            {
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.    
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream    
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }
    }
}
