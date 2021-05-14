using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UBudget.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UBudget.Views
{
    public sealed partial class ProjectionPage : Page, INotifyPropertyChanged
    {
        private PlotModel data;
        private PlotModel Data
        {
            get { return data; }
            set { data = value; OnPropertyChanged(nameof(Data)); }
        }

        public ProjectionPage()
        {
            this.InitializeComponent();

            Data = new PlotModel();

            var accountTotal = new BarSeries();
            var incomeTotal = new BarSeries();


            accountTotal.Items.Add(new BarItem(getAccountsTotal()) { Color=OxyColors.RosyBrown});
            incomeTotal.Items.Add(new BarItem(getRecentPaystubTotal()) { Color=OxyColors.ForestGreen });

            accountTotal.IsStacked = true;
            incomeTotal.IsStacked = true;

            Data.Series.Add(accountTotal);
            Data.Series.Add(incomeTotal);
            MainPage.setCommandsToPage(this);
        }

        private double getAccountsTotal()
        {
            var accounts = App.Servicer.getAll();
            double total = 0.0;

            foreach (Account account in accounts)
            {
                total += account.Value;
            }

            return total;
        }
        private double getRecentPaystubTotal()
        {
            var stubs = App.Servicer.getAllIncome();
            var lastStub = (stubs.Count != 0 ) ? stubs.Last() : null;
            return (lastStub != null) ? lastStub.GrossAmount : 0.0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
