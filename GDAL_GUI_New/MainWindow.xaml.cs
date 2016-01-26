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
using Gat.Controls;

namespace GDAL_GUI_New
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Переменные

        
        
        // Конструкторы

        public MainWindow()
        {
            InitializeComponent();

            EventAndPropertiesInitialization();
        }

        // Обработчики событий

        // Выход из приложения по нажатию на кнопку Выход в меню
        private void Menu_File_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Show();
        }

        // Остальные функции

        private void EventAndPropertiesInitialization()
        {
            // Подписка на события
            Menu_File_Exit.Click += new RoutedEventHandler(Menu_File_Exit_Click);
            Menu_About.Click += new RoutedEventHandler(Menu_About_Click);
        }
    }
}
