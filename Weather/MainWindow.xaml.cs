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
using Microsoft.EntityFrameworkCore;

namespace Weather
{
    public partial class MainWindow : Window
    {
        // 변수 : 오늘 내일 모레 등의 글자가 선택되면 사용할 컬러코드
        Brush colorForDays = (Brush)new BrushConverter().ConvertFrom("#3104B4");

        public MainWindow()
        {
            InitializeComponent();
        }

        #region button events
        // 메소드 : X 버튼 클릭 시 프로그램 종료
        private void Close(object sender, RoutedEventArgs e)
        { Close(); }

        // 메소드 : 창 드래그
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {   Cursor = Cursors.SizeAll;
            this.DragMove(); }
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        { Cursor = Cursors.Arrow; }


        // 메소드 : 오늘 버튼 눌렀을 때 오늘 날씨 데이터 전달
        private void GetDataForToday(object sender, RoutedEventArgs e)
        {
            AddWeatherImageAndDateDataList("오늘");
            AddWeatherDataList("오늘");

            today.Foreground = colorForDays;
            tomorrow.Foreground = Brushes.Black;
            dayAfterTomorrow.Foreground = Brushes.Black;
        }

        // 메소드 : 내일 버튼 눌렀을 때 내일 날씨 데이터 전달
        private void GetDataForTomorrow(object sender, RoutedEventArgs e)
        {
            AddWeatherImageAndDateDataList("내일");
            AddWeatherDataList("내일");

            today.Foreground = Brushes.Black;
            tomorrow.Foreground = colorForDays;
            dayAfterTomorrow.Foreground = Brushes.Black;
        }

        // 메소드 : 모레 버튼 눌렀을 때 모레 날씨 데이터 전달
        private void GetDataForTheDayAfterTomorrow(object sender, RoutedEventArgs e)
        {
            AddWeatherImageAndDateDataList("모레");
            AddWeatherDataList("모레");

            today.Foreground = Brushes.Black;
            tomorrow.Foreground = Brushes.Black;
            dayAfterTomorrow.Foreground = colorForDays;
        }

        // 메소드 : Area1 콤보 박스의 값에 따라 Area2 콤보 박스의 값 변경
        private async void ChangeCombo2ForCombo1(object sender, SelectionChangedEventArgs e)
        {
            string area1 = Area1ComboBox.SelectedItem.ToString().Split(" ")[1]; // Area1 콤보박스 선택값

            if (area1 == "선택해주세요") return;

            Area2ComboBox.Items.Clear();

            using (var ctx = new MariaContext())
            {
                var list = await (from a in ctx.Area where a.area1 == area1 && a.area2 != "" && a.area3 == "" select a.area2).ToListAsync() ?? new List<string>();

                foreach (var l in list)
                {
                    Area2ComboBox.Items.Add(l);
                }
            }
            Area2ComboBox.SelectedIndex = 0;
        }

        // 메소드 : 검색 버튼 클릭 이벤트
        private async void Search(object sender, RoutedEventArgs e)
        {
            WeatherController weatherController = new WeatherController();

            // Area1 콤보박스가 선택되지 않았다면 리턴
            if (Area1ComboBox.SelectedItem.ToString().Split(" ")[1] == "선택해주세요")
            {
                MessageBox.Show("날씨가 궁금한 지역을 선택해주세요");
                return;
            }

            Dictionary<string, int> nxAndNy = GetNxAndNy();

            string url = weatherController.GenerateUrl(nxAndNy["nx"], nxAndNy["ny"]);

            string? results = await weatherController.GetWeatherApi(url);
            if (string.IsNullOrWhiteSpace(results))
                return;

            weatherController.DeleteWeatherDataBeforeInsertion();
            weatherController.ParseXmlAndPutInDb(results);

            AddWeatherDataForMain();

            AddWeatherImageAndDateDataList("오늘");
            AddWeatherDataList("오늘");

            ManageOptionalDetailsWhenSearching();

            Debug.WriteLine($" - - - Data for x={nxAndNy["nx"]} and {nxAndNy["ny"]} - - - ");
        }
        #endregion

        // 메소드 : 검색 버튼 클릭 시
        //        : 1) Question gif, LilStar 특수문자,  오늘/내일/모레 글자의 Visibility를 관리 
        //        : 2) 오늘/내일/모레 중 오늘의 글자색을 보라색으로 변경, 다른 두 날짜의 글자색은 검정색으로 변경.
        private void ManageOptionalDetailsWhenSearching()
        {
            QuestionMarkGif.Visibility = Visibility.Hidden;
            today.Visibility = Visibility.Visible;
            tomorrow.Visibility = Visibility.Visible;
            dayAfterTomorrow.Visibility = Visibility.Visible;
            LilStar.Visibility = Visibility.Visible;

            today.Foreground = colorForDays;
            tomorrow.Foreground = Brushes.Black;
            dayAfterTomorrow.Foreground = Brushes.Black;
        }

