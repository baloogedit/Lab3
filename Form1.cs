using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3
{
    public partial class Form1 : Form
    {

        //trace listener enabled(?)
        private BooleanSwitch traceSwitch = new BooleanSwitch("TraceSwitch", "Trace switch enabled");

        //log file path
        private string logFile = "log.txt";

        // list of blocked keywords
        private List<string> blockedKeywords = new List<string>
        {
            "facebook","twitter","umfst","youtube","instagram", "tiktok"
        };
        
        public Form1()
        {
            InitializeComponent();

            // clear listeners
            Trace.Listeners.Clear();

            // add trace listener to log file
            Trace.Listeners.Add(new TextWriterTraceListener(logFile));

            //autoflush, enable
            Trace.AutoFlush = true;
            traceSwitch.Enabled = true;
            File.WriteAllText(logFile, ""); // clear log file at start
            Trace.WriteLineIf(traceSwitch.Enabled, $"Application started at {DateTime.Now}");

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
                    Trace.WriteLineIf(traceSwitch.Enabled, $"Inserted https:// to URL: {url} at {DateTime.Now}");
                }
                webBrowser1.Navigate(url);

            }
            Trace.WriteLineIf(traceSwitch.Enabled, $"URL entered by clicking Go: {txtURL.Text} at {DateTime.Now}");
            txtURL.Clear();

        }

        private void txtURL_Click(object sender, EventArgs e)
        {

        }

        private void txtURL_KeyDown(object sender, KeyEventArgs e)
        {
            string url = txtURL.Text;
            if (e.KeyCode == Keys.Enter)
            {
                // go to URL
                webBrowser1.Navigate(url);
            }
            Trace.WriteLineIf (traceSwitch.Enabled, $"URL entered by clicking Enter: {txtURL.Text} at {DateTime.Now}");
            txtURL.Clear();
        }

        // check if URL has blocked keywords
        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString().ToLower();

            /*
            // for every keyword in the blocked list
            foreach (var keyword in blockedKeywords)
            {
                // if contains
                if (url.Contains(keyword))
                {
                    //stop navigation
                    e.Cancel = true; 
                    MessageBox.Show(
                        $"Access to sites containing \"{keyword}\" is blocked.",
                        "Blocked Site",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }
            }
            */

            // searck blocked keywords with LINQ
            var foundKeyword = (from keyword in blockedKeywords
                                where url.Contains(keyword)
                                select keyword).FirstOrDefault();
            // if found
            if (foundKeyword != null)
            {
                //stop navigation and show message box
                e.Cancel = true;
                MessageBox.Show(
                    $"Access to sites containing \"{foundKeyword}\" is blocked.",
                    "Blocked Site",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                // log to file
                Trace.WriteLineIf(traceSwitch.Enabled, $"Blocked navigation to {url} at {DateTime.Now} due to keyword \"{foundKeyword}\"");
                return;

                
            }
        }

        private void toolStripBtnAddKeyword_Click(object sender, EventArgs e)
        {
            string newKeyword = toolStriptxtKeyword.Text;

            if (!string.IsNullOrEmpty(newKeyword))
            {
                // LINQ check for keyword
                var existingKeyword =
                     (from k in blockedKeywords
                      where k.Equals(newKeyword, StringComparison.OrdinalIgnoreCase)
                      select k).FirstOrDefault();

                if (existingKeyword == null)
                {
                    blockedKeywords.Add(newKeyword);

                    // log the addition
                    Trace.WriteLineIf(traceSwitch.Enabled, $"Keyword added: {newKeyword} at {DateTime.Now}");

                    // show updated list
                    MessageBox.Show(
                        $"Keyword \"{newKeyword}\" added to the blocked list.\n\n" +
                        $"Current blocked keywords:\n{string.Join(", ", blockedKeywords)}",
                        "Keyword Added",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                else
                {
                    MessageBox.Show(
                        $"Keyword \"{existingKeyword}\" is already blocked.",
                        "Duplicate Keyword",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            else
            {
                MessageBox.Show(
                    "Please enter a keyword to block.",
                    "Empty Input",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }


            toolStriptxtKeyword.Clear();
        }


    }
}
