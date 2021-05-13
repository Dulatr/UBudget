﻿using System;
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
    public sealed partial class BudgetPage : Page
    {
        private ObservableCollection<BudgetCategory> categories = new ObservableCollection<BudgetCategory>();

        public BudgetPage()
        {
            this.InitializeComponent();

            this.Categories.ItemsSource = null;
            this.Categories.ItemsSource = categories;

            foreach (Transaction tx in App.Servicer.getAllTx())
            {
                if (!String.IsNullOrEmpty(tx.Label))
                {
                    if(!categories.Any((x) => x.Name == tx.Label))
                    {
                        categories.Add(new BudgetCategory()
                        {
                            Name=tx.Label,
                            Amount=tx.Amount,
                        });
                    }
                    else
                    {
                        var category = categories.First((x) => x.Name == tx.Label);
                        category.Amount += tx.Amount;
                    }
                }                
            }

            MainPage.setCommandsToPage(this);
            MainPage.setFlyoutButtonClickEvent("AddColorButton",OnButtonClick);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            MainPage.removeFlyoutClickEvent("AddColorButton", OnButtonClick);
        }

        private void OnButtonClick(object sender,RoutedEventArgs e)
        {
            var selectedColor = (MainPage.FlyoutComboBoxInputs.Find((x) => x.Name == "CategoryColorSelection").SelectedItem as ComboBoxItem)?.Content.ToString();
            var selectedCategory = Categories.SelectedItem as BudgetCategory;
            if (selectedCategory != null && selectedColor != null)
            {
                SolidColorBrush chosenColor;
                switch (selectedColor)
                {
                    case "Red":
                        chosenColor = new SolidColorBrush(Windows.UI.Colors.Red);
                        break;
                    case "Blue":
                        chosenColor = new SolidColorBrush(Windows.UI.Colors.Blue);
                        break;
                    case "Green":
                        chosenColor = new SolidColorBrush(Windows.UI.Colors.Green);
                        break;
                    default:
                        chosenColor = new SolidColorBrush(Windows.UI.Colors.DarkGray);
                        break;
                }
                categories.FirstOrDefault((x) => x.Name == selectedCategory.Name).Brush = chosenColor;

                //just force the updated collection on the itemsource
                //it's moderately consentual. and the items are tiny anyways
                Categories.ItemsSource = null;
                Categories.ItemsSource = categories;
            }

        }


    }
}
