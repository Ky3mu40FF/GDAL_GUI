using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GDAL_GUI_New
{
    public static class CheckAvailableUtilities
    {
        public static int GetNumOfAvailableUtilities(string path, List<string> utilitiesNames)
        {
            int numOfAvailableUtilities = 0;
            try
            {
                //List<string> utilitiesInDirectory = Directory.GetFiles(path, "gdal*.exe").ToList<string>();
                List<string> utilitiesInDirectory = Directory.GetFiles(path, "*.exe").ToList<string>();
                if (utilitiesInDirectory.Count > 0)
                {
                    for (int i = 0; i < utilitiesInDirectory.Count; i++)
                    {
                        utilitiesInDirectory[i] = 
                            Path.GetFileNameWithoutExtension(utilitiesInDirectory[i]);
                    }
                }
                List<string> availableUtilities = 
                    utilitiesNames.Intersect(utilitiesInDirectory).ToList<string>();
                numOfAvailableUtilities = availableUtilities.Count;
            }
            catch(Exception e)
            {
                MessageBox.Show("Не удалось определить количество доступных утилит" + 
                    Environment.NewLine + e.Message);
            }
            return numOfAvailableUtilities;
        }

        public static List<string> GetListOfAvailableUtilities(string path, List<string> utilitiesNames)
        {
            List<string> availableUtilities = new List<string>();
            try
            {
                //List<string> utilitiesInDirectory = Directory.GetFiles(path, "gdal*.exe").ToList<string>();
                List<string> utilitiesInDirectory = Directory.GetFiles(path, "*.exe").ToList<string>();
                if (utilitiesInDirectory.Count > 0)
                {
                    for (int i = 0; i < utilitiesInDirectory.Count; i++)
                    {
                        utilitiesInDirectory[i] =
                            Path.GetFileNameWithoutExtension(utilitiesInDirectory[i]);
                    }
                }
                availableUtilities =
                    utilitiesNames.Intersect(utilitiesInDirectory).ToList<string>();
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось определить количество доступных утилит" +
                    Environment.NewLine + e.Message);
            }
            return availableUtilities;
        }
    }
}
