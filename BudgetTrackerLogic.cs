using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoMVC.Loggers;

namespace BudgetTracker
{
    public class BudgetTrackerLogic
    {
        public bool successCategorized;
        public List<aTransaction> unCategorizedTransactionList{get;private set;}
        public List<CategorizedTransaction> categorizedTransactionList { get; private set; }

        public Dictionary<string, int> filterCategoryList { get; private set; }
        public Dictionary<string, int> categoryList { get; private set; }

        public StringBuilder myError { get; private set; }
        private IBudgetTrackerDataAccess BudgetTrackerDataAccess;
        private IMyLogger MyLogger;
        private IMyNewDatas MyNewData;

        public BudgetTrackerLogic(IBudgetTrackerDataAccess budgetTrackerDataAccess,IMyLogger logger,IMyNewDatas myNewData)
        {
            this.MyNewData = myNewData;
            this.BudgetTrackerDataAccess = budgetTrackerDataAccess;
            this.MyLogger = logger;
            unCategorizedTransactionList = new List<aTransaction>();
            categorizedTransactionList = new List<CategorizedTransaction>();
            filterCategoryList = GetFilterCategoriesList();
            categoryList = GetCategoriesList();
            successCategorized = false;
        }

        private DateTime? GetLatestTransactionDateTime()
        {
            return BudgetTrackerDataAccess.GetLatestDateTimeTransaction();
        }
        public List<displayCategorizedTransaction> GetDisplayCategorizedTransactionLatest()
        {
            List<displayCategorizedTransaction> wantedTransactionList = null;

            if (GetLatestTransactionDateTime().HasValue)
            {
                DateTime latestTransactionDateTime = GetLatestTransactionDateTime().Value;

                DateTime beginTransaction = new DateTime(latestTransactionDateTime.Year, latestTransactionDateTime.Month, 1);
                DateTime endTransaction = new DateTime(latestTransactionDateTime.Year, latestTransactionDateTime.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

                wantedTransactionList = BudgetTrackerDataAccess.GetDisplayCategorizedTransaction(beginTransaction, endTransaction);
            }
            else
            {
                //there are no transaction data in DB
                MyLogger.Warning("There is no transaction  data found.");
            }            

            return wantedTransactionList;
        }
        public List<displayCategorizedTransaction> GetDisplayCategorizedTransaction(DateTime beginTransaction, DateTime endTransaction)
        {
            List<displayCategorizedTransaction> wantedTransactionList = null;

            wantedTransactionList = BudgetTrackerDataAccess.GetDisplayCategorizedTransaction(beginTransaction, endTransaction);

            return wantedTransactionList;
        }

        public Dictionary<string, int> GetCategoriesList()
        {
            Dictionary<string, int> CategoriesList = null;

            CategoriesList = BudgetTrackerDataAccess.GetCategoriesList();
            return CategoriesList;
        }

        public Dictionary<string, int> GetFilterCategoriesList()
        {
            Dictionary<string, int> FilterCategoriesList = null;

            FilterCategoriesList = BudgetTrackerDataAccess.GetFilterCategoriesList();
            return FilterCategoriesList;
        }

        public bool SaveDataToDB(List<CategorizedTransaction> categorizedTransactionList, Dictionary<string, int> filterCategoryList)
        {
            bool bReturn = false;

            bReturn = BudgetTrackerDataAccess.SaveDataToDB(categorizedTransactionList, filterCategoryList);

            return bReturn;
        }

        public bool AnalyzeMyNewData()
        {            
            bool bReturn = false;

            List<aTransaction> tmpTransactions = new List<aTransaction>();
            tmpTransactions = MyNewData.ReadNewData();

            if (tmpTransactions.Count > 0)
            {
                MyLogger.Info( string.Format("{0} transaction[s] loaded.", tmpTransactions.Count));
                unCategorizedTransactionList.AddRange(tmpTransactions);
                        
                //now have raw transactions, will try to categorized it with filters in the db

                if (CategorizeTransactionListWithExistingFilters(tmpTransactions, categorizedTransactionList))
                {
                    successCategorized = true;
                    bReturn = true;
                }
                else
                {
                    MyLogger.Warning( "Unable to categorize all transactions with existing filters data.");
                }
            }
            else
            {
                MyLogger.Error( string.Format("No transaction found."));
            }

            //remove the file once we done reading ??

            return bReturn;                
        }

        
        //Try to categorize List<aTransaction> into List<CategorizedTransaction>
        //After this method run, transaction[s] that can't be categorize will left in List<aTransaction> transactionlist
        //Transaction[s] that were able to categorized will be store in List<CategorizedTransaction>
        private bool CategorizeTransactionListWithExistingFilters(List<aTransaction> transactionlist, List<CategorizedTransaction>  categorizedTransactionList)
        {            
            bool bReturn = false;

            if(filterCategoryList.Count > 0)
            { 
                //can't delete an item from a list in a foreach loop of that list
                //going to loop backward.
                foreach (aTransaction transaction in transactionlist.ToArray())
                {
                    if (CategorizeSingleTransactionWithExistingFilterPhrase(transaction, filterCategoryList, categorizedTransactionList))
                    {
                        //remove the transaction once its categorized
                        transactionlist.Remove(transaction);
                    }
                }
            }
            else
            {
                MyLogger.Warning( "No filter phrase found.");
            }

            if (transactionlist.Count() == 0)
            {
                bReturn = true;
            }

            return bReturn;
        }

        //No duplication of transaction will be allow to add
        public bool CategorizeSingleTransactionWithExistingFilterPhrase(aTransaction transaction, Dictionary<string, int> oldFilterCategoryList, List<CategorizedTransaction> newCagetorizedTransactionList)
        {
            bool bReturn = false;

            foreach (KeyValuePair<string, int> filter in oldFilterCategoryList)
            {
                if (transaction.Description.Contains(filter.Key.ToString()))
                {
                    CategorizedTransaction aTransaction = new CategorizedTransaction(transaction.myDate, transaction.Description, transaction.check, transaction.Amt, filter.Value, filter.Key);
                    if (!newCagetorizedTransactionList.Contains(aTransaction))
                    {
                        newCagetorizedTransactionList.Add(aTransaction);
                    }
                    else
                    {
                        //duplicated transaction will not be add to the list
                        MyLogger.Warning( string.Format("Duplicated transaction found: {0}.", aTransaction.ToString()));
                    }
                    bReturn = true;
                    break;
                }
            }

            return bReturn;
        }
    }
}
