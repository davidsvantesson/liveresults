using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SQLite;
using System.IO;

namespace LiveResults.Client
{
    public partial class PrintTotalResults : Form
    {
        public PrintTotalResults()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (separateClasses.Checked)
            {
                string totaldb = ConfigurationManager.AppSettings["totalDatabase"];
                if (totaldb == null) return;
                SQLiteConnection m_connection;
                string m_totalConnStr = "DataSource=" + totaldb + ";";
                m_connection = new SQLiteConnection(m_totalConnStr);
                m_connection.Open();
                SQLiteCommand cmd = m_connection.CreateCommand();
                cmd.CommandText = "SELECT distinct class FROM runners";
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    exportToHtml(totalname.Text, Convert.ToInt32(nrStages.Text), reader["class"].ToString());
                }
            }
            else {
                exportToHtml(totalname.Text, Convert.ToInt32(nrStages.Text));
            }
            
        }

        public void exportToHtml(string name, int etappnr, string exportClass = "")
        {
            string totaldb = ConfigurationManager.AppSettings["totalDatabase"];
            if (totaldb == null) return;
            SQLiteConnection m_connection;
            string m_totalConnStr = "DataSource=" + totaldb + ";";
            m_connection = new SQLiteConnection(m_totalConnStr);
            m_connection.Open();
            SQLiteCommand cmd = m_connection.CreateCommand();

            string cmdt = "SELECT class, name, club, totaltid, totalstatus FROM etappresults, runners WHERE etappresults.idrunners=runners.idrunners AND etappnr=" + etappnr;
            if (exportClass != "") cmdt += " AND class like '" + exportClass + "'";
            cmdt += " ORDER BY class, totalstatus ASC, totaltid ASC, etapptid DESC"; // If equal total time, sort is done on best total time stage before
            cmd.CommandText = cmdt;
            SQLiteDataReader reader = cmd.ExecuteReader();

            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string filename = "Total";
            if (exportClass != "") filename += "_" + exportClass;
            filename += ".html";
            string filepath = Path.Combine(dir,filename);

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(filepath, false))
            {
                file.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">");
                file.WriteLine("<HTML>");
                file.WriteLine("<HEAD>");
                file.WriteLine("<META http-equiv=\"Content-Type\" content=\"text/html; charset= utf-8\"><link href=\"default.css\" rel=\"stylesheet\" type=\"text/css\" /><TITLE>"+name+ " Totalresultat</TITLE>");
                file.WriteLine("</HEAD><BODY>");
                file.WriteLine("<H1>"+name+", Totalresultat</H1>");
                file.WriteLine("<H2>"+ DateTime.Today.ToString("yyyy-MM-dd") +" </H2>");
                string classn = "";
                int pl = 0;
                string resultatrad;
                string totaltidstring;
                int totaltid;
                int totaltidsegrare = 0;
                int difftid;
                string difftidstring;
                bool validresult = true;

                while (reader.Read())
                {
                    if (reader["class"].ToString() != classn)
                    {
                        // Ny klass
                        if (classn != "")
                        {
                            file.WriteLine("</TABLE>");
                        }

                        classn = reader["class"].ToString();
                        file.WriteLine("<BR><TABLE class=\"klassTabell\"><tr><td class=\"klassRad\">" + classn + "</td><td class=\"klassRad\"></td><td class=\"klassRad\"></td><td class=\"klassRad\"></td><td class=\"klassRad\"></td><td class=\"klassRad\"></td></tr>");

                        file.WriteLine("<TABLE></TABLE>");
                        file.WriteLine("<TABLE>");

                        pl = 0;
                        totaltidsegrare = Convert.ToInt32(reader["totaltid"]);
                    }

                    totaltid = Convert.ToInt32(reader["totaltid"]);
                    difftid = totaltid - totaltidsegrare;
                    totaltidstring = (totaltid / 6000) + ":" + ((totaltid % 6000) / 100).ToString("D2");
                    difftidstring = "+" + (difftid / 6000) + ":" + ((difftid % 6000) / 100).ToString("D2");

                    switch (Convert.ToInt32(reader["totalstatus"]))
                    {
                        case 0:
                            validresult = true;
                            break;
                        case 1:
                            continue; // Skriv inte ut Ej start
                                      //totaltidstring = "Ej start";
                                      //validresult = false;
                                      //break;
                        case 3:
                            totaltidstring = "Ej godkänd";
                            validresult = false;
                            break;
                        case 4:
                            totaltidstring = "Diskvalificerad";
                            validresult = false;
                            break;
                        default:
                            continue; // Skriv inte ut okända
                            //totaltidstring = "Okänd status";
                            //validresult = false;
                            //break;
                    }
                    pl++;

                    if ((pl % 2) == 0)
                    {
                        resultatrad = "<tr class=trDark>";
                    }
                    else
                    {
                        resultatrad = "<tr class=trLight>";
                    }
                    if (validresult)
                    {
                        resultatrad += "<td>" + pl + "</td>";
                    }
                    else
                    {
                        resultatrad += "<td>-</td>";
                    }
                    resultatrad += "<td></td><td>" + reader["name"].ToString() + "</td><td>" + reader["club"].ToString() + "</td><td>" + totaltidstring + "</td>";
                    if (validresult)
                    {
                        resultatrad += "<td>" + difftidstring + "</td>";
                    }
                    else
                    {
                        resultatrad += "<td></td>";
                    }
                    resultatrad += "</tr>";


                    file.WriteLine(resultatrad);

                }

                file.WriteLine("</TABLE>");
                file.WriteLine("<BR><FONT size=\"-1\"><I>Skapad " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</I></FONT>");
                file.WriteLine("</BODY></ HTML >");
            }

            reader.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void totalname_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
