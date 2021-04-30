using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UBudget.DAO;
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
    public sealed partial class IncomePage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<PayStub> payStubList = new ObservableCollection<PayStub>();

        public event PropertyChangedEventHandler PropertyChanged;

        // Properties
        private PayStub selectedStubView;
        public PayStub SelectedStubView {
            get { return selectedStubView; }
            set
            {
                selectedStubView = value;
                OnPropertyChanged(nameof(SelectedStubView));
                OnPropertyChanged(nameof(StubDateConverter));
            }
        }
        public string StubDateConverter {
            get
            {
                if (SelectedStubView != null)
                    return $"{SelectedStubView.Date.Month}/{SelectedStubView.Date.Day}/{SelectedStubView.Date.Year}";
                else
                    return "";
            }
            set
            {
                if(SelectedStubView != null)
                {
                    value = $"{SelectedStubView.Date.Month}/{SelectedStubView.Date.Day}/{SelectedStubView.Date.Year}";
                    OnPropertyChanged(nameof(StubDateConverter));
                }
            }
        }

        // Constructor
        public IncomePage()
        {
            this.InitializeComponent();

            PayStubs.ItemsSource = payStubList;
            foreach (PayStub stub in App.Servicer.getAllIncome())
            {
                payStubList.Add(stub);
            }

            PayStubs.SelectionChanged += PayStubs_SelectionChanged;

            MainPage.setCommandsToPage(this);

            MainPage.setFlyoutButtonClickEvent("AddIncomeFormButton", AddIncomeFormButton_Click);
            MainPage.setCommandButtonClickEvent("RemoveIncomeButton", RemoveIncomeButton_Click);
        }

        // Events
        private void PayStubs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedStubView = PayStubs.SelectedItem as PayStub;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            MainPage.removeFlyoutClickEvent("AddIncomeFormButton", AddIncomeFormButton_Click);
            MainPage.removeCommandClickEvent("RemoveIncomeButton", RemoveIncomeButton_Click);
        }

        private void RemoveIncomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (PayStubs.SelectedItem != null)
                App.Servicer.rmIncome((PayStubs.SelectedItem as PayStub).PayStubID);

            payStubList.Clear();
            foreach (PayStub stub in App.Servicer.getAllIncome())
            {
                payStubList.Add(stub);
            }
            
        }
        private void AddIncomeFormButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime date;
            int jobID, accountID;
            double netAmount, sit, fit;

            var paystub = new PayStub() {
                Date = DateTime.TryParse(
                    MainPage.FlyoutTextBoxInputs.First(x=>x.Name == "IncomeDateBox").Text,out date) ? date : DateTime.Now,
                Label = MainPage.FlyoutTextBoxInputs.First(x=>x.Name == "IncomeLabelBox").Text,
                JobID = int.TryParse(
                    MainPage.FlyoutTextBoxInputs.First(x=>x.Name == "JobIDBox").Text,out jobID) ? jobID : -1,
                AccountID = int.TryParse(
                    MainPage.FlyoutTextBoxInputs.First(x=>x.Name == "IncomeAccountIDBox").Text,out accountID) ? accountID : -1,
                NetAmount = double.TryParse(
                    MainPage.FlyoutTextBoxInputs.First(x=>x.Name == "NetPayBox").Text,out netAmount) ? netAmount : 0.0,
                SIT = double.TryParse(
                    MainPage.FlyoutTextBoxInputs.First(x=>x.Name == "SITBox").Text,out sit) ? sit : 0.0,
                FIT = double.TryParse(
                    MainPage.FlyoutTextBoxInputs.First(x=>x.Name == "FITBox").Text,out fit) ? fit : 0.0
            };

            App.Servicer.addIncome(paystub);

            payStubList.Clear();
            foreach (PayStub stub in App.Servicer.getAllIncome())
            {
                payStubList.Add(stub);
            }
                
            SelectedStubView = payStubList.Last();
        }

    }
}
