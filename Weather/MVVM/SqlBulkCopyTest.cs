using LINQtoCSV;
using System.Diagnostics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.MVVM.Model;
using System.Data.SqlClient;

namespace Weather.MVVM
{
    /*class SqlBulkCopyTest
    {
        static void Main(string[] args)
        {

        }

        private static void RunProcess()
        {
            var csvFile = "../Data/testcsv.csv";
            var cc = new CsvContext();

            var csvFileDescription = new CsvFileDescription()
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var people = cc.Read<AreaDatum>(csvFile, csvFileDescription).ToList();
        }

    }*/
}
