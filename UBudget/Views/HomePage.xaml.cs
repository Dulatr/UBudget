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
    public sealed partial class HomePage : Page, INotifyPropertyChanged
    {
        public HomePage()
        {
            this.InitializeComponent();

            if (App.Servicer.getAll().Count != 0 && App.Servicer.getAll() != null)
                DisplayAccount = App.Servicer.getAll().First();

            App.Servicer.DataBaseAccountUpdate += BaseServicer_DataBaseUpdate;

            MainPage.setCommandsToPage(this);
        }

        // Properties
        private Account displayAccount;
        public Account DisplayAccount
        {
            get
            {
                if (displayAccount == null)
                    displayAccount = new Account();
                return displayAccount;
            }
            set
            {
                displayAccount = value;
                OnPropertyChanged(nameof(DisplayAccount));
            }
        }

        // Events
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void BaseServicer_DataBaseUpdate(object sender, EventArgs e)
        {
            DisplayAccount = (sender as Account);
        }

    }
}
