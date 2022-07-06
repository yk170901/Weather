using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;

namespace Weather.MVVM
{
    class AreaCoordinates
    {
        public static void SendCsvPath()
        {
            DataTable csvFileData = ConvertCsvToDataTable("D:/area2.csv");
            Console.WriteLine("CSV to DataTable DONE");
            Console.ReadLine();
            InsertDataIntoSQLServer(csvFileData);
            Console.WriteLine("DataTable to Database on Server DONE");
            Console.ReadLine();
        }
        public static DataTable ConvertCsvToDataTable(string path)
        {
            DataTable csvDataTable = new DataTable();

            try
            { 
                using(TextFieldParser csvReader = new TextFieldParser(path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields(); // 반환 값 : array형태
                    
                    foreach(string column in colFields)
                    {
                        DataColumn dataColumn = new DataColumn(column);
                        dataColumn.AllowDBNull = true;
                        csvDataTable.Columns.Add(dataColumn);
                    }

                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "") fieldData[i] = null;
                        }

                        csvDataTable.Rows.Add(fieldData);
                    }
                }
            }
            catch
            {
                Console.WriteLine("ERROR");
                return null;
            }
            return csvDataTable;
        }

        public static void InsertDataIntoSQLServer(DataTable csvFileData)
        {

            using (SqlConnection dbConnection = new SqlConnection("Data Source=DESKTOP-L3IA7DE;Initial Catalog=weatherapp;Integrated Security=SSPI;User ID=root;Password=yk170901"))
            {
                dbConnection.Open();
                using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                {
                    s.DestinationTableName = "weatherapp";
                    foreach (var column in csvFileData.Columns)
                        s.ColumnMappings.Add(column.ToString(), column.ToString());
                    s.WriteToServer(csvFileData);
                }
            }
        }

        public static void testMethod()
        {
            /*using (SqlConnection conn = new SqlConnection("Database=weatherapp;Server=127.0.0.1;User=root;Password=yk170901"))*/
            using (SqlConnection conn = new SqlConnection("Data Source=DESKTOP-L3IA7DE;Integrated Security=SSPI"))
            {   
                conn.Open();
                using (StreamReader reader = new StreamReader(@"D:/area.csv"))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        var values = line.Split(",");

                        var sql = "INSERT INTO area VALUES('"+values[0] +"','"+ values[1] + "','" + values[2] + "','" + values[3] + "','" + values[4]+"')";

                        var cmd = new SqlCommand();
                        cmd.CommandText = sql;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Connection = conn;
                        cmd.ExecuteNonQuery();
                    }
                }
                conn.Close();
            }
        }
    }
}
