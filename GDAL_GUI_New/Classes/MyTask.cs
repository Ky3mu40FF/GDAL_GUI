using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace GDAL_GUI_New
{
    public class MyTask
    {
        // Переменные
                #region Переменные
        private Process m_Process;
        private TaskElement m_TaskElement;
        private string m_UtilityName;
        private string m_SrcFileName;
        private string m_ThumbnailFile;
        //private string m_OutputFileName;
        private string m_ProcessArguments;
        private int m_TaskID;
        private bool m_IsEditing;
        
        public enum State
        {
            Default,
            Completed,
            Error
        }
        private State m_State;
        #endregion

        // Конструкторы
                #region Конструкторы
        public MyTask(MainWindow mainWindow)
        {
            m_TaskID = mainWindow.GetTasksCounter + 1;
            m_Process = new Process();
            m_TaskElement = new TaskElement(mainWindow, this, m_TaskID);
            m_UtilityName = String.Empty;
            m_SrcFileName = String.Empty;
            m_ThumbnailFile = String.Empty;
            //m_OutputFileName = "";
            m_ProcessArguments = String.Empty;
            m_IsEditing = false;
            m_State = State.Default;

            // Необходимо для перенаправления входного и выходного потоков командной строки
            m_Process.StartInfo.UseShellExecute = false;
            // Разрешаем перенаправить выходной поток
            m_Process.StartInfo.RedirectStandardOutput = true;
            // Разрешаем перенаправить поток ошибок 
            m_Process.StartInfo.RedirectStandardError = true;
            // Устанавливаем кодировку выходного потока (поддержка русского языка)  
            m_Process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("cp866");
            // Не создавать окно процесса
            m_Process.StartInfo.CreateNoWindow = true;    
        }
        #endregion

        // Свойства
                #region Свойства
        // Выдаёт и задаёт имя утилиты
        public string UtilityName
        {
            get { return m_UtilityName; }
            set
            {
                if (m_IsEditing == true)
                {
                    if (String.IsNullOrEmpty(value))
                    {
                        throw new ArgumentNullException(nameof(value), "Был передан Null или пустая строка.");
                    }
                    else
                    {
                        m_UtilityName = value;
                    }
                }
                else
                {
                    throw new NotEditingException();
                }
            }
        }
        // Выдаёт и задаёт путь до входного файла
        public string SrcFileName
        {
            get { return m_SrcFileName; }
            set
            {
                if (m_IsEditing == true)
                {
                    if (String.IsNullOrEmpty(value))
                    {
                        throw new ArgumentNullException(nameof(value), "Нельзя передавать null в качестве значения");
                    }
                    else
                    {
                        m_SrcFileName = value;
                    }
                }
                else
                {
                    throw new NotEditingException();
                }
            }
        }

        public string ThumbnailPath
        {
            get { return m_ThumbnailFile; }
            set
            {
                if (m_IsEditing == true)
                {
                    if (String.IsNullOrEmpty(value))
                    {
                        throw new ArgumentNullException(nameof(value), "Нельзя передавать null в качестве значения");
                    }
                    else
                    {
                        m_ThumbnailFile = value;
                    }
                }
                else
                {
                    throw new NotEditingException();
                }
            }
        }
        // Выдаёт экземпляр процесса
        public Process GetProcess
        {
            get { return m_Process; }
        }
        // Выдаёт экземпляр графического представления задачи
        public TaskElement GetTaskElement
        {
            get { return m_TaskElement; }
        }
        // Выдаёт ID задачи
        public int GetTaskID
        {
            get { return m_TaskID; }
        }

        public string ParametersString
        {
            get { return m_ProcessArguments; }
            set { m_ProcessArguments = value; }
        }
        #endregion

        // Методы
                #region Методы
        // Устанавливает флаг разрешающий редактирование
        public bool BeginEdit()
        {
            if (m_IsEditing == true)
            {
                return false;
            }
            else
            {
                m_IsEditing = !m_IsEditing;
                return true;
            }
        }
        // Устанавливает флаг запрещающий редактирование
        public bool EndEdit()
        {
            if (m_IsEditing == false)
            {
                return false;
            }
            else
            {
                m_IsEditing = !m_IsEditing;
                AcceptSettings();
                return true;
            }
        }
        // Принимает изменения, внесённые в режиме редактирования
        private void AcceptSettings()
        {
            m_Process.StartInfo.FileName = Properties.Settings.Default.UtilitiesDirectory + m_UtilityName;
            m_Process.StartInfo.Arguments = m_ProcessArguments;
            m_TaskElement.SetUtilityName = m_UtilityName;
            m_TaskElement.SetFileName = m_SrcFileName;
            m_TaskElement.SetImage = m_ThumbnailFile;
            //m_TaskElement.SetImage = m_SrcFileName;
        }
        #endregion
    }
}
