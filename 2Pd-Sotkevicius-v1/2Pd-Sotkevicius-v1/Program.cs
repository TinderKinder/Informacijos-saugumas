using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace _2Pd_Sotkevicius_v1
{ 
    class Program
    {
        // converts password to 128 bit hash
        static byte[] GetKey(string password)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(password);
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(keyBytes);
            }
        }


        static void Main(string[] args)
        {
            bool encrypted = false;
            string choice; 
            byte[] encryptedText; 
            string decryptedText;
            byte[] decryptedBytes;
            string text;
            string checkTextVsEncrypted; // tikrinama ivestas tekstas su jau uzsifruotu tekstu
            string password;
            byte[] key; 
            byte[] keyBytes;
            byte[] encryptedBytes;

            Console.WriteLine("AES cipher\nSelect your option:\n1.Encrypt --- 2.Decrypt --- 0.Close\nOption: ");
            choice = Console.ReadLine();
            do
            {
                switch (choice)
                {
                    case "1": // encryption
                        Console.WriteLine("\nInput desired text to encrypt: ");
                        text = Console.ReadLine();
                        Console.WriteLine("Input desired key: ");
                        password = Console.ReadLine();
                        
                        keyBytes = Encoding.UTF8.GetBytes(password); // konvertuojamas ivestas raktas i baitus
                        using (MD5 md5 = MD5.Create()) // Message digest algorithm 5, hash algoritmas, maisos funkcija leidziantis sukurti 128 bitu stringa
                        {
                            key = md5.ComputeHash(keyBytes); // sukuriamas 128 bitu raktas
                            using Aes aes = Aes.Create(); // sukuriamas objektas naudojamas algoritmui
                            using ICryptoTransform encryptor = aes.CreateEncryptor(key, key); // sukuriamas sifravimo raktas
                            encryptedBytes = Encoding.UTF8.GetBytes(text); // konvertuojamas tekstas i baitus
                            encryptedText = encryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length); // transformuojamas tekstas pagal sifravimo rakta

                            File.WriteAllBytes("encrypted.txt", encryptedText); // baitu issaugojimas txt faile
                            Console.WriteLine("encrypted text: " + Convert.ToBase64String(encryptedText)); // konvertuojami baitai i string formata, su tikslu atvaizduoti
                            encrypted = true;
                        }
                        Console.WriteLine("\n--------------------------------------------------\nSelect your option:\n1.Encrypt --- 2.Decrypt --- 0.Close\nOption: ");
                        choice = Console.ReadLine();
                        break;

                    case "2": // decryption
                        if(!encrypted) // No word to decrypt
                        {
                            Console.WriteLine("\nFirst encrypt a message.\n--------------------------------------------------\nSelect your option:\n1.Encrypt --- 2.Decrypt --- 0.Close\nOption: ");
                            choice = Console.ReadLine();
                            break;
                        }
                        else if(encrypted)
                        {
                            Console.WriteLine("\nInput desired text to decrypt: ");
                            text = Console.ReadLine();
                            Console.WriteLine("Input correct key: ");
                            password = Console.ReadLine();

                            keyBytes = Encoding.UTF8.GetBytes(password); // konvertuojamas raktas i baitus
                            using (MD5 md5 = MD5.Create()) // hash algoritmas, leidziantis sukurti 128 bitu stringa
                            {
                                key = md5.ComputeHash(keyBytes); // sukuriamas 128 bitu raktas
                                using Aes aes = Aes.Create(); // objektas
                                using ICryptoTransform decryptor = aes.CreateDecryptor(key, key); // sukuriamas desifravimo raktas
                                encryptedText = File.ReadAllBytes("encrypted.txt"); // paimami uzkoduodo zodzio baitai is failo

                                if (text == Convert.ToBase64String(encryptedText)) // tikrina ar ivesta sifruote atitinka sifruote issaugota faile
                                    {
                                    try
                                    {
                                        decryptedBytes = decryptor.TransformFinalBlock(encryptedText, 0, encryptedText.Length); // baitai yra desifruojami pagal rakta
                                        decryptedText = Encoding.UTF8.GetString(decryptedBytes); // baitai paverciami i zodi
                                        Console.WriteLine("Decrypted text: " + decryptedText);
                                    }
                                    catch (System.Security.Cryptography.CryptographicException) // jeigu blogas slaptazodis meta exceptiona
                                    {
                                        Console.WriteLine("\nThe key you have entered is wrong.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("\nThe cipher you have entered is wrong.");
                                }
                            }
                        }
                        Console.WriteLine("\n--------------------------------------------------\nSelect your option:\n1.Encrypt --- 2.Decrypt --- 0.Close\nOption: ");
                        choice = Console.ReadLine();
                        break;

                    case "0": // close
                        break;

                    default:
                        Console.WriteLine("\nSelect your option:\n1.Encrypt --- 2.Decrypt --- 0.Close\nOption: ");
                        choice = Console.ReadLine();
                        break;
                }
            }
            while (choice != "0");
        }
    }
}
