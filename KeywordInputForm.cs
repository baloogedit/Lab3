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
    public partial class KeywordInputForm : Form
    {
        public KeywordInputForm()
        {
            InitializeComponent();
        }

        // Public property to get the text (better than getInputText())
        public string KeywordText
        {
            get { return txtKeyword.Text; }
        }

        // Set DialogResult for OK button
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtKeyword.Text))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a keyword.");
            }
        }

        // Set DialogResult for Cancel button
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        
    }
}
