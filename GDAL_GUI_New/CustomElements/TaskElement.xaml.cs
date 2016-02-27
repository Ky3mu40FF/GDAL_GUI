
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Windows.Media.Animation;

namespace GDAL_GUI_New
{
    /// <summary>
    /// Логика взаимодействия для TaskElement.xaml
    /// </summary>
    public partial class TaskElement : UserControl, INotifyPropertyChanged
    {
        // Переменныые
                #region Переменные
        private MainWindow m_MainWindow;
        private MyTask m_ParentTask;
        private int m_TaskID;
        private bool m_IsCurrent;
        private System.Windows.Media.SolidColorBrush m_BackgroundBrush;

        public static SolidColorBrush m_NormalStateBrush = 
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(230, 230, 230));
        public static SolidColorBrush m_HighlightedBrush = 
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
        public static SolidColorBrush m_SelectedBrush =
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(50, 180, 180));
        public static SolidColorBrush m_CompletedTaskBrush = 
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(25, 200, 25));
        public static SolidColorBrush m_FailedTaskBrush =
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 50, 50));

        public enum TaskElementState
        {
            Normal,
            Highlighted,
            Selected,
            Completed,
            Failed
        }
        private TaskElementState m_TaskElementState;
        private TaskElementState m_PreviousTaskElementState;

        public event PropertyChangedEventHandler PropertyChanged;

        //private void OnPropertyChanged(string propertyName)
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        // Конструкторы
        #region Конструкторы
        public TaskElement()
        {
            InitializeComponent();
            EventsAndOtherSettings();
        }

        public TaskElement(MainWindow mainWindow, MyTask task, int processId)
        {
            InitializeComponent();
            m_MainWindow = mainWindow;
            m_ParentTask = task;
            m_TaskID = processId;
            m_IsCurrent = false;
            m_TaskElementState = TaskElementState.Normal;
            m_PreviousTaskElementState = m_TaskElementState;
            SetBackgroundBrush = TaskElement.m_NormalStateBrush;
            EventsAndOtherSettings();
        }
        #endregion

        // Свойства
        #region Свойства

        public bool IsCurrent
        {
            get { return m_IsCurrent; }
            set { m_IsCurrent = value; }
        }

        public System.Windows.Media.SolidColorBrush SetBackgroundBrush
        {
            get { return m_BackgroundBrush; }
            set
            {
                if (value != null)
                {
                    m_BackgroundBrush = value;
                    OnPropertyChanged("SetBackgroundBrush");
                }
                else
                {
                    m_BackgroundBrush = m_NormalStateBrush;
                    OnPropertyChanged("SetBackgroundBrush");
                }
            }
        }
        #endregion

        // Методы
        #region Методы

        public void SetImageToImagePreviewElement(string path)
        {
            if (!String.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    image_SrcImagePreview.Source = new BitmapImage(new Uri(path));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Не удалось установить миниатюру: " + path);
                    image_SrcImagePreview.Source = 
                        new BitmapImage(new Uri(Properties.Settings.Default.ImageNotAvailableRelativePath, UriKind.Relative));
                }
            }
            else
            {
                Console.WriteLine("Не найдена миниатюра или указан некорректный путь");
                //image_SrcImagePreview.Source = null;
                image_SrcImagePreview.Source = 
                    new BitmapImage(new Uri(Properties.Settings.Default.ImageNotAvailableRelativePath, UriKind.Relative));
            }
        }

        public void SetTaskIDToLabel(int id = 0)
        {
            label_TaskID.Content = id.ToString();
        }

        public void SetUtilityNameToLabel(string utilityName)
        {
            if (utilityName != null)
            {
                label_UtilityName.Content = utilityName;
            }
            else
            {
                label_UtilityName.Content = String.Empty;
            }
        }

        public void SetFileNameToLabelAndToolTip(string inputFileName)
        {
            if (!String.IsNullOrEmpty(inputFileName))
            {
                label_ImageName.Content = System.IO.Path.GetFileName(inputFileName);
                label_ImageName.ToolTip = inputFileName;
            }
            else
            {
                label_ImageName.Content = String.Empty;
                label_ImageName.ToolTip = String.Empty;
            }
        }

        public void SetTaskElementState(TaskElementState taskElementState)
        {
            if (m_TaskElementState != TaskElementState.Highlighted)
            {
                m_PreviousTaskElementState = m_TaskElementState;
            }
            m_TaskElementState = taskElementState;
            ChangeBackground();
        }

        public void SetPreviousState()
        {
            m_PreviousTaskElementState = m_TaskElementState;
        }

        public void ReturnToPreviousElementState()
        {
            m_TaskElementState = m_PreviousTaskElementState;
            ChangeBackground();
        }

        private void ChangeBackground()
        {
            switch (m_TaskElementState)
            {
                case TaskElementState.Normal:
                    this.Background = TaskElement.m_NormalStateBrush;
                    break;
                case TaskElementState.Highlighted:
                    this.Background = TaskElement.m_HighlightedBrush;
                    break;
                case TaskElementState.Selected:
                    this.Background = TaskElement.m_SelectedBrush;
                    break;
                case TaskElementState.Completed:
                    //this.Background = m_CompletedTaskBrush;
                    SetBackgroundBrush = TaskElement.m_CompletedTaskBrush;
                    break;
                case TaskElementState.Failed:
                    //this.Background = m_FailedTaskBrush;
                    SetBackgroundBrush = TaskElement.m_FailedTaskBrush;
                    break;
                default:
                    this.Background = TaskElement.m_NormalStateBrush;
                    break;
            }
        }


        private void EventsAndOtherSettings()
        {
            this.MouseEnter += new MouseEventHandler(taskElement_MouseEnter);
            this.MouseLeave += new MouseEventHandler(taskElement_MouseLeave);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(taskElement_MouseLeftButtonDown);
            this.MouseRightButtonDown += new MouseButtonEventHandler(taskElement_MouseRightButtonDown);
            this.MouseRightButtonUp += new MouseButtonEventHandler(taskElement_MouseRightButtonUp);
            image_SrcImagePreview.MouseEnter += new MouseEventHandler(Image_SrcImagePreview_MouseEnter);
            image_SrcImagePreview.MouseLeave += new MouseEventHandler(Image_SrcImagePreview_MouseLeave);
            (this.ContextMenu.Items.GetItemAt(0) as MenuItem).Click +=
                new RoutedEventHandler(taskElement_ContextMenu_RunTask_Click);
            (this.ContextMenu.Items.GetItemAt(2) as MenuItem).Click += 
                new RoutedEventHandler(taskElement_ContextMenu_EditTask_Click);
            (this.ContextMenu.Items.GetItemAt(4) as MenuItem).Click +=
                new RoutedEventHandler(taskElement_ContextMenu_RemoveTask_Click);
            Binding myBinding = new Binding();
            myBinding.Path = new PropertyPath("SetBackgroundBrush");
            myBinding.Mode = BindingMode.TwoWay;
            //myBinding.Source = m_BackgroundBrush;
            myBinding.Source = this;
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.SetBinding(UserControl.BackgroundProperty, myBinding);
        }
        #endregion

        // Обработчики событий
                #region Обработчики событий
        private void taskElement_MouseEnter(object sender, RoutedEventArgs e)
        {
            TaskElement taskElement = sender as TaskElement;
            if (taskElement.IsCurrent != true)
            {
                //taskElement.Background = m_HighlightedBrush;
                m_PreviousTaskElementState = m_TaskElementState;
                //m_MainWindow.CurrentTask.GetTaskElement.SetTaskElementState(TaskElementState.Highlighted);
                taskElement.SetTaskElementState(TaskElementState.Highlighted);
            }
        }

        private void taskElement_MouseLeave(object sender, RoutedEventArgs e)
        {
            TaskElement taskElement = sender as TaskElement;
            if (taskElement.IsCurrent != true)
            {
                //taskElement.Background = m_NormalStateBrush;
                /*
                m_MainWindow.CurrentTask.GetTaskElement.SetTaskElementState(TaskElementState.Normal);
                m_TaskElementState = m_PreviousTaskElementState;
                */
                taskElement.ReturnToPreviousElementState();
            }
        }

        private void taskElement_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            TaskElement taskElement = sender as TaskElement;

            if (m_MainWindow.CurrentTask != null)
            {
                //m_MainWindow.CurrentTask.GetTaskElement.Background = m_NormalStateBrush;
                //m_MainWindow.CurrentTask.GetTaskElement.SetTaskElementState(TaskElementState.Highlighted);
                m_MainWindow.CurrentTask.GetTaskElement.ReturnToPreviousElementState();
                m_MainWindow.CurrentTask.GetTaskElement.IsCurrent = false;
                if (taskElement == m_MainWindow.CurrentTask.GetTaskElement)
                {
                    m_MainWindow.CurrentTask = null;
                    return;
                }
            }

            MyTask selectedTask = m_MainWindow.GetTasksList.Where(
                x => x.GetTaskID == taskElement.m_TaskID
                ).FirstOrDefault();
            //taskElement.Background = m_SelectedBrush;
            //m_MainWindow.CurrentTask.GetTaskElement.SetTaskElementState(TaskElementState.Selected);
            //taskElement.SetPreviousState();
            taskElement.SetTaskElementState(TaskElementState.Selected);
            taskElement.IsCurrent = true;
            m_MainWindow.CurrentTask = selectedTask;

        }

        private void Image_SrcImagePreview_MouseEnter(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Image imageControl = sender as System.Windows.Controls.Image;

            double h1 = imageControl.Height;
            double h2 = 192;
            DoubleAnimation animHeight = new DoubleAnimation();
            animHeight.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            animHeight.From = h1;
            animHeight.To = h2;
            animHeight.FillBehavior = FillBehavior.HoldEnd;

            double w1 = imageControl.Width;
            double w2 = 192;
            DoubleAnimation animWidth = new DoubleAnimation();
            animWidth.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            animWidth.From = w1;
            animWidth.To = w2;
            animWidth.FillBehavior = FillBehavior.HoldEnd;

            imageControl.BeginAnimation(System.Windows.Controls.Image.HeightProperty, animHeight);
            imageControl.BeginAnimation(System.Windows.Controls.Image.WidthProperty, animWidth);
        }

        private void Image_SrcImagePreview_MouseLeave(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Image imageControl = sender as System.Windows.Controls.Image;

            double h1 = imageControl.Height;
            double h2 = 64;
            DoubleAnimation animHeight = new DoubleAnimation();
            animHeight.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            animHeight.From = h1;
            animHeight.To = h2;
            animHeight.FillBehavior = FillBehavior.HoldEnd;

            double w1 = imageControl.Width;
            double w2 = 64;
            DoubleAnimation animWidth = new DoubleAnimation();
            animWidth.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            animWidth.From = w1;
            animWidth.To = w2;
            animWidth.FillBehavior = FillBehavior.HoldEnd;

            imageControl.BeginAnimation(System.Windows.Controls.Image.HeightProperty, animHeight);
            imageControl.BeginAnimation(System.Windows.Controls.Image.WidthProperty, animWidth);
        }

        private void taskElement_MouseRightButtonDown(object sender, RoutedEventArgs e)
        {
            this.ContextMenu.IsOpen = true;
        }

        private void taskElement_MouseRightButtonUp(object sender, RoutedEventArgs e)
        {
            //this.ContextMenu.IsOpen = true;
            e.Handled = true;
        }

        private void taskElement_ContextMenu_RunTask_Click(object sender, RoutedEventArgs e)
        {
            m_MainWindow.RunSelectedTask(m_ParentTask);
        }

        private void taskElement_ContextMenu_EditTask_Click(object sender, RoutedEventArgs e)
        {
            m_MainWindow.EditSelectedTask(m_ParentTask);
        }

        private void taskElement_ContextMenu_RemoveTask_Click(object sender, RoutedEventArgs e)
        {
            m_MainWindow.RemoveTask(m_ParentTask);
        }

        #endregion
    }
}
