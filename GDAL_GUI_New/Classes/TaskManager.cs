using System.Collections.ObjectModel;

namespace GDAL_GUI_New
{
    public static class TaskManager
    {
        // Переменные
            #region Переменные
        private static ObservableCollection<MyTask> m_Tasks;
        #endregion

        // Конструкторы
        #region Конструкторы

        #endregion

        // Свойства
        #region Свойства

        public static ObservableCollection<MyTask> TasksCollection
        {
            get { return m_Tasks; }
            set
            {
                if (value != null)
                {
                    m_Tasks = value;
                }
            }
        }
        #endregion

        // Методы
        #region Методы

        public static void InitializeProcessManager()
        {
            
        }
        #endregion

        // Обработчики событий
        #region Обработчики событий

        #endregion
    }
}