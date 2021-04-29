using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UBudget.Models
{
    public class PayStub
    {
        [BsonId]
        public int PayStubID { get; set; }
        public int JobID { get; set; }
        public int AccountID { get; set; }

        public double NetAmount { get; set; }

        public double SIT { get; set; }
        public double FIT { get; set; }
        public double MiscTax { get; set; }
        public double GrossAmount { get { return NetAmount - (SIT + FIT + MiscTax); } }

        public string Label { get; set; }

        public DateTime Date { get; set; }
    }
}
