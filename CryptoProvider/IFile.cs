using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoProvider
{
    interface IFile
    {
        int DecrSize { get; set; }
        int EncrSize { get; set; }
        byte[] Hash { get; set; }
        string Name { get; set; }
        string Extension { get; set; }

        string Path { get; set; }
       
        //delegate void MoveHandler(string todo);
        //event MoveHandler MoveEvent;
        

    }
}
