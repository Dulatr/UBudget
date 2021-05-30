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
using Windows.UI;
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
        private MenuFlyout flyout = new MenuFlyout();
        private ContentDialog status = new ContentDialog() { Title = "Category Creation Status", CloseButtonText = "Ok" };
        private MenuFlyoutItem flyoutSelection = new MenuFlyoutItem() { Text = "Delete" };
        private BudgetCategory selected_category;

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

            foreach (Settings setting in App.Servicer.getSettings())
            {
                var settingFor = categories.FirstOrDefault((x) => x.Name == setting.categoryName);
                if (settingFor != null)
                    settingFor.Brush = toColor(setting.categoryColor);
            }

            flyout.Items.Add(flyoutSelection);
            flyoutSelection.Click += (sender,e) => {
                Delete_SelectedAsync();
            };

            Categories.RightTapped += Categories_RightTapped;

            MainPage.setCommandsToPage(this);
            MainPage.setFlyoutButtonClickEvent("AddCategoryFlyoutButton", AddCategoryAsync);
            MainPage.setFlyoutButtonClickEvent("AddColorButton",OnButtonClick);
        }

        private void Categories_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            this.flyout.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));

            selected_category = (e.OriginalSource as FrameworkElement).DataContext as BudgetCategory;
        }

        private async void Delete_SelectedAsync()
        {
            if (selected_category == null)
                return;

            categories.Remove(selected_category);
            App.Servicer.rmCategory(selected_category.Name);

            // need to search for all the transactions and clear their labels
            foreach (Transaction tx in App.Servicer.getAllTx())
            {
                if (tx.Label == selected_category.Name)
                {
                    App.Servicer.rmLabel(tx.TxID);
                }
            }

            Frame mf = Window.Current.Content as Frame;
            MainPage mp = mf.Content as MainPage;
            mp.BudgetCategories.Remove(mp.BudgetCategories.First((x) => x.Name == selected_category.Name));
            mp.LabelsBox.SelectedIndex = 0;
            status.Content = $"Category: `{selected_category.Name.ToUpper()}`, has been succesfully deleted!";
            await status.ShowAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            MainPage.removeFlyoutClickEvent("AddColorButton", OnButtonClick);
            MainPage.removeCommandClickEvent("AddCategoryFlyoutButton", AddCategoryAsync);
        }

        private async void AddCategoryAsync(object sender, RoutedEventArgs e)
        {
            Frame mf = Window.Current.Content as Frame;
            MainPage mp = mf.Content as MainPage;

            string name = MainPage.FlyoutTextBoxInputs.Find((x) => x.Name == "AddCategoryTextBox").Text;
            
            if (App.Servicer.categoryExists(name))
            {
                status.Content = $"A category already exists with the name: `{name.ToUpper()}`.";
                await status.ShowAsync();
                return;
            }

            App.Servicer.addCategory(name);

            // Pull from the servicer so you make sure you're referencing what you just added
            var addedCategory = App.Servicer.getAllBudgetCategories().Last();

            // If the property is not an observable collection, very very bad things happen Q.Q
            mp.BudgetCategories.Add(addedCategory);
            status.Content = $"`{name.ToUpper()}` successfully added as a category!\n\n".Replace("\n",Environment.NewLine) +
                            "This page won't display the new category until you start labeling transactions.\n".Replace("\n",Environment.NewLine) +
                            "After labeling a couple transactions can then come back here to see your tracked" +
                            "totals.";
            await status.ShowAsync();
        }

        private void OnButtonClick(object sender,RoutedEventArgs e)
        {
            var selectedColor = (MainPage.FlyoutComboBoxInputs.Find((x) => x.Name == "CategoryColorSelection").SelectedItem as ComboBoxItem)?.Content.ToString();
            var selectedCategory = Categories.SelectedItem as BudgetCategory;
            if (selectedCategory != null && selectedColor != null)
            {
                categories.FirstOrDefault((x) => x.Name == selectedCategory.Name).Brush = toColor(selectedColor);

                //just force the updated collection on the itemsource
                //it's moderately consensual. and the items are tiny anyways
                Categories.ItemsSource = null;
                Categories.ItemsSource = categories;

                if (App.Servicer.getSettings().FirstOrDefault<Settings>((x) => x.categoryName == selectedCategory.Name) == null)
                    App.Servicer.addSetting(new Settings() { categoryName = selectedCategory.Name, categoryColor = selectedColor });
                else
                    App.Servicer.updateSetting(selectedCategory.Name, selectedColor);
            }

        }

        private SolidColorBrush toColor(string color)
        {
            SolidColorBrush chosenColor;
            switch (color)
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
            return chosenColor;
        }

    }
}
