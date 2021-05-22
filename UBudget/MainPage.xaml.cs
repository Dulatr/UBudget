using System;
using System.Diagnostics;
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

using UBudget.Views;
using UBudget.Models;
using System.ComponentModel;


namespace UBudget
{

    public sealed partial class MainPage : Page
    {
        // Properties
        private static List<string> IncomeButtons = new List<string>() { "AddIncomeCommandButton", "RemoveIncomeButton" };
        private static List<string> TransactionButtons = new List<string>() { "AddButton","LabelButton","RmLabelButton","DeleteButton" };
        private static CommandBar commands;
        private static CommandBar Commands
        {
            get
            {
                if (commands == null)
                {
                    commands = new CommandBar();
                }
                return commands;
            }
            set
            {
                commands = value;
            }
        }
        private static List<Button> flyoutButtons;
        public static List<Button> FlyoutButtons
        {
            get
            {
                if (flyoutButtons == null)
                {
                    flyoutButtons = new List<Button>();
                }
                return flyoutButtons;
            }
            set
            {
                flyoutButtons = value;
            }
        }
        private static List<TextBox> flyoutTextBoxInputs;
        public static List<TextBox> FlyoutTextBoxInputs
        {
            get
            {
                if (flyoutTextBoxInputs == null)
                    flyoutTextBoxInputs = new List<TextBox>();
                return flyoutTextBoxInputs;
            }
        }
        private static List<ComboBox> flyoutComboBoxInputs;
        public static List<ComboBox> FlyoutComboBoxInputs
        {
            get
            {
                if (flyoutComboBoxInputs == null)
                    flyoutComboBoxInputs = new List<ComboBox>();
                return flyoutComboBoxInputs;
            }
        }
        private static List<RoutedEventHandler> eventsThatHaveBeenRouted;
        private static List<RoutedEventHandler> EventsThatHaveBeenRouted
        {
            get
            {
                if (eventsThatHaveBeenRouted == null)
                    eventsThatHaveBeenRouted = new List<RoutedEventHandler>();
                return eventsThatHaveBeenRouted;
            }

        }
        
        // Constructor
        public MainPage()
        {
            this.InitializeComponent();

            this.AppNav.ItemInvoked += AppNav_ItemInvoked;
            this.MainFrame.Navigate(typeof(HomePage));

            Commands = MainCommandBar;

            var flyoutPanels = new List<StackPanel>();
            flyoutPanels.Add(AddAccountFlyoutPanel);
            flyoutPanels.Add(AddTxFlyoutPanel);
            flyoutPanels.Add(LabelButtonFlyoutPanel);
            flyoutPanels.Add(DeleteButtonFlyoutPanel);
            flyoutPanels.Add(AddIncomeCommandFlyoutPanel);
            flyoutPanels.Add(AddCategoryColorFlyout);

            foreach (StackPanel panel in flyoutPanels)
            {
                foreach(object child in panel.Children)
                {
                    if (child is Button)
                        FlyoutButtons.Add(child as Button);
                    else if (child is TextBox)
                        FlyoutTextBoxInputs.Add(child as TextBox);
                    else if (child is ComboBox)
                        FlyoutComboBoxInputs.Add(child as ComboBox);
                }
            }

            setCommandsToPage(this);
        }
        
        // Events
        private void AppNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem is string)
            {
                if (args.InvokedItem is "Home")
                {
                    this.MainFrame.Navigate(typeof(HomePage));
                }
                else if (args.InvokedItem is "Transactions")
                {
                    this.MainFrame.Navigate(typeof(TransactionsPage));
                }
                else if(args.InvokedItem is "Income")
                {
                    this.MainFrame.Navigate(typeof(IncomePage));
                }
                else if (args.InvokedItem is "Budget")
                {
                    this.MainFrame.Navigate(typeof(BudgetPage));
                }
                else if (args.InvokedItem is "Projection")
                {
                    this.MainFrame.Navigate(typeof(ProjectionPage));
                }
                else if (args.InvokedItem is "Taxes")
                {
                    this.MainFrame.Navigate(typeof(TaxPage));
                }

            }
        }
        
        // Class methods
        public static void setCommandsToPage(Page page)
        {
            if (page is MainPage || page is HomePage)
            {
                UpdateCommands(new List<string>());
            }
            else if (page is IncomePage)
            {
                UpdateCommands(IncomeButtons);
            }
            else if (page is TransactionsPage)
            {
                UpdateCommands(TransactionButtons);
            }
            else if (page is BudgetPage)
            {
                UpdateCommands(new List<string>() { "AddCategoryColor" });
            }
            else if (page is ProjectionPage)
            {
                UpdateCommands(new List<string>());
            }
            //else if (page is TaxPage)
            //{
            //    UpdateCommands(new List<string>() { "AddTaxFormCommandButton", "RemoveTaxFormCommandButton" });
            //}
        }
        public static void setFlyoutButtonClickEvent(string flyoutButtonName,RoutedEventHandler method)
        {
            if(EventsThatHaveBeenRouted.Any(x=>x.Method == method.Method))
            {
                return;
            }

            EventsThatHaveBeenRouted.Add(method);

            foreach (Button button in FlyoutButtons)
            {
                if (button.Name == flyoutButtonName)
                {
                    button.Click += method;
                }
            }

        }
        public static void setCommandButtonClickEvent(string buttonName, RoutedEventHandler method)
        {
            foreach (AppBarButton button in Commands.PrimaryCommands)
            {
                if (button.Name == buttonName)
                {
                    button.Click += method;
                }
            }
        }

        // Use these methods to unregister events to the MainPage. Leaving events subscribed
        // has the unfortunate side effect of keeping the subscribed page in memory
        public static void removeFlyoutClickEvent(string flyoutButtonName, RoutedEventHandler method)
		{
            foreach (Button button in FlyoutButtons)
			{
                if (button.Name == flyoutButtonName)
				{
                    button.Click -= method;
                    EventsThatHaveBeenRouted.Remove(method);
				}
			}
		}
        public static void removeCommandClickEvent(string commandButtonName, RoutedEventHandler method)
        {
            foreach (Button button in Commands.PrimaryCommands)
            {
                if (button.Name == commandButtonName)
                {
                    button.Click -= method;
                    EventsThatHaveBeenRouted.Remove(method);
                }
            }
        }

        // Helper Methods
        private static void UpdateCommands(List<string> buttonList)
        {
            foreach (Button button in Commands.PrimaryCommands)
            {
                if (buttonList.Any((x)=>x==button.Name))
                {
                    button.Visibility = Visibility.Visible;
                }
                else
                {
                    button.Visibility = Visibility.Collapsed;
                }
            }
        }
    }

}
