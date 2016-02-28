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
using System.Data;

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
        
        public enum TaskState
        {
            Default,
            Completed,
            Error
        }
        private TaskState m_State;
        // Нужно для того, чтобы можно было определить, можно ли
        // использовать выходной результат этой утилиты как
        // входной для другой утилиты
        private bool m_IsThereOutput;


        // Переменные, хранящие данные для восстановления состояния окна
        // при выборе редактирования задачи (т.е. чтобы все выбранные параметры и т.д.)
        // были на месте
        // И их свойства
        private string m_OutputPath;
        // Список нужен, чтобы при добавлении выделений в ListBox
        // сравнение происходило корректно (если сравнивать со списком,
        // заново загруженным из базы, то, даже несмотря на то, что они одинаковые,
        // сравнение объектов давало в результате ложь)
        private List<MyDataRow> m_ParametersList;   
        private MyDataRow[] m_SelectedParametersList;
        private GroupBox[] m_AdditionalParameters;
        private List<DataTable> m_AdditionalParametersInputs;

        public string OutputPath
        {
            get { return m_OutputPath; }
            set
            {
                if (value != null)
                {
                    m_OutputPath = value;
                }
                else
                {
                    m_OutputPath = String.Empty;
                }
            }
        }
        public List<MyDataRow> ParametersList
        {
            get { return m_ParametersList; }
            set { m_ParametersList = value; }
        }
        public MyDataRow[] SelectedParametersList
        {
            get { return m_SelectedParametersList; }
            set { m_SelectedParametersList = value; }
        }
        public GroupBox[] AdditionalParameters
        {
            get { return m_AdditionalParameters; }
            set { m_AdditionalParameters = value; }
        }
        public List<DataTable> AdditionalParametersInputs
        {
            get { return m_AdditionalParametersInputs; }
            set { m_AdditionalParametersInputs = value; }
        }

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
            m_State = TaskState.Default;
            m_IsThereOutput = false;

            // Необходимо для перенаправления входного и выходного потоков командной строки
            m_Process.StartInfo.UseShellExecute = false;
            // Разрешаем перенаправить выходной поток
            m_Process.StartInfo.RedirectStandardOutput = true;
            // Разрешаем перенаправить поток ошибок 
            m_Process.StartInfo.RedirectStandardError = true;
            // Устанавливаем кодировку выходного потока (поддержка русского языка)  
            m_Process.StartInfo.StandardOutputEncoding = Encoding.GetEncoding("cp866");
            m_Process.StartInfo.StandardErrorEncoding = Encoding.GetEncoding("cp866");
            // Не создавать окно процесса
            m_Process.StartInfo.CreateNoWindow = true;
            // Нужно, чтобы вызывалось событие Exited
            m_Process.EnableRaisingEvents = true;
            // Добавляем обработчик события Exited
            m_Process.Exited += new EventHandler(Process_Exited);
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
        // Выдаёт и задаёт путь до обзорного изображения (Thumbnail)
        public string ThumbnailPath
        {
            get { return m_ThumbnailFile; }
            set
            {
                if (m_IsEditing == true)
                {
                    if (value == null)
                    {
                        m_ThumbnailFile = String.Empty;
                        //throw new ArgumentNullException(nameof(value), "Нельзя передавать null в качестве значения");
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

        public bool IsThereOutput
        {
            get { return m_IsThereOutput; }
            set
            { m_IsThereOutput = value; }
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
            m_TaskElement.SetTaskIDToLabel(m_TaskID);
            m_TaskElement.SetUtilityNameToLabel(m_UtilityName);
            m_TaskElement.SetFileNameToLabelAndToolTip(m_SrcFileName);
            m_TaskElement.SetImageToImagePreviewElement(m_ThumbnailFile);
            //m_TaskElement.SetImage = m_SrcFileName;
        }

        public void SubscribeOutputDataAndErrorReceivedHandler(DataReceivedEventHandler outputDataReceivedHandler)
        {
            m_Process.OutputDataReceived += outputDataReceivedHandler;
            m_Process.ErrorDataReceived += outputDataReceivedHandler;
        }

        public void UnubscribeOutputDataAndErrorReceivedHandler(DataReceivedEventHandler outputDataReceivedHandler)
        {
            m_Process.OutputDataReceived -= outputDataReceivedHandler;
            m_Process.ErrorDataReceived -= outputDataReceivedHandler;
        }

        public void StartProcess()
        {
            m_Process.Start();
            m_Process.BeginOutputReadLine();
            m_Process.BeginErrorReadLine();
        }

        public void SetStateOfTask(TaskState taskState)
        {
            m_State = taskState;
            switch (m_State)
            {
                case TaskState.Default:
                    m_TaskElement.SetTaskElementState(TaskElement.TaskElementState.Normal);
                    break;
                case TaskState.Completed:
                    m_TaskElement.SetTaskElementState(TaskElement.TaskElementState.Completed);
                    break;
                case TaskState.Error:
                    m_TaskElement.SetTaskElementState(TaskElement.TaskElementState.Failed);
                    break;
                default:
                    m_TaskElement.SetTaskElementState(TaskElement.TaskElementState.Normal);
                    break;
            }
        }

        #endregion

        // Обработчики событий
        #region Обработчики событий

        private void Process_Exited(object sender, EventArgs e)
        {
            Process process = sender as Process;
            process.CancelOutputRead();
            process.CancelErrorRead();
            int exitCode = process.ExitCode;
            process.Close();
            //process.OutputDataReceived -= new DataReceivedEventHandler(OutputDataReceivedHandler);
            //process.Exited -= new EventHandler(ProcessExited);
            TaskManager.GetExitedTask(this, exitCode);
        }
        #endregion
    }
}
