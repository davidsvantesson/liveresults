﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using LiveResults.Client.Model;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using LiveResults.Model;
namespace LiveResults.Client
{
    
    public class TotalParser : IExternalSystemResultParser
    {
        private readonly IDbConnection m_connection;
        private readonly int m_nrStages;
        public event ResultDelegate OnResult;
        public event LogMessageDelegate OnLogMessage;

        private bool m_continue;
        
        public TotalParser(IDbConnection conn, int nrStages)
        {
            m_connection = conn;
            m_nrStages = nrStages;
        }


        private void FireOnResult(Result newResult)
        {
            if (OnResult != null)
            {
                OnResult(newResult);
            }
        }
        private void FireLogMsg(string msg)
        {
            if (OnLogMessage != null)
                OnLogMessage(msg);
        }

        Thread m_monitorThread;

        public void Start()
        {
            m_continue = true;
            m_monitorThread = new Thread(Run);
            m_monitorThread.Start();
        }

        public void Stop()
        {
            m_continue = false;
        }

        private void Run()
        {
            while (m_continue)
            {
                try
                {
                    if (m_connection.State != ConnectionState.Open)
                    {
                        if (m_connection is System.Data.H2.H2Connection)
                        {
                            (m_connection as System.Data.H2.H2Connection).Open("root", "");
                        }
                        else
                        {
                            m_connection.Open();
                        }
                    }

                    string paramOper = "?";
                    if (m_connection is MySql.Data.MySqlClient.MySqlConnection)
                    {
                        paramOper = "?date";
                    }

                    IDbCommand cmd = m_connection.CreateCommand();


                    string baseCommand = "SELECT etappresults.changed, etappresults.idrunners, etappnr, totaltid, totalstatus, name, club, class FROM etappresults, runners WHERE etappresults.idrunners = runners.idrunners AND etappresults.changed > " + paramOper;

                    ReadRadioControls();

                    cmd.CommandText = baseCommand;
                    IDbDataParameter param = cmd.CreateParameter();
                    param.ParameterName = "date";
                    if (m_connection is MySql.Data.MySqlClient.MySqlConnection || m_connection is System.Data.H2.H2Connection)
                    {
                        param.DbType = DbType.String;
                        param.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        param.DbType = DbType.DateTime;
                        param.Value = DateTime.Now;
                    }
                                       
                    DateTime lastDateTime = DateTime.Now.AddMonths(-120);
                    param.Value = lastDateTime;

                    cmd.Parameters.Add(param);

                    FireLogMsg("Total Monitor thread started");
                    IDataReader reader = null;
                    var runnerPairs = new Dictionary<int, RunnerPair>();
                    while (m_continue)
                    {
                        string lastRunner = "";
                        try
                        {
                            /*Kontrollera om nya klasser*/
                            /*Kontrollera om nya resultat*/
                            if (cmd is MySql.Data.MySqlClient.MySqlCommand || m_connection is System.Data.H2.H2Connection)
                            {
                                (cmd.Parameters["date"] as IDbDataParameter).Value = lastDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            }
                            else
                            {
                                (cmd.Parameters["date"] as IDbDataParameter).Value = lastDateTime;
                            }


                            cmd.Prepare();
                            reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                int time = 0, runnerID = 0, iStartTime = 0, status = -1, etappnr = 0;
                                string name = "", club = "", classN = "";
                                
                                try
                                {
                                    string sModDate = Convert.ToString(reader["changed"]);
                                    DateTime modDate = ParseDateTime(sModDate);
                                    lastDateTime = (modDate > lastDateTime ? modDate : lastDateTime);
                                    runnerID = Convert.ToInt32(reader["idrunners"].ToString());

                                    time = -9;
                                    if (reader["totaltid"] != null && reader["totaltid"] != DBNull.Value)
                                        time = Convert.ToInt32(reader["totaltid"].ToString());

                                    name = (reader["name"] as string);

                                    club = (reader["club"] as string);
                                    classN = (reader["class"] as string);
                                    status = Convert.ToInt32(reader["totalstatus"]);
                                    etappnr = Convert.ToInt32(reader["etappnr"]);
                                }
                                catch (Exception ee)
                                {
                                    FireLogMsg(ee.Message);
                                }


                                /*
                                    time is seconds * 100
                                 */

                                if (status != 999)
                                {
                                    if (etappnr==m_nrStages)
                                    {
                                        var res = new Result
                                        {
                                            ID = runnerID,
                                            RunnerName = name,
                                            RunnerClub = club,
                                            Class = classN,
                                            StartTime = 0,
                                            Time = time,
                                            Status = status

                                        };

                                        FireOnResult(res);
                                    }
                                    else
                                    {
                                        var times = new List<ResultStruct>();
                                        var t = new ResultStruct
                                        {
                                            ControlCode = etappnr + 1000,
                                            ControlNo = etappnr,
                                            Time = (int)time
                                        };
                                        times.Add(t);


                                        var res = new Result
                                        {
                                            ID = runnerID,
                                            RunnerName = name,
                                            RunnerClub = club,
                                            Class = classN,
                                            StartTime = 0,
                                            Time = -2,
                                            Status = status,
                                            SplitTimes = times

                                        };


                                        FireOnResult(res);
                                    }


                                }
                            }
                            reader.Close();


                            Thread.Sleep(1000);
                        }
                        catch (Exception ee)
                        {
                            if (reader != null)
                                reader.Close();
                            FireLogMsg("Total Parser: " + ee.Message + " {parsing: " + lastRunner);

                            Thread.Sleep(100);

                            switch (m_connection.State)
                            {
                                case ConnectionState.Broken:
                                case ConnectionState.Closed:
                                    m_connection.Close();
                                    m_connection.Open();
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    FireLogMsg("Total Parser: " +ee.Message);
                }
                finally
                {
                    if (m_connection != null)
                    {
                        m_connection.Close();
                    }
                    FireLogMsg("Disconnected");
                    FireLogMsg("Total Monitor thread stopped");

                }
            }
        }

        private void ReadRadioControls()
        {
            using (var cmd = m_connection.CreateCommand())
            {
                cmd.CommandText =
                    @"select distinct class FROM runners";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dlg = OnRadioControl;
                        for (int i = 1; i < m_nrStages; i++)
                        {
                            var name = "Efter dag " + i;

                            var className = reader["class"] as string;

                            int code = i;
                            int order = i;
                            int rcode = 1000 * 1 + code;

                            if (dlg != null)
                                dlg(name, rcode, className, order);
                        }
                    }
                }
            }
        }

        private static DateTime ParseDateTime(string tTime)
        {
            DateTime startTime;
            if (!DateTime.TryParseExact(tTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
            {
                if (!DateTime.TryParseExact(tTime, "yyyy-MM-dd HH:mm:ss.f", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
                {
                    if (!DateTime.TryParseExact(tTime, "yyyy-MM-dd HH:mm:ss.ff", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
                    {
                        if (!DateTime.TryParseExact(tTime, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime))
                        {
                        }
                    }
                }
            }
            return startTime;
        }


        public event RadioControlDelegate OnRadioControl;
    }
}
