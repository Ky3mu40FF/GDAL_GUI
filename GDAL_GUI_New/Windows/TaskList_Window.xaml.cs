using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace GDAL_GUI_New
{
    /// <summary>
    /// Логика взаимодействия для TaskList_Window.xaml
    /// </summary>
    public partial class TaskList_Window : Window
    {
        // Переменные
        private ObservableCollection<MyTask> m_TaskList;
        private MyTask m_SelectedTask;
        private string m_GottenOutputPath;

        // Конструкторы
        public TaskList_Window(ObservableCollection<MyTask> taskList)
        {
            InitializeComponent();
            m_TaskList = taskList;
            m_GottenOutputPath = String.Empty;

            EventAndPropertiesInitialization();

            //ShowTaskList();
        }

        // Свойства
        public string FilePath
        {
            get { return m_GottenOutputPath; }
        }

        // Методы
        private void EventAndPropertiesInitialization()
        {
            m_TaskList.CollectionChanged += Tasks_CollectionChanged;
            Button_SelectTask.Click += new RoutedEventHandler(Button_SelectTask_Click);
            Button_Cancel.Click += new RoutedEventHandler(Button_Cancel_Click);
        }

        private void ShowTaskList()
        {
            /*
            foreach (MyTask task in m_TaskList)
            {
                
            }
            */
        }

        // Обработчики событий
        private void Tasks_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems[0] != null)
            {
                MyTask task = e.NewItems[0] as MyTask;
                //StackPanel_TaskElements.Children.Add(task.GetTaskElement);
                StackPanel_TaskElements.Children.Insert(e.NewStartingIndex, task.GetTaskElement);
            }
            else if (e.OldItems != null && e.OldItems[0] != null)
            {
                MyTask task = e.OldItems[0] as MyTask;
                StackPanel_TaskElements.Children.Remove(task.GetTaskElement);
            }
        }

        private void Button_SelectTask_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
