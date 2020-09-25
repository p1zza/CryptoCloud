using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;

namespace CryptoProvider
{
    class CryptoAgent:IFile
    {
        [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(IntPtr Destination, int Length);
        public byte[] encrypted { get; set; }
        public byte[] decrypted { get; set; }
        public byte[] filevalue {get;set;}

        public int DecrSize { get; set; }
        public int EncrSize { get; set; }
        public byte[] Hash { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }



        //public CryptoAgent(string path,byte[] key,byte[] IV)
        //{

        //    byte[] original = File.ReadAllBytes(path);
        //    //using(FileStream fs = new FileStream(path, FileMode.Open))

        //    Name = System.IO.Path.GetFileNameWithoutExtension(path);
        //    Extension = System.IO.Path.GetExtension(path);

        //    Path = path;
        //    using(SHA1 sha1 = SHA1.Create())
        //    {
        //        Hash = sha1.ComputeHash(original);
        //    }

        //    using (Aes myAes = Aes.Create())
        //    {
        //        encrypted = Encrypt(original, myAes.Key, myAes.IV);
        //        EncrSize = encrypted.Length;

        //        decrypted = Decrypt(encrypted, myAes.Key, myAes.IV);
        //        DecrSize = decrypted.Length;
        //    }
        //}


        public CryptoAgent(string path, string password)
        {
            GCHandle gCHandle = GCHandle.Alloc(password, GCHandleType.Pinned);
            FileEncrypt(path, password);
            FileDecrypt(path + ".aes", path + "decr", password);
            //if(decrypted.GetHashCode()==filevalue.GetHashCode())
            //{
            //    Console.WriteLine("everything is ok");
            //}
            //else
            //{
            //    throw new Exception("FileHash is not equal");
            //}
            //ZeroMemory(gCHandle.AddrOfPinnedObject(), password.Length * 2);
            //gCHandle.Free();
        }
        public static byte[] GenerateSalt(string password)
        {
            while(password.Length <32)
            {
                password += password;
            }
            byte[] data = Encoding.UTF8.GetBytes(password, 0, 32);
            using (RNGCryptoServiceProvider rgnCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                rgnCryptoServiceProvider.GetBytes(data);
            }
            return data;
        }

        private void FileConstructor()
        {

        }

        

        private void FileEncrypt(string inputFilePath, string password)
        {
            byte[] salt = GenerateSalt(password);
            byte[] passwords = Encoding.UTF8.GetBytes(password);
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.ISO10126;
            var key = new Rfc2898DeriveBytes(passwords, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Mode = CipherMode.ECB;
            using (FileStream fsCrypt = new FileStream(inputFilePath + ".aes", FileMode.Create))
            {
                filevalue = File.ReadAllBytes(inputFilePath);
                fsCrypt.Write(salt, 0, salt.Length);
                using (CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (FileStream fsIn = new FileStream(inputFilePath, FileMode.Open))
                    {
                        byte[] buffer = new byte[1048576];
                        int read;
                        while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cs.Write(buffer, 0, read);
                            encrypted = buffer;
                        }
                    }
                }
            }
        }

        private void FileDecrypt(string inputFilePath, string outputFileName, string password)
        {
            byte[] passwords = Encoding.UTF8.GetBytes(password);
            byte[] salt = GenerateSalt(password);
            using (FileStream fsCrypt = new FileStream(inputFilePath, FileMode.Open))
            {
                fsCrypt.Read(salt, 0, salt.Length);
                RijndaelManaged AES = new RijndaelManaged();
                AES.KeySize = 256;  
                AES.BlockSize = 128;
                var key = new Rfc2898DeriveBytes(passwords, salt, 50000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                AES.Padding = PaddingMode.ISO10126;
                AES.Mode = CipherMode.ECB;
                using (CryptoStream cryptoStream = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (FileStream fsOut = new FileStream(outputFileName, FileMode.Create))
                    {
                        int read;
                        //byte[] buffer = new byte[1048576];
                        byte[] buffer = new byte[fsCrypt.Length];
                        while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fsOut.Write(buffer, 0, read);
                            decrypted = buffer;
                        }
                    }
                }
            }
        }

        //public static RijndaelManaged GenerateKeys()
        //{
        //    using (RijndaelManaged myRijndael = new RijndaelManaged())
        //    {

        //        myRijndael.GenerateKey();
        //        myRijndael.GenerateIV();

        //        return myRijndael;
        //    }
        //}

        //private byte[] Decrypt(byte[] encrypted, byte[] Key, byte[] IV)
        //{
        //    if (encrypted == null || encrypted.Length <= 0)
        //        throw new ArgumentNullException("cipherText");
        //    if (Key == null || Key.Length <= 0)
        //        throw new ArgumentNullException("Key");
        //    if (IV == null || IV.Length <= 0)
        //        throw new ArgumentNullException("IV");

        //    // Declare the string used to hold
        //    // the decrypted text.
        //    byte[] plaintext = null;

        //    // Create an Aes object
        //    // with the specified key and IV.
        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        aesAlg.Key = Key;
        //        aesAlg.IV = IV;

        //        // Create a decryptor to perform the stream transform.
        //        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        //        // Create the streams used for decryption.
        //        using (MemoryStream msDecrypt = new MemoryStream(encrypted))
        //        {
        //            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
        //                {
        //                    using (var memstream = new MemoryStream())
        //                    {
        //                        srDecrypt.BaseStream.CopyTo(memstream);
        //                        plaintext = memstream.ToArray();
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return plaintext;
        //}

        //private byte[] Encrypt(byte[] original, byte[] Key, byte[] IV)
        //{
        //    // Check arguments.
        //    if (original == null || original.Length <= 0)
        //        throw new ArgumentNullException("bytes");
        //    if (Key == null || Key.Length <= 0)
        //        throw new ArgumentNullException("Key");
        //    if (IV == null || IV.Length <= 0)
        //        throw new ArgumentNullException("IV");
        //    byte[] encrypted;

        //    // Create an Aes object
        //    // with the specified key and IV.
        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        aesAlg.Key = Key;
        //        aesAlg.IV = IV;

        //        // Create an encryptor to perform the stream transform.
        //        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        //        // Create the streams used for encryption.
        //        using (MemoryStream msEncrypt = new MemoryStream())
        //        {
        //            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
        //                {
        //                    //Write all data to the stream.
        //                    //swEncrypt.Write(original);
        //                    swEncrypt.WriteLine(original);
        //                }
        //                encrypted = msEncrypt.ToArray();
        //            }
        //        }
        //    }

        //    // Return the encrypted bytes from the memory stream.
        //    return encrypted;
        //}

       
    }
}