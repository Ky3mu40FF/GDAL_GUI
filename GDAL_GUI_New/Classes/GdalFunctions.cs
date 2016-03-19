using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using OSGeo.GDAL;
//using OSGeo.OGR;


namespace GDAL_GUI_New
{
    public static class GdalFunctions
    {
        private static bool m_IsInitialized = false;

        public static void InitializeGdal()
        {
            if (m_IsInitialized != true)
            {
                try
                {
                    GdalConfiguration.ConfigureGdal();
                    //GdalConfiguration.ConfigureOgr();
                    Gdal.AllRegister();
                    //Ogr.RegisterAll();
                    m_IsInitialized = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Не удалось инициализировать компоненты" +
                                    "библиотеки GDAL/OGR.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public static void MakeThumbnail(string path, ref string outputPath)
        {
            if (!File.Exists(Properties.Settings.Default.UtilitiesDirectory + "gdal_translate.exe"))
            {
                MessageBox.Show("В папке с утилитами отсутствует утилита gdal_translate.", "Ошибка!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                /* -------------------------------------------------------------------- */
                /*      Register driver(s).                                             */
                /* -------------------------------------------------------------------- */
                GdalConfiguration.ConfigureGdal();
                //Gdal.AllRegister();

                /* -------------------------------------------------------------------- */
                /*      Open dataset.                                                   */
                /* -------------------------------------------------------------------- */
                Dataset ds = Gdal.Open(path, Access.GA_ReadOnly);

                if (ds == null)
                {
                    Console.WriteLine("Can't open " + path);
                    return;
                    //System.Environment.Exit(-1);
                }

                Console.WriteLine("Raster dataset parameters:");
                Console.WriteLine("  Projection: " + ds.GetProjectionRef());
                Console.WriteLine("  RasterCount: " + ds.RasterCount);
                Console.WriteLine("  RasterSize (" + ds.RasterXSize + "," + ds.RasterYSize + ")");

                /* -------------------------------------------------------------------- */
                /*      Get driver                                                      */
                /* -------------------------------------------------------------------- */
                Driver drv = ds.GetDriver();

                if (drv == null)
                {
                    Console.WriteLine("Can't get driver.");
                    return;
                    //System.Environment.Exit(-1);
                }

                Console.WriteLine("Using driver " + drv.LongName);

                string m_Gdal_Translate_Arguments = String.Empty;

                Band band = ds.GetRasterBand(1);
                if (band.DataType == DataType.GDT_Byte || drv == Gdal.GetDriverByName("JPEG"))
                {
                    m_Gdal_Translate_Arguments = "-of JPEG -co \"QUALITY=50\" ";
                    outputPath = Path.ChangeExtension(outputPath, "jpg");
                }
                else
                {
                    m_Gdal_Translate_Arguments = "-of GTiff -co \"COMPRESS = DEFLATE\" -co \"ZLEVEL = 9\" ";
                    outputPath = Path.ChangeExtension(outputPath, "tif");
                }

                // Get the width and height of the Dataset
                int width = band.XSize;
                int height = band.YSize;

                float scaleCoeff = 1;
                float maxSize = 256;
                scaleCoeff = width > height ? maxSize / (float)width : maxSize / (float)height;
                int newWidth = (int)((float)width * scaleCoeff);
                int newHeight = (int)((float)height * scaleCoeff);

                m_Gdal_Translate_Arguments += 
                    String.Format("-outsize {0} {1} \"{2}\" \"{3}\"", newWidth, newHeight, path, outputPath);

                //UseGdalTranslate(path, outputPath, newWidth, newHeight);
                UseGdalTranslate(m_Gdal_Translate_Arguments);
            }
            catch (Exception e)
            {
                Console.WriteLine("Application error: " + e.Message);
                outputPath = String.Empty;
            }

        }

        public static int ProgressFunc(double Complete, IntPtr Message, IntPtr Data)
        {
            Console.Write("Processing ... " + Complete * 100 + "% Completed.");
            if (Message != IntPtr.Zero)
                Console.Write(" Message:" + System.Runtime.InteropServices.Marshal.PtrToStringAnsi(Message));
            if (Data != IntPtr.Zero)
                Console.Write(" Data:" + System.Runtime.InteropServices.Marshal.PtrToStringAnsi(Data));

            Console.WriteLine("");
            return 1;
        }

        //public static void UseGdalTranslate(string path, string outputPath, int width, int height)
        public static void UseGdalTranslate(string arguments)
        {
            Process proc = new Process();
            ProcessStartInfo info = new ProcessStartInfo
            {
                //Arguments = String.Format("-of GTiff -outsize {0} {1} \"{2}\" \"{3}\"", width, height, path, outputPath),
                Arguments = arguments,
                CreateNoWindow = true,
                FileName = Properties.Settings.Default.UtilitiesDirectory + "gdal_translate",
                UseShellExecute = false
            };
            try
            {
                proc.StartInfo = info;
                proc.Start();
                proc.WaitForExit();
                Console.WriteLine("Thumbnail generation is complete!");
                proc.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не найдена утилита gdal_translate");
            }
        }

        public static string GetInfoAboutRaster(string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("Файл не существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return String.Empty;
            }
            try
            {
                StringBuilder stringBuilder = new StringBuilder();

                GdalConfiguration.ConfigureGdal();

                Dataset ds = Gdal.Open(path, Access.GA_ReadOnly);

                if (ds == null)
                {
                    Console.WriteLine("Can't open " + path);
                    return String.Empty;
                }

                stringBuilder.Append("Параметры растра" + Path.GetFileName(path) + ":");
                stringBuilder.Append("  Проекция: " + ds.GetProjectionRef());
                stringBuilder.Append("  Количество слоёв: " + ds.RasterCount);
                stringBuilder.Append("  Размер: (" + ds.RasterXSize + "," + ds.RasterYSize + ")");
                

                Driver drv = ds.GetDriver();

                if (drv == null)
                {
                    Console.WriteLine("Can't get driver.");
                    return stringBuilder.ToString();
                }

                stringBuilder.Append("Используемый драйвер: " + drv.LongName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Application error: " + e.Message);
            }

            string m_Info = String.Empty;

            return m_Info;
        }
    }
}
