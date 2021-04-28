using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreamsInventory
{
    public partial class Form1 : Form
    {
        public int _pageIndex = 0;
        private string _streamAPI = String.Empty;

        public InventoryManager _manager;
        public Form1()
        {
            _streamAPI = ConfigurationManager.AppSettings["StreamAPI"].ToString();
            _pageIndex = Convert.ToInt32(ConfigurationManager.AppSettings["StartPageIndex"]);
            InitializeComponent();
            InitializeAsync();
        }
        async void InitializeAsync()
        {
            //string userdataPath = "C:\\Users\\tachen\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1";
            //string userdataPath = "C:\\Users\\tachen\\AppData\\Local\\Microsoft\\Edge\\User Data\\Profile 4";
            string userdataPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "WebView2WorkDir");
            if (!Directory.Exists(userdataPath))
            {
                Directory.CreateDirectory(userdataPath);
            }
            //CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions("--profile-directory=Profile 4");
            var webView2Environment = await CoreWebView2Environment.CreateAsync(null, userdataPath,null);
            await webViewMain.EnsureCoreWebView2Async(webView2Environment);
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            //string url = "https://use2-2.api.microsoftstream.com/api/videos?NoSignUpCheck=1&$top=100&$orderby=publishedDate%20desc&$expand=creator,events&$filter=published%20and%20(state%20eq%20%27Completed%27%20or%20contentSource%20eq%20%27livestream%27)&adminmode=true&api-version=1.4-private&$skip=1";
            //string url = "https://login.microsoftonline.com/common/oauth2/authorize?client_id=cf53fce8-def6-4aeb-8d30-b158e7b1cf83&response_mode=form_post&response_type=code+id_token&scope=openid+profile&state=OpenIdConnect.AuthenticationProperties%3dAQAAAAIAAAAJLnJlZGlyZWN0Nmh0dHBzOi8vd2ViLm1pY3Jvc29mdHN0cmVhbS5jb20vYnJvd3NlP25vU2lnblVwQ2hlY2s9MQhub25jZUtleZsBZDk1UG1GSlpBMkNJWFlEV3IwdnlQbEdHRTlkOHNMWGR3d0p0TXJSWDdrbWpyZnNZelo2TTFRT3BRdU9ZRG1pSDhfc2FWR045RXhhNF9RQmgxdUozQ3BaRmpIeU9MZTNETllQTzdzSl94Q1ZEZTA5VHIxRHljVFNRaHd3R1NPdGVwUnZIWm9fb1VBVWVORmdDTmRZX3JRd1dBUDQ&nonce=637510184373881103.NmNhZjQ4YWItZWVjZS00MDQ0LTg3ZWYtNmYxNzU4NGMwNzc2YWRmNmFkZGItYTlmNy00MjE5LWJhYTUtYWE5NjUzYTg4NDM2&nonceKey=OpenIdConnect.nonce.%2bnS%2bEuRK6xLLXuuCGaaST2njuksMrw7mcuWxIxTSGfw%3d&site_id=500453&redirect_uri=https%3a%2f%2fweb.microsoftstream.com%2f&post_logout_redirect_uri=https%3a%2f%2fproducts.office.com%2fmicrosoft-stream&msafed=0";
            string url = "https://web.microsoftstream.com/browse";

            webViewMain.Source = new Uri(url);
            //webViewMain.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
        }

        private async void CoreWebView2_DOMContentLoaded(object sender, Microsoft.Web.WebView2.Core.CoreWebView2DOMContentLoadedEventArgs e)
        {
            //string html = await webViewMain.ExecuteScriptAsync("document.documentElement.outerHTML;");
            string html = await webViewMain.ExecuteScriptAsync("document.getElementsByTagName('pre')[0].innerHTML;");
            html = html.Replace("\\n", System.Environment.NewLine);
            html = html.Replace("\\", String.Empty);
            html = html.Replace("\"{", "{");
            html = html.Replace("}\"", "}");

            InventoryResult result = _manager.AppendJson(html);

            if (result.Result)
            {
                this.LogMessage(String.Format("Read {0} records, total: {1}, current page: {2}.",
                    result.ProcessCount,
                    result.TotalCount,
                    _pageIndex - 1));

                if (result.ProcessCount > 0)
                {
                    webViewMain.Source = new Uri(this.GetNextPageUrl());
                }
            }
            else
            {
                this.LogMessage(String.Format("Process page {0} failed. {1}",
                    _pageIndex - 1,
                    result.Error == null ? "Unknow error" : result.Error.Message));
                this.LogMessage(result.Error == null ? "" : result.Error.StackTrace);
            }

            //html.Trim(new char[] { '\\','\"'});
            //string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "stream.json");
            //using (StreamWriter sw = new StreamWriter(file))
            //{
            //    sw.Write(html);
            //}
            //string url = "https://usso-1.api.microsoftstream.com/api/videos?NoSignUpCheck=1&$top=100&$orderby=publishedDate%20desc&$expand=creator,events&$filter=published%20and%20(state%20eq%20%27Completed%27%20or%20contentSource%20eq%20%27livestream%27)&adminmode=true&api-version=1.4-private&$skip=1";
            //webViewMain.Source = new Uri(url);

            //Console.WriteLine(e);
        }

        private void btnAccess_Click(object sender, EventArgs e)
        {
            //string url = "https://usso-1.api.microsoftstream.com/api/videos?NoSignUpCheck=1&$top=100&$orderby=publishedDate%20desc&$expand=creator,events&$filter=published%20and%20(state%20eq%20%27Completed%27%20or%20contentSource%20eq%20%27livestream%27)&adminmode=true&api-version=1.4-private&$skip=0";

            webViewMain.Source = new Uri(this.GetNextPageUrl());
            webViewMain.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;

        }
        private string GetNextPageUrl()
        {
            string url = string.Format("{0}/api/videos?NoSignUpCheck=1&$top=100&$orderby=publishedDate%20desc&$expand=creator,events&$filter=published%20and%20(state%20eq%20%27Completed%27%20or%20contentSource%20eq%20%27livestream%27)&adminmode=true&api-version=1.4-private&$skip={1}",
                this._streamAPI,
                _pageIndex);

            _pageIndex++;
            return url;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string csvFile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "inventory.csv");
            _manager = new InventoryManager(csvFile);
        }

        private void LogMessage(string message)
        {
            txtMessage.AppendText(String.Format("{0}{1}", message, System.Environment.NewLine));
        }
    }
}
