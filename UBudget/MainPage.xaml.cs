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
                if (args.InvokedItem is "Transactions")
                {
                    this.MainFrame.Navigate(typeof(TransactionsPage));
                }
                if(args.InvokedItem is "Income")
                {
                    this.MainFrame.Navigate(typeof(IncomePage));
                }
            }
        }
        
        // Class methods
        public static void setCommandsToPage(Page page)
        {
            if (page is MainPage || page is HomePage)
            {
                foreach (AppBarButton button in Commands.PrimaryCommands)
                {
                    button.Visibility = Visibility.Collapsed;
                }
            }
            else if (page is IncomePage)
            {
                foreach (AppBarButton button in Commands.PrimaryCommands){
                    if (button.Name == "AddIncomeCommandButton" || button.Name == "RemoveIncomeButton")
                    {
                        button.Visibility = Visibility.Visible;
                    }
                    else
                        button.Visibility = Visibility.Collapsed;
                }
            }
            else if (page is TransactionsPage)
            {
                foreach (AppBarButton button in Commands.PrimaryCommands)
                {
                    if (button.Name == "AddButton" || button.Name == "LabelButton" || button.Name == "DeleteButton")
                    {
                        button.Visibility = Visibility.Visible;
                    }
                    else
                        button.Visibility = Visibility.Collapsed;
                }
            }
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
    }

}
