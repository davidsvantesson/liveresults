using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
#if _CASPARCG_
using LiveResults.CasparClient;
#endif
using LiveResults.Client.Parsers;
using LiveResults.Model;
using System.Configuration;
using System.Data.SQLite;

namespace LiveResults.Client
{
    public partial class FrmNewCompetition : Form
    {
        public FrmNewCompetition()
        {
            InitializeComponent();

            if (ConfigurationManager.AppSettings["calculateTotals"] != "true")
            {
                OeventInfo.Text = "Beräkning av totalresultat inte aktiverat";
            }
            else if (ConfigurationManager.AppSettings["totalDatabase"] == null)
            {
                OeventInfo.Text = "Databas inte konfigurerard för totalresultat!";
            }
            else if (!File.Exists(ConfigurationManager.AppSettings["totalDatabase"]))
            {
                OeventInfo.Text = "Databas för totalresultat finns inte. Klicka på Skapa Ny ovan";
            }
            else
            {
                string totaldb = ConfigurationManager.AppSettings["totalDatabase"];
                string totalConnStr = "DataSource=" + totaldb + ";";
                SQLiteConnection conntemp = new SQLiteConnection(totalConnStr);
                conntemp.Open();
                SQLiteCommand cmd = conntemp.CreateCommand();
                cmd.CommandText = "SELECT etappnr FROM settings WHERE setting_id=1";
                SQLiteDataReader reader = cmd.ExecuteReader();
                reader.Read();
                int etappNr = Convert.ToInt32(reader["etappnr"]);
                reader.Close();
                conntemp.Close();

                OeventInfo.Text = "Totalresultat aktivt. Nuvarande etapp: " + etappNr;



            }

            if (ConfigurationManager.AppSettings["totalIgnoreClasses"] != null)
            {
                OeventInfo.Text += System.Environment.NewLine + "Totalresultat ignorerade klasser: ";
                bool first = true;
                List<string> totalIgnoredClasses = new List<string>(ConfigurationManager.AppSettings["totalIgnoreClasses"].Split(new char[] { ';' }));
                totalIgnoredClasses.ForEach(delegate (string ignoredClass)
                {
                    if (first) first = false;
                    else OeventInfo.Text += ", ";
                    OeventInfo.Text += ignoredClass;
                });
            }


            if (ConfigurationManager.AppSettings["runoffline"] == "true")
            {
                OeventInfo.Text += System.Environment.NewLine + "Offline mode aktivt";
            }
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnOLA_MouseHover(object sender, EventArgs e)
        {
            lblInfo.Text = "Export liveresults from SOFTs OLA-system";
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            lblInfo.Text = "Export liveresults by generic IOF-XML v2 and v3 (SportSoftware 2010, MeOS, Helga, AutoDownload,...)";
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            lblInfo.Text = "Export liveresults from SSF-Timing (BETA)";
        }
        private void button4_MouseEnter(object sender, EventArgs e)
        {
            lblInfo.Text = "Export liveresults from Sportsoftware OE/OS with CSV-format. Historic reasons, please use IOF XML V3 when possible";
        }

        private void buttonRacom_MouseEnter(object sender, EventArgs e)
        {
            lblInfo.Text = "Export liveresults from RaCom special fileset format";
        }

        private void btn_MouseLeave(object sender, EventArgs e)
        {
            lblInfo.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            lblInfo.Text = "Export liveresults from OL-Staffel automatic CSV-export";
        }

        private void btnOLA_Click(object sender, EventArgs e)
        {
            NewOLAComp cmp = new NewOLAComp();
            cmp.ShowDialog(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OEForm frm = new OEForm(false);
            frm.ShowDialog();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            FrmMonitor monForm = new FrmMonitor();
            string[] lines = File.ReadAllLines("wocinfo.txt");
            int compId = int.Parse(lines[0]);
            List<string> urls = new List<string>();
            for (int i = 1; i < lines.Length; i++)
                urls.Add(lines[i]);
            /*WocParser wp = new WocParser(new string[] {
                "http://www.woc2012.ch/results/live/" + comp + "-women-heat-a.json",
                "http://www.woc2012.ch/results/live/" + comp + "-women-heat-b.json",
                "http://www.woc2012.ch/results/live/" + comp + "-women-heat-c.json",
            "http://www.woc2012.ch/results/live/" + comp + "-men-heat-a.json",
                "http://www.woc2012.ch/results/live/" + comp + "-men-heat-b.json",
                "http://www.woc2012.ch/results/live/" + comp + "-men-heat-c.json"});*/
            WocParser wp = new WocParser(urls.ToArray());
            monForm.SetParser(wp as IExternalSystemResultParser);
            monForm.CompetitionID = compId;
            monForm.ShowDialog(this);
        }



        private void button1_Click_2(object sender, EventArgs e)
        {
            FrmMonitor monForm = new FrmMonitor();
            //string[] lines = File.ReadAllLines("wocinfo.txt");
            //int compId = int.Parse(lines[0]);
            int compId = -127;
            List<string> urls = new List<string>();
            /*for (int i = 1; i < lines.Length; i++)
                urls.Add(lines[i]);*/
            urls.Add("http://www.ori-live.com/special/go-live-eoc2014-6/EOC2014-6-Men%20A.html");
            urls.Add("http://www.ori-live.com/special/go-live-eoc2014-6/EOC2014-6-Woman%20A.html");

            OriLiveParser wp = new OriLiveParser(urls.ToArray());
            monForm.SetParser(wp as IExternalSystemResultParser);
            monForm.CompetitionID = compId;
            monForm.ShowDialog(this);
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            FrmNewRacomComp newRacomComp = new FrmNewRacomComp();
            if (newRacomComp.ShowDialog(this) == DialogResult.OK)
            {
                FrmMonitor monForm = new FrmMonitor();
                monForm.SetParser(new RacomFileSetParser(newRacomComp.txtStartlist.Text, newRacomComp.txtRawSplits.Text, newRacomComp.txtRaceFile.Text,
                    newRacomComp.txtDSQFile.Text, newRacomComp.txtRadioControls.Text, newRacomComp.dtZeroTime.Value, newRacomComp.checkBox1.Checked));
                monForm.CompetitionID = int.Parse(newRacomComp.txtCompID.Text);
                monForm.ShowDialog(this);
            }
            //FrmMonitor monForm = new FrmMonitor();
            ////string[] lines = File.ReadAllLines("wocinfo.txt");
            ////int compId = int.Parse(lines[0]);
            //int compId = -50;
            //List<string> urls = new List<string>();
            ///*for (int i = 1; i < lines.Length; i++)
            //    urls.Add(lines[i]);*/
            //urls.Add("http://www.liveresultater.no/includes/individuell/orientering/lister/allelister.php?q=alle&w=alle&s=DESC&c=radiopost.tid&a=439&acc=sek&lang=en&sid=0.3296072955708951");

            //var wp = new LiveResultaterNoParser(urls.ToArray());
            //monForm.SetParser(wp as IExternalSystemResultParser);
            //monForm.CompetitionID = compId;
            //monForm.ShowDialog(this);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            NewSSFTimingComp cmp = new NewSSFTimingComp();
            cmp.ShowDialog(this);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OEForm frm = new OEForm();
            frm.ShowDialog();
        }
#if _CASPARCG_
        private void label2_Click(object sender, EventArgs e)
        {
            CasparClient.CasparControlFrm frm = new CasparControlFrm();
            frm.Show();
        }
#endif

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void FrmNewCompetition_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                var parser = new RaceTimerParser(new string[]
                {
                    "http://www.racetimer.se/sv/race/resultlist/3647?race_id=3646&layout=racetimer&rc_id=14200&per_page=2500&commit=Visa+resultat+%3E%3E",
                    "http://www.racetimer.se/sv/race/resultlist/3647?race_id=3646&layout=racetimer&rc_id=14417&per_page=2500&commit=Visa+resultat+%3E%3E",
                    "http://www.racetimer.se/sv/race/resultlist/3647?race_id=3646&layout=racetimer&rc_id=14418&per_page=2005&commit=Visa+resultat+%3E%3E",
                    "http://www.racetimer.se/sv/race/resultlist/3647?race_id=3646&layout=racetimer&rc_id=14419&per_page=2500&commit=Visa+resultat+%3E%3E"

                });
                var mon = new FrmMonitor();
                mon.CompetitionID = -112;
                mon.SetParser(parser);
                mon.ShowDialog(this);
            }

            if (e.KeyCode == Keys.W)
            {
                var parser = new TulospalveluParser(new string[]
                {
                    "http://online4.tulospalvelu.fi/tulokset/en/2017_yllaslong/men/smart/1/",
                    "http://online4.tulospalvelu.fi/tulokset/en/2017_yllaslong/women/smart/1/"


                });
                var mon = new FrmMonitor();
                mon.CompetitionID = 13187;
                mon.SetParser(parser);
                mon.ShowDialog(this);
            }
            if (e.KeyCode == Keys.S)
            {
                var frm = new FrmReSimulateEvent();
                frm.ShowDialog(this);
            }

        }

