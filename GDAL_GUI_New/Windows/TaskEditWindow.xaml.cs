﻿using System;
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
using System.Diagnostics;
using System.Text.RegularExpressions;
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
        // Этот флаг используется при закрытии данного окна, чтобы выводить/не выводить диалог
        private bool m_IsThisTaskAdded;
        private string[] m_InputFiles;
        private string m_OutputFile;
        private Process m_ProcessForVersion;
        private Version m_UtilityVersion;

        private enum InputMode
        {
            OneFile,
            MultipleFiles,
            FromAnotherUtility,
            TxtList
        };
        private InputMode m_CurrentMode;

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
            m_CurrentMode = InputMode.OneFile;

            // Инициализируем экземпляр процесса, чтобы узнавать версии утилит
            m_ProcessForVersion = new Process();
            m_ProcessForVersion.StartInfo.UseShellExecute = false;
            m_ProcessForVersion.StartInfo.CreateNoWindow = true;
            m_ProcessForVersion.StartInfo.RedirectStandardOutput = true;
            m_ProcessForVersion.StartInfo.Arguments = "--version";
            m_ProcessForVersion.OutputDataReceived += new DataReceivedEventHandler(ProcessForVerion_DataReceived);

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
            TaskEdit_Menu_AddTask.Click += 
                new RoutedEventHandler(TaskEdit_Menu_AddTask_Click);
            TaskEdit_Menu_Exit.Click += 
                new RoutedEventHandler(TaskEdit_Menu_ExitWithoutAdding_Click);
            TaskEdit_Menu_ManualInput.Click += 
                new RoutedEventHandler(TaskEdit_Menu_InputParametersManually_Click);
            ComboBox_UtilitiesNames.SelectionChanged += 
                new SelectionChangedEventHandler(ComboBox_UtilitiesNames_SelectionChanged);
            RadioButton_InputMode_OneFile.Checked +=
                new RoutedEventHandler(RadioButton_InputMode_Checked);
            RadioButton_InputMode_MultipleFiles.Checked +=
                new RoutedEventHandler(RadioButton_InputMode_Checked);
            RadioButton_InputMode_FromAnotherUtility.Checked +=
                new RoutedEventHandler(RadioButton_InputMode_Checked);
            RadioButton_InputMode_TxtList.Checked +=
                new RoutedEventHandler(RadioButton_InputMode_Checked);
            ListBox_AvailableParameters.SelectionChanged +=
                new SelectionChangedEventHandler(ListBox_AvailableParameters_SelectionChanged);

            Binding outputBinding = new Binding()
            {
                Path = new PropertyPath("OutputFilePath"),
                Mode = BindingMode.TwoWay,
                Source = this,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            TextBox_OutputFile.SetBinding(TextBox.TextProperty, outputBinding);

            // Дополнительная инициализация параметров объектов
            RadioButton_InputMode_OneFile.Tag = InputMode.OneFile;
            RadioButton_InputMode_MultipleFiles.Tag = InputMode.MultipleFiles;
            RadioButton_InputMode_FromAnotherUtility.Tag = InputMode.FromAnotherUtility;
            RadioButton_InputMode_TxtList.Tag = InputMode.TxtList;
        }

        private void ConnectToDbAndGetNecessaryData()
        {
            if (DataBaseControl.ConnectToDB() == true)
            {
                m_UtilitiesNames = DataBaseControl.GetUtilitiesNames();
                ComboBox_UtilitiesNames.ItemsSource = m_UtilitiesNames;
                ComboBox_UtilitiesNames.SelectedIndex = 0;
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            switch (m_CurrentMode)
            {
                case InputMode.OneFile:
                    openFileDialog. Multiselect = false;
                    openFileDialog.CheckPathExists = true;
                    if (openFileDialog.ShowDialog() == true)
                    {
                        m_InputFiles = openFileDialog.FileNames;
                    }
                    break;
                case InputMode.MultipleFiles:
                    openFileDialog.Multiselect = true;
                    openFileDialog.CheckPathExists = true;
                    if (openFileDialog.ShowDialog() == true)
                    {
                        m_InputFiles = openFileDialog.FileNames;
                    }
                    break;
                case InputMode.FromAnotherUtility:
                    MessageBox.Show("Заглушка. Выбор одного из добавленных заданий, чтобы взять " +
                                    "путь выходного файла в качестве входного для данного задания");
                    break;
                case InputMode.TxtList:
                    openFileDialog.Multiselect = false;
                    openFileDialog.CheckPathExists = true;
                    openFileDialog.Filter = "Text Files (*.txt)|*.txt";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        m_InputFiles = openFileDialog.FileNames;
                    }
                    break;
            }

            /*
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
            */
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

        private void TaskEdit_Menu_AddTask_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Заглушка. AddTask");

            m_MainWindow.AddNewTask(m_Task);
            m_IsThisTaskAdded = true;
            this.Close();
        }

        private void TaskEdit_Menu_ExitWithoutAdding_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TaskEdit_Menu_InputParametersManually_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Заглушка. AddParametersManually");
        }

        // При смене утилиты определяется её версия, 
        // очищается список доступных параметров и загружается новый список,
        // а также выводится описание выбранной утилиты
        private void ComboBox_UtilitiesNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получаем имя выбранной утилиты (т.к. выбирается только один объект,
            // то в массиве AddedItems он будет всегда находиться под индексом 0)
            string name = e.AddedItems[0].ToString();

            // Получаем запускаем процесс с параметром --version, 
            // чтобы получить версию выбранной утилиты
            m_ProcessForVersion.StartInfo.FileName = 
                Properties.Settings.Default.UtilitiesDirectory + name;
            m_ProcessForVersion.Start();
            m_ProcessForVersion.BeginOutputReadLine();
            m_ProcessForVersion.WaitForExit();
            m_ProcessForVersion.CancelOutputRead();
            m_ProcessForVersion.Close();
            TextBox_UtilityVersion.Text = m_UtilityVersion.ToString();

            // Получаем параметры для данной утилиты, 
            // проверяем их совместимость с выбранной версией утилиты и
            // выводим список в ListBox
            m_UtilityParameters = DataBaseControl.GetUtilityParameters(name);
            m_UtilityParameters.RemoveAll(x => new Version(x.GetDataRow["Version"].ToString()) >= m_UtilityVersion);
            ListBox_AvailableParameters.ItemsSource = m_UtilityParameters;

            // Выводим описание выбранной утилиты в соответствии с 
            // выбранным в настройках языком
            m_UtilityInfo = DataBaseControl.GetUtilityInfo(e.AddedItems[0].ToString());
            TextBlock_UtilityDescription.Text = 
                m_UtilityInfo["Description"+Properties.Settings.Default.DescriptionsLanguage].ToString();
        }

        // Меняет режим выбора входного файла при выборе одного из RadioButton
        private void RadioButton_InputMode_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rB = sender as RadioButton;
            if (rB.Tag != null & rB.IsChecked == true)
            {
                m_CurrentMode = (InputMode) rB.Tag;
            }
        }

        // При выборе параметра из ListBox проверяется наличие у него дополнительных параметров.
        // Эти параметры либо добавляются в StackPanel, либо убираются оттуда, если с параметра
        // сняли выделение
        // ДОДЕЛАТЬ: Если тип дополнительного параметра - Selecting
        // ДОДЕЛАТЬ: Добавлять для кнопок обработчик события нажатия, при котором
        // будет добавляться новая строкав DataGrid
        private void ListBox_AvailableParameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                MyDataRow currentParameter = e.AddedItems[0] as MyDataRow;
                if ((bool) currentParameter.GetDataRow["IsThereAdditionalParameters"] == true)
                {
                    GroupBox gB = new GroupBox()
                    {
                        Tag = currentParameter.GetDataRow["NameOfTheParameter"].ToString(),
                        Header = currentParameter.GetDataRow["NameOfTheParameter"].ToString()
                    };
                    Grid grid = new Grid();
                    string[] additionalParameters =
                            currentParameter.GetDataRow["AdditionalParameters"].ToString().Split(
                                new char[2] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if ((bool) currentParameter.GetDataRow["MultipleCalls"] == true)
                    {
                        RowDefinition rowDataGrid = new RowDefinition();
                        RowDefinition rowButton = new RowDefinition();
                        grid.RowDefinitions.Add(rowDataGrid);
                        grid.RowDefinitions.Add(rowButton);
                        DataGrid dataGrid = new DataGrid()
                        {
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            GridLinesVisibility = DataGridGridLinesVisibility.All
                        };
                        
                        foreach (string parameter in additionalParameters)
                        {
                            DataGridTextColumn textColumn = new DataGridTextColumn();
                            textColumn.Header = parameter;
                            textColumn.Binding = new Binding(parameter);
                            dataGrid.Columns.Add(textColumn);
                        }

                        Button button = new Button()
                        {
                            Content = "Add new row",
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Bottom
                        };

                        Grid.SetRow(dataGrid, 0);
                        Grid.SetRow(button, 1);
                        grid.Children.Add(dataGrid);
                        grid.Children.Add(button);
                        gB.Content = grid;
                        StackPanel_AdditionalParameters.Children.Add(gB);
                    }
                    else
                    {
                        int rowCount = 0;
                        foreach (string parameter in additionalParameters)
                        {
                            grid.RowDefinitions.Add(new RowDefinition());
                            grid.RowDefinitions.Add(new RowDefinition());
                            Label label = new Label()
                            {
                                Name = "Label_" + parameter,
                                Content = parameter,
                                VerticalAlignment = VerticalAlignment.Top,
                                HorizontalAlignment = HorizontalAlignment.Left
                            };
                            TextBox textBox = new TextBox()
                            {
                                Name = "TextBox_" + parameter,
                                Tag = parameter,
                                VerticalAlignment = VerticalAlignment.Bottom,
                                HorizontalAlignment = HorizontalAlignment.Stretch
                            };
                            Grid.SetRow(label, rowCount++);
                            Grid.SetRow(textBox, rowCount);
                            rowCount++;
                            grid.Children.Add(label);
                            grid.Children.Add(textBox);
                        }
                        gB.Content = grid;
                        StackPanel_AdditionalParameters.Children.Add(gB);
                    }
                }
            }
            else if (e.RemovedItems != null && e.RemovedItems.Count > 0)
            {
                MyDataRow currentParameter = e.RemovedItems[0] as MyDataRow;
                foreach (var child in StackPanel_AdditionalParameters.Children)
                {
                    if (child is GroupBox)
                    {
                        GroupBox gB = child as GroupBox;
                        if (gB.Tag == currentParameter.GetDataRow["NameOfTheParameter"])
                        {
                            StackPanel_AdditionalParameters.Children.Remove(gB);
                            return;
                        }
                    }
                }
            }
        }

        // Обрабатывает событие Получения выходных данных от запущенного процесса.
        // Получаем версию утилиты (нужно доработать, чтобы учитывались постфиксы, вроде dev, b и т.д.)
        private void ProcessForVerion_DataReceived(object sender, DataReceivedEventArgs e)
        {
            //string[] splittedOutput = e.Data.Split(',');
            if (!String.IsNullOrEmpty(e.Data))
            {
                Regex rgx = new Regex(@"\d+\.\d+\.\d");
                m_UtilityVersion = new Version(rgx.Match(e.Data).ToString());
            }
        }
        #endregion
    }
}
