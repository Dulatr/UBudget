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

            foreach (Account account in App.Servicer.getAll())
            {
                Accounts.Add(account);
            }

            App.Servicer.DataBaseAccountUpdate += BaseServicer_DataBaseUpdate;

            MainPage.setCommandsToPage(this);
        }

        // Properties
        private ObservableCollection<Account> _accounts;
        public ObservableCollection<Account> Accounts
        {
            get
            {
                if (_accounts == null)
                    _accounts = new ObservableCollection<Account>();
                return _accounts;
            }
            set
            {
                _accounts = value;
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
            var _account = Accounts.FirstOrDefault((x) => x.ID == (sender as Account).ID);
            if (_account == null)
            {
                Accounts.Add(_account);
                return;
            }
            _account.Value = (sender as Account).Value;
        }

    }
}
