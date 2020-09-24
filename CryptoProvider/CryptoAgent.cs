using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace CryptoProvider
{
    class CryptoAgent:IFile
    {

        public byte[] encrypted { get; set; }
        public byte[] decrypted { get; set; }
        public int DecrSize { get; set; }
        public int EncrSize { get; set; }
        public byte[] Hash { get; set; }
        //public string Name { get { return Name; }; set }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }



        public CryptoAgent(string path,byte[] key,byte[] IV)
        {

            byte[] original = File.ReadAllBytes(path);
            //using(FileStream fs = new FileStream(path, FileMode.Open))
            
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
            Extension = System.IO.Path.GetExtension(path);

            Path = path;
            using(SHA1 sha1 = SHA1.Create())
            {
                Hash = sha1.ComputeHash(original);
            }
            
            using (Aes myAes = Aes.Create())
            {
                encrypted = Encrypt(original, myAes.Key, myAes.IV);
                EncrSize = encrypted.Length;

                decrypted = Decrypt(encrypted, myAes.Key, myAes.IV);
                DecrSize = decrypted.Length;
            }
        }


        public static RijndaelManaged GenerateKeys()
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {

                myRijndael.GenerateKey();
                myRijndael.GenerateIV();

                return myRijndael;
            }
        }

        private byte[] Decrypt(byte[] encrypted, byte[] Key, byte[] IV)
        {
            if (encrypted == null || encrypted.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            byte[] plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(encrypted))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            using (var memstream = new MemoryStream())
                            {
                                srDecrypt.BaseStream.CopyTo(memstream);
                                plaintext = memstream.ToArray();
                            }
                        }
                    }
                }
            }

            return plaintext;
        }

        private byte[] Encrypt(byte[] original, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (original == null || original.Length <= 0)
                throw new ArgumentNullException("bytes");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            //swEncrypt.Write(original);
                            swEncrypt.WriteLine(original);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

       
    }
}