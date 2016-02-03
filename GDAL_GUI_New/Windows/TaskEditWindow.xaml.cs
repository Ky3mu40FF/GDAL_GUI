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

namespace GDAL_GUI_New
{
    /// <summary>
    /// Логика взаимодействия для TaskEditWindow.xaml
    /// </summary>
    public partial class TaskEditWindow : Window
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

        #endregion

        // Методы
                #region Методы
        private void EventAndPropertiesInitialization()
        {
            // Подписка на события
            this.Closing += 
                new CancelEventHandler(ThisWindow_Closing);
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
