using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoMVC.Loggers;

namespace BudgetTracker
{
    class ReadCVSFile:IMyNewDatas
    {
        private string MyFullFilePath;
        private IMyLogger MyLogger;
        public ReadCVSFile(string fullFilePath, IMyLogger logger)
        {
            //check to see file
            if (File.Exists(fullFilePath))
            {
                this.MyFullFilePath = fullFilePath;
            }
            else
            {
                throw new FileNotFoundException(fullFilePath);
            }

            this.MyLogger = logger;
        }
        //Read .csv file and load transaction into List<aTransaction>
        public List<aTransaction> ReadNewData()
        {
            List<aTransaction> listTransactions = new List<aTransaction>();
            DateTime myDate;
            string myDescription = "";
            string myCheck = "";
            decimal myAmount = 0.00M;
            int seekPosition = 0;
            string tmpString = "";
            int line = 0;

            try
            {
                //will attempt to read all lines in the file
                //each line with error will be log and ignore and continue to the next line
                using (StreamReader myStreamReader = new StreamReader(MyFullFilePath))
                {
                    while (myStreamReader.Peek() >= 0)
                    {
                        //Date,Description,Check Number,Amount
                        string aReadLine = myStreamReader.ReadLine();
                        //string[] tmpLineArray = aReadLine.Split(',');
                        //since the description could be in the format of "description,lou,ky"
                        //a string split is not the way to go
                        //get the first index of comma then pull the date out
                        //then get the last index of comma and pull the amount out

                        if (aReadLine.Length > 0)
                        {
                            line++;

                            seekPosition = aReadLine.IndexOf(',');
                            tmpString = aReadLine.Remove(seekPosition);

                            if (DateTime.TryParse(tmpString, out myDate))
                            {
                                //remove the date
                                aReadLine = aReadLine.Substring(seekPosition + 1);
                                seekPosition = aReadLine.LastIndexOf(',');

                                tmpString = aReadLine.Substring(seekPosition + 1);

                                if (decimal.TryParse(tmpString, out myAmount))
                                {
                                    //remove the amount
                                    aReadLine = aReadLine.Remove(seekPosition);
                                    //get the description
                                    seekPosition = aReadLine.LastIndexOf('"');
                                    tmpString = aReadLine.Remove(seekPosition + 1);

                                    myDescription = tmpString;

                                    //get the last remaining of check
                                    aReadLine = aReadLine.Substring(seekPosition + 1);

                                    myCheck = aReadLine.Substring(1, aReadLine.Length - 1);

                                    aTransaction myTransaction = new aTransaction(myDate, myDescription, myCheck, myAmount);
                                    listTransactions.Add(myTransaction);
                                }
                                else
                                {
                                    MyLogger.Warning(string.Format("Unable to convert amount. {0} in file {1}.", aReadLine, MyFullFilePath));
                                }
                            }
                            else
                            {
                                //1st column ned to be a Date
                                MyLogger.Warning(string.Format("Unable to convert date. {0} in file {1}.", aReadLine, MyFullFilePath));
                            }
                        }
                        else
                        {
                            //invalid Data, need to be 4 column
                            MyLogger.Warning(string.Format("Invalid data, need to be 4 column. {0}", tmpString));
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MyLogger.Error( string.Format("Unable to access file: {0}.", ex.Message));
                throw;
            }
            catch (FileNotFoundException ex)
            {
                MyLogger.Error(string.Format("File not found: {0}.", ex.Message));
                throw;
            }
            catch (Exception ex)
            {
                MyLogger.Error(string.Format("Unknow exception: {0}.", ex.Message));
                throw;
            }

            return listTransactions;
        }
    }
}
