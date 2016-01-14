using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

    public class Resume
    {
        public string[] Skills;
        public string About;
    }
    public class HeadHunter
    {
        public CookieContainer cookies;
        public string token;
        /// <summary>
        /// Имя текущего пользователя.
        /// </summary>
        public string UserName;
        public HeadHunter()
        {
            cookies = new CookieContainer();
        }
        /// <summary>
        /// Авторизовывает пользователя на сайте.
        /// </summary>
        /// <param name="email">Почта</param>
        /// <param name="pass">Пароль</param>
        public void Authentication(string email, string pass)
        {
            var GetRequest = WebRequest.Create("http://hh.ru/") as HttpWebRequest;
            GetRequest.CookieContainer = cookies;
            GetRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.10240";
            using (WebResponse GetResponse = GetRequest.GetResponse())
                token = cookies.GetCookieHeader(new Uri("http://hh.ru/")).Split(';')[6].Split('=')[1];

            var PostRequest = WebRequest.Create("https://hh.ru/account/login") as HttpWebRequest;
            PostRequest.CookieContainer = cookies;
            PostRequest.Method = "POST";
            PostRequest.ContentType = "application/x-www-form-urlencoded";
            byte[] queryArr = Encoding.UTF8.GetBytes("username=" + Uri.EscapeDataString(email) + "&password=" + Uri.EscapeDataString(pass) + "&action=%D0%92%D0%BE%D0%B9%D1%82%D0%B8&_xsrf=" + token);
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
                    UserName = html.Remove(0, html.IndexOf(tag) + tag.Length);
                    UserName = UserName.Remove(UserName.IndexOf('<'));
                }
            }
        }
        /// <summary>
        /// Асинхронно авторизовывает пользователя на сайте.
        /// </summary>
        /// <param name="email">Почта</param>
        /// <param name="pass">Пароль</param>
        public async void AuthenticationAsync(string email, string pass)
        {
            var GetRequest = WebRequest.Create("http://hh.ru/") as HttpWebRequest;
            GetRequest.CookieContainer = cookies;
            GetRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.10240";
            using (WebResponse GetResponse = await GetRequest.GetResponseAsync())
                token = cookies.GetCookieHeader(new Uri("http://hh.ru/")).Split(';')[6].Split('=')[1];

            var PostRequest = WebRequest.Create("https://hh.ru/account/login") as HttpWebRequest;
            PostRequest.CookieContainer = cookies;
            PostRequest.Method = "POST";
            PostRequest.ContentType = "application/x-www-form-urlencoded";
            byte[] queryArr = Encoding.UTF8.GetBytes("username=" + Uri.EscapeDataString(email) + "&password=" + Uri.EscapeDataString(pass) + "&action=%D0%92%D0%BE%D0%B9%D1%82%D0%B8&_xsrf=" + token);
            PostRequest.ContentLength = queryArr.Length;
            using (Stream stream = await PostRequest.GetRequestStreamAsync())
            {
                await stream.WriteAsync(queryArr, 0, queryArr.Length);
            }
            using (WebResponse PostResponse = PostRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(PostResponse.GetResponseStream()))
                {
                    string html = await reader.ReadToEndAsync();
                    string tag = "\"mainmenu_userName\">";
                    UserName = html.Remove(0, html.IndexOf(tag) + tag.Length);
                    UserName = UserName.Remove(UserName.IndexOf('<'));
                }
            }
        }
        /// <summary>
        /// Асинхронно возвращает массив умений.
        /// </summary>
        /// <param name="id">Идентификатор резюме</param>
        /// <returns>Асинхронно возвращает массив умений.</returns>

        /// <summary>
        /// Асинхронно возвращает резюме.
        /// </summary>
        /// <param name="id">Идентификатор резюме</param>
        /// <returns>Асинхронно возвращает резюме.</returns>
        public async Task<Resume> GetResumeByIdAsync(string id)
        {
            List<string> array = new List<string>();
            Resume resume = new Resume();
            string url = "http://hh.ru/applicant/resumes/view?resume=" + id;
            using (StreamReader reader = await GetAsync(url))
            {
                string html = await reader.ReadToEndAsync();
                string tag = "HH/Resume/Endorsement";
                html = html.Remove(0, html.IndexOf(tag) + tag.Length);
                string html1 = html.Remove(html.IndexOf("></script>"));
                html = html.Remove(0, html.IndexOf("Обо мне"));
                string tag1 = "<string>";
                html = html.Remove(0, html.IndexOf(tag1) + tag1.Length);
                html = html.Remove(html.IndexOf("</string>"));
                resume.About = WebUtility.HtmlDecode(html.Replace("<br>", Environment.NewLine));
                while (html1.IndexOf("text") > -1)
                {
                    html1 = html1.Remove(0, html1.IndexOf("text") + 8);
                    array.Add(WebUtility.HtmlDecode(html1.Remove(html1.IndexOf(',') - 1)));
                }
                resume.Skills = array.ToArray();
            }
            return resume;
        }
        /// <summary>
        /// Возвращает резюме.
        /// </summary>
        /// <param name="id">Идентификатор резюме</param>
        /// <returns>Возвращает резюме.</returns>
        public Resume GetResumeById(string id)
        {
            List<string> array = new List<string>();
            Resume resume = new Resume();
            string url = "http://hh.ru/applicant/resumes/view?resume=" + id;
                using (StreamReader reader = Get(url))
                {
                    string html = reader.ReadToEnd();
                    string tag = "HH/Resume/Endorsement";
                    html = html.Remove(0, html.IndexOf(tag) + tag.Length);
                    string html1 = html.Remove(html.IndexOf("></script>"));
                    html = html.Remove(0, html.IndexOf("Обо мне"));
                    string tag1 = "<string>";
                    html = html.Remove(0, html.IndexOf(tag1) + tag1.Length);
                    html = html.Remove(html.IndexOf("</string>"));
                    resume.About = WebUtility.HtmlDecode(html.Replace("<br>", Environment.NewLine));
                    while (html1.IndexOf("text") > -1)
                    {
                        html1 = html1.Remove(0, html1.IndexOf("text") + 8);
                        array.Add(WebUtility.HtmlDecode(html1.Remove(html1.IndexOf(',') - 1)));
                    }
                    resume.Skills = array.ToArray();
                }
            return resume;
        }
        /// <summary>
        /// Добавляет ключевые навыки в резюме.
        /// </summary>
        /// <param name="id">Идентификатор резюме</param>
        /// <param name="skills">Массив навыков</param>
        public void SetSkillsById(string id, string[] skills)
        {
            string url = String.Format("http://hh.ru/applicant/resumes/edit/experience?resume={0}&field=keySkills", id);
            string data = "";
            foreach (var skill in skills)
            {
                data += "keySkills.string=" + skill + "&";
            }
            data += "_xsrf=" + token;
            Post(url, data);
        }
        /// <summary>
        /// Асинхронно добавляет "О себе" в резюме.
        /// </summary>
        /// <param name="id">Идентификатор резюме</param>
        /// <param name="text">Добавляемый текст</param>
        public async Task SetAboutByIdAsync(string id, string text)
        {
            string url = String.Format("http://hh.ru/applicant/resumes/edit/experience?resume={0}&field=skills", id);
            string data = "skills.string=" + text + "&_xsrf=" + token;
            await PostAsync(url, data);
        }
        /// <summary>
        /// Добавляет "О себе" в резюме.
        /// </summary>
        /// <param name="id">Идентификатор резюме</param>
        /// <param name="text">Добавляемый текст</param>
        public void SetAboutById(string id, string text)
        {
            string url = String.Format("http://hh.ru/applicant/resumes/edit/experience?resume={0}&field=skills", id);
            string data = "skills.string=" + text + "&_xsrf=" + token;
            Post(url, data);
        }
        /// <summary>
        /// Асихнорнно добавляет ключевые навыки в резюме.
        /// </summary>
        /// <param name="id">Идентификатор резюме</param>
        /// <param name="skills">Массив навыков</param>
        public async Task SetSkillsByIdAsync(string id, string[] skills)
        {
            string url = String.Format("http://hh.ru/applicant/resumes/edit/experience?resume={0}&field=keySkills", id);
            string data = "";
            foreach (var skill in skills)
            {
                data += "keySkills.string=" + skill + "&";
            }
            data += "_xsrf=" + token;
            await PostAsync(url, data);
        }
        /// <summary>
        /// Асинхронно возвращает список идентификаторов.
        /// </summary>
        /// <returns>Асинхронно возвращает список идентификаторов.</returns>
        public async Task<List<string>> GetAllIdAsync()
        {
            List<string> collection = new List<string>();
            string url = "http://hh.ru/applicant/resumes";
            using (StreamReader reader = await GetAsync(url))
            {
                {
                    string html = await reader.ReadToEndAsync();
                    while (html.IndexOf("resumeId") > -1)
                    {
                        html = html.Remove(0, html.IndexOf("resumeId"));
                        html = html.Remove(0, html.IndexOf("resumeHash=") + "resumeHash=".Length);
                        collection.Add((html.Remove(html.IndexOf('"'))));
                    }
                }
            }
            return collection;
        }
        /// <summary>
        /// Возвращает список идентификаторов.
        /// </summary>
        /// <returns>Возвращает список идентификаторов.</returns>
        public List<string> GetAllId()
        {
            List<string> collection = new List<string>();
            string url = "http://hh.ru/applicant/resumes";
            using (StreamReader reader = Get(url))
            {
                    {
                        string html = reader.ReadToEnd();
                        while (html.IndexOf("resumeId") > -1)
                        {
                            html = html.Remove(0, html.IndexOf("resumeId"));
                            html = html.Remove(0, html.IndexOf("resumeHash=") + "resumeHash=".Length);
                            collection.Add((html.Remove(html.IndexOf('"'))));
                        }
                    }
                }
            return collection;
        }
        /// <summary>
        /// Обновляет все резюме.
        /// </summary>
        public void UpdateAll()
        {
            List<string> collection = new List<string>();
            collection = GetAllId();
            foreach (var resume in collection)
            {
                UpdateResumeById(resume);
            }
        }
        /// <summary>
        /// Асинхронно обновляет все резюме.
        /// </summary>
        public async Task UpdateAllAsync()
        {
            List<string> collection = new List<string>();
            collection = await GetAllIdAsync();
            foreach (var resume in collection)
            {
                await UpdateResumeByIdAsync(resume);
            }
        }
        /// <summary>
        /// Обновляет резюме с выбранным идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор</param>
        public void UpdateResumeById(string id)
        {
            string url = "http://hh.ru/applicant/resumes/touch";
            string data = "resume=" + id + "&undirectable=true";
            Post(url, data);
        }
        /// <summary>
        /// Асинхронно обновляет резюме с выбранным идентификатором.
        /// </summary>
        /// <param name="id">Идентификатор</param>
        public async Task UpdateResumeByIdAsync(string id)
        {
            string url = "http://hh.ru/applicant/resumes/touch";
            string data = "resume=" + id + "&undirectable=true";
            await PostAsync(url, data);
        }
        /// <summary>
        /// Осуществляет GET-запрос.
        /// </summary>
        /// <param name="url">Ссылка</param>
        /// <returns></returns>
        internal StreamReader Get (string url)
        {
            var Get = WebRequest.Create(url) as HttpWebRequest;
            Get.CookieContainer = cookies;
            Get.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.10240";
            WebResponse GetResponse = Get.GetResponse();
            Stream stream = GetResponse.GetResponseStream();
            return new StreamReader(stream);
        } 
        /// <summary>
        /// Асинхронно осуществляет GET-запрос.
        /// </summary>
        /// <param name="url">Ссылка</param>
        /// <returns></returns>
        internal async Task<StreamReader> GetAsync(string url)
        {
            var Get = WebRequest.Create(url) as HttpWebRequest;
            Get.CookieContainer = cookies;
            Get.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.10240";
            using (WebResponse GetResponse = await Get.GetResponseAsync())
            {
                using (Stream stream = GetResponse.GetResponseStream())
                {
                    return new StreamReader(stream);
                }
            }
        }
        /// <summary>
        /// Получает коллекцию откликов с указанной страницы.
        /// </summary>
        /// <param name="id_page">Идентификатор страницы</param>
        /// <returns></returns>
        public Dictionary<int, string> GetAllNegotiations(int id_page)
        {
            Dictionary<int, string> negotiations = new Dictionary<int, string>();

            string url = "http://hh.ru/applicant/negotiations?page=" + id_page;
            using (StreamReader reader = Get(url))
            {
                string html = reader.ReadToEnd();
                while (html.IndexOf("data-hh-negotiations-responses-topic-id") > -1)
                {
                    html = html.Remove(0, html.IndexOf("data-hh-negotiations-responses-topic-id"));
                    html = html.Remove(0, html.IndexOf("=") + 2);
                    int id = int.Parse((html.Remove(html.IndexOf('"'))));
                    string denial = "";
                    if (html.Contains("negotiations__denial"))
                    {
                        html = html.Remove(0, html.IndexOf("negotiations__denial"));
                        html = html.Remove(0, html.IndexOf(">") + 1);
                        denial = (html.Remove(html.IndexOf("</span>")));
                    }
                    else if (html.Contains("prosper-table__cell prosper-table__cell_nowrap"))
                    {
                        html = html.Remove(0, html.IndexOf("prosper-table__cell prosper-table__cell_nowrap"));
                        html = html.Remove(0, html.IndexOf(">") + 1);
                        denial = (html.Remove(html.IndexOf("</td>")));
                    }
                    negotiations.Add(id, denial);
                }
                return negotiations;
            }
        }
        /// <summary>
        /// Асинхронно удаляет отклик с указанным идентификатором на указанной странице.
        /// </summary>
        /// <param name="id">Идентификатор отклика</param>
        /// <param name="id_page">Идентификатор страницы</param>
        /// <returns></returns>
        public async Task DeleteNegotiationByIdAsync(int id,int id_page)
        {
            string url = "http://hh.ru/applicant/negotiations/trash/";
            string data = "topic=" + id + "&query=%3Fpage%3D" + id_page + "%26filter%3Dactive_and_archived%26&substate=HIDE&_xsrf=" + token;
            await PostAsync(url,data);
        }
        /// <summary>
        /// Асинхронно осуществляет POST-запрос.
        /// </summary>
        /// <param name="url">Ссылка</param>
        /// <param name="data">Данные</param>
        /// <returns></returns>
        internal async Task PostAsync(string url, string data)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = cookies;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req.Headers.Add("X-Xsrftoken", token);
            byte[] sentData = Encoding.ASCII.GetBytes(data);
            req.ContentLength = sentData.Length;
            using (Stream stream = await req.GetRequestStreamAsync())
            {
                await stream.WriteAsync(sentData, 0, sentData.Length);
            }
            using (WebResponse GetResponse = await req.GetResponseAsync()) ;
        }
        /// <summary>
        /// Осуществляет POST-запрос.
        /// </summary>
        /// <param name="url">Ссылка</param>
        /// <param name="data">Данные</param>
        /// <returns></returns>
        internal void Post(string url, string data)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.CookieContainer = cookies;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req.Headers.Add("X-Xsrftoken", token);
            byte[] sentData = Encoding.ASCII.GetBytes(data);
            req.ContentLength = sentData.Length;
            using (Stream stream = req.GetRequestStream())
            {
                stream.Write(sentData, 0, sentData.Length);
            }
            using (WebResponse GetResponse = req.GetResponse()) ;
        }
    }