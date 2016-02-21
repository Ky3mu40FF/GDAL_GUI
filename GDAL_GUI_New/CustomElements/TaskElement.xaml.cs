
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
using System.Drawing;
using System.IO;
using System.Windows.Media.Animation;

namespace GDAL_GUI_New
{
    /// <summary>
    /// Логика взаимодействия для TaskElement.xaml
    /// </summary>
    public partial class TaskElement : UserControl
    {
        // Переменныые
                #region Переменные
        private MainWindow m_MainWindow;
        private MyTask m_ParentTask;
        private int m_TaskID;
        private bool m_IsCurrent;

        private SolidColorBrush m_NormalStateBrush = 
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(230, 230, 230));
        private SolidColorBrush m_HighlightedBrush = 
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
        private SolidColorBrush m_SelectedBrush =
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 255));
        private SolidColorBrush m_CompletedTaskBrush = 
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0));
        private SolidColorBrush m_FailedTaskBrush =
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));

        public enum TaskElementState
        {
            Normal,
            Highlighted,
            Selected,
            Completed,
            Failed
        }
        private TaskElementState m_TaskElementState;

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
            EventsAndOtherSettings();
            m_MainWindow = mainWindow;
            m_ParentTask = task;
            m_TaskID = processId;
            m_IsCurrent = false;
            m_TaskElementState = TaskElementState.Normal;
        }
        #endregion

        // Свойства
                #region Свойства
        public string SetImage
        {
            set
            {
                /*
                CreateThumbnail(value, (value+"thumbnail.jpg"), 128,128);
                ImageSource imgSource = new BitmapImage(new Uri(value + "thumbnail.jpg"));
                image_SrcImagePreview.Source = imgSource;
                label_PictureName.Content = value.Substring(value.LastIndexOf('\\') + 1);
                label_PictureName.ToolTip = label_PictureName.Content;
                */
                image_SrcImagePreview.Source = new BitmapImage(new Uri(value));
            }
        }

        public int SetTaskID
        {
            set { label_TaskID.Content = value.ToString(); }
        }

        public string SetUtilityName
        {
            set { label_UtilityName.Content = value; }
        }

        public string SetFileName
        {
            set
            {
                label_ImageName.Content = System.IO.Path.GetFileName(value);
                label_ImageName.ToolTip = value;
            }
        }

        public bool IsCurrent
        {
            get { return m_IsCurrent; }
            set { m_IsCurrent = value; }
        }
        #endregion

        // Методы
        #region Методы

        public void SetTaskElementState(TaskElementState taskElementState)
        {
            m_TaskElementState = taskElementState;
            ChangeBackground();
        }

        private void ChangeBackground()
        {
            switch (m_TaskElementState)
            {
                case TaskElementState.Normal:
                    this.Background = m_NormalStateBrush;
                    break;
                case TaskElementState.Highlighted:
                    this.Background = m_HighlightedBrush;
                    break;
                case TaskElementState.Selected:
                    this.Background = m_SelectedBrush;
                    break;
                case TaskElementState.Completed:
                    //this.Background = m_CompletedTaskBrush;
                    break;
                case TaskElementState.Failed:
                    this.Background = m_FailedTaskBrush;
                    break;
                default:
                    this.Background = m_NormalStateBrush;
                    break;
            }
        }

        private void EventsAndOtherSettings()
        {
            this.MouseEnter += new MouseEventHandler(taskElement_MouseEnter);
            this.MouseLeave += new MouseEventHandler(taskElement_MouseLeave);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(taskElement_MouseLeftButtonDown);
            this.image_SrcImagePreview.MouseEnter += new MouseEventHandler(Image_SrcImagePreview_MouseEnter);
            this.image_SrcImagePreview.MouseLeave += new MouseEventHandler(Image_SrcImagePreview_MouseLeave);
        }
        #endregion

        // Обработчики событий
                #region Обработчики событий
        private void taskElement_MouseEnter(object sender, RoutedEventArgs e)
        {
            TaskElement taskElement = sender as TaskElement;
            if (taskElement.IsCurrent != true)
            {
                taskElement.Background = m_HighlightedBrush;
            }
        }

        private void taskElement_MouseLeave(object sender, RoutedEventArgs e)
        {
            TaskElement taskElement = sender as TaskElement;
            if (taskElement.IsCurrent != true)
            {
                taskElement.Background = m_NormalStateBrush;
            }
        }

        private void taskElement_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            TaskElement taskElement = sender as TaskElement;

            if (m_MainWindow.CurrentTask != null)
            {
                m_MainWindow.CurrentTask.GetTaskElement.Background = m_NormalStateBrush;
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
            m_MainWindow.CurrentTask = selectedTask;
            taskElement.Background = m_SelectedBrush;
            taskElement.IsCurrent = true;

            /*
            foreach (MyProcess process in m_MainWindow.GetProcessesList)
            {
                if(process.GetProcessID == taskElement.m_ProcessID)
                {
                    m_MainWindow.CurrentProcess.GetTaskElement.IsCurrent = false;
                    m_MainWindow.CurrentProcess = process;
                    taskElement.Background = m_SelectedBrush;
                    taskElement.IsCurrent = true;
                    return;
                }
            }
            */
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
        #endregion
        
    }
}
