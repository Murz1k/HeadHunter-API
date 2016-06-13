using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;

namespace HeadHunterAPI.Test
{
    /// <summary>
    /// Сводное описание для APITest
    /// </summary>
    [TestClass]
    public class APITest
    {
        public CookieContainer cookies;
        string token;
        string UserName;
        private string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Safari/537.36 Edge/13.10586";
        public APITest()
        {
            cookies = new CookieContainer();
        }
        [TestMethod]
        public void AuthenticationTest()
        {
            string email = "Max89701@gmail.com";
            string pass = "55543298max";
            var GetRequest = WebRequest.Create("http://hh.ru/") as HttpWebRequest;
            GetRequest.CookieContainer = cookies;
            GetRequest.UserAgent = UserAgent;
            using (WebResponse GetResponse = GetRequest.GetResponse())
            token = cookies.GetCookieHeader(new Uri("http://hh.ru/")).Split(';')[6].Split('=')[1];

            var PostRequest = WebRequest.Create("https://hh.ru/account/login") as HttpWebRequest;
            PostRequest.CookieContainer = cookies;
            PostRequest.Method = "POST";
            PostRequest.ContentType = "application/x-www-form-urlencoded";
            byte[] queryArr = Encoding.UTF8.GetBytes("backUrl=https%3A%2F%2Fhh.ru%2F&failUrl=%2Faccount%2Flogin%3Fbackurl%3D%252F%26role%3D&username=" + Uri.EscapeDataString(email) + "&password=" + Uri.EscapeDataString(pass) + "&_xsrf=" + token);
            PostRequest.ContentLength = queryArr.Length;
            using (Stream stream = PostRequest.GetRequestStream())
            {
                stream.Write(queryArr, 0, queryArr.Length);
            }
            using (WebResponse PostResponse = PostRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(PostResponse.GetResponseStream()))
                {
                    string html = reader.ReadToEnd();
                    string tag = "\"mainmenu_userName\">";
                    html = html.Remove(0, html.IndexOf(tag) + tag.Length);
                    html = html.Remove(0, html.IndexOf("</span>") + 7);
                    UserName = html.Remove(html.IndexOf("</span>"));
                }
            }
            Assert.AreEqual("Максим Станиславович Захаров", UserName);
        }
    }
}
