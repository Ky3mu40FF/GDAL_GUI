/*
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

namespace TestForGDAL
{
    public class MyProcess
    {
        private Process m_Process;
        private TaskElement m_TaskElement;
        private string m_UtilityName;
        private string m_SrcFileName;
        private string m_OutputFileName;
        private string m_ProcessArguments;
        private bool m_IsEditing;
        private int m_ProcessID;


        public MyProcess()
        {
            //m_ProcessID = 0;
            m_Process = new Process();
            m_TaskElement = new TaskElement();
            m_UtilityName = "";
            m_SrcFileName = "";
            m_OutputFileName = "";
            m_ProcessArguments = "";
            m_IsEditing = false;

            m_Process.StartInfo.UseShellExecute = false;  //Необходимо для перенаправления входного и выходного потоков командной строки
            m_Process.StartInfo.RedirectStandardOutput = true;    //Разрешаем перенаправить выходной поток
            m_Process.StartInfo.RedirectStandardError = true;     //Разрешаем перенаправить поток ошибок
            m_Process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("cp866");   //Устанавливаем кодировку выходного потока (поддержка русского языка)
            m_Process.StartInfo.CreateNoWindow = true;    //Запуск процесса будет происходить без отрисовки окна
        }

        public MyProcess(MainWindow mainWindow)
        {
            m_ProcessID = mainWindow.GetProcessesCounter;
            m_Process = new Process();
            m_TaskElement = new TaskElement(mainWindow, m_ProcessID);
            m_UtilityName = "";
            m_SrcFileName = "";
            m_OutputFileName = "";
            m_ProcessArguments = "";
            m_IsEditing = false;

            m_Process.StartInfo.UseShellExecute = false;  //Необходимо для перенаправления входного и выходного потоков командной строки
            m_Process.StartInfo.RedirectStandardOutput = true;    //Разрешаем перенаправить выходной поток
            m_Process.StartInfo.RedirectStandardError = true;     //Разрешаем перенаправить поток ошибок
            m_Process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("cp866");   //Устанавливаем кодировку выходного потока (поддержка русского языка)
            m_Process.StartInfo.CreateNoWindow = true;    //Запуск процесса будет происходить без отрисовки окна
        }

        public string UtilityName
        {
            get { return m_UtilityName; }
            set
            {
                if (m_IsEditing == true)
                {
                    if (value != null)
                    {
                        m_UtilityName = value;
                    }
                    else
                    {
                        throw new ArgumentNullException(nameof(value), "Нельзя передавать null в качестве значения");
                    }
                }
                else
                {
                    throw new NotEditingException();
                }
            }
        }
        public string SrcFileName
        {
            get { return m_SrcFileName; }
            set
            {
                if (m_IsEditing == true)
                {
                    if (value != null)
                    {
                        m_SrcFileName = value;
                    }
                    else
                    {
                        throw new ArgumentNullException(nameof(value), "Нельзя передавать null в качестве значения");
                    }
                }
                else
                {
                    throw new NotEditingException();
                }
            }
        }
        public string OutputFileName
        {
            get { return m_OutputFileName; }
            set
            {
                if (m_IsEditing == true)
                {
                    if (value != null)
                    {
                        m_OutputFileName = value;
                    }
                    else
                    {
                        throw new ArgumentNullException(nameof(value), "Нельзя передавать null в качестве значения");
                    }
                }
                else
                {
                    throw new NotEditingException();
                }
            }
        }
        public Process GetProcess
        {
            get { return m_Process; }
        }
        public TaskElement GetTaskElement
        {
            get { return m_TaskElement; }
        }
        public int GetProcessID
        {
            get { return m_ProcessID; }
        }

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

        public bool StopEdit()
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

        private void AcceptSettings()
        {
            m_Process.StartInfo.FileName = @"C:\Users\Ky3mu40FF\Documents\Utilities_bin\" + m_UtilityName;
            FormArguments();
            m_Process.StartInfo.Arguments = m_ProcessArguments;
            //m_Process.StartInfo.Arguments = "-of jpeg -outsize 200% 200% " + m_ProcessArguments;
            m_TaskElement.SetImage = m_SrcFileName;
            m_TaskElement.SetUtilityName = m_UtilityName;
        }

        private void FormArguments()
        {
            if (m_UtilityName != "gdalinfo")
            {
                m_ProcessArguments = m_SrcFileName + " " + m_OutputFileName;
            }
            else
            {
                m_ProcessArguments = m_SrcFileName;
            }
        }

    }
}
*/