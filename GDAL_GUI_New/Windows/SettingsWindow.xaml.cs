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
using System.Windows.Forms;
using System.Globalization;

namespace GDAL_GUI_New
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        // Переменные
            #region Переменные
        private string m_BundledUtilitiesPath = @"Resources\GDAL_Bundle\";
        #endregion

        // Конструкторы
        #region Конструкторы
        public SettingsWindow()
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            EventsAndOtherProperties();

            App.LanguageChanged += LanguageChanged;
            CultureInfo currentLanguage = App.Language;

            comboBox_ProgramLanguage.Items.Clear();
            foreach (var lang in App.Languages)
            {
                comboBox_ProgramLanguage.Items.Add(lang);
            }
            comboBox_ProgramLanguage.SelectedItem = Properties.Settings.Default.ProgramLanguage;

            comboBox_ContentLanguage.Items.Clear();
            comboBox_ContentLanguage.ItemsSource = new string[] {"Eng", "Rus"};
            comboBox_ContentLanguage.SelectedItem = Properties.Settings.Default.DescriptionsLanguage;

            checkBox_UseBundledUtilities.IsChecked = Properties.Settings.Default.UseTheBundledUtilities;
        }
        #endregion

        // Свойства
            #region Свойства

        #endregion

        // Методы
            #region Методы

        private void EventsAndOtherProperties()
        {
            button_SaveSettings.Click +=
                new RoutedEventHandler(Button_SaveSettings_Click);
            button_BrowseUtilitiesFolderPath.Click +=
                new RoutedEventHandler(Button_BrowseUtilitiesFolderPath_Click);
            checkBox_UseBundledUtilities.Checked +=
                new RoutedEventHandler(CheckBox_UseBundledUtilities_Checked);
            checkBox_UseBundledUtilities.Unchecked +=
                new RoutedEventHandler(CheckBox_UseBundledUtilities_Unchecked);
        }

        private void LanguageChanged(object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;
            comboBox_ProgramLanguage.SelectedItem = currLang;
        }


        #endregion

        // Обработчики событий
            #region Обработчики событий

        private void Button_SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            // Сохранение состояния CheckBox элемента
            if (checkBox_UseBundledUtilities.IsChecked != null)
            {
                Properties.Settings.Default.UseTheBundledUtilities =
                    (bool) checkBox_UseBundledUtilities.IsChecked;
            }

            // Сохранение выбранного языка программы
            if (comboBox_ProgramLanguage.SelectedItem != null)
            {
                CultureInfo lang = comboBox_ProgramLanguage.SelectedItem as CultureInfo;
                if (lang != null)
                {
                    App.Language = lang;
                }
            }

            // Сохранение выбранного языка описаний
            if (comboBox_ContentLanguage.SelectedItem != null)
            {
                Properties.Settings.Default.DescriptionsLanguage =
                    comboBox_ContentLanguage.SelectedItem.ToString();
            }

            // Сохранение настроек создания миниатюр
            if (checkBox_GenerateThumbnail.IsChecked != null)
            {
                Properties.Settings.Default.GenerateThumbnails =
                    (bool) checkBox_GenerateThumbnail.IsChecked;
            }

            // Сохранение всех изменений в настройках
            Properties.Settings.Default.Save();
            System.Windows.MessageBox.Show("Изменения сохранены!", "Успех!",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_BrowseUtilitiesFolderPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DataBaseControl.ConnectToDB();
                try
                {
                    int numOfAvailableUtilities = 0;

                    numOfAvailableUtilities = CheckAvailableUtilities.GetNumOfAvailableUtilities(
                        folderBrowserDialog.SelectedPath,
                        DataBaseControl.GetUtilitiesNames());
                    if (numOfAvailableUtilities > 0)
                    {
                        System.Windows.MessageBox.Show("Найдено поддерживаемых утилит: " + numOfAvailableUtilities,
                            "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                        Properties.Settings.Default.UtilitiesDirectory =
                            folderBrowserDialog.SelectedPath + "\\";
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("В указанной папке не найдены поддерживаемые утилиты.\n" +
                                                       "Программа переключится на встроенный пакет утилит.",
                            "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        checkBox_UseBundledUtilities.IsChecked = true;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        "Произошла ошибка. Не удалось установить папку с утилитами." +
                        Environment.NewLine + ex.Data,
                        "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    DataBaseControl.CloseConnection();
                }
            }
        }

        private void CheckBox_UseBundledUtilities_Checked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox cB = sender as System.Windows.Controls.CheckBox;

            DataBaseControl.ConnectToDB();
            try
            {
                int numOfAvailableUtilities = 0;

                numOfAvailableUtilities = CheckAvailableUtilities.GetNumOfAvailableUtilities(
                    m_BundledUtilitiesPath,
                    DataBaseControl.GetUtilitiesNames());
                if (numOfAvailableUtilities > 0)
                {
                    System.Windows.MessageBox.Show("Найдено поддерживаемых утилит: " + numOfAvailableUtilities,
                        "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Properties.Settings.Default.UtilitiesDirectory = m_BundledUtilitiesPath;
                    textBox_UtilitiesFolderPath.IsEnabled = false;
                    button_BrowseUtilitiesFolderPath.IsEnabled = false;
                }
                else
                {
                    TextBlock tBlock = checkBox_UseBundledUtilities.Content as TextBlock;
                    System.Windows.MessageBox.Show(
                        "Не найден комплектный пакет утилит." + Environment.NewLine +
                        "Утилиты должны распологаться по пути:" + Environment.NewLine + 
                        m_BundledUtilitiesPath + Environment.NewLine +
                        "Пожалуйста, выберите папку, в которой есть поддерживаемые утилиты, " +
                        "или же добавьте их в каталог \"" + m_BundledUtilitiesPath + 
                        "\" и отметьте поле \"" + tBlock.Text + "\".",
                        "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    cB.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    "Произошла ошибка. Не удалось установить папку с утилитами." +
                    Environment.NewLine + ex.Data,
                    "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DataBaseControl.CloseConnection();
            }
        }

        private void CheckBox_UseBundledUtilities_Unchecked(object sender, RoutedEventArgs e)
        {
            textBox_UtilitiesFolderPath.IsEnabled = true;
            button_BrowseUtilitiesFolderPath.IsEnabled = true;
        }

        #endregion

    }
}
