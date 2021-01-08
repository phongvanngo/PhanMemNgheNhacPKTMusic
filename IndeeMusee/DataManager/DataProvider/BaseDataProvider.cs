using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndeeMusee.DataManager.DataProvider
{
    class BaseDataProvider
    {
        public static string LoadConnectionString()
        {
           string a = "Data Source=" + GeneralDataManagement.DatabaseFilePath;
            Console.WriteLine();
            return (@"Data Source=" + GeneralDataManagement.DatabaseFilePath);
        }
    }
}