        // 메소드 : 메인 창에 현재 시간/장소 날씨 데이터 디스플레이
        private void AddWeatherDataForMain()
        {
            string hourNow = System.DateTime.Now.Hour + "00";
            if (hourNow.Length == 3) hourNow = "0" + hourNow;

            using (var ctx = new MariaContext())
            {
                // 당일 데이터이며 해당 시간의 데이터만 가져오고 싶었는데 당일 데이터는 얻어와지지가 않는다.
                    // System.Date.Now를 하면 w.fcstDate와 매치가 안 되고, String으로 비교를 하려고 하면 자료형이 달라 비교할 수 없다.
                var list = (from w in ctx.Weather where w.fcstTime == hourNow select w).ToList();

                foreach (var l in list)
                {
                    if (l.fcstDate.ToString("yyyyMMdd")  == System.DateTime.Now.ToString("yyyyMMdd"))
                    {
                        ChangeMainValuesOnScreen(l.category, l.fcstValue);
                    }

                    // 강수형태가 1이상일 경우 (비나 눈이 내릴 경우) SKY 이미지를 바꿈. PTY 값은 SKY 값보다 나중에 전달되므로 이 방식에는 문제 없음.
                    if(l.category=="PTY" && l.fcstValue != "0" && l.fcstDate.ToString("yyyyMMdd") == System.DateTime.Now.ToString("yyyyMMdd"))
                    {
                        MainSKY.Source = new BitmapImage(new Uri("Images/rainy.png", UriKind.Relative));
                    }
                }
            }

            TimeDataCalled.Text = System.DateTime.Now.ToString("MM월 dd일 HH시 mm분 날씨");
            Region.Text = Area1ComboBox.SelectedItem.ToString().Split(" ")[1] + " " + Area2ComboBox.SelectedItem.ToString();
        }

        // 메소드 : 메인 화면에 디스플레이 할 현재 시각의 날씨 정보를 category에 따라 생성
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

        // 메소드 : 강수형태의 값을 받아 해당 값이 의미하는 날씨 정보 문장 생성
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
            Dictionary<string,int> nxAndNy = new Dictionary<string,int>();

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
        private void AddWeatherImageAndDateDataList(string day)
        {
            List<WeatherDatum> weatherImagesDatum = new List<WeatherDatum>();

            using (var ctx = new MariaContext())
            {
                // SKY를 기반으로 오늘/내일/모레의 날짜 Image를 디스플레이 하나 PTY(강수형태) 값이 0(비 없음)이 아니면 rainy.png를 보일 수 있도록 PTY 값도 가져온다.
                var list = (from w in ctx.Weather where w.category == "SKY" || w.category == "PTY" select w).ToList();
                byte dayNumber = GetNumberForTheDay(day);

                string image;

                // 각 시간별로 SKY와 PTY라는 값이 하나씩, 총 2개가 들어 있다. 이중 SKY의 값만을 중점적으로 사용할 것이기에
                // 짝수 자리에 위치하는 SKY 데이터를 기준으로 두어 로직을 짰다.
                for (int i = 0; i < (list.Count())/2; i++)
                {
                    if ((int.Parse(list[i * 2].fcstDate.ToString("yyyyMMdd")) - int.Parse(System.DateTime.Now.ToString("yyyyMMdd"))) == dayNumber
                          && int.Parse(list[i * 2].fcstTime) / 100 % 2 == 0 // 짝수이며
                          && int.Parse(list[i * 2].fcstTime) / 100 > 3)     // 03시 이후의 날짜 값만 취급한다.
                    {
                        image = GetSkyValueToDisplay(list[i * 2].fcstValue);

                        if (list[i * 2 + 1].fcstValue != "0") image = "Images/rainy.png"; // PTY(강수형태)가 0(비없음)이 아니면 image를 rainy.png로.

                        weatherImagesDatum.Add(new WeatherDatum() { category = list[i*2].category, fcstDate = list[i * 2].fcstDate, fcstTime = int.Parse(list[i * 2].fcstTime.Substring(0, 2)) + "시", fcstValue = image });
                    }
                }
            }
            weatherImage.ItemsSource = weatherImagesDatum;
            Debug.WriteLine("AddWeatherImageList DONE");
        }

        // 메소드 : WeatherDatum의 형태로 리스트 데이터 나타내보기
        private void AddWeatherDataList(string day)
        {
            List<WeatherListDatum> weatherListDatum = new List<WeatherListDatum>();

            using (var ctx = new MariaContext())
            {
                var list = (from w in ctx.Weather where w.category != "SKY" && w.category != "PTY" select w).ToList();
                byte dayNumber = GetNumberForTheDay(day);

                int i = 0; // for문이 아닌 while문 쓰는 이유는 그냥 for(~;~;i+=3)이 되는 줄 몰랐다.
                while (i < list.Count()) // 한꺼번에 3개의 데이터를 넣는 식으로 처리한다.
                {
                    if ((int.Parse(list[i].fcstDate.ToString("yyyyMMdd")) - int.Parse(System.DateTime.Now.ToString("yyyyMMdd"))) == dayNumber
                        && int.Parse(list[i].fcstTime)/100 %2 == 0 // 짝수이며
                        && int.Parse(list[i].fcstTime) / 100 > 3)  // 03시 이후의 날짜 값만 취급한다.
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

        // 메소드 : SKY(하늘상태) 값에 따라 디스플레이 할 이미지 특정
        private string GetSkyValueToDisplay(string fcstValue)
        {
            switch (fcstValue)
            {
                // 하늘상태
                case "1" : return "Images/sunny.png";
                case "3" : return "Images/very_cloudy.png";
                case "4" : return "Images/cloudy.png";
            }
            return null;
        }
    }
}
