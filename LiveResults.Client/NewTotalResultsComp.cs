using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.H2;
using System.IO;
using System.Xml.Serialization;
using LiveResults.Model;
using System.Configuration;
using System.Data.SQLite;

namespace LiveResults.Client
{
    public partial class NewTotalResultsComp : Form
    {
        public NewTotalResultsComp()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmMonitor monForm = new FrmMonitor(true);
            this.Hide();

            string totalConnStr;
            SQLiteConnection totalConnection;
            string totaldb = ConfigurationManager.AppSettings["totalDatabase"];
            totalConnStr = "DataSource=" + totaldb + ";";
            totalConnection = new SQLiteConnection(totalConnStr);

            TotalParser pars = new TotalParser(totalConnection, Convert.ToInt32(nrStages.Text));
            monForm.SetParser(pars as IExternalSystemResultParser);
            monForm.CompetitionID = Convert.ToInt32(txtCompID.Text);
            monForm.ShowDialog(this);
        }
    }
}
