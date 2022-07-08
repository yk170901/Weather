using System.Linq;
using System.Windows;
using System.Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Controls;
using Weather.MariaDb;
using Weather.Controllers;
using System.Windows.Input;
using Weather.MariaDb.Datums;
using System.ComponentModel;

namespace Weather
{
    public partial class MainWindow : Window // , INotifyPropertyChanged
    {

        WeatherController weatherController = new WeatherController();

        // public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();
            DataBindingTest();
        }

        // 메소드 : 윈도우 창 드래그 구현
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {   Cursor = Cursors.SizeAll;
            this.DragMove(); }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        { Cursor = Cursors.Arrow; }
        
        
        private void Combo1Changed(object sender, SelectionChangedEventArgs e)
        {
            string area1 = Area1ComboBox.SelectedItem.ToString().Split(" ")[1]; // Area1 콤보박스 선택값

            if (area1 == "선택해주세요") return;
            else ChangeCombo2ForCombo1(area1);
        }

        // 메소드 : Area1 콤보 박스의 값에 따라 Area2 콤보 박스의 값 변경
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

        // 메소드 : 검색 버튼 클릭 이벤트
        private void Search(object sender, RoutedEventArgs e)
        {
            Dictionary<string, int> nxAndNy = GetNxAndNy();

            string url = weatherController.GenerateUrl(nxAndNy["nx"], nxAndNy["ny"]);

            string results = weatherController.GetWeatherApi(url);

            weatherController.DeleteWeatherDataBeforeInsertion();
            weatherController.ParseXmlAndPutInDb(results);

            // ui



            // 아래의 해당 지역 문장 바꾸기
            Region.Text = Area1ComboBox.SelectedItem.ToString().Split(" ")[1]+ " " + Area2ComboBox.SelectedItem.ToString();
        }

        // 메소드 : 해당 지역의 X와 Y 좌표 얻기
        private Dictionary<string, int> GetNxAndNy()
        {
            Dictionary<string, int> nxAndNy = new Dictionary<string, int>();

            string area1 = Area1ComboBox.SelectedItem.ToString().Split(" ")[1]; // Area1 콤보박스 선택값
            string area2 = Area2ComboBox.SelectedValue.ToString();              // Area2 콤보박스 선택값

            using (var ctx = new MariaContext())
            {
                var list = (from a in ctx.Area where a.area1 == area1 && a.area2 == area2 && a.area3 == "" select a).ToList();

                nxAndNy.Add("nx", list[0].nx);
                nxAndNy.Add("ny", list[0].ny);

                return nxAndNy;
            }
        }

        /*// 메소드 : 실험. WeatherDatum의 형태로 데이터 나타내보기
        private void DataBindingTest()
        {
            List<WeatherDatum> weatherDatum = new List<WeatherDatum>();

            weatherDatum.Add(new WeatherDatum() { category = "testCategory1", fcstDate = System.DateTime.Now, fcstTime = "0500", fcstValue = "test1" });
            weatherDatum.Add(new WeatherDatum() { category = "testCategory2", fcstDate = System.DateTime.Now, fcstTime = "0600", fcstValue = "test2" });
            weatherDatum.Add(new WeatherDatum() { category = "testCategory3", fcstDate = System.DateTime.Now, fcstTime = "0700", fcstValue = "test3" });

            weatherList.ItemsSource = weatherDatum;

            Debug.WriteLine(weatherList.Items.SourceCollection.ToString());
            Debug.WriteLine(weatherDatum[0].category);
        }*/

        // 메소드 : WeatherDatum의 형태로 데이터 나타내보기
        private void DataBindingTest()
        {
            List<WeatherDatum> weatherDatum = new List<WeatherDatum>();

            using (var ctx = new MariaContext())
            {
                var list = (from w in ctx.Weather where w.category == "SKY" select w).ToList();

                foreach (var l in list)
                {
                    if (l.fcstDate.ToString("yyyyMMdd") == System.DateTime.Now.ToString("yyyyMMdd"))
                    {
                        Debug.WriteLine(l.fcstDate + " : " + l.fcstTime + " : "  + l.fcstValue);
                        weatherDatum.Add(new WeatherDatum() { category = l.category, fcstDate = l.fcstDate, fcstTime = l.fcstTime, fcstValue = GetValueToDisplay(l.category, l.fcstValue) });
                    }

                }
            }

            weatherList.ItemsSource = weatherDatum;

            Debug.WriteLine("DataBindingTest DONE");
        }

        private string GetValueToDisplay(string category, string fcstValue)
        {
            switch (category)
            {
                // 하늘상태
                case "SKY":
                    if (fcstValue == "1") return "맑음";
                    if (fcstValue == "3") return "구름많음";
                    if (fcstValue == "4") return "흐림";

                    Debug.WriteLine($"GetValueToDisplay ERROR. category :{category}, fcstValue : {fcstValue}");
                    return null;
                // 강수확률
                case "POP":
                    return fcstValue+"%";
                // 기온
                case "TMP":
                    return fcstValue + "℃";
            }
            return null;
        }

    }
}
