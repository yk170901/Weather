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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;

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


        // 메소드 : 오늘 버튼 눌렀을 때 오늘 날씨 데이터 가져옴
        private void GetDataForToday(object sender, RoutedEventArgs e)
        {
            // 날짜와 상관없이 보려는 지역의 데이터는 같기 때문에 검색 버튼을 눌렀을 때처럼 좌표나 API부터 가져오지 않아도 된다.
            AddWeatherImageAndDateData("오늘");
            AddWeatherListData("오늘");

            today.Foreground = colorForDays;
            tomorrow.Foreground = Brushes.Black;
            dayAfterTomorrow.Foreground = Brushes.Black;
        }

        // 메소드 : 내일 버튼 눌렀을 때 내일 날씨 데이터 가져옴
        private void GetDataForTomorrow(object sender, RoutedEventArgs e)
        {
            AddWeatherImageAndDateData("내일");
            AddWeatherListData("내일");

            today.Foreground = Brushes.Black;
            tomorrow.Foreground = colorForDays;
            dayAfterTomorrow.Foreground = Brushes.Black;
        }

        // 메소드 : 모레 버튼 눌렀을 때 모레 날씨 데이터 가져옴
        private void GetDataForTheDayAfterTomorrow(object sender, RoutedEventArgs e)
        {
            AddWeatherImageAndDateData("모레");
            AddWeatherListData("모레");

            today.Foreground = Brushes.Black;
            tomorrow.Foreground = Brushes.Black;
            dayAfterTomorrow.Foreground = colorForDays;
        }

        // 메소드 : Area1 콤보 박스의 값에 따라 Area2 콤보 박스의 값 변경
        private void ChangeCombo2ForCombo1(object sender, SelectionChangedEventArgs e)
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

            AddWeatherDataForMain();

            AddWeatherImageAndDateData("오늘");
            AddWeatherListData("오늘");
            today.Foreground = colorForDays;

            Debug.WriteLine($" - - - Data for x={nxAndNy["nx"]} and {nxAndNy["ny"]} - - - ");
            
            Region.Text = Area1ComboBox.SelectedItem.ToString().Split(" ")[1]+ " " + Area2ComboBox.SelectedItem.ToString();
        }

        // 메소드 : 메인 창에 현재 시간/장소 날씨 데이터 디스플레이
        private void AddWeatherDataForMain()
        {
            using (var ctx = new MariaContext())
            {
                string hourNow = System.DateTime.Now.Hour+"00";

                if (hourNow.Length == 3) hourNow = "0" + hourNow;

                // 당일 데이터이며 해당 시간의 데이터만 가져오고 싶었는데
                // 당일 데이터는 얻어와지지가 않는다.
                    // System.Date.Now를 하면 w.fcstDate와 매치가 안 되고, String으로 비교를 하려고 하면 자료형이 달라 비교할 수 없다.
                var list = (from w in ctx.Weather where w.fcstTime == hourNow select w).ToList();

                foreach (var l in list)
                {
                    if (l.fcstDate.ToString("yyyyMMdd")  == System.DateTime.Now.ToString("yyyyMMdd"))
                    {
                        ChangeMainValuesOnScreen(l.category, l.fcstValue);
                    }

                    // 강수형태가 1이상일 경우 (비나 눈이 내릴 경우) SKY 이미지를 바꿈. PTY 값은 SKY 값보다 나중에 전달되므로 이 방식에는 문제 없음.
                    if(l.category=="PTY" && l.fcstValue != "0")
                    {
                        MainSKY.Source = new BitmapImage(new Uri("Images/rainy.png", UriKind.Relative));
                    }
                }
            }

            TimeDataCalled.Text = System.DateTime.Now.ToString();
        }

        // 메소드 : 
        private void ChangeMainValuesOnScreen(string category, string fcstValue)
        {
            switch (category)
            {
                case "TMP":
                    MainTMP.Text = "온도: "+ fcstValue +"℃";
                    return;
                case "SKY":
                    MainSKY.Source = new BitmapImage(new Uri(GetSkyValueToDisplay(fcstValue), UriKind.Relative));
                    return;
                case "PTY":
                    MainPTY.Text = GetSentenceForPTY(fcstValue);
                    return;
                case "POP":
                    MainPOP.Text = "강수확률: " + fcstValue + "%";
                    return;
                case "REH":
                    MainREH.Text = "습도: " + fcstValue + "%";
                    return;
            }
        }

        // 메소드 : 
        private string GetSentenceForPTY(string fcstValue)
        {
            switch (fcstValue)
            {
                case "0":
                    return "지금은 비가 내리고 있지 않습니다.";
                case "1":
                    return "비가 내리고 있습니다.";
                case "2":
                    return "비나 눈이 내리고 있습니다.";
                case "3":
                    return "눈이 내리고 있습니다.";
                case "4":
                    return "소나기가 내리고 있습니다.";
            }
            return null;
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
                var list = (from w in ctx.Weather where w.category == "SKY" || w.category == "PTY" select w).ToList();
                byte dayNumber = GetNumberForTheDay(day);

                string image = null;

                foreach (var l in list)
                {
                    if ((int.Parse(l.fcstDate.ToString("yyyyMMdd")) - int.Parse(System.DateTime.Now.ToString("yyyyMMdd"))) == dayNumber
                        && int.Parse(l.fcstTime) / 100 % 2 == 0
                        && int.Parse(l.fcstTime) / 100 > 3)
                    {
                        // 지옥의 if문을 버리고 새로운 메소드를 만들자
                        if (l.category == "SKY")
                        {
                            image = GetSkyValueToDisplay(l.fcstValue);
                        }
                        else
                        {
                            if(l.fcstValue != "0")
                            {
                                image = "Images/rainy.png";
                            }
                        }

                    }
                    else { continue; }

                    weatherImagesDatum.Add(new WeatherDatum() { category = l.category, fcstDate = l.fcstDate, fcstTime = int.Parse(l.fcstTime.Substring(0, 2)) + "시", fcstValue = image }) ;
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
                        && int.Parse(list[i].fcstTime)/100 %2 == 0
                        && int.Parse(list[i].fcstTime) / 100 > 3)
                    {
                        // API Data 순서 - 1번째 TMP, 2번째 POP, 3번째 REH
                        weatherListDatum.Add(new WeatherListDatum() { fcstTime = list[i].fcstTime, TMP = list[i].fcstValue + "℃", POP = "강수확률 : " + list[i+1].fcstValue + "%", REH = "습도 : " + list[i+2].fcstValue + "%" });
                    }
                    i= i+3;
                }
            }

            weatherList.ItemsSource = weatherListDatum;

            Debug.WriteLine("AddWeatherListData DONE");
        }

        private string GetSkyValueToDisplay(string fcstValue)
        {
            switch (fcstValue)
            {
                // 하늘상태
                case "1" : return "Images/sunny.png";
                case "3" : return "Images/cloudy.png"; // 추후 구름 2개로 바꾸기
                case "4" : return "Images/cloudy.png";
            }
            return null;
        }

    }
}
