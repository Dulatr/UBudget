using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBudget.Models
{
    public class Transaction
    {
        [BsonId]
        public int TxID { get; set; }

        public int AccountID { get; set; }

        public DateTime DateTime { get; set; }
        public string Label { get; set; }
        public double Amount { get; set; }
    }
}
