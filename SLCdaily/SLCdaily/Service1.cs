using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Net.Mail;
using MySql.Data.MySqlClient;

namespace SLCdaily
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.WriteToFile("Simple Service started {0}");
            this.ScheduleService();

        }

        protected override void OnStop()
        {
            this.Schedular.Dispose();
        }

        private Timer Schedular;

        public void ScheduleService()
        {
            try
            {
                Schedular = new Timer(new TimerCallback(SchedularCallback));
                string mode = ConfigurationManager.AppSettings["Mode"].ToUpper();
                this.WriteToFile("Simple Service Mode: " + mode + " {0}");

                DateTime scheduledTime = DateTime.MinValue;

                if(mode == "DAILY")
                {
                    scheduledTime = DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["ScheduledTime"]);
                    if(DateTime.Now > scheduledTime)
                    {
                        scheduledTime = scheduledTime.AddDays(1);
                    }
                }

                if(mode.ToUpper() == "INTERVAL")
                {
                    int intervalMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalMinutes"]);
                    scheduledTime = DateTime.Now.AddMinutes(intervalMinutes);
                    if(DateTime.Now > scheduledTime)
                    {
                        scheduledTime = scheduledTime.AddMinutes(intervalMinutes);
                    }
                }

                TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                string schedule = string.Format("{0} day(s) {1} hours(s) {2} minute(s) {3} second(s)",
                    timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

                this.WriteToFile("Simple Service scheduled to run after: " + schedule + " {0}");

                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                Schedular.Change(dueTime, Timeout.Infinite);
            }
            catch(Exception e)
            {
                WriteToFile("Schedule Simple Service Error on: {0} " + e.Message + e.StackTrace);

                using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController("Simple Service"))
                {
                    serviceController.Stop();
                }
            }
        }

        private void SchedularCallback(object e)
        {
            try
            {
                
                DataTable dt = new DataTable();
                string query = "SELECT name, email FROM users";
                string mysql = ConfigurationManager.ConnectionStrings["MYSQL"].ConnectionString;
                using (MySqlConnection con = new MySqlConnection(mysql))
                {
                    using (MySqlCommand cmd = new MySqlCommand(query))
                    {
                        cmd.Connection = con;
                        using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                        {
                            sda.Fill(dt);
                        } 
                    }
                }

                foreach(DataRow row in dt.Rows)
                { 
                
                    string name = row["name"].ToString();
                    string email = row["email"].ToString(); 

                    WriteToFile("Trying to send email to: " + name + " " + email);

                    using(MailMessage mm = new MailMessage("jamh927@gmail.com", email))
                    {
                        mm.Subject = "This is a test";
                        mm.Body = string.Format("<b>Hello </b>{0}.", name);
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
                        credentials.UserName = "jamh927@gmail.com";
                        credentials.Password = "A1b2c3d4";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = credentials;
                        smtp.Port = 587;
                        smtp.Send(mm);
                        WriteToFile("Email sent successfully to: " + name + " " + email);
                    }
         
                }
                this.ScheduleService();
            }
            catch (Exception ex)
            {
                WriteToFile("Email Simple Service Error on: {0} " + ex.Message + ex.StackTrace);

                using (System.ServiceProcess.ServiceController serviceController = new System.ServiceProcess.ServiceController("SimpleServce"))
                {
                    serviceController.Stop();
                }
            }

        }

        private void WriteToFile(string text)
        {
            string path = "C:\\ServiceLog.txt";
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(string.Format(text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                writer.Close();
            }
        }
    }
}
