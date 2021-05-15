using OxyPlot;
using OxyPlot.Axes;
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
            var accountTotal = new ColumnSeries();
            var billTotal = new ColumnSeries();
            var categoryAxis = new CategoryAxis();
            var linearAxis = new LinearAxis();

            #region Plot and Axis Settings

            ProjectionsPlot.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
 
            Data.Title = "Estimate of account total based on previous income";
            Data.TitleFontSize = 24;
            Data.TitleColor = OxyColors.White;
            Data.PlotAreaBorderColor = OxyColors.White;

            linearAxis.Title = "Net Value in USD";
            linearAxis.TitleFontSize = 18;
            linearAxis.AxisTitleDistance = 15;
            linearAxis.MinorTickSize = 250;
            linearAxis.MinorTickSize = 3;
            linearAxis.IsZoomEnabled = false;

            linearAxis.TextColor = OxyColors.White;
            linearAxis.TicklineColor = OxyColors.White;
            linearAxis.TitleColor = OxyColors.White;

            categoryAxis.Title = "Date";
            categoryAxis.TitleFontSize = 18;
            categoryAxis.AxisTitleDistance = 15;
            categoryAxis.IsZoomEnabled = false;

            categoryAxis.TextColor = OxyColors.White;
            categoryAxis.TicklineColor = OxyColors.White;
            categoryAxis.TitleColor = OxyColors.White;

            #endregion

            DateTime today = DateTime.Today;
            categoryAxis.Labels.Add($"{today.Month}/{DateTime.DaysInMonth(today.Year, today.Month)}");
            categoryAxis.Labels.Add($"{today.AddMonths(1).Month}/{DateTime.DaysInMonth(today.Year,today.AddMonths(1).Month)}");
            categoryAxis.Labels.Add($"{today.AddMonths(2).Month}/{DateTime.DaysInMonth(today.Year, today.AddMonths(2).Month)}");

            Data.Axes.Add(categoryAxis);
            Data.Axes.Add(linearAxis);

            double _billAmount = getBillTotal();
            double _incomeAmount = getRecentPaystubTotal();
            double _accountTotal = getAccountsTotal();

            billTotal.Items.Add(new ColumnItem(_billAmount) { Color = OxyColors.Red });
            billTotal.Items.Add(new ColumnItem(_billAmount) { Color = OxyColors.Red });
            billTotal.Items.Add(new ColumnItem(_billAmount) { Color = OxyColors.Red });

            accountTotal.Items.Add(new ColumnItem(_accountTotal) { Color = OxyColors.ForestGreen });
            accountTotal.Items.Add(new ColumnItem(_accountTotal + _incomeAmount * 2.0 - _billAmount) { Color = OxyColors.ForestGreen });
            accountTotal.Items.Add(new ColumnItem(_accountTotal + _incomeAmount * 4.0 - _billAmount * 2.0) { Color = OxyColors.ForestGreen });

            accountTotal.IsStacked = true;
            billTotal.IsStacked = true;

            Data.Series.Add(accountTotal);
            Data.Series.Add(billTotal);

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
            double _stubTotal = 0.0;

            var stubs = App.Servicer.getAllIncome(
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month))
            );

            foreach (PayStub stub in stubs)
            {
                _stubTotal += stub.GrossAmount;
            }

            return _stubTotal;
        }
        private double getBillTotal()
        {
            double _billAmount = 0.0;

            var txs = App.Servicer.getAllTx(
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month))
            );

            var bills = txs?.FindAll(
                (x) => x.Label == "Bills"
            );

            foreach (Transaction bill in bills)
            {
                _billAmount += bill.Amount;
            }

            return _billAmount;
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
