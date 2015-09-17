using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker
{
    public class CategorizedTransaction : aTransaction, IEquatable<CategorizedTransaction>
    {
        int tranCategory;
        string tranFilterPhrase;

        public int TranCategory
        {
            get
            {
                return tranCategory;
            }
            private set
            {
                tranCategory = value;
            }
        }

        public string TranFilterPhrase
        {
            get
            {
                return tranFilterPhrase;
            }
            private set
            {
                tranFilterPhrase = value;
            }
        }

        public CategorizedTransaction(DateTime myDate, string mytranDescription, string myCheck, decimal myAmt, int category, string filterPhrase)
            : base(myDate, mytranDescription, myCheck, myAmt)
        {
            tranCategory = category;
            tranFilterPhrase = filterPhrase;
        }


        public bool Equals(CategorizedTransaction other)
        {
            if (this.Amt == other.Amt && this.check == other.check && this.Description == other.Description &&
                this.myDate == other.myDate && this.tranCategory == other.tranCategory && this.tranFilterPhrase == other.tranFilterPhrase)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
