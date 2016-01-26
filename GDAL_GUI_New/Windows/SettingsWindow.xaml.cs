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
using System.Globalization;

namespace GDAL_GUI_New
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            App.LanguageChanged += LanguageChanged;
            CultureInfo currentLanguage = App.Language;

            comboBox_ProgramLanguage.Items.Clear();
            foreach (var lang in App.Languages)
            {
                comboBox_ProgramLanguage.Items.Add(lang);
            }

            comboBox_ProgramLanguage.SelectionChanged += 
                new SelectionChangedEventHandler(ComboBox_ProgramLanguage_SelectionChanged);
        }


        private void LanguageChanged(object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;
            comboBox_ProgramLanguage.SelectedItem = currLang;
        }

        private void ComboBox_ProgramLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cB = sender as ComboBox;
            if (cB.SelectedItem != null)
            {
                CultureInfo lang = cB.SelectedItem as CultureInfo;
                if (lang != null)
                {
                    App.Language = lang;
                }
            }

        }


    }
}
