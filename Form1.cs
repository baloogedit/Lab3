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
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate("https://www.bing.com"); //default home page
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            webBrowser1.GoHome();
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

    }
}
