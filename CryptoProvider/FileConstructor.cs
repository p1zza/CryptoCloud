using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CryptoProvider
{
    class FileConstructor
    {
        public FileConstructor(CryptoAgent cryptoAgent)
        {
            //FileStream fileStream = new FileStream(cryptoAgent.Path, FileMode.Open);
            File.SetAttributes(cryptoAgent.Path, FileAttributes.Hidden);

            FileStream encrFile = new FileStream(cryptoAgent.Name + "-encr" + cryptoAgent.Extension, FileMode.OpenOrCreate);
            encrFile.Write(cryptoAgent.encrypted, 0, cryptoAgent.EncrSize);
            encrFile.Close();
            FileStream decrFile = new FileStream(cryptoAgent.Name + "-decr" + cryptoAgent.Extension, FileMode.OpenOrCreate);
            decrFile.Write(cryptoAgent.decrypted, 0, cryptoAgent.DecrSize);
            decrFile.Close();
        }
    }
}
