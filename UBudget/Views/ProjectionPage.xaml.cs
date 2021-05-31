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
        private ColumnSeries accountTotal = new ColumnSeries() { FillColor = OxyColors.ForestGreen };
        private CategoryAxis categoryAxis = new CategoryAxis();
        private LinearAxis linearAxis = new LinearAxis();

        private List<ColumnSeries> series = new List<ColumnSeries>();

        private bool HasLoaded = false;
        private readonly int[] months = new int[] { 3, 6, 12, 24 };

        public ProjectionPage()
        {
            this.InitializeComponent();

            Data = new PlotModel();   
            DateTime today = DateTime.Today;
            double _incomeAmount = getRecentPaystubTotal();
            double _accountTotal = getAccountsTotal();
            
            #region Plot and Axis Settings

            ProjectionsPlot.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
 
            Data.Title = "Estimate of account total based on previous income";
            linearAxis.Title = "Net Value in USD";
            categoryAxis.Title = "Date";
            accountTotal.Title = "Account Total";

            Data.TitleFontSize = 24;
            Data.LegendPlacement = LegendPlacement.Inside;
            Data.LegendPosition = LegendPosition.RightMiddle;
            Data.IsLegendVisible = true;
            Data.TitleColor = OxyColors.White;
            Data.PlotAreaBorderColor = OxyColors.White;
            Data.LegendBackground = OxyColors.White;

            linearAxis.TitleFontSize = 18;
            linearAxis.AxisTitleDistance = 15;
            linearAxis.MinorTickSize = 250;
            linearAxis.MinorTickSize = 3;
            linearAxis.IsZoomEnabled = false;
            linearAxis.TextColor = OxyColors.White;
            linearAxis.TicklineColor = OxyColors.White;
            linearAxis.TitleColor = OxyColors.White;

            categoryAxis.TitleFontSize = 18;
            categoryAxis.AxisTitleDistance = 15;
            categoryAxis.IsZoomEnabled = false;
            categoryAxis.TextColor = OxyColors.White;
            categoryAxis.TicklineColor = OxyColors.White;
            categoryAxis.TitleColor = OxyColors.White;

            #endregion

            #region Initialize Plot Series

            categoryAxis.Labels.Add($"{today.Month}/{DateTime.DaysInMonth(today.Year, today.Month)}");
            categoryAxis.Labels.Add($"{today.AddMonths(1).Month}/{DateTime.DaysInMonth(today.Year, today.AddMonths(1).Month)}");
            categoryAxis.Labels.Add($"{today.AddMonths(2).Month}/{DateTime.DaysInMonth(today.Year, today.AddMonths(2).Month)}");

            Data.Axes.Add(categoryAxis);
            Data.Axes.Add(linearAxis);

            for (int i = 0; i < 3; i++)
            {
                // Default is bi-weekly for selection, so 2 * indexer
                accountTotal.Items.Add(new ColumnItem(_accountTotal + _incomeAmount * 2.0 * i));
            }

            accountTotal.IsStacked = true;

            // the 0th series is always the account totals
            series.Add(accountTotal);

            foreach (BudgetCategory category in App.Servicer.getAllBudgetCategories())
            {
                series.Add(new ColumnSeries()
                {
                    Title = category.Name,
                    IsStacked = true,
                });
                for (int i = 0; i < 3; i++)
                {
                    series.Last().Items.Add(new ColumnItem(category.Amount));
                    accountTotal.Items[i].Value -= series.Last().Items[i].Value * i;
                }
            }

            #endregion

            foreach (ColumnSeries s in series)
            {
                Data.Series.Add(s);
            }

            LengthOfTimeSelectionBox.SelectedIndex = 0;
            LengthOfTimeSelectionBox.SelectionChanged += OptionsSelectionChanged;

            PayFrequencySelectionBox.SelectedIndex = 0;
            PayFrequencySelectionBox.SelectionChanged += OptionsSelectionChanged;

            MainPage.setCommandsToPage(this);
        }

        private void OptionsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HasLoaded)
            {
                var frequency = ((PayFrequencySelectionBox.SelectedItem as ComboBoxItem).Content.ToString() == "Bi-weekly") ? 2.0 : 1.0;
                UpdateSeries(frequency,(int)LengthOfTimeSelectionBox.SelectedItem);
            }
            HasLoaded = true;
        }

        private void UpdateSeries(double frequency = 1.0, double length = 3.0)
        {
            DateTime today = DateTime.Today;
            var budgets = App.Servicer.getAllBudgetCategories();
            double _incomeAmount = getRecentPaystubTotal();
            double _accountTotal = getAccountsTotal();

            accountTotal.Items.Clear();
            
            foreach (ColumnSeries s in series)
            {
                s.Items.Clear();
            }

            categoryAxis.Labels.Clear();

            for (int i = 0; i < length; i++)
            {
                accountTotal.Items.Add(new ColumnItem(_accountTotal + _incomeAmount * frequency * i));
            }

            foreach (ColumnSeries s in series)
            {
                // get the current month budget totals
                if (s != accountTotal)
                {
                    for (int i = 0; i < length; i++)
                    {
                        s.Items.Add(new ColumnItem(
                            budgets.Find((x) => x.Name == s.Title).Amount
                        ));

                        accountTotal.Items[i].Value -= series.Last().Items[i].Value * i;
                    }

                }
            }

            for (int i = 0; i < length; i++)
            {
                categoryAxis.Labels.Add($"{today.AddMonths(i).Month}/{DateTime.DaysInMonth(today.Year, today.AddMonths(i).Month)}");
            }

            Data.InvalidatePlot(true);
            Data.Series.Clear();
            Data.Axes.Clear();

            Data.Axes.Add(categoryAxis);
            Data.Axes.Add(linearAxis);
            
            foreach (ColumnSeries s in series)
            {
                Data.Series.Add(s);
            }
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
        //private double getBillTotal()
        //{
        //    double _billAmount = 0.0;

        //    var txs = App.Servicer.getAllTx(
        //        new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
        //        new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month))
        //    );

        //    var bills = txs?.FindAll(
        //        (x) => x.Label == "Bills"
        //    );

        //    foreach (Transaction bill in bills)
        //    {
        //        _billAmount += bill.Amount;
        //    }

        //    return _billAmount;
        //}
        //private double getFoodTotal()
        //{
        //    double _foodAmount = 0.0;

        //    var txs = App.Servicer.getAllTx(
        //        new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
        //        new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month))
        //    );

        //    var foodTxs = txs?.FindAll(
        //        (x) => x.Label == "Food"
        //    );

        //    foreach (Transaction foodPurchase in foodTxs)
        //    {
        //        _foodAmount += foodPurchase.Amount;
        //    }
        //    return _foodAmount;
        //}
        //private double getMiscTotal()
        //{
        //    double _miscAmount = 0.0;

        //    var txs = App.Servicer.getAllTx(
        //        new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
        //        new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month))
        //    );

        //    var miscTxs = txs?.FindAll(
        //        (x) => x.Label == "Misc."
        //    );

        //    foreach (Transaction miscPurchase in miscTxs)
        //    {
        //        _miscAmount += miscPurchase.Amount;
        //    }
        //    return _miscAmount;
        //}

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
