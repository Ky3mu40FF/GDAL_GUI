/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows;
using System.Data;

namespace TestForGDAL
{
    class DataBaseControl
    {
        
        private static OleDbConnection m_Connection = 
            new OleDbConnection(@"Provider = Microsoft.ACE.OLEDB.12.0; Data Source = C:\Users\Ky3mu40FF\Documents\Visual Studio 2015\Projects\TestForGDAL\TestForGDAL\DB\GDAL_DB.accdb");

        public static void ConnectToDB()
        {
            try
            {
                m_Connection.Open();
                //MessageBox.Show("Соединение выполнено успешно." + Environment.NewLine +
                //    m_Connection.State);
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось выполнить подключение к базе данных"+
                    Environment.NewLine + e.Message);
            }
        }

        public static List<string> GetUtilitiesNames()
        {
             
            List<string> utilities = new List<string>();
            OleDbCommand command = new OleDbCommand();
            command.Connection = m_Connection;
            command.CommandText =
                "SELECT UtilitiesDescriptions.NameOfUtility FROM UtilitiesDescriptions WHERE UtilitiesDescriptions.HasATable = True";
            DataSet dataSet = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            try
            {
                adapter.SelectCommand = command;
                adapter.Fill(dataSet, "UtilitiesDescriptions");
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    utilities.Add(row["NameOfUtility"].ToString());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось получить имена утилит." + Environment.NewLine +
                                e.Message);
            }
            finally
            {
                command.Dispose();
                dataSet.Dispose();
                adapter.Dispose();
            }
            return utilities;
        }

        public static DataRow GetUtilityInfo(string utilityName)
        {
            DataRow utilityInfo = null;
            OleDbCommand command = new OleDbCommand();
            command.Connection = m_Connection;
            command.CommandText = 
                "SELECT * FROM UtilitiesDescriptions WHERE UtilitiesDescriptions.NameOfUtility LIKE " + utilityName;
            DataSet dataSet = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            try
            {
                adapter.SelectCommand = command;
                adapter.Fill(dataSet, utilityName);
                utilityInfo = dataSet.Tables[utilityName].Rows[0];
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось получить информацию об утилите " + utilityName + "."
                    + Environment.NewLine + e.Message);
            }
            finally
            {
                command.Dispose();
                dataSet.Dispose();
                adapter.Dispose();
            }
            return utilityInfo;
        }
        /*
        public static List<DataRow> GetUtilityParameters(string utilityName)
        {
            List<DataRow> utilityParameters = new List<DataRow>();
            OleDbCommand command = new OleDbCommand();
            command.Connection = m_Connection;
            command.CommandText = "SELECT * FROM " + utilityName;
            DataSet dataSet = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            try
            {
                adapter.SelectCommand = command;
                adapter.Fill(dataSet, utilityName);
                utilityParameters.AddRange(dataSet.Tables[utilityName].Select());
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось получить параметры утилиты " + utilityName + "." 
                    + Environment.NewLine + e.Message);
            }
            finally
            {
                command.Dispose();
                dataSet.Dispose();
                adapter.Dispose();
            }
            return utilityParameters;
        }
        */
        /*
        public static List<MyDataRow> GetUtilityParameters(string utilityName)
        {
            List<MyDataRow> utilityParameters = new List<MyDataRow>();
            OleDbCommand command = new OleDbCommand();
            command.Connection = m_Connection;
            command.CommandText = "SELECT * FROM " + utilityName;
            DataSet dataSet = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            try
            {
                adapter.SelectCommand = command;
                adapter.Fill(dataSet, utilityName);
                //utilityParameters.AddRange(dataSet.Tables[utilityName].Select());
                foreach (DataRow row in dataSet.Tables[utilityName].Rows)
                {
                    utilityParameters.Add(new MyDataRow(row));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось получить параметры утилиты " + utilityName + "."
                    + Environment.NewLine + e.Message);
            }
            finally
            {
                command.Dispose();
                dataSet.Dispose();
                adapter.Dispose();
            }
            return utilityParameters;
        }

        public static OleDbConnection GetConnection
        {
            get { return m_Connection; }
        }
    }
}
*/