        private void FrmNewCompetition_KeyPress(object sender, KeyPressEventArgs e)
        {


        }

        private void button5_MouseEnter(object sender, EventArgs e)
        {
            lblInfo.Text = "Export liveresults from MeOS";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            NewMeosComp frm = new NewMeosComp();
            frm.ShowDialog(this);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            NewTotalResultsComp frm = new NewTotalResultsComp();
            frm.ShowDialog(this);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Create db
            // Assumes it does not exist
            string totaldb = ConfigurationManager.AppSettings["totalDatabase"];
            if (totaldb == null) return;
            SQLiteConnection m_connection;
            string m_totalConnStr = "DataSource=" + totaldb + ";";
            m_connection = new SQLiteConnection(m_totalConnStr);
            m_connection.Open();
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "CREATE TABLE \"etappresults\" ( `idrunners` INTEGER NOT NULL, `etappnr` INTEGER NOT NULL, `etapptid` INTEGER DEFAULT NULL, `totaltid` INTEGER DEFAULT NULL, `etappstatus` INTEGER DEFAULT NULL, `totalstatus` INTEGER DEFAULT NULL, `etappstarttime` INTEGER DEFAULT NULL, `predictionstarttime` INTEGER DEFAULT NULL, `changed` DATETIME DEFAULT(STRFTIME('%Y-%m-%d %H:%M:%f', 'NOW')), PRIMARY KEY(`idrunners`,`etappnr`) );" +
                "CREATE TABLE `runners` ( `idrunners` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `name` TEXT, `club` TEXT, `class` TEXT, `changed` DATETIME DEFAULT(STRFTIME('%Y-%m-%d %H:%M:%f', 'NOW')) );" +
                "CREATE TABLE \"settings\" ( `setting_id` INTEGER NOT NULL CHECK(setting_id=1), `etappnr` INTEGER NOT NULL, `startreadfromtime` DATETIME NOT NULL DEFAULT '2000-01-01 00:00:00', PRIMARY KEY(`setting_id`) );" +
                "INSERT INTO settings (etappnr) VALUES (1);";
            cmd.ExecuteNonQuery();

            m_connection.Close();

            OeventInfo.Text += System.Environment.NewLine + "Databas för totalresultat skapat (Etapp 1 aktivt)";

        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Set next etapp
            string totaldb = ConfigurationManager.AppSettings["totalDatabase"];
            if (totaldb == null) return;
            SQLiteConnection m_connection;
            string m_totalConnStr = "DataSource=" + totaldb + ";";
            m_connection = new SQLiteConnection(m_totalConnStr);
            m_connection.Open();
            SQLiteCommand cmd = m_connection.CreateCommand();

            // Hämta nuvarande etappnr
            cmd.CommandText = "SELECT etappnr FROM settings WHERE setting_id=1";
            SQLiteDataReader reader = cmd.ExecuteReader();
            reader.Read();
            int etappnr = Convert.ToInt32(reader["etappnr"]);
            int nextetappnr = etappnr + 1;
            reader.Close();

            // Skapa nya resultatrader för nextetappnr (default Ej Start)
            cmd.CommandText = "INSERT INTO etappresults (idrunners, etappnr, etapptid, totaltid, etappstatus, totalstatus, etappstarttime, predictionstarttime) SELECT idrunners, " + nextetappnr + ", 0, 0, 1, 1, 0, 0 FROM etappresults WHERE etappnr = " + etappnr;
            cmd.ExecuteNonQuery();

            cmd.CommandText = "UPDATE settings SET etappnr=" + nextetappnr + " WHERE setting_id=1";
            cmd.ExecuteNonQuery();

            m_connection.Close();

            OeventInfo.Text += System.Environment.NewLine + "Etapp nr " + nextetappnr + " aktiverat.";

        }

        private void button9_Click(object sender, EventArgs e)
        {
            PrintTotalResults frm = new PrintTotalResults();
            frm.ShowDialog();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string totaldb = ConfigurationManager.AppSettings["totalDatabase"];
            if (totaldb == null) return;
            SQLiteConnection m_connection;
            string m_totalConnStr = "DataSource=" + totaldb + ";";
            m_connection = new SQLiteConnection(m_totalConnStr);
            m_connection.Open();
            SQLiteCommand cmd = m_connection.CreateCommand();
            cmd.CommandText = "DELETE FROM etappresults";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "UPDATE settings SET etappnr=1 WHERE setting_id=1";
            cmd.ExecuteNonQuery();

            m_connection.Close();

            OeventInfo.Text += System.Environment.NewLine + "Resultat raderade. Etapp nr 1 aktiverat.";
        }
    }
}