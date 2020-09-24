using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CryptoCloud.Models
{
    class MainViewModel
    {

        private int _Clicks;
        public int Clicks
        {
            get { return _Clicks; }
            set
            {
                _Clicks = value;
            }
        }

        public ICommand ClickAdd
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    Clicks++;
                });
            }
        }
    }
}
