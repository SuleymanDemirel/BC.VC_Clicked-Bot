using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using System.Net;
using HtmlAgilityPack;
using System.IO;
using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;

namespace Service
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

        }

        IWebDriver driver;
        public async void DriverBaslat()
        {

            ChromeOptions options = new ChromeOptions();

            options.AddArguments("--disable-notifications");

            if (cbxDriverHideAndShow.Checked == true)
            {
                options.AddArgument("--headless");
            }

            options.AddExcludedArgument("enable-automation");

            ChromeDriverService cds = ChromeDriverService.CreateDefaultService();
            cds.HideCommandPromptWindow = true;

            driver = new ChromeDriver(cds, options);

            await System.Threading.Tasks.Task.Delay(2000);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LinkleriCek();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            LinkleriCek();
        }

        private void LinkleriCek()
        {
            StreamReader oku = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "\\BCVC_Links.txt");
            string yazi;
            while ((yazi = oku.ReadLine()) != null) //satır boş olana kadar satır satır okumaya devam eder
            {
                listBox_Linklist.Items.Add(yazi.ToString());
            }
            oku.Close();//okumayı kapat
            label3.Text = listBox_Linklist.Items.Count.ToString();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (cbxBaslangicdaCalistir.Checked == true)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                key.SetValue("Service.exe", "\"" + Application.ExecutablePath + "\"");
            }

            await System.Threading.Tasks.Task.Run(async () =>
            {
                if (cbxFormGizle.Checked == true)
                {
                    this.Hide();
                }


                try
                {
                    for (int i = 0; i < listBox_Linklist.Items.Count; i++)
                    {

                        listBox_Linklist.SelectedIndex = i;
                        label1.Text = listBox_Linklist.SelectedItem.ToString();

                        label3.Text = listBox_Linklist.Items.Count.ToString();
                        label2.Text = listBox_Linklist.SelectedIndex.ToString();
                        DriverBaslat();

                        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                        driver.Navigate().GoToUrl(listBox_Linklist.SelectedItem.ToString());
                        await System.Threading.Tasks.Task.Delay(10000);//10 saniye beklet
                        IWebElement ReklamiGec = driver.FindElement(By.CssSelector("#getLink"));
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", ReklamiGec);
                        driver.SwitchTo().Window(driver.WindowHandles.First());
                        await System.Threading.Tasks.Task.Delay(5000);
                        driver.SwitchTo().Window(driver.WindowHandles.First());

                        driver.Close();
                        driver.Quit();
                        driver.Dispose();

                    }
                    if (cbxMesajVer.Checked == true)
                    {
                        MessageBox.Show("Finish!");
                    }

                }
                catch (Exception ex)
                {

                    this.Close();
                    Application.Exit();
                    driver.Close();
                    driver.Quit();
                    driver.Dispose();
                }

            });
        }
    }
}
