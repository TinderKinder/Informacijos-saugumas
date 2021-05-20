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
using System.Runtime.InteropServices;
using Scrypt;

namespace _4Pd_Sotkevicius_v1
{
    public partial class Account : Form
    {
        public Account()
        {
            InitializeComponent();
            informationLabel.Hide();
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            new Register().Show();
            informationLabel.Hide();
            this.Hide();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            informationLabel.Hide();
            if (username.Text.Length != 0)
            {
                if (password.Text.Length != 0)
                {
                    // Decrypt users.txt file using string "passwordForUsersFile" AES
                    string usersTXTpassword = "foxlearn.com";
                    GCHandle gch = GCHandle.Alloc(usersTXTpassword, GCHandleType.Pinned);
                    FileDecrypt("users.txt.aes", "users.txt", usersTXTpassword);
                    gch.Free();
                    File.Delete("users.txt.aes");




                    // Decrypt username.txt file AES





                    // if password.text SCRYPT == users.txt.line.split(' ')[1]
                    // Decrypt username.txt file using AES algorithm with AES.Key = password.text.encryptedScrypt
                    // else
                    // information label displays "Wrong Password."



                    // checking if username exists
                    bool foundUser = false;
                    string line;
                    string stringPassword = "";
                    StreamReader file = new StreamReader("users.txt");
                    while ((line = file.ReadLine()) != null)
                    {
                        if (username.Text == line.Split(' ')[0])
                        {
                            stringPassword = line.Split(' ')[1];
                            foundUser = true;
                        }
                    }
                    file.Close();

                    if (foundUser) // check if username exists
                    {
                        // Check whether passwords match
                        ScryptEncoder encoder = new ScryptEncoder();
                        
                        if (encoder.Compare(password.Text, stringPassword))
                        {
                            
                            // Decrypt username.txt using AES
                            GCHandle gch2 = GCHandle.Alloc(stringPassword, GCHandleType.Pinned);
                            FileDecrypt(@".\database\" + username.Text + ".txt.aes", @".\database\" + username.Text + ".txt", stringPassword);
                            gch2.Free();
                            File.Delete(@".\database\" + username.Text + ".txt.aes");

                            informationLabel.Hide();
                            this.Hide();
                            new Login().Show();
                        }
                        else
                        {
                            // Decrypt users.txt file using string "passwordForUsersFile" AES
                            usersTXTpassword = "foxlearn.com";
                            GCHandle gch3 = GCHandle.Alloc(usersTXTpassword, GCHandleType.Pinned);
                            FileDecrypt("users.txt.aes", "users.txt", usersTXTpassword);
                            gch3.Free();
                            File.Delete("users.txt.aes");

                            informationLabel.Text = "Wrong password.";
                            informationLabel.Show();
                        }
                    }
                    else
                    {
                        informationLabel.Text = "No such User.";
                        informationLabel.Show();
                    }
                }
                else
                {
                    informationLabel.Text = "Please enter your password.";
                    informationLabel.Show();
                }
            }
            else
            {
                informationLabel.Text = "Please enter your username.";
                informationLabel.Show();
            }

            // if username exists in users.txt
                // check password, if correct
                    // Decipher account database file
                    // new login window
                    // informationLabel.hide();
                    // close account window
                // else
                    // information label displays "wrong password"
            // else
                // information label displays "no such user"
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
        public string getUsername
        {
            get { return username.Text; }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            if (File.Exists("users.txt"))
            {
                string usersTXTpassword = "foxlearn.com";
                GCHandle gCHandle = GCHandle.Alloc(usersTXTpassword, GCHandleType.Pinned);
                FileEncrypt("users.txt", usersTXTpassword);
                gCHandle.Free();
                File.Delete("users.txt");
            }
            this.Close();
        }

        // File encryption and decription

        public static byte[] GenerateSalt()
        {
            byte[] data = new byte[32];
            using (RNGCryptoServiceProvider rgnCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                rgnCryptoServiceProvider.GetBytes(data);
            }
            return data;
        }
        private void FileEncrypt(string inputFile, string password)
        {
            byte[] salt = GenerateSalt();
            byte[] passwords = Encoding.UTF8.GetBytes(password);
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;//aes 256 bit encryption c#
            AES.BlockSize = 128;//aes 128 bit encryption c#
            AES.Padding = PaddingMode.PKCS7;
            var key = new Rfc2898DeriveBytes(passwords, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Mode = CipherMode.CFB;
            using (FileStream fsCrypt = new FileStream(inputFile + ".aes", FileMode.Create))
            {
                fsCrypt.Write(salt, 0, salt.Length);
                using (CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                    {
                        byte[] buffer = new byte[1048576];
                        int read;
                        while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cs.Write(buffer, 0, read);
                        }
                    }
                }
            }
        }
        private void FileDecrypt(string inputFileName, string outputFileName, string password)
        {
            byte[] passwords = Encoding.UTF8.GetBytes(password);
            byte[] salt = new byte[32];
            using (FileStream fsCrypt = new FileStream(inputFileName, FileMode.Open))
            {
                fsCrypt.Read(salt, 0, salt.Length);
                RijndaelManaged AES = new RijndaelManaged();
                AES.KeySize = 256;//aes 256 bit encryption c#
                AES.BlockSize = 128;//aes 128 bit encryption c#
                var key = new Rfc2898DeriveBytes(passwords, salt, 50000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                AES.Padding = PaddingMode.PKCS7;
                AES.Mode = CipherMode.CFB;
                using (CryptoStream cryptoStream = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (FileStream fsOut = new FileStream(outputFileName, FileMode.Create))
                    {
                        int read;
                        byte[] buffer = new byte[1048576];
                        while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fsOut.Write(buffer, 0, read);
                        }
                    }
                }
            }
        }
    }
}
