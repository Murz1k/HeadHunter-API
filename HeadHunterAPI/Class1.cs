using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


public class HeadHunter
{
    private CookieContainer cookies;
    private string token;
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
    /// Добавляет ключевые навыки в резюме.
    /// </summary>
    /// <param name="id">Идентификатор резюме</param>
    /// <param name="skills">Массив навыков</param>
    public void SetSkills(string id, string[] skills)
    {
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(String.Format("http://hh.ru/applicant/resumes/edit/experience?resume={0}&field=keySkills", id));
        req.CookieContainer = cookies;
        req.Method = "POST";
        req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
        string data = "";
        foreach (var skill in skills)
        {
            data += "keySkills.string="+skill + "&";
        }
        data += "_xsrf="+token;
        byte[] sentData = Encoding.ASCII.GetBytes(data);
        req.ContentLength = sentData.Length;
        using (Stream stream = req.GetRequestStream())
        {
            stream.Write(sentData, 0, sentData.Length);
        }
        using (WebResponse GetResponse = req.GetResponse()) ;
    }
    /// <summary>
    /// Асихнорнно добавляет ключевые навыки в резюме.
    /// </summary>
    /// <param name="id">Идентификатор резюме</param>
    /// <param name="skills">Массив навыков</param>
    public async Task SetSkillsAsync(string id, string[] skills)
    {
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(String.Format("http://hh.ru/applicant/resumes/edit/experience?resume={0}&field=keySkills", id));
        req.CookieContainer = cookies;
        req.Method = "POST";
        req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
        string data = "";
        foreach (var skill in skills)
        {
            data += "keySkills.string=" + skill + "&";
        }
        data += "_xsrf=" + token;
        byte[] sentData = Encoding.ASCII.GetBytes(data);
        req.ContentLength = sentData.Length;
        using (Stream stream = await req.GetRequestStreamAsync())
        {
            await stream.WriteAsync(sentData, 0, sentData.Length);
        }
        using (WebResponse GetResponse = await req.GetResponseAsync()) ;
    }
    /// <summary>
    /// Асинхронно возвращает список идентификаторов.
    /// </summary>
    /// <returns>Асинхронно возвращает список идентификаторов.</returns>
    public async Task<List<string>> GetAllIdAsync()
    {
        List<string> collection = new List<string>();
        var Get = WebRequest.Create("http://hh.ru/applicant/resumes") as HttpWebRequest;
        Get.CookieContainer = cookies;
        Get.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.10240";
        using (WebResponse GetResponse = await Get.GetResponseAsync())
        {
            using (Stream stream = GetResponse.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
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
        var Get = WebRequest.Create("http://hh.ru/applicant/resumes") as HttpWebRequest;
        Get.CookieContainer = cookies;
        Get.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.10240";
        using (WebResponse GetResponse = Get.GetResponse())
        {
            using (Stream stream = GetResponse.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
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
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://hh.ru/applicant/resumes/touch");
        req.CookieContainer = cookies;
        req.Method = "POST";
        req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
        req.Headers.Add("X-Xsrftoken", token);
        byte[] sentData = Encoding.ASCII.GetBytes("resume=" + id + "&undirectable=true");
        req.ContentLength = sentData.Length;
        using (Stream stream = req.GetRequestStream())
        {
            stream.Write(sentData, 0, sentData.Length);
        }
        using (WebResponse GetResponse = req.GetResponse()) ;
    }
    /// <summary>
    /// Асинхронно обновляет резюме с выбранным идентификатором.
    /// </summary>
    /// <param name="id">Идентификатор</param>
    public async Task UpdateResumeByIdAsync(string id)
    {
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://hh.ru/applicant/resumes/touch");
        req.CookieContainer = cookies;
        req.Method = "POST";
        req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
        req.Headers.Add("X-Xsrftoken", token);
        byte[] sentData = Encoding.ASCII.GetBytes("resume=" + id + "&undirectable=true");
        req.ContentLength = sentData.Length;
        using (Stream stream = await req.GetRequestStreamAsync())
        {
            await stream.WriteAsync(sentData, 0, sentData.Length);
        }
        using (WebResponse GetResponse = await req.GetResponseAsync()) ;
    }
}
