using System;
using System.Text;
using System.IO;
using System.Numerics;
using System.Collections.Generic;


namespace _3Pd_Sotkevicius_v1
{
    class Program
    {
        static bool IsPrime(int number)
        {
            int half = number / 2;
            int flag = 0;
            for(int i = 2; i <= half; i++)
            {
                if(number % i == 0)
                {
                    flag = 1;
                    break;
                }
            }
            if (flag == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        static void Main(string[] args)
        {
            string menu = "\nChoose an option:\n1.Encrypt\n2.Decrypt\n0.Close\n-----------------\nOption: ";
            Console.Write("RSA Algorithm" + menu);
            string option = Console.ReadLine();
            do
            {
                switch (option)
                {
                    case "1": // Encryption
                        string text;
                        int p, q;
                        
                        Console.Clear();
                        Console.Write("RSA Algorithm Encryption\nEnter text that You want to encrypt: ");
                        text = Console.ReadLine();

                        // Checking if user entered Prime numbers
                        do
                        {
                            Console.Write("Enter first Prime number: ");
                            p = int.Parse(Console.ReadLine());
                            if (!IsPrime(p))
                            {
                                p = 4;
                                Console.WriteLine("\nThis number is not a Prime number.\n");
                            }
                            else
                            {
                                if(p < 10)
                                {
                                    p = 4;
                                    Console.WriteLine("\nChoose a bigger Prime number.\n");
                                }
                            }
                        } while (!IsPrime(p));
                        do
                        {
                            Console.Write("Enter second Prime number: ");
                            q = int.Parse(Console.ReadLine());
                            if (!IsPrime(q))
                            {
                                Console.WriteLine("\nThis number is not a Prime number.\n");
                            }
                            else
                            {
                                if (q < 10)
                                {
                                    q = 4;
                                    Console.WriteLine("\nChoose a bigger Prime number.\n");
                                }
                                else
                                {
                                    if(q == p)
                                    {
                                        q = 4;
                                        Console.WriteLine("\nChoose different Prime numbers.\n");
                                    }
                                }
                            }
                        } while (!IsPrime(q));

                        // Generating Public and Private keys
                        int n = p * q;
                        int φ = (p - 1) * (q - 1);

                        // Choosing Public Key
                        int publicKey = 0;
                        for (int i = 2; i < φ; i++)
                        {
                            if (IsPrime(i))
                            {
                                int temp;
                                int temp2 = φ;
                                publicKey = i;
                                // Checking Greatest Common Divisor GCD (Didziausia bendra daugikli DBD) so that it equals to 1
                                // Setting the public Key to GCD value
                                while (temp2 != 0)
                                {
                                    temp = temp2;
                                    temp2 = i % temp2;
                                    i = temp;
                                }
                                if (i == 1)
                                {
                                    break;
                                }
                            }
                        }

                        // Choosing Private Key
                        int privateKey;
                        for (int i = 1; i < n; i++)
                        {
                            if ((i * publicKey % φ) == 1 && i != publicKey)
                            {
                                privateKey = i;
                                break;
                            }
                            else
                                continue;
                        }
                        // Saving publicKey to a file and clearing it beforehand
                        TextWriter savePublicKey = new StreamWriter("duomenys.txt");
                        savePublicKey.WriteLine(n);
                        savePublicKey.WriteLine(publicKey);
                        savePublicKey.Close();
                        // Convert each text char to its ascii value
                        byte[] asciiBytes = Encoding.ASCII.GetBytes(text);
                        // Encrypting
                        List<BigInteger> result = new List<BigInteger>();
                        foreach (char c in asciiBytes)
                        {
                            BigInteger encryptedChar = BigInteger.Pow(c, publicKey) % n;
                            result.Add(encryptedChar);
                            TextWriter saveEncrypted = new StreamWriter("duomenys.txt", true);
                            saveEncrypted.WriteLine(encryptedChar);
                            saveEncrypted.Close();
                        }
                        Console.Clear();
                        Console.Write("RSA Algorithm\nEncrypted text: ");
                        result.ForEach(i => Console.Write(i));
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                        option = "3";
                        break;

                    //
                    //
                    //

                    case "2": // Decryption
                        List<string> lines = new List<string>();
                        int lineCount = File.ReadAllLines("duomenys.txt").Length;
                        TextReader readFile = new StreamReader("duomenys.txt");
                        // Add line values to a list
                        for(int i = 0; i < lineCount; i++)
                        {
                            lines.Add(readFile.ReadLine());
                        }
                        readFile.Close();
                        // Convert list to array
                        string[] values = lines.ToArray();
                        // Assign values to n, e and encrypted text
                        BigInteger n2 = Int32.Parse(values[0]);
                        int publicKey2 = Int32.Parse(values[1]);
                        BigInteger[] integerValues = new BigInteger[values.Length-2];
                        for(int i = 2; i < values.Length; i++)
                        {
                            integerValues[i-2] = BigInteger.Parse(values[i]);
                        }
                        
                        // Finding p and q
                        int p2 = 0;
                        int q2 = 0;
                        for(int i = 2; i < 9999; i++)
                        {
                            for(int j = 2; j < 9999; j++)
                            {
                                if (IsPrime(i) && IsPrime(j))
                                {
                                    if(i * j == n2) // Multiplication must equal n and the numbers must be prime
                                    {
                                        p2 = j;
                                        q2 = i;
                                        break;
                                    }
                                }
                            }
                            if(p2 != 0 && q2 != 0)
                            {
                                break;
                            }
                        }
                        int φ2 = (p2 - 1) * (q2 - 1);
                        
                        // Calculating private key
                        int privateKey2 = 0;
                        for (int i = 1; i < n2; i++)
                        {
                            if ((i * publicKey2 % φ2) == 1 && i != publicKey2)
                            {
                                privateKey2 = i;
                                break;
                            }
                            else
                                continue;
                        }
                        // Calculating ASCII values
                        BigInteger[] asciiValues = new BigInteger[integerValues.Length];
                        string text2 = "";
                        for(int i = 0; i < integerValues.Length; i++)
                        {
                            asciiValues[i] = BigInteger.Pow(integerValues[i], privateKey2) % n2;
                            text2 = text2 + (char)asciiValues[i];
                            
                        }
                        Console.Clear();
                        Console.Write("RSA Algorithm\nDecrypted text: " + text2);
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                        option = "3";

                        break;

                    //
                    //
                    //

                    case "3":
                        Console.Clear();
                        Console.Write("RSA Algorithm" + menu);
                        option = Console.ReadLine();
                        break;

                    //
                    //
                    //

                    case "0":
                        option = "0";
                        break;

                    //
                    //
                    //

                    default:
                        Console.Clear();
                        Console.Write("RSA Algorithm\n\nWrong input!\n" + menu);
                        option = Console.ReadLine();
                        break;
                }
            } while (option != "0");
        }
    }
}
