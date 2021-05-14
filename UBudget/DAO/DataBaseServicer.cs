using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LiteDB;
using UBudget.Models;

namespace UBudget.DAO
{
    public class DataBaseServicer
    {
        // Events
        public event EventHandler DataBaseAccountUpdate;

        // Retrieval 
        public List<Account> getAll()
        {
            return App.Database.GetCollection<Account>("accounts").FindAll().ToList();
        }
        public List<Transaction> getAllTx()
        {
            return App.Database.GetCollection<Transaction>("transactions").FindAll().ToList();
        }
        public List<PayStub> getAllIncome()
        {
            var stubs = App.Database.GetCollection<PayStub>("paystubs").FindAll();
            return (stubs != null) ? stubs.ToList() : new List<PayStub>();
        }
        public List<Settings> getSettings()
        {
            return App.Database.GetCollection<Settings>("categoryColorSettings").FindAll().ToList();
        }
        public Account getAccountById(int ID)
        {
            var collection = App.Database.GetCollection<Account>("accounts");
            return collection.FindOne(x => x.ID == ID);
        }
        public Account getAccountByName(string name)
        {
            var collection = App.Database.GetCollection<Account>("accounts");
            return collection.FindOne(x => x.Name == name);
        }

        // Make Changes To Documents
        public void addAccount(Account acc)
        {
            if (acc == null)
            {
                return;
            }
            App.Database.GetCollection<Account>("accounts").Insert(acc);
        }
        public void rmAccount(Account acc)
        {
            if (acc == null)
            {
                return;
            }

            var accounts = App.Database.GetCollection<Account>("accounts");
            var associatedTxs = App.Database.GetCollection<Transaction>("transactions");

            accounts.DeleteMany(x => x.ID == acc.ID);
            associatedTxs.DeleteMany(x => x.AccountID == acc.ID);
        }
        public void addTx(Transaction transaction)
        {

            App.Database.GetCollection<Transaction>("transactions").Insert(transaction);

            var accounts = App.Database.GetCollection<Account>("accounts");
            var account = accounts.FindOne(x => x.ID == transaction.AccountID);

            account.Value -= transaction.Amount;
            accounts.Update(account);

            DataBaseAccountUpdate(account, new AccountUpdateEventArgs());
        }
        public void rmTx(Transaction transaction)
        {
            var transactions = App.Database.GetCollection<Transaction>("transactions");
            var accounts = App.Database.GetCollection<Account>("accounts");
            var account = accounts.FindOne(x => x.ID == transaction.AccountID);

            transactions.Delete(transaction.TxID);

            account.Value += transaction.Amount;
            accounts.Update(account);
            
            DataBaseAccountUpdate(account, new AccountUpdateEventArgs());
        }
        public void addLabel(string label,int txID)
        {
            var collection = App.Database.GetCollection<Transaction>("transactions");
            var transaction = collection.FindOne(x => x.TxID == txID);

            transaction.Label = label;

            collection.Update(transaction);
        }
        public void rmLabel(int txID)
        {
            var colllection = App.Database.GetCollection<Transaction>("transactions");
            var transaction = colllection.FindOne(x => x.TxID == txID);

            transaction.Label = "";

            colllection.Update(transaction);
        }
        public void addIncome(PayStub payStub)
        {
            var stubCollection = App.Database.GetCollection<PayStub>("paystubs");
            var accountCollection = App.Database.GetCollection<Account>("accounts");
            var accountAssociated = accountCollection.FindOne(x => x.ID == payStub.AccountID);

            stubCollection.Insert(payStub);

            if (accountAssociated != null)
            {
                accountAssociated.Value += payStub.GrossAmount;
                accountCollection.Update(accountAssociated);
                if (DataBaseAccountUpdate != null)
                    DataBaseAccountUpdate(accountAssociated, new AccountUpdateEventArgs() { Name = accountAssociated.Name, AccountID = accountAssociated.ID, Value = accountAssociated.Value});
            }
        }
        public void rmIncome(int stubID)
        {
            var stubCollection = App.Database.GetCollection<PayStub>("paystubs");
            var stubAssociated = stubCollection.FindById(stubID);

            if (stubAssociated == null || stubCollection == null)
            {
                return;
            }

            var accountCollection = App.Database.GetCollection<Account>("accounts");
            var accountAssociated = accountCollection.FindOne(x => x.ID == stubAssociated.AccountID);

            if (accountAssociated != null)
            {
                accountAssociated.Value -= stubCollection.FindById(stubID).GrossAmount;
                accountCollection.Update(accountAssociated);
                DataBaseAccountUpdate(accountAssociated, new AccountUpdateEventArgs() { Name = accountAssociated.Name, AccountID = accountAssociated.ID, Value = accountAssociated.Value });
            }

            stubCollection.Delete(stubID);

        }
        public void addSetting(Settings setting)
        {
            if (setting == null)
            {
                return;
            }
            App.Database.GetCollection<Settings>("categoryColorSettings").Insert(setting);
        }
        public void updateSetting(string name, string color)
        {
            var setting = getSettings().First((x) => x.categoryName == name);
            setting.categoryColor = color;
            App.Database.GetCollection<Settings>("categoryColorSettings").Update(setting);
        }
    }

    class AccountUpdateEventArgs : EventArgs
    {
        public string Name;
        public int AccountID;
        public double Value;
    }
}
