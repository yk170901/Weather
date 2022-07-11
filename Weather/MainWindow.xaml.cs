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
using System.Windows.Media;

namespace Weather
{
    public partial class MainWindow : Window // , INotifyPropertyChanged
    {

        WeatherController weatherController = new WeatherController();
        Brush colorForDays = (Brush)new BrushConverter().ConvertFrom("#3104B4");

        public MainWindow()
        {
            InitializeComponent();
        }

        // 메소드 : 윈도우 창 드래그 구현
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {   Cursor = Cursors.SizeAll;
            this.DragMove(); }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        { Cursor = Cursors.Arrow; }

        private void ChangeDaytoToday(object sender, RoutedEventArgs e)
        {
            // 날짜와 상관없이 보려는 지역의 데이터는 같기 때문에 검색 버튼을 눌렀을 때처럼 좌표나 API부터 가져오지 않아도 된다.
            AddWeatherImageAndDateData("오늘");
            AddWeatherListData("오늘");

            today.Foreground = colorForDays;
            tomorrow.Foreground = Brushes.Black;
            dayAfterTomorrow.Foreground = Brushes.Black;
        }
        private void ChangeDaytoTomorrow(object sender, RoutedEventArgs e)
        {
            AddWeatherImageAndDateData("내일");
            AddWeatherListData("내일");

            today.Foreground = Brushes.Black;
            tomorrow.Foreground = colorForDays;
            dayAfterTomorrow.Foreground = Brushes.Black;
        }
        private void ChangeDaytoTheDayAfterTomorrow(object sender, RoutedEventArgs e)
        {
            AddWeatherImageAndDateData("모레");
            AddWeatherListData("모레");

            today.Foreground = Brushes.Black;
            tomorrow.Foreground = Brushes.Black;
            dayAfterTomorrow.Foreground = colorForDays;
        }

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

            AddWeatherImageAndDateData("오늘");
            AddWeatherListData("오늘");

            today.Foreground = colorForDays;

            Debug.WriteLine($" - - - Data for x={nxAndNy["nx"]} and {nxAndNy["ny"]} - - - ");
            
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

        // 메소드 : 날짜에 따른 mysql의 select 조건 숫자 가져오기.
        // 너무 간단한 작업이라 만들지 말까 싶었지만 메인 코드들을 더럽게 하고 만들고 싶지 않고,
        // 유지보수 시 명확한 안내를 제공하기 위해 만듦 
        private byte GetNumberForTheDay(string day)
        {
            switch (day)
            {
                case "오늘": return 0;
                case "내일": return 1;
                case "모레": return 2;
            }

            return 100;
        }

        // 메소드 : WeatherDatum의 형태로 이미지 데이터 나타내보기
        private void AddWeatherImageAndDateData(string day)
        {
            List<WeatherDatum> weatherImagesDatum = new List<WeatherDatum>();

            using (var ctx = new MariaContext())
            {
                var list = (from w in ctx.Weather where w.category == "SKY" select w).ToList();
                byte dayNumber = GetNumberForTheDay(day);

                foreach (var l in list)
                {
                    if ((int.Parse(l.fcstDate.ToString("yyyyMMdd")) - int.Parse(System.DateTime.Now.ToString("yyyyMMdd"))) == dayNumber
                        && int.Parse(l.fcstTime) / 100 % 2 == 0 && int.Parse(l.fcstTime) / 100 > 3) // 짝수시간 정보이면
                    {
                        weatherImagesDatum.Add(new WeatherDatum() { category = l.category, fcstDate = l.fcstDate, fcstTime = int.Parse(l.fcstTime.Substring(0,2)) + "시", fcstValue = GetSkyValueToDisplay(l.category, l.fcstValue) });
                    }
                }
            }

            weatherImage.ItemsSource = weatherImagesDatum;

            Debug.WriteLine("AddWeatherImageList DONE");
        }

        // 메소드 : WeatherDatum의 형태로 리스트 데이터 나타내보기
        private void AddWeatherListData(string day)
        {
            List<WeatherListDatum> weatherListDatum = new List<WeatherListDatum>();

            using (var ctx = new MariaContext())
            {
                var list = (from w in ctx.Weather where w.category != "SKY" && w.category != "PTY" select w).ToList();
                byte dayNumber = GetNumberForTheDay(day);

                int i = 0;
                while (i < list.Count())
                {
                    if ((int.Parse(list[i].fcstDate.ToString("yyyyMMdd")) - int.Parse(System.DateTime.Now.ToString("yyyyMMdd"))) == dayNumber
                        && int.Parse(list[i].fcstTime)/100 %2 == 0 && int.Parse(list[i].fcstTime) / 100 > 3) // 짝수시간 정보이면
                    {
                        /*Debug.WriteLine(" - - - - - - - - - - - - \n"
                                        + "시간 : " + list[i].fcstTime + " " + list[i+1].fcstTime + " " + list[i+2].fcstTime + " \n"
                                        + "1번째 : " + list[i].category + " - " + list[i].fcstValue 
                                        + " 2번째 : " + list[i+1].category + " - " + list[i+1].fcstValue 
                                        + " 3번째 : " + list[i+2].category + " - " + list[i+2].fcstValue
                                        + "\n - - - - - - - - - - - - \n\n");*/

                        // API Data 순서 - 1번째 TMP, 2번째 POP, 3번째 REH
                        weatherListDatum.Add(new WeatherListDatum() { fcstTime = list[i].fcstTime, TMP = list[i].fcstValue + "℃", POP = "강수확률 : " + list[i+1].fcstValue + "%", REH = "습도 : " + list[i+2].fcstValue + "%" });
                    }
                    i= i+3;
                }
            }

            weatherList.ItemsSource = weatherListDatum;

            Debug.WriteLine("AddWeatherListData DONE");
        }

        private string GetSkyValueToDisplay(string category, string fcstValue)
        {
            switch (category)
            {
                // 하늘상태
                case "SKY":
                    if (fcstValue == "1") return "Images/sunny.png";
                    if (fcstValue == "3") return "Images/cloudy.png"; // 추후 구름 2개로 바꾸기
                    if (fcstValue == "4") return "Images/cloudy.png";

                    Debug.WriteLine($"GetValueToDisplay ERROR. category :{category}, fcstValue : {fcstValue}");
                    return null;
            }
            return null;
        }

    }
}
