using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EmailAlertService
{
    public partial class EmailAlertService : ServiceBase
    {
        public EmailAlertService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (IsConnectionAvailable() && IsInternetConnectionAvailable())
                SendEmail().Wait();
        }

        protected override void OnStop()
        {
        }

        static async Task SendEmail()
        {
            string apiKey = @"your_SendGrid_Key";
            dynamic sg = new SendGridAPIClient(apiKey);

            Email from = new Email("home@localhost");
            string subject = "PC is ready";
            Email to = new Email("email@address.com");
            Content content = new Content("text/plain", $"PC was turned on at {DateTime.UtcNow}");
            Mail mail = new Mail(from, subject, to, content);

            dynamic response = await sg.client.mail.send.post(requestBody: mail.Get());
        }
        
        private static bool IsConnectionAvailable(int retry = 0)
        {
            if (retry == 3) return false;

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                Thread.Sleep(2000);
                retry++;
                IsConnectionAvailable(retry);
            }

            return true;
        }

        private static bool IsInternetConnectionAvailable()
        {
            var pingReply = new Ping().Send("google.com", 500);

            return pingReply != null && pingReply.Status == IPStatus.Success;
        }
    }
}
