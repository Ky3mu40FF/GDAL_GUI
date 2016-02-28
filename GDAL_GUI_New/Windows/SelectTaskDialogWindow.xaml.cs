using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Логика взаимодействия для SelectTaskDialogWindow.xaml
    /// </summary>
    public partial class SelectTaskDialogWindow : Window, INotifyPropertyChanged
    {
        // Переменные
        private ObservableCollection<TaskElement> m_TaskElementList;
        private MyTask m_SelectedTask;
        private string m_GottenOutputPath;

        public event PropertyChangedEventHandler PropertyChanged;

        //private void OnPropertyChanged(string propertyName)
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        // Конструкторы
        public SelectTaskDialogWindow(ObservableCollection<MyTask> taskList)
        {
            InitializeComponent();
            //m_TaskList = taskList;
            m_TaskElementList = new ObservableCollection<TaskElement>();
            m_GottenOutputPath = String.Empty;
            EventAndPropertiesInitialization();
            ShowTaskList(taskList);
            /*
            Binding myBinding = new Binding();
            myBinding.Path = new PropertyPath("SelectedTask");
            myBinding.Mode = BindingMode.TwoWay;
            myBinding.Source = this;
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.SetBinding(DependencyProperty.Register("MyTaskProperty", typeof(MyTask), typeof(UserControl)), myBinding);
            */
        }

        // Свойства
        public string FilePath
        {
            get { return m_GottenOutputPath; }
        }

        public MyTask SelectedTask
        {
            get { return m_SelectedTask; }
            set
            {
                if (value != null)
                {
                    m_SelectedTask = value;
                    m_GottenOutputPath = m_SelectedTask.OutputPath;
                    this.TextBox_OutputPathFromTask.Text = m_SelectedTask.OutputPath;
                }
                else
                {
                    m_SelectedTask = value;
                    m_GottenOutputPath = String.Empty;
                    this.TextBox_OutputPathFromTask.Text = String.Empty;
                }
                
            }
        }

        public ObservableCollection<TaskElement> GetTaskElementList
        {
            get { return m_TaskElementList; }
        }

        // Методы
        private void EventAndPropertiesInitialization()
        {
            m_TaskElementList.CollectionChanged += Tasks_CollectionChanged;
            Button_SelectTask.Click += new RoutedEventHandler(Button_SelectTask_Click);
            Button_Cancel.Click += new RoutedEventHandler(Button_Cancel_Click);
        }

        private void ShowTaskList(ObservableCollection<MyTask> TaskList)
        {
            foreach (MyTask task in TaskList)
            {
                if (task.IsThereOutput)
                {
                    m_TaskElementList.Add(new TaskElement(task.GetTaskElement, this));
                }
            }
        }

        // Обработчики событий
        private void Tasks_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems[0] != null)
            {
                TaskElement taskElement = e.NewItems[0] as TaskElement;
                //StackPanel_TaskElements.Children.Add(task.GetTaskElement);
                StackPanel_TaskElements.Children.Insert(e.NewStartingIndex, taskElement);
            }
            else if (e.OldItems != null && e.OldItems[0] != null)
            {
                TaskElement taskElement = e.OldItems[0] as TaskElement;
                StackPanel_TaskElements.Children.Remove(taskElement);
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
