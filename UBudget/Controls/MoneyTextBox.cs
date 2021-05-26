using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UBudget.Controls
{
    /// <summary>
    /// Use this extension of the TextBox control to format the input as a monetary amount.
    /// This control only allows numeric input, left and right key presses, and backspace.
    /// Currently only supports USD sorry :c
    /// </summary>
    public class MoneyTextBox : TextBox
    {
        private readonly string numbers = "0123456789";
        private readonly List<string> allowedStrings = new List<string>() {"Back", "Left", "Right" };
        private string keyString = "";

        public MoneyTextBox()
        {
            PreviewKeyDown += MoneyTextBox_PreviewKeyDown;
            TextChanged += MoneyTextBox_TextChanged;
        }

        private void MoneyTextBox_PreviewKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            keyString = e.Key.ToString();
            // number row keys show up as 'NumberN', numpad keys show up as 'NumberPadN'
            keyString = keyString.Replace("Number", "").Replace("Pad", "");
            if (!numbers.Contains(keyString) && !allowedStrings.Contains(keyString))
            {
                // don't do anything with the key that was pressed
                e.Handled = true;
                return;
            }

           if (keyString == "Back")
            {
                if (this.Text.Length == 1)
                {
                    e.Handled = true;
                    return;
                }
            }

            // otherwise allow the keypress
            e.Handled = false;
            return;
        }

        private void MoneyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // get the unformatted value: just the digits
            string value = Text.Replace(",", "").Replace("$", "").Replace(".", "").TrimStart('0');

            // verify the stripped-down text is actually a number
            if (decimal.TryParse(value, out decimal centified))
            {
                // divide by 100 to get a decimal value with cents
                centified /= 100;
                
                // remove the events temporarily since we change the text here
                TextChanged -= MoneyTextBox_TextChanged;
                // convert it to US money format
                Text = string.Format(CultureInfo.CreateSpecificCulture("en-US"), "{0:C2}", centified);
                
                // add the events back since we're done changing the text
                TextChanged += MoneyTextBox_TextChanged;

                if (keyString == "Back")
                    Select(cursorStart, 0);
                else
                    // move cursor to end of text
                    Select(Text.Length, 0);
            }
            else
            {
                TextChanged -= MoneyTextBox_TextChanged;

                if (this.Text != "")
                    this.SelectedText = "0";
                else
                    this.Text = "$0.00";
                Select(Text.Length, 0);

                TextChanged += MoneyTextBox_TextChanged;
            }
        }
    }
}
