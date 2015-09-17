using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BudgetTracker
{
    public class displayCategorizedTransaction:CategorizedTransaction
    {
        string businessCategory;

        public string BusinessCategory
        {
            get
            {
                return businessCategory;
            }
            private set
            {
                businessCategory = value;
            }
        }

        public displayCategorizedTransaction(string myBusinessCategory, DateTime myDate, string mytranDescription, string myCheck, decimal myAmt, int category, string filterPhrase)
            : base(myDate, mytranDescription, myCheck, myAmt, category, filterPhrase)
        {
            businessCategory = myBusinessCategory;
        }
    }
}
