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
using System.Windows.Shapes;

namespace GDAL_GUI_New
{
    /// <summary>
    /// Логика взаимодействия для TaskEditWindow.xaml
    /// </summary>
    public partial class TaskEditWindow : Window
    {
        // Переменные
        #region Переменные

        #endregion

        // Свойства
        #region Свойства

        #endregion

        // Конструкторы
        #region Конструкторы
        public TaskEditWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            EventAndPropertiesInitialization();
        }
        #endregion

        // Обработчики событий
        #region Обработчики событий
        
        #endregion

        // Остальные функции
        #region Остальные функции
        private void EventAndPropertiesInitialization()
        {
            // Подписка на события
            
        }
        #endregion
    }
}
