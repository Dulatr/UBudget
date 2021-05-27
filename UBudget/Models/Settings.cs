using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBudget.Models
{
    public class Settings
    {
        [BsonId]
        public int ID { get; set; }

        public string categoryName { get; set; }
        public string categoryColor { get; set; }
    }

    public class UserSettings : Settings
    {
        public UserSettings()
        {
            categoryName = "User";
            categoryColor = "";
            newUser = true;
        }
        public bool newUser { get; set; }
    }
}
