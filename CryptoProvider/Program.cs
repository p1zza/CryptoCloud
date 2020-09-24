using System;
using System.IO;
using System.Security.Cryptography;

namespace CryptoProvider
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Write full path to file");
            string path = Console.ReadLine();
            Console.WriteLine("Full path is:" + path);

            RijndaelManaged myRijndael = CryptoAgent.GenerateKeys();

            CryptoAgent cryptoAgent = new CryptoAgent(path, myRijndael.Key, myRijndael.IV);
            FileConstructor fileConstructor = new FileConstructor(cryptoAgent);

            

        }
    }
}
