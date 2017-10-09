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
using MySql.Data.MySqlClient;

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
            FrmMonitor monForm = new FrmMonitor();
            this.Hide();

            string totalConnStr;
            MySqlConnection totalConnection;
            string tserver = ConfigurationManager.AppSettings["totalServer"];
            string[] parts = tserver.Split(';');
            totalConnStr = "Database=" + parts[3] + ";Data Source=" + parts[0] + ";User Id=" + parts[1] + ";Password=" + parts[2];
            totalConnection = new MySqlConnection(totalConnStr);

            TotalParser pars = new TotalParser(totalConnection, Convert.ToInt32(nrStages.Text));
            monForm.SetParser(pars as IExternalSystemResultParser);
            monForm.CompetitionID = Convert.ToInt32(txtCompID.Text);
            monForm.ShowDialog(this);
        }
    }
}
