using PLEXOS.Core.Network;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;

namespace PLEXOS.SocketServer.SQLStuff
{
    public class SQLNotify
    {


        #region Instance Variables / Events / Delegates

        public delegate void ChangeRecievedEventHandler(object sender, ChangeRecievedEventArgs e);
        public event ChangeRecievedEventHandler ChangeRecieved;

        private string _sqlConnectionStr;
        private SqlConnection _Con;
        private int _PollTime = 1000;

        #endregion
        #region Constructors
        public SQLNotify()
        {
            SqlConnectionStringBuilder SB = new SqlConnectionStringBuilder();
            SB.DataSource = "plexosmodel.database.windows.net";
            SB.InitialCatalog = "PLEXOSModel";
            SB.IntegratedSecurity = false;
            SB.UserID = "kyri";
            SB.Password = "P1exo$17";
            _Con = new SqlConnection(SB.ConnectionString);
        }
        #endregion
        #region Commands   
        #endregion
        #region Properties
        #endregion
        #region Methods   
        public void StartListening()
        {

            if(_Con.State == System.Data.ConnectionState.Closed)
            {
                _Con.Open();
            }
            SqlCommand CMD = _Con.CreateCommand();
            CMD.CommandText = "SELECT  [ID],[TableName],[NotificationSent],[DateChanged] FROM [dbo].[DataChanges] where notificationsent = 0";
            SqlCommand UpdateCMD = _Con.CreateCommand();
            UpdateCMD.CommandText = "UPDATE   [dbo].[DataChanges] set [NotificationSent]= 1 where ID = @ChngeID";
            List<long> UpdateList = new List<long>();
            Console.WriteLine("Running SQL Listener... type stop s");

            while (true)
            {
                Thread.Sleep(_PollTime);

                using (SqlDataReader DR = CMD.ExecuteReader())
                {
                    while (DR.Read())
                    {                       
                        DBChangeData CD = new DBChangeData() { DataSource = "SQL", DataType = DR["TableName"].ToString(), DataKey =string.Empty};
                        Console.WriteLine($"Table {CD.DataType} Changed");
                        OnChangeRecieved(new ChangeRecievedEventArgs(CD));
                        UpdateList.Add((long)DR["ID"]);
                       
                    }
                }
                foreach(long Item in UpdateList)
                {
                    SqlParameter parm = UpdateCMD.CreateParameter();
                    parm.ParameterName = "ChngeID";
                    parm.Value = Item;
                    UpdateCMD.Parameters.Clear();
                    UpdateCMD.Parameters.Add(parm);
                    UpdateCMD.ExecuteNonQuery();
                    Console.WriteLine($"Flagged Item {Item} Notified");
                }
                UpdateList.Clear();
            }
        }
        #endregion
        protected virtual void OnChangeRecieved(ChangeRecievedEventArgs e)
        {
            if (ChangeRecieved != null)
            {
                ChangeRecieved(this, e);
            }
        }
    }
}
