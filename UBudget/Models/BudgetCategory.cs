using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace UBudget.Models
{
    public class BudgetCategory
    {
        [BsonId]
        public int ID { get; set; }

        public SolidColorBrush Brush { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
    }
}
