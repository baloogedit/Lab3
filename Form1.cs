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
using System.Data.SQLite;

namespace Lab3
{
    public partial class Form1 : Form
    {

        //trace listener enabled(?)
        private BooleanSwitch traceSwitch = new BooleanSwitch("TraceSwitch", "Trace switch enabled");

        //log file path
        private string logFile = "log.txt";

        private SQLiteHandler dbHandler;

        // new list
        private List<string> blockedKeywords = new List<string>();

        //old list       
        //private List<string> blockedKeywords = new List<string> {"facebook","twitter","umfst","youtube","instagram", "tiktok"};
        
        public Form1()
        {
            InitializeComponent();
            dbHandler = new SQLiteHandler();

            // clear listeners
            Trace.Listeners.Clear();

            // add trace listener to log file
            Trace.Listeners.Add(new TextWriterTraceListener(logFile));

            //autoflush, enable
            Trace.AutoFlush = true;
            traceSwitch.Enabled = true;

            // clear log file at start
            File.WriteAllText(logFile, "");
            _ = LogTraceAsync("Application started");

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // connect and load keywords
            dbHandler.ConnectToDb();
            blockedKeywords = dbHandler.GetAllKeywords();

            // clear and reload the ComboBox
            toolStripComboBoxBlocked.Items.Clear();
            toolStripComboBoxBlocked.Items.AddRange(blockedKeywords.ToArray());

            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate("https://www.google.com"); //default home page

            // not needed anymore
            //toolStripComboBoxBlocked.Items.AddRange(blockedKeywords.ToArray());
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
                    _ = LogTraceAsync($"Inserted https:// to URL: {url}");
                }
                webBrowser1.Navigate(url);

            }
            _ = LogTraceAsync($"URL entered by clicking Go: {txtURL.Text}");
            txtURL.Clear();

        }

        private void txtURL_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string url = txtURL.Text;
                // go to URL
                webBrowser1.Navigate(url);


                _ = LogTraceAsync($"URL entered by clicking Enter: {txtURL.Text}");
                //only clear after navigation attempt-the correction mentioned in git commmit
                txtURL.Clear();
            }
        }

        // check if URL has blocked keywords
        private async void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString().ToLower();

            /*
            // foreach seach: every keyword in the blocked list

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
            string foundKeyword = await Task.Run(() =>
            {
                // This LINQ query now runs on a background thread
                return (from keyword in blockedKeywords
                        where url.Contains(keyword)
                        select keyword).FirstOrDefault();
            });

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
                _ = LogTraceAsync($"Blocked navigation to {url} due to keyword \"{foundKeyword}\"");
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
                      where k.Equals(newKeyword)
                      select k).FirstOrDefault();

                if (existingKeyword == null)
                {
                    dbHandler.InsertKeyword(newKeyword); // Save to database

                    //add to list
                    blockedKeywords.Add(newKeyword);

                    //add to combobox
                    toolStripComboBoxBlocked.Items.Add(newKeyword);


                    // log the addition
                    _ = LogTraceAsync($"Keyword added: {newKeyword}");

                    // show confirmation
                    MessageBox.Show(
                        $"Keyword \"{newKeyword}\" added to the blocked list.\n\n",
                        "Keyword Added",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                else
                {
                    _ = LogTraceAsync($"Tried adding duplicate keyword: {newKeyword}");

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

                _ = LogTraceAsync($"Empty keyword add");

                MessageBox.Show(
                    "Please enter a keyword to block.",
                    "Empty Input",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }


            toolStriptxtKeyword.Clear();
        }


        private async Task LogTraceAsync(string message)
        {
            // Task.Run moves the work to a background thread.
            await Task.Run(() =>
            {
                // This code now runs on a different thread,
                // so it won't block the UI.
                Trace.WriteLineIf(traceSwitch.Enabled, $"{message} at {DateTime.Now}");
            });
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dbHandler.ConnectToDb();
            // Reload keywords from DB
            blockedKeywords = dbHandler.GetAllKeywords();
            toolStripComboBoxBlocked.Items.Clear();
            toolStripComboBoxBlocked.Items.AddRange(blockedKeywords.ToArray());
            MessageBox.Show("Connected to database.");
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dbHandler.DisconnectFromDb();
            MessageBox.Show("Disconnected from database.");
        }

        private void addKeywordToolStripMenuItem_Click(object sender, EventArgs e)
        {
           //Use the new KeywordInputForm
            using (KeywordInputForm kwdForm = new KeywordInputForm())
            {
                // Show as dialog
                if (kwdForm.ShowDialog() == DialogResult.OK) // [cite: 145]
                {
                    string newKeyword = kwdForm.KeywordText.ToLower(); // [cite: 280]

                    // LINQ check for duplicates
                    if (blockedKeywords.Contains(newKeyword))
                    {
                        MessageBox.Show($"Keyword \"{newKeyword}\" is already blocked.", "Duplicate Keyword");
                    }
                    else
                    {
                        // Add to DB
                        dbHandler.InsertKeyword(newKeyword);

                        // Add to local list and UI
                        blockedKeywords.Add(newKeyword);
                        toolStripComboBoxBlocked.Items.Add(newKeyword);
                        MessageBox.Show($"Keyword \"{newKeyword}\" added.", "Keyword Added");
                    }
                }
            }
        }

        private void viewKeywordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get all keywords from DB
            List<string> keywords = dbHandler.GetAllKeywords();
            string keywordList = string.Join("\n", keywords);
            MessageBox.Show(keywordList, "Blocked Keywords");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Lab 4 - Web Browser with SQLite", "About");
        }
    }
}
