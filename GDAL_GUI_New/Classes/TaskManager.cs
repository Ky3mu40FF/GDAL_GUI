using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
        }

        public static void RunAll()
        {
            if (m_Tasks == null)
            {
                System.Windows.MessageBox.Show("Отсутствуют задачи!", "Ошибка!",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            //m_Tasks[0].StartProcess();
            m_RunMode = RunMode.All;
            m_CurrentTask = m_Tasks[0];
            m_CurrentTask.SubscribeOutputDataReceivedHandler(m_DataReceivedHandler);
            m_TaskCounter++;
            Execution(m_CurrentTask);
        }

        public static void RunSelected(MyTask selectedTask)
        {
            if (m_Tasks == null)
            {
                System.Windows.MessageBox.Show("Отсутствуют задачи!", "Ошибка!",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            //m_Tasks[0].StartProcess();
            m_RunMode = RunMode.Selected;
            m_CurrentTask = selectedTask;
            m_CurrentTask.SubscribeOutputDataReceivedHandler(m_DataReceivedHandler);
            Execution(m_CurrentTask);
        }

        private static void Execution(MyTask task)
        {
            if (task == null)
            {
                return;
            }
            m_MainWindow.SendMessageToTextBox(Environment.NewLine + 
                "Запуск процесса № " + task.GetTaskID.ToString() + 
                Environment.NewLine + "Время запуска: " + DateTime.Now.ToString());
            task.StartProcess();
        }

        public static void GetExitedTask(MyTask task, int exitCode)
        {
            m_CurrentTask = task;
            if (exitCode == 0)
            {
                m_CurrentTask.SetStateOfTask(MyTask.TaskState.Completed);
                m_CurrentTask.UnubscribeOutputDataReceivedHandler(m_DataReceivedHandler);
            }
            else
            {
                m_CurrentTask.SetStateOfTask(MyTask.TaskState.Error);
                m_CurrentTask.UnubscribeOutputDataReceivedHandler(m_DataReceivedHandler);
            }

            if (m_RunMode == RunMode.All)
            {
                if (m_TaskCounter < m_Tasks.Count)
                {
                    m_CurrentTask = m_Tasks[m_Tasks.IndexOf(m_CurrentTask) + 1];
                    m_CurrentTask.SubscribeOutputDataReceivedHandler(m_DataReceivedHandler);
                    m_TaskCounter++;
                    Execution(m_CurrentTask);
                }
                else
                {
                    m_TaskCounter = 0;
                    m_MainWindow.SendMessageToTextBox(Environment.NewLine + 
                        "Все задачи выполнены!" + Environment.NewLine + 
                        "Время завершения: " + DateTime.Now.ToString());
                }
            }
            else
            {
                m_MainWindow.SendMessageToTextBox(Environment.NewLine +
                        "Выбранная задача выполнена!" + Environment.NewLine +
                        "Время завершения: " + DateTime.Now.ToString());
            }
        }
        #endregion

        // Обработчики событий
        #region Обработчики событий

        #endregion
    }
}