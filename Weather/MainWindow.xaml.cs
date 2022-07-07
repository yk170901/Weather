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
        WeatherController wc = new WeatherController();
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Search(object sender, RoutedEventArgs e) // 검색 버튼 클릭 이벤트
        {
            // nx와 ny의 값을 받아 저장
            Dictionary<string, int> nxAndNy =  GetNxAndNy();

            // url 생성
            string url = wc.GenerateUrl(nxAndNy["nx"], nxAndNy["ny"]);

            // api 만들기 메소드인데, 굳이 이건 view에서 불러낼 필요가 있나?
            // db 불러오기라는 메소드만 view에서 부르고, 그 메소드 안에 getApi가 있든 뭐하든 하면 되지 않을까?
            string results = wc.GetApi(url);

            //parsing
            wc.ParseXml(results);

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

                dgridTest.ItemsSource = list;

                return nxAndNy;

                /*                               
                var buffer = new List<ViewItem>();
                foreach (var l in list) // nx와 ny의 값을 굳이 저장할 필요는 없으니 아래와 같이 list[0].nx로 사용할 것이다.
                                        // 이 foreach문은 사용하지 않을 것이고, 추후 Weather에서 데이터를 받아와 하나씩 보이는 데에 참고할지도 모른다.
                {
                    ViewItem item = new ViewItem();
                    item.nx = l.nx;
                    item.ny = l.ny;
                    buffer.Add(item);
                }

                dgridTest.ItemsSource = buffer;
                 */
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
