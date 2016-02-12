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
        private string m_FormedParametersArgument;
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
        private List<DataTable> m_AdditionalParametersInputs;

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
            m_FormedParametersArgument = String.Empty;
            m_AdditionalParametersInputs = new List<DataTable>();

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

        private void ParameterAdded(SelectionChangedEventArgs e)
        {
            // Получаем отмеченный параметр
            MyDataRow currentParameter = e.AddedItems[0] as MyDataRow;
            // Если для этого параметра есть дополнительные параметры, то
            // добавим элементы ввода для этих дополнительных параметров в StackPanel
            if ((bool)currentParameter.GetDataRow["IsThereAdditionalParameters"] == true)
            {
                // Создаём GroupBox, в котором будут храниться элементы ввода
                GroupBox gB = new GroupBox()
                {
                    Tag = currentParameter.GetDataRow["NameOfTheParameter"].ToString(),
                    Header = currentParameter.GetDataRow["NameOfTheParameter"].ToString()
                };
                Grid grid = new Grid();
                // Получаем все дополнительные параметры
                string[] additionalParameters =
                        currentParameter.GetDataRow["AdditionalParameters"].ToString().Split(
                            new char[2] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                // Проверяем, должен ли пользователь вводить эти параметры  вручную.
                // Если да, то создаём поля для ввода
                if (currentParameter.GetDataRow["AdditionalParametersType"].ToString() == "ManualInput")
                {
                    // Проверяем, можно ли этот параметр вызвать несколько раз
                    if ((bool) currentParameter.GetDataRow["MultipleCalls"] == true)
                    {
                        // Создаём определения строк и столбцов для сетки в GroupBox
                        RowDefinition rowDataGrid = new RowDefinition();
                        RowDefinition rowButton = new RowDefinition();
                        grid.RowDefinitions.Add(rowDataGrid);
                        grid.RowDefinitions.Add(rowButton);
                        // Создаём экземпляр DataGrid, в который будем вводить дополнительные параметры
                        DataGrid dataGrid = new DataGrid()
                        {
                            Tag = currentParameter.GetDataRow["NameOfTheParameter"].ToString(),
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            GridLinesVisibility = DataGridGridLinesVisibility.All,
                            CanUserAddRows = true
                        };
                        // Создаём таблицу, в которой будут храниться введённые 
                        // значения дополнительных параметров
                        DataTable table = new DataTable()
                        {
                            TableName = currentParameter.GetDataRow["NameOfTheParameter"].ToString()
                        };
                        // Для каждого дополнительного параметра создаём столбец в таблице
                        foreach (string parameter in additionalParameters)
                        {
                            DataColumn column = new DataColumn()
                            {
                                Caption = parameter,
                                ColumnName = parameter
                            };
                            table.Columns.Add(column);
                        }
                        // Добавляем созданную таблицу в список таблиц для доп. параметров
                        // и назначаем экземпляру DataGrid в качестве источника эту таблицу
                        // из списка
                        m_AdditionalParametersInputs.Add(table);
                        dataGrid.ItemsSource = m_AdditionalParametersInputs.Last<DataTable>().DefaultView;
                        // Создаём кнопку, при нажатии на которую будет добавляться новая строка
                        // в DataGrid
                        // (В принципе не особо нужна, т.к. DataGrid сам добавляет строки по необходимости)
                        Button button = new Button()
                        {
                            Content = "Add new row",
                            Tag = currentParameter.GetDataRow["NameOfTheParameter"].ToString(),
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Bottom
                        };
                        // Добавляем этой кнопке обработчик события
                        button.Click += new RoutedEventHandler(Button_AddRow_Click);
                        // Формируем GroupBox и добавляем его в StackPanel
                        Grid.SetRow(dataGrid, 0);
                        Grid.SetRow(button, 1);
                        grid.Children.Add(dataGrid);
                        grid.Children.Add(button);
                        gB.Content = grid;
                        StackPanel_AdditionalParameters.Children.Add(gB);
                    }
                    // Если этот параметр может вызываться только единожды
                    else
                    {
                        // Переменная-счётчик для строк (GridRow) в сетке (Grid)
                        int rowCount = 0;
                        // Для каждого дополнительного параметра добавляем две строки 
                        // в сетке (для Label и TextBox) и сами элементы: Label и TextBox
                        foreach (string parameter in additionalParameters)
                        {
                            // Добавляем две строки в сетку
                            grid.RowDefinitions.Add(new RowDefinition());
                            grid.RowDefinitions.Add(new RowDefinition());
                            // Создаём экземпляр Label
                            Label label = new Label()
                            {
                                Name = "Label_" + parameter,
                                Content = parameter,
                                VerticalAlignment = VerticalAlignment.Top,
                                HorizontalAlignment = HorizontalAlignment.Left
                            };
                            // Создаём экземпляр TextBox
                            TextBox textBox = new TextBox()
                            {
                                Name = "TextBox_" + parameter,
                                Tag = parameter,
                                VerticalAlignment = VerticalAlignment.Bottom,
                                HorizontalAlignment = HorizontalAlignment.Stretch
                            };
                            // Устанавливаем для Label и TextBox индексы добавленных строк
                            // (т.е. в каких строках в Grid элементы находятся)
                            // (прямо при добавлении сразу инкрементируем счётчик)
                            Grid.SetRow(label, rowCount++);
                            Grid.SetRow(textBox, rowCount++);
                            //rowCount++;
                            // Добавляем созданные элементы в Grid
                            grid.Children.Add(label);
                            grid.Children.Add(textBox);
                        }
                        // Добавляем Grid в GroupBox
                        gB.Content = grid;
                        // Добавляем GroupBox в StackPanel
                        StackPanel_AdditionalParameters.Children.Add(gB);
                    }
                }
                // Если параметр требует от пользователя выбрать один из вариантов
                else if (currentParameter.GetDataRow["AdditionalParametersType"].ToString() == "Selecting")
                {
                    // Создаём новый ComboBox, в котором будeт храниться доступные варианты
                    ComboBox cB = new ComboBox()
                    {
                        // Возможно это (тэг) не потребуется
                        Tag = currentParameter.GetDataRow["NameOfTheParameter"]
                    };
                    // Добавляем источник данных для ComboBox и добавляем его 
                    // в GroupBox и затем в StackPanel
                    cB.ItemsSource = additionalParameters;
                    gB.Content = cB;
                    StackPanel_AdditionalParameters.Children.Add(gB);
                }
            }
        }

        private void ParameterRemoved(SelectionChangedEventArgs e)
        {
            // Получаем элемент, с которого сняли выделение
            MyDataRow currentParameter = e.RemovedItems[0] as MyDataRow;
            // В StackPanel, где хранятся элементы ввода дополнительных параметров,
            // ищем и удаляем GroupBox, в котором хранятся доп. параметры
            // текущего параметра
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
            // Если этот параметр можно вызывать несколько раз, то заодно
            // удаляем DataTable для этого параметра из списка
            if ((bool)currentParameter.GetDataRow["MultipleCalls"] == true)
            {
                m_AdditionalParametersInputs.RemoveAll(
                    x => x.TableName == currentParameter.GetDataRow["NameOfTheParameter"].ToString());
            }
        }

        private void ParametersArgumentForming()
        {
            foreach (MyDataRow parameter in ListBox_AvailableParameters.SelectedItems)
            {
                if ((bool) parameter.GetDataRow["IsThereAdditionalParameters"] == true)
                {

                    // Проверяем, должен ли пользователь вводить эти параметры  вручную.
                    if (parameter.GetDataRow["AdditionalParametersType"].ToString() == "ManualInput")
                    {
                        // Проверяем, можно ли этот параметр вызвать несколько раз
                        if ((bool) parameter.GetDataRow["MultipleCalls"] == true)
                        {

                        }
                        // Если этот параметр может вызываться только единожды
                        else
                        {

                        }
                    }
                    // Если параметр требует от пользователя выбрать один из вариантов
                    else if (parameter.GetDataRow["AdditionalParametersType"].ToString() == "Selecting")
                    {

                    }
                }
                else
                {
                    
                }
            }
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
                ParameterAdded(e);
            }
            else if (e.RemovedItems != null && e.RemovedItems.Count > 0)
            {
                ParameterRemoved(e);
            }
        }

        private void Button_AddRow_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            DataTable table = m_AdditionalParametersInputs.Find(x => x.TableName == btn.Tag.ToString());
            table.Rows.Add();
            /*
            Grid grid = btn.Parent as Grid;
            foreach (UIElement child in grid.Children)
            {
                if (child.GetType() == typeof (DataGrid) && (child as DataGrid).Tag == btn.Tag)
                {
                    (child as DataGrid).Items.Add(null);
                }
            }
            */
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
