using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using UBudget.Models;

namespace UBudget.Views.StatusViews
{
    public sealed partial class AccountCreationPage : Page
    {
        public AccountCreationPage()
        {
            this.InitializeComponent();
            SubmitBttn.Click += SubmitBttn_Click;
            
        }

        private void SubmitBttn_Click(object sender, RoutedEventArgs e)
        {
            Frame mf = Window.Current.Content as Frame;
            MainPage mp = mf.Content as MainPage;
            mp.MainFrame.Navigate(typeof(HomePage));

            if (AccountValueBx.Text == "")
            {
                AccountValueBx.Text = "0.00";
            }

            App.Servicer.addAccount(new Account()
            {
                Name = AccountNameBx.Text,
                Value = Double.Parse(AccountValueBx.Text.Replace("$",""))
            });

            // commented for testing currently
            //App.Servicer.updateUserSetting();
        }
    }
}
