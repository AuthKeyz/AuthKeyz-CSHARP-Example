using Loader;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace AuthKeyz
{
    public partial class Main : Form
    {
        /*
        * 
        * WATCH THIS VIDEO TO SETUP APPLICATION: https://www.youtube.com/watch?v=RfDTdiBq4_o
        * 
	     * READ HERE TO LEARN ABOUT AUTHKEYZ FUNCTIONS https://github.com/AuthKeyz/AuthKeyz-CSHARP-Example#authkeyzapp-instance-definition
		 *
        */

        string chatchannel = "test"; // chat channel name, must be set in order to send/retrieve messages


        public Main()
        {
            InitializeComponent();
            Drag.MakeDraggable(this);
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            userDataField.Items.Add($"Username: {Login.AuthKeyzApp.user_data.username}");
            userDataField.Items.Add($"License: {Login.AuthKeyzApp.user_data.subscriptions[0].key}"); // this can be used if the user used a license, username, and password for register. It'll display the license assigned to the user
            userDataField.Items.Add($"Expires: {Login.AuthKeyzApp.user_data.subscriptions[0].expiration}"); // this has been changed from expiry to expiration
            userDataField.Items.Add($"Subscription: {Login.AuthKeyzApp.user_data.subscriptions[0].subscription}");
            userDataField.Items.Add($"IP: {Login.AuthKeyzApp.user_data.ip}");
            userDataField.Items.Add($"HWID: {Login.AuthKeyzApp.user_data.hwid}");
            userDataField.Items.Add($"Creation Date: {Login.AuthKeyzApp.user_data.CreationDate}"); // this has a capital "C" , if you use a lowercase "c" it won't convert unix
            userDataField.Items.Add($"Last Login: {Login.AuthKeyzApp.user_data.LastLoginDate}"); // this has a capital "L", if you use a lowercase "l" it won't convert unix
            userDataField.Items.Add($"Time Left: {Login.AuthKeyzApp.expirydaysleft()}");

            var onlineUsers = await Login.AuthKeyzApp.fetchOnline();
            if (onlineUsers != null)
            {
                Console.Write("\n Online users: ");
                foreach (var user in onlineUsers)
                {
                    onlineUsersField.Items.Add(user.credential + ", ");
                }
                Console.WriteLine("\n");
            }
        }

        public static bool SubExist(string name, int len)
        {
            for (var i = 0; i < len; i++)
            {
                if (Login.AuthKeyzApp.user_data.subscriptions[i].subscription == name)
                {
                    return true;
                }
            }
            return false;
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            chatroomGrid.Rows.Clear();
            timer1.Interval = 15000; // get chat messages every 15 seconds
            if (!String.IsNullOrEmpty(chatchannel))
            {
                var messages = await Login.AuthKeyzApp.chatget(chatchannel);
                if (messages == null)
                {
                    chatroomGrid.Rows.Insert(0, "AuthKeyz", "No Chat Messages", api.UnixTimeToDateTime(DateTimeOffset.Now.ToUnixTimeSeconds()));
                }
                else
                {
                    foreach (var message in messages)
                    {
                        chatroomGrid.Rows.Insert(0, message.author, message.message, api.UnixTimeToDateTime(long.Parse(message.timestamp)));
                    }
                }
            }
            else
            {
                timer1.Stop();
                chatroomGrid.Rows.Insert(0, "AuthKeyz", "No Chat Messages", api.UnixTimeToDateTime(DateTimeOffset.Now.ToUnixTimeSeconds()));
            }
        }

        private async void sendWebhookBtn_Click_1(object sender, EventArgs e)
        {
            await Login.AuthKeyzApp.webhook(webhookID.Text, webhookBaseURL.Text);
            MessageBox.Show(Login.AuthKeyzApp.response.message);
        }

        private async void setUserVarBtn_Click_1(object sender, EventArgs e)
        {
            await Login.AuthKeyzApp.setvar(varField.Text, varDataField.Text);
            MessageBox.Show(Login.AuthKeyzApp.response.message);
        }

        private async void fetchUserVarBtn_Click_1(object sender, EventArgs e)
        {
            await Login.AuthKeyzApp.getvar(varField.Text);
            MessageBox.Show(Login.AuthKeyzApp.response.message);
        }

        private async void sendLogDataBtn_Click(object sender, EventArgs e)
        {
            await Login.AuthKeyzApp.log(logDataField.Text);
            MessageBox.Show(Login.AuthKeyzApp.response.message);
        }

        private async void checkSessionBtn_Click_1(object sender, EventArgs e)
        {
            await Login.AuthKeyzApp.check();
            MessageBox.Show(Login.AuthKeyzApp.response.message);
        }

        private async void fetchGlobalVariableBtn_Click_1(object sender, EventArgs e)
        {
            string globalVal = await Login.AuthKeyzApp.var(globalVariableField.Text);
            MessageBox.Show(globalVal);
            MessageBox.Show(Login.AuthKeyzApp.response.message); // optional since it'll show the response in the var (if it's valid or not)
        }

        private async void sendMsgBtn_Click_1(object sender, EventArgs e)
        {
            if (await Login.AuthKeyzApp.chatsend(chatMsgField.Text, chatchannel))
            {
                chatroomGrid.Rows.Insert(0, Login.AuthKeyzApp.user_data.username, chatMsgField.Text, api.UnixTimeToDateTime(DateTimeOffset.Now.ToUnixTimeSeconds()));
            }
            else
            {
                MessageBox.Show(Login.AuthKeyzApp.response.message);
            }
        }

        private async void closeBtn_Click(object sender, EventArgs e)
        {
            await Login.AuthKeyzApp.logout(); // ends the sessions once the application closes
            Environment.Exit(0);
        }

        private void minBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private async void downloadFileBtn_Click(object sender, EventArgs e)
        {
            byte[] result = await Login.AuthKeyzApp.download("");
            if (!Login.AuthKeyzApp.response.success)
            {
                Console.WriteLine("\n Status: " + Login.AuthKeyzApp.response.message);
                Thread.Sleep(2500);
                Environment.Exit(0);
            }
            else
                File.WriteAllBytes($@"{filePathField.Text}" + $"\\{fileExtensionField.Text}", result);
        }

        private async void enableTfaBtn_Click(object sender, EventArgs e)
        {
            string code = string.IsNullOrEmpty(tfaField.Text) ? null : tfaField.Text;

            await Login.AuthKeyzApp.enable2fa(code);

            MessageBox.Show(Login.AuthKeyzApp.response.message);
        }

        private async void disableTfaBtn_Click(object sender, EventArgs e)
        {
            await Login.AuthKeyzApp.disable2fa(tfaField.Text);
            MessageBox.Show(Login.AuthKeyzApp.response.message);
        }

        private async void banBtn_Click(object sender, EventArgs e)
        {
            await Login.AuthKeyzApp.ban("Testing ban function");
            MessageBox.Show(Login.AuthKeyzApp.response.message);
        }
    }
}