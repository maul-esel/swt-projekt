using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MobileApp.MainPage();
            KeyPressCommand = new Command<string>(key => OnKeyPress(key));
            MainPage.BindingContext = this;
        }

        public Command<string> KeyPressCommand { get; private set; }

        private int value1 = 0;
        private int value2 = 0;
        private bool isFinished = false;
        private string calculationText = "";
        private string method = "";
        private const string URL = "http://lingvo.azurewebsites.net/api/calculator/";

        void OnKeyPress(string key)
        {
            switch(key)
            {
                case "Clear": CalculationText = ""; break;
                case "Multiply": method = "multiply"; value1 = Int32.Parse(CalculationText); CalculationText = ""; break;
                case "Plus": method = "add";  value1 = Int32.Parse(CalculationText);  CalculationText = ""; break;
                case "Equal": value2 = Int32.Parse(CalculationText); computeResult(); isFinished = true;  break;
                default:
                    if (isFinished)
                    {
                        CalculationText = "";
                        isFinished = false;
                    }
                    CalculationText = CalculationText + key; break;
            }
        }

        private async void computeResult()
        {
            await FetchResultAsync();
        }

        private async Task FetchResultAsync()
        {
            string url = URL + method + "/" + value1 + "/" + value2;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.CreateHttp(new Uri(url));
            request.ContentType = "text/html";
            request.Method = "GET";

            using(WebResponse response = await request.GetResponseAsync())
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                CalculationText = reader.ReadToEnd();
                value1 = value2 = 0;
            }           
        }

        public string CalculationText
        {
            get { return calculationText; }
            set
            {
                calculationText = value;
                if (string.IsNullOrEmpty(calculationText))
                {
                    calculationText = " "; // HACK to force grid view to allocate space.
                }
                OnPropertyChanged("CalculationText");
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
