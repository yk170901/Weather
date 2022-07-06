using System.Linq;
using System.Windows;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using Weather.MVVM.ViewModel;
using Weather.MVVM;
using MySql.Data.MySqlClient;
using System;

namespace Weather
{
    public class ViewItem
    {
        // property
        public int nx { get; set; }
        public int ny { get; set; }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string area1 = Area1ComboBox.SelectedItem.ToString().Split(" ")[1]; // Area1 콤보박스 선택값
            string area2 = Area2ComboBox.SelectedValue.ToString();              // Area2 콤보박스 선택값

            using (var ctx = new MariaContext())
            {
                /*List<AreaDatum> areaList = ctx.Area.ToList();

                var list = (from areaData in areaList.AsEnumerable()
                            where areaData.area1 == area1 && areaData.area2 == area2
                            select areaData);*/

                var buffer = new List<ViewItem>();
                var list = (from a in ctx.Area where a.area1 == area1 && a.area2 == area2 && a.area3 == "" select a).ToList();

                foreach (var l in list) // nx와 ny의 값을 굳이 저장할 필요는 없으니 아래와 같이 list[0].nx로 사용할 것이다.
                                        // 이 foreach문은 사용하지 않을 것이고, 추후 Weather에서 데이터를 받아와 하나씩 보이는 데에 참고할지도 모른다.
                {
                    ViewItem item = new ViewItem();
                    item.nx = l.nx;
                    item.ny = l.ny;
                    buffer.Add(item);
                }


                dgridTest.ItemsSource = buffer;
                // dgridTest.ItemsSource = list;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Started");

            MySqlConnectionStringBuilder connBuilder = new MySqlConnectionStringBuilder();

            string ip = "127.0.0.1";
            string port = "3306";
            connBuilder.Server = ip;
            connBuilder.Port = uint.Parse(port);
            connBuilder.UserID = "root";
            connBuilder.Password = "yk170901";
            connBuilder.Database = "weatherapp";
            connBuilder.CharacterSet = "utf8";

            string connStr = connBuilder.ConnectionString;

            MySqlConnection conn = new MySqlConnection(connStr); 
            var bl = new MySqlBulkLoader(conn);
            bl.TableName = "area";
            bl.FieldTerminator = ",";      // 열 구분
            bl.LineTerminator = "\r\n";    // 줄 구분
            bl.FileName = "D:/area.csv";
            bl.NumberOfLinesToSkip = 1;
            var inserted = bl.Load();

            Debug.WriteLine("Done");
        }

        private void Area1ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string area1 = Area1ComboBox.SelectedItem.ToString().Split(" ")[1]; // Area1 콤보박스 선택값

            if (area1 == "선택해주세요") return;
            Area2ComboBox.Items.Clear();

            using (var ctx = new MariaContext())
            {
                var list = (from a in ctx.Area where a.area1 == area1 && a.area2 != "" && a.area3 == "" select a.area2).ToList();

                foreach (var l in list)
                {
                    Area2ComboBox.Items.Add(l);
                }

                Area2ComboBox.SelectedIndex = 0;

            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("In other words, hold my hand");

            WebApi wa  = new WebApi();
            string url = wa.GenerateUrl(55,127);
            Debug.WriteLine(url);
            var data = wa.GetApi(url);

            //parsing

            // ui 

            Debug.WriteLine(wa.ToString());

            Debug.WriteLine("In other words, baby, kiss me");
        }
    }
}
