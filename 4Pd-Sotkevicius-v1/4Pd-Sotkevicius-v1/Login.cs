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

namespace _4Pd_Sotkevicius_v1
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            new NewPassword().Show();
            this.Hide();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            new SearchPassword().Show();
            this.Hide();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            new UpdatePassword().Show();
            this.Hide();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            new DeletePassword().Show();
            this.Hide();
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            string Username = getUsername();
            string line;
            string stringPassword = "";
            StreamReader file2 = new StreamReader("users.txt");
            while ((line = file2.ReadLine()) != null)
            {
                if (Username == line.Split(' ')[0])
                {
                    stringPassword = line.Split(' ')[1];
                }
            }
            file2.Close();

            // Encrypt users.txt file
            string usersTXTpassword = "foxlearn.com";
            GCHandle gCHandle = GCHandle.Alloc(usersTXTpassword, GCHandleType.Pinned);
            FileEncrypt("users.txt", usersTXTpassword);
            gCHandle.Free();
            File.Delete("users.txt");

            // Encrypt username.txt file AES
            GCHandle gCHandle2 = GCHandle.Alloc(stringPassword, GCHandleType.Pinned);
            FileEncrypt(@".\database\" + Username + ".txt", stringPassword);
            gCHandle2.Free();
            File.Delete(@".\database\" + Username + ".txt");
            








            Application.OpenForms["Account"].Show();
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
