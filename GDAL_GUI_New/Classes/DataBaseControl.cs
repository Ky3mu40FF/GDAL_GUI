using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Windows;
using System.Data;
using System.IO;

namespace GDAL_GUI_New
{
    class DataBaseControl
    {
        
        private static OleDbConnection m_Connection = 
            new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + 
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\DB\GDAL_DB.accdb"));

        private static string m_OptionsTableName = "Options";
        private static string m_OptionsTypeColumnName = "OptionsTypeKey";
        private static string m_GroupsColumnName = "Group";
        private static string m_KeysColumnName = "Key";
        private static string m_ValuesColumnName = "Val";

        public static bool ConnectToDB()
        {
            try
            {
                if (m_Connection.State != ConnectionState.Open)
                {
                    m_Connection.Open();
                    //MessageBox.Show("Соединение выполнено успешно." + Environment.NewLine +
                    //    m_Connection.State);
                    Console.WriteLine("Соединение выполнено успешно.");
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось выполнить подключение к базе данных"+
                    Environment.NewLine + e.Message);
                return false;
            }
        }

        public static void CloseConnection()
        {
            try
            {
                if (m_Connection.State != ConnectionState.Closed)
                {
                    m_Connection.Close();
                    Console.WriteLine("Соединение было успешно закрыто.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось закрыть соединение." + Environment.NewLine + e.Message, 
                    "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static OleDbConnection GetConnection
        {
            get { return m_Connection; }
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
                "SELECT * FROM UtilitiesDescriptions WHERE UtilitiesDescriptions.NameOfUtility LIKE @Name";
            //command.Parameters.Add("@Name", utilityName);
            command.Parameters.AddWithValue("@Name", utilityName);
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

        public static List<string> GetOptionsTypes()
        {
            List<string> optionsTypes = new List<string>();
            OleDbCommand command = new OleDbCommand();
            command.Connection = m_Connection;
            command.CommandText = 
                "SELECT DISTINCT "+ m_OptionsTableName + "." + m_OptionsTypeColumnName + 
                " FROM " + m_OptionsTableName;
            DataSet dataSet = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            try
            {
                adapter.SelectCommand = command;
                adapter.Fill(dataSet, m_OptionsTableName);
                foreach (DataRow row in dataSet.Tables[m_OptionsTableName].Rows)
                {
                    optionsTypes.Add(row["OptionsType"].ToString());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось получить типы параметров Options из таблицы "+ m_OptionsTableName + "."
                    + Environment.NewLine + e.Message);
            }
            finally
            {
                command.Dispose();
                dataSet.Dispose();
                adapter.Dispose();
            }
            return optionsTypes;
        }

        public static List<string> GetGroupsFromOptionsType(string optionsType)
        {
            List<string> groups = new List<string>();
            OleDbCommand command = new OleDbCommand();
            command.Connection = m_Connection;
            command.CommandText = 
                "SELECT DISTINCT " + m_OptionsTableName + "." + m_GroupsColumnName + 
                " FROM " + m_OptionsTableName + 
                " WHERE " + m_OptionsTableName + "." + m_OptionsTypeColumnName + " LIKE '" + optionsType +"'";
            DataSet dataSet = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            try
            {
                adapter.SelectCommand = command;
                adapter.Fill(dataSet, m_OptionsTableName);
                foreach (DataRow row in dataSet.Tables[m_OptionsTableName].Rows)
                {
                    groups.Add(row[m_GroupsColumnName].ToString());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось получить группы типа " + optionsType + " из таблицы " + m_OptionsTableName + "."
                    + Environment.NewLine + e.Message);
            }
            finally
            {
                command.Dispose();
                dataSet.Dispose();
                adapter.Dispose();
            }
            return groups;
        }

        public static List<string> GetGroupsFromOptionsTypeByTypeKey(string optionsTypeKey)
        {
            List<string> groups = new List<string>();
            OleDbCommand command = new OleDbCommand();
            command.Connection = m_Connection;
            command.CommandText =
                "SELECT DISTINCT " + m_OptionsTableName + "." + m_GroupsColumnName +
                " FROM " + m_OptionsTableName +
                " WHERE " + m_OptionsTableName + "." + m_OptionsTypeColumnName + " LIKE '" + optionsTypeKey + "'";
            DataSet dataSet = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            try
            {
                adapter.SelectCommand = command;
                adapter.Fill(dataSet, m_OptionsTableName);
                foreach (DataRow row in dataSet.Tables[m_OptionsTableName].Rows)
                {
                    groups.Add(row[m_GroupsColumnName].ToString());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось получить группы типа " + optionsTypeKey + " из таблицы " + m_OptionsTableName + "."
                    + Environment.NewLine + e.Message);
            }
            finally
            {
                command.Dispose();
                dataSet.Dispose();
                adapter.Dispose();
            }
            return groups;
        }

        public static List<DataRow> GetKeysWithValuesFromGroup(string group)
        {
            List<DataRow> keysWithValues = new List<DataRow>();
            OleDbCommand command = new OleDbCommand();
            command.Connection = m_Connection;
            command.CommandText = 
                "SELECT " + m_OptionsTableName + "." + m_KeysColumnName + ", " + m_OptionsTableName + "." + m_ValuesColumnName +
                " FROM " + m_OptionsTableName +
                " WHERE " + m_OptionsTableName + "." + m_GroupsColumnName + " LIKE '" + group + "'";
            DataSet dataSet = new DataSet();
            OleDbDataAdapter adapter = new OleDbDataAdapter();
            try
            {
                adapter.SelectCommand = command;
                adapter.Fill(dataSet, m_OptionsTableName);
                foreach (DataRow row in dataSet.Tables[m_OptionsTableName].Rows)
                {
                    keysWithValues.Add(row);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось получить ключи со значениями группы " + group + 
                    " из таблицы " + m_OptionsTableName + "."
                    + Environment.NewLine + e.Message);
            }
            finally
            {
                command.Dispose();
                dataSet.Dispose();
                adapter.Dispose();
            }
            return keysWithValues;
        }

        
    }
}
