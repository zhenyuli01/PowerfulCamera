using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<TestClass> vsd = new ObservableCollection<TestClass>();
        public ObservableCollection<TestClass> ListBoxItemSource
        {
            set
            {
                vsd = value;
                RaisePropertyChanged();
            }
            get => vsd;
        }
        public RelayCommand AddButton { set; get; }
        public RelayCommand SubButton { set; get; }
        public MainWindowViewModel()
        {
            vsd.Add(new TestClass("小明", 123));
            vsd.Add(new TestClass("小红", 456));
            AddButton = new RelayCommand(() =>
            {
                vsd.Add(new TestClass("123d", 123));
                vsd.Add(new TestClass("456d", 456));
            });
            SubButton = new RelayCommand(() =>
            {
            });
        }
    }
    public class TestClass
    {
        private string name;
        private int num;
        public string Name
        {
            set => name = value;
            get => name;
        }
        public int Num
        {
            set => num = value;
            get => num;
        }
        public TestClass(string _name, int _num)
        {
            Name = _name;
            Num = _num;
        }
    }
}
