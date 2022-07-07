using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Diagnostics;
using System.Xml;

namespace Weather.Controllers
{
    internal class WeatherController
    {
        // 메소드 : API에 요청할 URL을 만드는 메소드
        public string GenerateUrl(int nx, int ny)
        {
            string serviceKey = "VRr9rp4jRHi1ssHGqFnhUx9KVqgvYvKOEtrRUSO3EIFE7x49nw%2FUK7P7IuXpabMz8Nwe44%2BgIG%2FrBY34WQnxMA%3D%3D";
            string baseDate = "20220706"; // 오늘의 날짜를 얻는 메소드를 사용하여 날짜를 얻은 후 yyyymmdd형태로 컨버팅하여 사용하기

            string url = $"http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getVilageFcst?serviceKey={serviceKey}&dataType=XML&numOfRows=1000&pageNo=1&base_date={baseDate}&base_time=0500&nx={nx}&ny={ny}";

            return url;
        }

        // 메소드 : 만들어진 URL로 API로부터 값을 얻어오는 메소드
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

            return results;
        }

        // 메소드 : API로 얻은 값을 파싱하여 DB에 넣기 좋은 형태로 자르는 메소드
        public void ParseXml(string results)
        {
            XmlDocument xml = new XmlDocument();

            xml.LoadXml(results);

            string resultMsg = xml.GetElementsByTagName("resultMsg")[0].InnerText;

            if (resultMsg != "NORMAL_SERVICE")
            {
                Debug.WriteLine("ERROR : "+resultMsg);
                return;
            }

            XmlNodeList nodeList = xml.GetElementsByTagName("items");

            foreach(XmlNode node in nodeList)
            {

            }


        }


        // 메소드 : 파싱된 API 값을 DB에 넣는 메소드

        // 메소드 : DB에 새로운 값이 넣어졌다며 View에게 알리는 메소드

        // 메소드 : View가 (새로운) 데이터를 요청하면 (새로운) 데이터를 밀어 넣어주는 메소드

        // 메소드 : 기존에 있던 DB의 데이터를 지우는 메소드
    }
}
