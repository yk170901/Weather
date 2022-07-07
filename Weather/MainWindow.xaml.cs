using System.Linq;
using System.Windows;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using Weather.MariaDb;
using MySql.Data.MySqlClient;
using System;
using Weather.Controllers;
using System.Windows.Input;

namespace Weather
{
    /*public class ViewItem
    {
        // property
        public int nx { get; set; }
        public int ny { get; set; }
    }*/

    public partial class MainWindow : Window
    {
        WeatherController wc = new WeatherController();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.SizeAll;
            this.DragMove();
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void Search(object sender, RoutedEventArgs e) // 검색 버튼 클릭 이벤트
        {
            Dictionary<string, int> nxAndNy =  GetNxAndNy();

            string url = wc.GenerateUrl(nxAndNy["nx"], nxAndNy["ny"]);

            string results = wc.GetWeatherApi(url);

            wc.DeleteWeatherDataBeforeInsertion();
            wc.ParseXmlAndPutInDb(results);

            // ui

        }
        private void Combo1Changed(object sender, SelectionChangedEventArgs e)
        {
            string area1 = Area1ComboBox.SelectedItem.ToString().Split(" ")[1]; // Area1 콤보박스 선택값

            if (area1 == "선택해주세요") return;
            else ChangeCombo2ForCombo1(area1);
        }


        private Dictionary<string, int> GetNxAndNy()
        {
            Dictionary<string, int> nxAndNy = new Dictionary<string, int>();

            string area1 = Area1ComboBox.SelectedItem.ToString().Split(" ")[1]; // Area1 콤보박스 선택값
            string area2 = Area2ComboBox.SelectedValue.ToString();              // Area2 콤보박스 선택값

            using (var ctx = new MariaContext())
            {
                var list = (from a in ctx.Area where a.area1 == area1 && a.area2 == area2 && a.area3 == "" select a).ToList();

                MessageBox.Show(list[0].nx + " " + list[0].ny);

                nxAndNy.Add("nx", list[0].nx);
                nxAndNy.Add("ny", list[0].ny);

                return nxAndNy;
            }
        }

        private void ChangeCombo2ForCombo1(string area1)
        {
            Area2ComboBox.Items.Clear();

            using (var ctx = new MariaContext())
            {
                var list = (from a in ctx.Area where a.area1 == area1 && a.area2 != "" && a.area3 == "" select a.area2).ToList();

                foreach (var l in list)
                {
                    Area2ComboBox.Items.Add(l);
                }
            }
            Area2ComboBox.SelectedIndex = 0;
        }
    }
}
