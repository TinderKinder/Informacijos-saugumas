using System;

namespace _1Pd_Sotkevicius_v1
{
    class Program
    {
        static void Encryption(string text, int shift)
        {
            int[] inputasc; // Duoto teksto raidziu ASCII reiksmes
            int[] outputasc; // Rezultato teksto raidziu ASCII reiksmes
            char[] inputchars; // Duoto teksto raides
            char[] outputchars; // Rezultato teksto raides
            string encryptedText = "";

            inputchars = new char[text.Length];
            inputasc = new int[text.Length];
            outputchars = new char[text.Length];
            outputasc = new int[text.Length];

            for (int i = 0; i < text.Length; i++)

            {
                inputchars[i] = text[i]; // atskiria teksto raides i masyva
                inputasc[i] = (int)inputchars[i]; // konvertuojame raides i ju ASCII reiksmes
                outputasc[i] = inputasc[i]; // naudojamas norint itraukti kitus simbolius uz abeceles ribu

                // uztikrina, kad poslinkis butu taikomas tik didziosioms ir mazosioms raidems
                if ((inputasc[i] > 96 & inputasc[i] < 123) || (inputasc[i] > 64 & inputasc[i] < 91)) // jei yra abeceles ribose

                {
                    outputasc[i] = inputasc[i] + shift; // abecelei pritaikomas poslinkis

                    if ((outputasc[i] > 122 & inputasc[i] > 96) || (outputasc[i] > 90 & inputasc[i] < 91)) // jei isvedimas uzeina abecele
                    {
                        outputasc[i] = outputasc[i] - 26;// abecele generuojasi is naujo, jei poslinkis ja visa praeina 
                    }
                    else if ((outputasc[i] < 97 & inputasc[i] > 96) || (outputasc[i] < 65 & inputasc[i] < 91)) 
                    {
                        outputasc[i] = outputasc[i] + 26; // jei poslinkis nepraeina abeceles, abecele susigeneruoja iki galo
                    }
                }
                outputchars[i] = (char)outputasc[i]; // konvertuojama uzkoduoto teksto ASCII reiksmes i raides
                encryptedText = encryptedText + outputchars[i]; // sudaromas zodis is raidziu
            }
            Console.WriteLine("Encrypted message: " + encryptedText);
        }

        static void Decryption(string text, int shift)
        {
            int[] inputasc; // Duoto teksto raidziu ASCII reiksmes
            int[] outputasc; // Rezultato teksto raidziu ASCII reiksmes
            char[] inputchars; // Duoto teksto raides
            char[] outputchars; // Rezultato teksto raides
            string decryptedText = "";

            inputchars = new char[text.Length];
            inputasc = new int[text.Length];
            outputchars = new char[text.Length];
            outputasc = new int[text.Length];

            for (int i = 0; i < text.Length; i++)

            {
                inputchars[i] = text[i]; // atskiria teksto raides i masyva
                inputasc[i] = (int)inputchars[i]; // konvertuojame raides i ju ASCII reiksmes
                outputasc[i] = inputasc[i]; // naudojamas norint itraukti kitus simbolius uz abeceles ribu

                // uztikrina, kad poslinkis butu taikomas tik didziosioms ir mazosioms raidems
                if ((inputasc[i] > 96 & inputasc[i] < 123) || (inputasc[i] > 64 & inputasc[i] < 91)) 

                {
                    outputasc[i] = inputasc[i] - shift; // abecelei pritaikomas atbulinis poslinkis

                    if ((outputasc[i] > 122 & inputasc[i] > 96) || (outputasc[i] > 90 & inputasc[i] < 91))
                    {
                        outputasc[i] = outputasc[i] - 26;// abecele generuojasi is naujo, jei poslinkis ja visa praeina 
                    }
                    else if ((outputasc[i] < 97 & inputasc[i] > 96) || (outputasc[i] < 65 & inputasc[i] < 91))
                    {
                        outputasc[i] = outputasc[i] + 26; // jei poslinkis nepraeina abeceles, abecele susigeneruoja iki galo
                    }
                }
                outputchars[i] = (char)outputasc[i]; // konvertuojama isversto teksto ASCII reiksmes i raides
                decryptedText = decryptedText + outputchars[i]; // sudaromas zodis
            }
            Console.WriteLine("Decrypted message: " + decryptedText);
        }

        static void Main(string[] args)
        {
            string text;
            int shift;
            
           
            Console.WriteLine("Ceasar Cipher\nChoose desired function:\n1.Encrypt --- 2.Decrypt --- 0.Close\nOption:");
            // Pasirenkama norima uzduoties funkcija
            string input = Console.ReadLine();
            do
            {
                switch (input)
                {
                    // Encryption
                    case "1":
                        Console.WriteLine("\nInput your desired text to encrypt:");
                        text = Console.ReadLine();
                        
                        Console.WriteLine("\nInput your desired shift:");
                        shift = Int32.Parse(Console.ReadLine());
                        shift = shift % 26;

                        Encryption(text, shift);
                        input = "3"; // tesiam programa
                        break;

                    // Decryption
                    case "2":
                        Console.WriteLine("\nInput your desired text to decrypt:");
                        text = Console.ReadLine();

                        Console.WriteLine("\nInput known shift number:");
                        shift = Int32.Parse(Console.ReadLine());
                        shift = shift % 26;

                        Decryption(text, shift);
                        input = "3"; // tesiam programa
                        break;

                    // Close
                    case "0":
                        break;

                    // No such function
                    default:
                        Console.WriteLine("\nChoose desired function:\n1.Encrypt --- 2.Decrypt --- 0.Close\nOption:");
                        input = Console.ReadLine();
                        break;
                }
            }
            while (input != "0");
        }
    }
}