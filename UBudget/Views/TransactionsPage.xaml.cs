using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using UBudget.Models;
using UBudget.DAO;
using Windows.UI.Xaml.Navigation;

namespace UBudget.Views
{
    public sealed partial class TransactionsPage : Page
    {
        private ObservableCollection<Transaction> txList = new ObservableCollection<Transaction>();
        private ObservableCollection<Account> accountList = new ObservableCollection<Account>();

        // Constructor
        public TransactionsPage()
        {
            this.InitializeComponent();

            this.Transactions.ItemsSource = txList;
            this.Accounts.ItemsSource = accountList;

            foreach (Account acc in App.Servicer.getAll())
            {
                accountList.Add(acc);
            }
            if (accountList.Count != 0)
            {
                foreach (Transaction tx in App.Servicer.getAllTx().Where(x=>x.AccountID == accountList.First().ID))
                {
                    txList.Add(tx);
                }
            }

            Accounts.SelectionChanged += Accounts_SelectionChanged;

            MainPage.setCommandsToPage(this);

            MainPage.setFlyoutButtonClickEvent("AddAccountBtn", AddAccountBtn_Click);
            MainPage.setFlyoutButtonClickEvent("AddTxButton", AddTxButton_Click);
            MainPage.setFlyoutButtonClickEvent("AddLabelButton", AddLabelButton_Click);
            MainPage.setFlyoutButtonClickEvent("RmLabelButton", RmLabelButton_Click);
            MainPage.setFlyoutButtonClickEvent("DeleteSelectedButton", DeleteSelectedButton_Click);
        }

        private void AddLabelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Transactions.SelectedItem == null)
            {
                return;
            }

            var transaction = Transactions.SelectedItem as Transaction;
            var found = (MainPage.FlyoutComboBoxInputs.First(x => x.Name == "LabelsBox").SelectedItem);
            App.Servicer.addLabel(
                (MainPage.FlyoutComboBoxInputs.First(x=>x.Name == "LabelsBox").SelectedItem as BudgetCategory).Name,
                transaction.TxID
            );

            txList.Clear();
            foreach (Transaction tx in App.Servicer.getAllTx().Where(x=>x.AccountID == transaction.AccountID))
            {
                txList.Add(tx);
            }
        }

        private void RmLabelButton_Click(object sender, RoutedEventArgs e)
        {
            if (Transactions.SelectedItem == null)
            {
                return;
            }

            var transaction = Transactions.SelectedItem as Transaction;
            App.Servicer.rmLabel(
                transaction.TxID
            );

            txList.Clear();
            foreach (Transaction tx in App.Servicer.getAllTx().Where(x => x.AccountID == transaction.AccountID))
            {
                txList.Add(tx);
            }
            Transactions.SelectedItem = transaction;
        }

        private void AddTxButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime date;
            double amount;
            var account = App.Servicer.getAccountByName(
                MainPage.FlyoutTextBoxInputs.Find(x=>x.Name == "TxAccountAssociatedTextBox").Text
            );

            if (account == null)
            {
                return;
            }

            var transaction = new Transaction()
            {
                AccountID = account.ID,

                DateTime = DateTime.TryParse(
                    MainPage.FlyoutTextBoxInputs.First(x=> x.Name == "TxDateTextBox").Text, out date) ? date : DateTime.Now,

                Amount = Double.TryParse(
                    MainPage.FlyoutTextBoxInputs.Find(x=>x.Name == "TxAmountTextBox").Text,out amount) ? amount : 0.0
            };

            App.Servicer.addTx(transaction);
            Accounts.SelectedItem = account;

            txList.Clear();
            if(App.Servicer.getAllTx().Where<Transaction>(x => x.AccountID == account.ID).Count() != 0)
            {
                foreach(Transaction tx in App.Servicer.getAllTx().Where(x=>x.AccountID == account.ID))
                {
                    if (tx.AccountID == account.ID)
                    {
                        txList.Add(tx);
                    }
                }
            }
        }

        private void DeleteSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem userSelected = MainPage.FlyoutComboBoxInputs.Find(x => x.Name == "AccountOrTransactionList").SelectedItem as ComboBoxItem;
            if (userSelected.Content.ToString() == "Account")
            {
                if (Accounts.SelectedItem is null)
                {
                    return;
                }
                App.Servicer.rmAccount((Accounts.SelectedItem as Account));
                var accList = App.Servicer.getAll();

                accountList.Clear();
                if (accList.Count != 0)
                {
                    foreach(Account account in accList)
                    {
                        accountList.Add(account);
                    }
                    Accounts.SelectedItem = accountList.First();
                }
                else
                {
                    txList.Clear();
                    return;
                }
                               
                txList.Clear();
                foreach (Transaction tx in App.Servicer.getAllTx())
                {
                    if(tx.AccountID == (Accounts.SelectedItem as Account).ID)
                    {
                        txList.Add(tx);
                    }
                }
                
            }
            else
            {
                if (Transactions.SelectedItem is null)
                {
                    return;
                }
                App.Servicer.rmTx(Transactions.SelectedItem as Transaction);
                txList.Remove(Transactions.SelectedItem as Transaction);
            }
        }

        private void AddAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            double value = 0.0;

            var account = new Account()
            {
                Name = MainPage.FlyoutTextBoxInputs.First(x=>x.Name == "AccountNameInputTextBox").Text,
                Value = Double.TryParse(
                    MainPage.FlyoutTextBoxInputs.First(x=>x.Name == "ValueInputTextBox").Text, out value) ? value : 0.0,
            };

            App.Servicer.addAccount(account);
            accountList.Add(account);

        }

        private void Accounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Accounts.SelectedItem is null)
            {
                return;
            }

            var response = App.Servicer.getAccountById((Accounts.SelectedItem as Account).ID);
            txList.Clear();
            foreach(Transaction transaction in App.Servicer.getAllTx())
            {
                if(response.ID == transaction.AccountID)
                {
                    txList.Add(transaction);
                }
            }
            
        }

        // Cleans up any subscriptions to the MainPage so nothing weird is kept around in memory
		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);

            MainPage.removeFlyoutClickEvent("AddAccountBtn", AddAccountBtn_Click);
            MainPage.removeFlyoutClickEvent("AddTxButton", AddTxButton_Click);
            MainPage.removeFlyoutClickEvent("AddLabelButton", AddLabelButton_Click);
            MainPage.removeFlyoutClickEvent("DeleteSelectedButton", DeleteSelectedButton_Click);
        }
	}
}
