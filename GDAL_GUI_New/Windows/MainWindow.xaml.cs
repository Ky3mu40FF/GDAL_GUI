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

        #endregion

        // Свойства
        #region Свойства

        #endregion

        // Конструкторы
        #region Конструкторы

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            EventAndPropertiesInitialization();
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
            TaskEditWindow taskEditWindow = new TaskEditWindow();
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

        #endregion

        // Остальные функции
        #region Остальные функции
        private void EventAndPropertiesInitialization()
        {
            // Подписка на события
            Menu_File_Exit.Click += new RoutedEventHandler(Menu_File_Exit_Click);
            Menu_Edit_AddTask.Click += new RoutedEventHandler(Menu_Edit_AddTask_Click);
            Menu_Run_RunAll.Click += new RoutedEventHandler(Menu_Run_RunAll_Click);
            Menu_Run_RunSelected.Click += new RoutedEventHandler(Menu_Run_RunSelected_Click);
            Menu_Settings.Click += new RoutedEventHandler(Menu_Settings_Click);
            Menu_About.Click += new RoutedEventHandler(Menu_About_Click);
        }

        #endregion
    }

    
}
