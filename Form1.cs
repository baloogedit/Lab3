using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3
{
    public partial class Form1 : Form
    {

        // list of blocked keywords
        private List<string> blockedKeywords = new List<string>
        {
            "facebook","twitter","umfst","youtube","instagram", "tiktok"
        };

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate("https://www.google.com"); //default home page
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://www.google.com");
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtURL.Text))
            {
                string url = txtURL.Text;
                if (!url.StartsWith("http"))
                {
                    url = "https://" + url;
                }
                webBrowser1.Navigate(url);
            }
        }

        private void txtURL_Click(object sender, EventArgs e)
        {

        }

        private void txtURL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // prevent the "ding" sound
                e.SuppressKeyPress = true;

                // simulate clicking the Go button
                goButton.PerformClick();
            }
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString().ToLower();

            foreach (var keyword in blockedKeywords)
            {
                if (url.Contains(keyword))
                {
                    e.Cancel = true; // stop navigation
                    MessageBox.Show(
                        $"Access to sites containing \"{keyword}\" is blocked.",
                        "Blocked Site",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
            }
        }
    }
}
