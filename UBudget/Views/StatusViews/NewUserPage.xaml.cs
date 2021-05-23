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

namespace UBudget.Views.StatusViews
{
    public sealed partial class NewUserPage : Page
    {
        public NewUserPage()
        {
            this.InitializeComponent();
            NavigateToCreateAccountPageBtn.Click += NavigateToCreateAccountPageBtn_Click;
        }

        private void NavigateToCreateAccountPageBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame mf = Window.Current.Content as Frame;
            MainPage mp = mf.Content as MainPage;
            mp.MainFrame.Navigate(typeof(AccountCreationPage));
        }
    }
}
