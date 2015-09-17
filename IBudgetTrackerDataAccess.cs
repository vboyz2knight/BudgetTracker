using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker
{
    public interface IBudgetTrackerDataAccess
    {
        DateTime? GetLatestDateTimeTransaction();
        List<displayCategorizedTransaction> GetDisplayCategorizedTransaction(DateTime beginTransaction, DateTime endTransaction);
        Dictionary<string, int> GetCategoriesList();
        Dictionary<string, int> GetFilterCategoriesList();
        bool SaveDataToDB(List<CategorizedTransaction> categorizedTransactionList,Dictionary<string,int> filterCategoryList);
    }
}
