using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public List<Item> List { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            List = new List<Item>();
            List.Add(new Item { Count = 100 });
            List.Add(new Item { Count = 1 });

            text1.SetBinding(TextBlock.TextProperty, new Binding("Count") { Source = this.List[0] }); //绑定设备号
            Changed(nameof(List));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Changed(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }
    }
}
