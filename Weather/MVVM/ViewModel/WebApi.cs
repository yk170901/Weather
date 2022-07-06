using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Diagnostics;

namespace Weather.MVVM.ViewModel
{
    public class WebApi
    {
        public readonly int _testValue;
        public WebApi()
        {
            _testValue = 10;
        }

        public string GenerateUrl(int nx, int ny)
        {
            string serviceKey = "VRr9rp4jRHi1ssHGqFnhUx9KVqgvYvKOEtrRUSO3EIFE7x49nw%2FUK7P7IuXpabMz8Nwe44%2BgIG%2FrBY34WQnxMA%3D%3D";
            string baseDate = "20220706"; // 오늘의 날짜를 얻는 메소드를 사용하여 날짜를 얻은 후 yyyymmdd형태로 컨버팅하여 사용하기

            string url = $"http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getVilageFcst?serviceKey={serviceKey}&dataType=XML&numOfRows=1000&pageNo=1&base_date={baseDate}&base_time=0500&nx={nx}&ny={ny}";

            return url;
        }

        public string GetApi(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            string results = string.Empty;
            HttpWebResponse response;
            using (response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                results = reader.ReadToEnd();
            }
            Debug.WriteLine("결과 : : : "+results);
            Debug.WriteLine("On Jupyter and Mars");

            return results;
        }

    }
}
