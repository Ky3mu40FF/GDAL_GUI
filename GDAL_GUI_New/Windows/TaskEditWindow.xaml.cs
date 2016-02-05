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
using System.Windows.Shapes;
using System.Data;
using System.Data.Common;
using Microsoft.Win32;

namespace GDAL_GUI_New
{
    /// <summary>
    /// Логика взаимодействия для TaskEditWindow.xaml
    /// </summary>
    public partial class TaskEditWindow : Window, INotifyPropertyChanged
    {
        // Переменные
                #region Переменные
        private MainWindow m_MainWindow;
        private MyTask m_Task;
        private List<string> m_UtilitiesNames;
        private List<MyDataRow> m_UtilityParameters;
        private DataRow m_UtilityInfo;
        // Используется при закрытии данного окна, чтобы выводить/не выводить диалог
        private bool m_IsThisTaskAdded;
        private string[] m_InputFiles;
        private string m_OutputFile;

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        // Конструкторы
                #region Конструкторы
        public TaskEditWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            m_MainWindow = mainWindow;
            m_Task = new MyTask(m_MainWindow);
            m_IsThisTaskAdded = false;
            m_UtilitiesNames = new List<string>();
            m_UtilityParameters = new List<MyDataRow>();
            EventAndPropertiesInitialization();
            ConnectToDbAndGetNecessaryData();
        }
        #endregion

        // Свойства
                #region Свойства

        public string OutputFilePath
        {
            get { return m_OutputFile; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    m_OutputFile = value;
                    OnPropertyChanged("OutputFilePath");
                }
            }
        }
        #endregion

        // Методы
                #region Методы
        private void EventAndPropertiesInitialization()
        {
            // Подписка на события
            this.Closing += 
                new CancelEventHandler(ThisWindow_Closing);
            Button_BrowseInputFile.Click +=
                new RoutedEventHandler(Button_BrowseInputFile_Click);
            Button_BrowseOutputFile.Click +=
                new RoutedEventHandler(Button_BrowseOutputFile_Click);
            Button_AddTask.Click += 
                new RoutedEventHandler(Button_AddTask_Click);
            Button_Exit.Click += 
                new RoutedEventHandler(Button_ExitWithoutSave_Click);
            Button_InputParametersManually.Click += 
                new RoutedEventHandler(Button_InputParametersManually_Click);
            ComboBox_UtilitiesNames.SelectionChanged += 
                new SelectionChangedEventHandler(ComboBox_UtilitiesNames_SelectionChanged);
            Expander_UtilityAndParameterDescription.Expanded += 
                new RoutedEventHandler(Expander_UtilityAndParameterDescription_Expanded);
            Expander_UtilityAndParameterDescription.Collapsed +=
                new RoutedEventHandler(Expander_UtilityAndParameterDescription_Collapsed);

            
            Binding outputBinding = new Binding()
            {
                Path = new PropertyPath("OutputFilePath"),
                Mode = BindingMode.TwoWay,
                Source = this,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            TextBox_OutputFile.SetBinding(TextBox.TextProperty, outputBinding);
            //TextBox_OutputFile.DataContext = this;
            
            
                //.DataBindings.Add(new Binding("Text", this, "Foo"));
        }

        private void ConnectToDbAndGetNecessaryData()
        {
            if (DataBaseControl.ConnectToDB() == true)
            {
                m_UtilitiesNames = DataBaseControl.GetUtilitiesNames();
                ComboBox_UtilitiesNames.ItemsSource = m_UtilitiesNames;
                ComboBox_UtilitiesNames.SelectedIndex = 0;
                m_UtilityInfo = DataBaseControl.GetUtilityInfo(ComboBox_UtilitiesNames.SelectedValue.ToString());
                TextBlock_UtilityDescription.Text = m_UtilityInfo["DescriptionEng"].ToString();
            }
            else
            {
                return;
            }
        }

        //protected virtual void OnPropertyChanged(string propertyName = null)
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        // Обработчики событий
        #region Обработчики событий

        private void ThisWindow_Closing(object sender, CancelEventArgs e)
        {
            if (m_IsThisTaskAdded == true)
            {
                DataBaseControl.CloseConnection();
                return;
            }
            MessageBoxResult result = MessageBox.Show("Изменения не будут сохранены. Вы уверены, что хотите выйти?",
                "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                DataBaseControl.CloseConnection();
                return;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void Button_BrowseInputFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                CheckFileExists = true,
                CheckPathExists = true
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                m_InputFiles = openFileDialog.FileNames;
            }
        }

        private void Button_BrowseOutputFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                //CheckFileExists = true,
                CheckPathExists = true
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                //m_OutputFile = saveFileDialog.FileName;
                OutputFilePath = saveFileDialog.FileName;
            }
        }

        private void Button_AddTask_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Заглушка. AddTask");

            m_MainWindow.AddNewTask(m_Task);
            m_IsThisTaskAdded = true;
            this.Close();
        }

        private void Button_ExitWithoutSave_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_InputParametersManually_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Заглушка. AddParametersManually");
        }

        private void ComboBox_UtilitiesNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = e.AddedItems[0].ToString();
            m_UtilityParameters = DataBaseControl.GetUtilityParameters(name);
            ListBox_AvailableParameters.Items.Clear();
            ListBox_AvailableParameters.ItemsSource = m_UtilityParameters;
        }

        private void Expander_UtilityAndParameterDescription_Expanded(object sender, RoutedEventArgs e)
        {
            this.Height += 55;
        }

        private void Expander_UtilityAndParameterDescription_Collapsed(object sender, RoutedEventArgs e)
        {
            this.Height -= 55;
        }
        #endregion
    }
}
