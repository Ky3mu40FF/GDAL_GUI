using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace GDAL_GUI_New
{
    public static class TaskManager
    {
        // Переменные
            #region Переменные
        private static MainWindow m_MainWindow;
        private static ObservableCollection<MyTask> m_Tasks;
        private static MyTask m_CurrentTask;
        private static DataReceivedEventHandler m_DataReceivedHandler;
        private static int m_TaskCounter;
        private static bool m_IsCurrentlySomeTaskRunning;

        private enum RunMode
        {
            All,
            Selected
        }

        private static RunMode m_RunMode;
        #endregion

        // Конструкторы
        #region Конструкторы

        #endregion

        // Свойства
        #region Свойства

        public static ObservableCollection<MyTask> TasksCollection
        {
            get { return m_Tasks; }
            set
            {
                if (value != null)
                {
                    m_Tasks = value;
                }
            }
        }

        public static DataReceivedEventHandler SetDataReceivedHandler
        {
            set
            {
                if (value != null)
                {
                    m_DataReceivedHandler = value;
                }
                else
                {
                    throw new ArgumentNullException(nameof(value), "Был передан Null!");
                }
            }
        }
        #endregion

        // Методы
        #region Методы

        public static void InitializeProcessManager(MainWindow mainWindow)
        {
            m_MainWindow = mainWindow;
            m_TaskCounter = 0;
            m_Tasks = null;
            m_CurrentTask = null;
            m_DataReceivedHandler = null;
            m_RunMode = RunMode.All;
            m_IsCurrentlySomeTaskRunning = false;
        }

        public static void RunAll()
        {
            if (m_IsCurrentlySomeTaskRunning)
            {
                MessageBox.Show("В данный момент выполняется задача №" + m_CurrentTask.GetTaskID +
                                Environment.NewLine + "Запуск задач отменён.");
                return;
            }
            if (m_Tasks == null || m_Tasks.Count == 0)
            {
                System.Windows.MessageBox.Show("Отсутствуют задачи!", "Ошибка!",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            //m_Tasks[0].StartProcess();
            m_RunMode = RunMode.All;
            m_CurrentTask = m_Tasks[0];
            m_CurrentTask.SubscribeOutputDataAndErrorReceivedHandler(m_DataReceivedHandler);
            m_MainWindow.StatusBarNumOfTasksToComplete = m_Tasks.Count;
            m_MainWindow.StatusBarCurrentTaskId = m_TaskCounter;
            m_TaskCounter++;
            m_IsCurrentlySomeTaskRunning = true;
            m_MainWindow.SetBordersForProgressBar(m_Tasks.Count);
            m_MainWindow.ProgressBarValue = 0;
            Execution(m_CurrentTask);
        }

        public static void RunSelected(MyTask selectedTask)
        {
            if (m_IsCurrentlySomeTaskRunning)
            {
                MessageBox.Show("В данный момент выполняется задача №" + m_CurrentTask.GetTaskID +
                                Environment.NewLine + "Запуск задачи отменён.");
                return;
            }
            if (m_Tasks == null || m_Tasks.Count == 0)
            {
                System.Windows.MessageBox.Show("Отсутствуют задачи!", "Ошибка!",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            //m_Tasks[0].StartProcess();
            m_RunMode = RunMode.Selected;
            m_CurrentTask = selectedTask;
            m_CurrentTask.SubscribeOutputDataAndErrorReceivedHandler(m_DataReceivedHandler);
            m_IsCurrentlySomeTaskRunning = true;
            m_MainWindow.SetBordersForProgressBar(1);
            m_MainWindow.ProgressBarValue = 0;
            m_MainWindow.StatusBarCurrentTaskId = 0;
            m_MainWindow.StatusBarNumOfTasksToComplete = 1;
            Execution(m_CurrentTask);
        }

        private static void Execution(MyTask task)
        {
            if (task == null)
            {
                m_IsCurrentlySomeTaskRunning = false;
                return;
            }
            m_MainWindow.SendMessageToTextBox(Environment.NewLine + 
                "\t\tЗапуск задачи № " + task.GetTaskID.ToString() + 
                Environment.NewLine + "\tВремя запуска: " + DateTime.Now.ToString() +
                Environment.NewLine + "\tСформированная строка с аргументами:" + Environment.NewLine + 
                m_CurrentTask.ParametersString + Environment.NewLine +
                "\tВывод утилиты:");
            m_MainWindow.StatusBarMessage = "Выполняется задача № " + task.GetTaskID;

            task.StartProcess();
        }

        public static void GetExitedTask(MyTask task, int exitCode)
        {
            m_CurrentTask = task;
            if (exitCode == 0)
            {
                m_CurrentTask.SetStateOfTask(MyTask.TaskState.Completed);
                m_CurrentTask.UnubscribeOutputDataAndErrorReceivedHandler(m_DataReceivedHandler);
                m_MainWindow.SendMessageToTextBox("Задача № " + task.GetTaskID.ToString() +
                    " выполнена!");
            }
            else
            {
                m_CurrentTask.SetStateOfTask(MyTask.TaskState.Error);
                m_CurrentTask.UnubscribeOutputDataAndErrorReceivedHandler(m_DataReceivedHandler);
                m_MainWindow.SendMessageToTextBox("Задача № " + task.GetTaskID.ToString() +
                    " завершилась с ошибкой!");
            }

            if (m_RunMode == RunMode.All)
            {
                if (m_TaskCounter < m_Tasks.Count)
                {
                    m_CurrentTask = m_Tasks[m_Tasks.IndexOf(m_CurrentTask) + 1];
                    m_CurrentTask.SubscribeOutputDataAndErrorReceivedHandler(m_DataReceivedHandler);
                    m_MainWindow.ProgressBarValue = m_TaskCounter;
                    m_MainWindow.StatusBarCurrentTaskId = m_TaskCounter;
                    m_TaskCounter++;
                    m_IsCurrentlySomeTaskRunning = true;
                    
                    Execution(m_CurrentTask);
                }
                else
                {
                    m_TaskCounter = 0;
                    m_IsCurrentlySomeTaskRunning = false;
                    m_MainWindow.SendMessageToTextBox(Environment.NewLine + 
                        "Все задачи выполнены!" + Environment.NewLine + 
                        "Время завершения: " + DateTime.Now.ToString());
                    m_MainWindow.StatusBarMessage = "Все задачи завершены";
                    m_MainWindow.ProgressBarValue = m_Tasks.Count;
                    m_MainWindow.StatusBarCurrentTaskId = m_Tasks.Count;
                }
            }
            else if (m_RunMode == RunMode.Selected)
            {
                m_IsCurrentlySomeTaskRunning = false;
                m_MainWindow.SendMessageToTextBox(Environment.NewLine +
                        "Выбранная задача выполнена!" + Environment.NewLine +
                        "Время завершения: " + DateTime.Now.ToString());
                m_MainWindow.StatusBarMessage = "Все задачи завершены";
                m_MainWindow.ProgressBarValue = 1.0d;
                m_MainWindow.StatusBarCurrentTaskId = 1;
            }
        }

        #endregion

        // Обработчики событий
        #region Обработчики событий

        #endregion
    }
}