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
using System.Data;
using System.Collections.ObjectModel;
using Gat.Controls;

namespace GDAL_GUI_New
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Переменные
                #region Переменные
        // Коллекция задач
        private ObservableCollection<MyTask> m_Tasks;
        // Счётчик количества добавленных задач
        private int m_TasksCounter;
        // Текущая (выбранная) задача 
        private MyTask m_CurrentTask;
        // StringBuilder, в который будет сохраняться выходная информация от утилит
        private MyStringBuilder m_OutputStringBuilder;

        #endregion

        // Конструкторы
                #region Конструкторы

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            m_Tasks = new ObservableCollection<MyTask>();
            m_TasksCounter = 0;
            m_CurrentTask = null;
            m_OutputStringBuilder = null;
            EventAndPropertiesInitialization();
        }
        #endregion

        // Свойства
                #region Свойства
        // Выдаёт коллекцию задач
        public ObservableCollection<MyTask> GetTasksList
        {
            get { return m_Tasks; }
        }
        // Выдаёт количество добавленных задач
        public int GetTasksCounter
        {
            get { return m_TasksCounter; }
        }
        // Выдаёт и задаёт текущую (выбранную) задачу
        public MyTask CurrentTask
        {
            get
            {
                return m_CurrentTask;
            }
            set
            {
                if (value != null)
                {
                    m_CurrentTask = value;
                }
            }
        }
        
        #endregion

        // Методы
                #region Методы
        // Подписка на события и другие инициализации
        private void EventAndPropertiesInitialization()
        {
            // Подписка на события
            Menu_File_Exit.Click += new RoutedEventHandler(Menu_File_Exit_Click);
            Menu_Edit_AddTask.Click += new RoutedEventHandler(Menu_Edit_AddTask_Click);
            Menu_Run_RunAll.Click += new RoutedEventHandler(Menu_Run_RunAll_Click);
            Menu_Run_RunSelected.Click += new RoutedEventHandler(Menu_Run_RunSelected_Click);
            Menu_Settings.Click += new RoutedEventHandler(Menu_Settings_Click);
            Menu_About.Click += new RoutedEventHandler(Menu_About_Click);

            m_Tasks.CollectionChanged +=
                new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Tasks_CollectionChanged);
        }

        // Добавляет переданную задачу и инкремирует счётчик
        public void AddNewTask(MyTask task)
        {
            if (task == null)
            {
                return;
            }
            m_Tasks.Add(task);
            m_TasksCounter++;
        }
        #endregion

        // Обработчики событий
                #region Обработчики событий
        // Выход из приложения по нажатию на кнопку Выход в меню
        private void Menu_File_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
        // Добавление нового задания
        private void Menu_Edit_AddTask_Click(object sender, RoutedEventArgs e)
        {
            TaskEditWindow taskEditWindow = new TaskEditWindow(this);
            taskEditWindow.ShowDialog();
        }
        // Запуск всех добавленных заданий по порядку
        private void Menu_Run_RunAll_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Заглушка. Run All");
        }
        // Запуск одного выбранного задания
        private void Menu_Run_RunSelected_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Заглушка. Run Selected");
        }
        // Открытие окна настроек
        private void Menu_Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        // Открытие окна "О программе"
        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Show();
        }

        private void Tasks_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems[0] != null)
            {
                MyTask task = e.NewItems[0] as MyTask;
                StackPanel_TaskElements.Children.Add(task.GetTaskElement);
            }
        }

        #endregion
    }


}
