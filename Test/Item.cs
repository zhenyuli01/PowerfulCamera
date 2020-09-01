using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Test
{
    public class Item : INotifyPropertyChanged
    {
        private int count;

        public int Count { get => count; set => OnCountChanged(value); }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Changed(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        private void OnCountChanged(int v)
        {
            count = v;
            Changed(nameof(Count));
        }
    }
}
