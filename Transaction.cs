using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker
{
    public class aTransaction
    {
        DateTime tranDate;
        decimal tranAmnt;
        string tranCheck;
        string tranDescription;

        public DateTime myDate
        {
            get
            {
                return tranDate;
            }
            private set
            {
                tranDate = value;
            }
        }

        public decimal Amt
        {
            get
            {
                return tranAmnt;
            }
            private set
            {
                tranAmnt = value;
            }
        }

        public string Description
        {
            get
            {
                return tranDescription;
            }
            private set
            {
                tranDescription = value;
            }
        }

        public string check
        {
            get
            {
                return tranCheck;
            }
            private set
            {
                tranCheck = value;
            }
        }


        public aTransaction(DateTime myDate, string mytranDescription, string myCheck, decimal myAmt)
        {
            tranDate = myDate;
            tranCheck = myCheck;
            tranAmnt = myAmt;
            tranDescription = mytranDescription;
        }

        public override string ToString() 
        {
            return string.Format("{0:MM/dd/yy} : {1} : {2} : {3:0.00}", myDate , tranDescription , tranCheck , tranAmnt);
        }
    }
}
