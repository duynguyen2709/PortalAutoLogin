using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PortalLogin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            for (int i = 1; i <= 5; i++)
                cb_Threads.Items.Add(i);

            //Task.Run(() => driver.Add(new ChromeDriver()));
            //Task.Run(() => driver.Add(new ChromeDriver()));
            //Task.Run(() => driver.Add(new ChromeDriver()));

            cts = new CancellationTokenSource();
        }

        private CancellationTokenSource cts;
        private List<IWebDriver> driver = new List<IWebDriver>();

        //private IWebDriver checkPassDriver;
        private string number;

        private string password;
        private System.Random rd = new System.Random();
        private string username;

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            int n = (int)cb_Threads.SelectedItem;

            List<Task> taskList = new List<Task>();

            for (int i = 0; i < n; i++)
            {
                taskList.Add(OpenBrowsersAsync());
            }

            await Task.WhenAll(taskList);

            taskList.Clear();
            for (int i = 0; i < n; i++)
            {
                taskList.Add(ReloadBrowserAsync(i));
            }

            await Task.WhenAny(taskList);

            cts.Cancel();
        }

        private bool CheckCorrectPassword(int i)
        {
            GetInfo();
            SetInfo(i);

            var error = driver[i].FindElements(By.Id("ctl00_ContentPlaceHolder1_msg"));

            if (error.Count > 0)
            {
                return false;
            }
            else
                return true;
        }

        private bool CheckWebpageError(int i)
        {
            var frontpage = driver[i].FindElements(By.LinkText("HCMUS"));

            if (frontpage.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void GetInfo()
        {
            //GET USERNAME
            Dispatcher.BeginInvoke(new ThreadStart(() => username = txt_Username.Text));

            //GET PASSWORD
            number = rd.Next(0, 100).ToString();
            Dispatcher.BeginInvoke(new ThreadStart(() => password = txt_Password.Text + number));
        }

        private async Task OpenBrowsersAsync()
        {
            await Task.Run(() =>
            {
                driver.Add(new ChromeDriver());
            });
        }

        private async Task ReloadBrowserAsync(int i)
        {
            await Task.Run(() =>
            {
                driver[i].Url = "https://portal2.hcmus.edu.vn/Login.aspx";

                while (true)
                {
                    if (cts.IsCancellationRequested)
                    {
                        driver[i].Close();
                        break;
                    }

                    Stopwatch watch = Stopwatch.StartNew();

                    if (CheckCorrectPassword(i) == false || CheckWebpageError(i) == true)
                    {
                        long time = watch.ElapsedMilliseconds;

                        if (time < 1000)
                        {
                            Thread.Sleep((int)(1000 - time));
                        }
                        driver[i].Navigate().Refresh();
                    }
                    else
                    {
                        break;
                    }
                }
            });
        }

        private void SetInfo(int i)
        {
            try
            {
                //SET USERNAME
                IWebElement usernameTextBox = driver[i].FindElement(By.Id("ctl00_ContentPlaceHolder1_txtUsername"));
                usernameTextBox.Clear();
                usernameTextBox.SendKeys(username);

                //SET PASSWORD
                IWebElement passwordTextBox = driver[i].FindElement(By.Id("ctl00_ContentPlaceHolder1_txtPassword"));
                passwordTextBox.Clear();
                passwordTextBox.SendKeys(password);

                //CLICK LOGIN BUTTON
                IWebElement loginButton = driver[i].FindElement(By.Id("ctl00_ContentPlaceHolder1_btnLogin"));
                loginButton.Click();
            }
            catch (NoSuchElementException)
            {
                return;
            }
        }
    }
}