using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFDialTest
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var items = new ObservableCollection<string>();
            DependencyService.Get<IDialSetting>().Setting(items);

            this.BindingContext = items;
    }
}
}
