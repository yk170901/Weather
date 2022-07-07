using System;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Xml;
using Weather.MariaDb;
using Microsoft.EntityFrameworkCore;

namespace Weather.Controllers
{
    internal class WeatherController
    {
        // 메소드 : API에 요청할 URL을 만드는 메소드
        public string GenerateUrl(int nx, int ny)
        {
            string serviceKey = "VRr9rp4jRHi1ssHGqFnhUx9KVqgvYvKOEtrRUSO3EIFE7x49nw%2FUK7P7IuXpabMz8Nwe44%2BgIG%2FrBY34WQnxMA%3D%3D";
            string baseDate = DateTime.Now.ToString("yyyyMMdd");

            string url = $"http://apis.data.go.kr/1360000/VilageFcstInfoService_2.0/getVilageFcst?serviceKey={serviceKey}&dataType=XML&numOfRows=1000&pageNo=1&base_date={baseDate}&base_time=0500&nx={nx}&ny={ny}";

            Debug.WriteLine("GenerateUrl DONE");

            return url;
        }

        // 메소드 : 만들어진 URL로 API로부터 값을 얻어오는 메소드
        public string GetWeatherApi(string url)
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

            Debug.WriteLine("GetWeatherApi DONE");

            return results;
        }

        // 메소드 : API로 얻은 값을 파싱하여 DB에 넣는 메소드
        public void ParseXmlAndPutInDb(string results)
        {
            XmlDocument xml = new XmlDocument();

            xml.LoadXml(results);

            string resultMsg = xml.GetElementsByTagName("resultMsg")[0].InnerText;

            if (resultMsg != "NORMAL_SERVICE")
            {
                Debug.WriteLine("ERROR : "+resultMsg);
                return;
            }

            XmlNodeList nodeList = xml.GetElementsByTagName("item");

            using (var ctx = new MariaContext())
            {
                var conn = ctx.Database.GetDbConnection();

                foreach (XmlNode node in nodeList)
                {
                    string category = node["category"].InnerText;
                    if (category == "POP" || category == "PTY" || category == "SKY" || category == "TMP" || category == "REH")
                    {
                        ctx.Database.ExecuteSqlRaw($"INSERT INTO weather(fcst_date, category, fcst_time, fcst_value)" +
                            $"VALUES({node["fcstDate"].InnerText}, '{category}', '{node["fcstTime"].InnerText}', '{node["fcstValue"].InnerText}')");
                        //ctx.SaveChanges();
                    }
                    else continue;
                }
            }

            Debug.WriteLine("ParseXmlAndPutInDb DONE");
        }

        // 메소드 : View가 (새로운) 데이터를 요청하면 (새로운) 데이터를 밀어 넣어주는 메소드

        // 메소드 : 새로운 Weather 데이터를 넣기 전 기존 데이터 지우기
        public void DeleteWeatherDataBeforeInsertion()
        {
            using (var ctx = new MariaContext())
            {
                var conn = ctx.Database.GetDbConnection();
                ctx.Database.ExecuteSqlRaw("DELETE FROM weather");
            }

            Debug.WriteLine("DeleteWeatherDataBeforeInsertion DONE");
        }
    }
}
