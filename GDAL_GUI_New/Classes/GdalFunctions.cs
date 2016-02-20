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

        public static BitmapImage SaveBitmap(string path)
        {
            //string newPath = String.Empty;
            BitmapImage bitmapImage = new BitmapImage();
            if (File.Exists(path))
                try
                {
                    Gdal.AllRegister();
                    OSGeo.GDAL.Dataset dS = Gdal.Open(path, Access.GA_ReadOnly);
                    OSGeo.GDAL.Driver driver = dS.GetDriver();
                    Console.WriteLine("HelpTopic: {0}\nLongName: {1}\nShortName: {2}\n" +
                                      "GetDescription: {3}",
                        driver.HelpTopic, driver.LongName, driver.ShortName, driver.GetDescription());

                    SaveBitmapDirect(path + "_New", dS, 0, 0, dS.RasterXSize, dS.RasterYSize, dS.RasterXSize,
                        dS.RasterYSize, ref bitmapImage);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Извините, возникла ошибка..." + ex.Message);
                    //System.Environment.Exit(-1);
                }
            return bitmapImage;
        }

        private static void SaveBitmapDirect(string filename, Dataset ds, int xOff, int yOff, int width, int height,
            int imageWidth, int imageHeight, ref BitmapImage bmpImage) //string newPath
        {
            if (ds.RasterCount == 0)
                return;

            int[] bandMap = new int[4] {1, 1, 1, 1};
            int channelCount = 1;
            bool hasAlpha = false;
            bool isIndexed = false;
            int channelSize = 8;
            ColorTable ct = null;
            // Evaluate the bands and find out a proper image transfer format
            for (int i = 0; i < ds.RasterCount; i++)
            {
                Band band = ds.GetRasterBand(i + 1);
                if (Gdal.GetDataTypeSize(band.DataType) > 8)
                    channelSize = 16;
                switch (band.GetRasterColorInterpretation())
                {
                    case ColorInterp.GCI_AlphaBand:
                        channelCount = 4;
                        hasAlpha = true;
                        bandMap[3] = i + 1;
                        break;
                    case ColorInterp.GCI_BlueBand:
                        if (channelCount < 3)
                            channelCount = 3;
                        bandMap[0] = i + 1;
                        break;
                    case ColorInterp.GCI_RedBand:
                        if (channelCount < 3)
                            channelCount = 3;
                        bandMap[2] = i + 1;
                        break;
                    case ColorInterp.GCI_GreenBand:
                        if (channelCount < 3)
                            channelCount = 3;
                        bandMap[1] = i + 1;
                        break;
                    case ColorInterp.GCI_PaletteIndex:
                        ct = band.GetRasterColorTable();
                        isIndexed = true;
                        bandMap[0] = i + 1;
                        break;
                    case ColorInterp.GCI_GrayIndex:
                        isIndexed = true;
                        bandMap[0] = i + 1;
                        break;
                    default:
                        // we create the bandmap using the dataset ordering by default
                        if (i < 4 && bandMap[i] == 0)
                        {
                            if (channelCount < i)
                                channelCount = i;
                            bandMap[i] = i + 1;
                        }
                        break;
                }
            }

            // find out the pixel format based on the gathered information
            PixelFormat pixelFormat;
            DataType dataType;
            int pixelSpace;

            if (isIndexed)
            {
                pixelFormat = PixelFormat.Format8bppIndexed;
                dataType = DataType.GDT_Byte;
                pixelSpace = 1;
            }
            else
            {
                if (channelCount == 1)
                {
                    if (channelSize > 8)
                    {
                        pixelFormat = PixelFormat.Format16bppGrayScale;
                        dataType = DataType.GDT_Int16;
                        pixelSpace = 2;
                    }
                    else
                    {
                        pixelFormat = PixelFormat.Format24bppRgb;
                        channelCount = 3;
                        dataType = DataType.GDT_Byte;
                        pixelSpace = 3;
                    }
                }
                else
                {
                    if (hasAlpha)
                    {
                        if (channelSize > 8)
                        {
                            pixelFormat = PixelFormat.Format64bppArgb;
                            dataType = DataType.GDT_UInt16;
                            pixelSpace = 8;
                        }
                        else
                        {
                            pixelFormat = PixelFormat.Format32bppArgb;
                            dataType = DataType.GDT_Byte;
                            pixelSpace = 4;
                        }
                        channelCount = 4;
                    }
                    else
                    {
                        if (channelSize > 8)
                        {
                            pixelFormat = PixelFormat.Format48bppRgb;
                            dataType = DataType.GDT_UInt16;
                            pixelSpace = 6;
                        }
                        else
                        {
                            pixelFormat = PixelFormat.Format24bppRgb;
                            dataType = DataType.GDT_Byte;
                            pixelSpace = 3;
                        }
                        channelCount = 3;
                    }
                }
            }


            // Create a Bitmap to store the GDAL image in
            Bitmap bitmap = new Bitmap(imageWidth, imageHeight, pixelFormat);

            if (isIndexed)
            {
                // setting up the color table
                if (ct != null)
                {
                    int iCol = ct.GetCount();
                    ColorPalette pal = bitmap.Palette;
                    for (int i = 0; i < iCol; i++)
                    {
                        ColorEntry ce = ct.GetColorEntry(i);
                        pal.Entries[i] = Color.FromArgb(ce.c4, ce.c1, ce.c2, ce.c3);
                    }
                    bitmap.Palette = pal;
                }
                else
                {
                    // grayscale
                    ColorPalette pal = bitmap.Palette;
                    for (int i = 0; i < 256; i++)
                        pal.Entries[i] = Color.FromArgb(255, i, i, i);
                    bitmap.Palette = pal;
                }
            }

            // Use GDAL raster reading methods to read the image data directly into the Bitmap
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, imageWidth, imageHeight),
                ImageLockMode.ReadWrite, pixelFormat);

            try
            {
                int stride = bitmapData.Stride;
                IntPtr buf = bitmapData.Scan0;

                ds.ReadRaster(xOff, yOff, width, height, buf, imageWidth, imageHeight, dataType,
                    channelCount, bandMap, pixelSpace, stride, 1);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);

            }

            MemoryStream mStream = new MemoryStream();
            bitmap.Save(mStream, ImageFormat.Png);
            mStream.Position = 0;
            //BitmapImage bmpImage = new BitmapImage();
            bmpImage.BeginInit();
            bmpImage.StreamSource = mStream;
            bmpImage.CacheOption = BitmapCacheOption.OnLoad;
            bmpImage.EndInit();
            bitmap.Dispose();
            mStream.Dispose();
            ds.FlushCache();
            //bitmap.Save(filename);
            //newPath = filename;


        }

        public static void ToMemory(string path)
        {
            Gdal.AllRegister();

            Bitmap bmp = new Bitmap(path);

            // set up MEM driver to read bitmap data
            int bandCount = 1;
            int pixelOffset = 1;
            DataType dataType = DataType.GDT_Byte;
            switch (bmp.PixelFormat)
            {
                case PixelFormat.Format16bppGrayScale:
                    dataType = DataType.GDT_Int16;
                    bandCount = 1;
                    pixelOffset = 2;
                    break;
                case PixelFormat.Format24bppRgb:
                    dataType = DataType.GDT_Byte;
                    bandCount = 3;
                    pixelOffset = 3;
                    break;
                case PixelFormat.Format32bppArgb:
                    dataType = DataType.GDT_Byte;
                    bandCount = 4;
                    pixelOffset = 4;
                    break;
                case PixelFormat.Format48bppRgb:
                    dataType = DataType.GDT_UInt16;
                    bandCount = 3;
                    pixelOffset = 6;
                    break;
                case PixelFormat.Format64bppArgb:
                    dataType = DataType.GDT_UInt16;
                    bandCount = 4;
                    pixelOffset = 8;
                    break;
                default:
                    Console.WriteLine("Invalid pixel format " + bmp.PixelFormat.ToString());
                    break;
            }

            // Use GDAL raster reading methods to read the image data directly into the Bitmap
            BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                bmp.PixelFormat);

            int stride = bitmapData.Stride;
            IntPtr buf = bitmapData.Scan0;

            try
            {
                Driver drvmem = Gdal.GetDriverByName("MEM");
                // create a MEM dataset
                Dataset ds = drvmem.Create("", bmp.Width, bmp.Height, 0, dataType, null);
                // add bands in a reverse order
                for (int i = 1; i <= bandCount; i++)
                {
                    ds.AddBand(dataType,
                        new string[]
                        {
                            "DATAPOINTER=" + Convert.ToString(buf.ToInt64() + bandCount - i), "PIXELOFFSET=" + pixelOffset,
                            "LINEOFFSET=" + stride
                        });
                }

                // display parameters
                Console.WriteLine("Raster dataset parameters:");
                Console.WriteLine("  RasterCount: " + ds.RasterCount);
                Console.WriteLine("  RasterSize (" + ds.RasterXSize + "," + ds.RasterYSize + ")");

                // write dataset to tif file
                Driver drv = Gdal.GetDriverByName("GTiff");

                if (drv == null)
                {
                    Console.WriteLine("Can't get driver.");
                    System.Environment.Exit(-1);
                }

                drv.CreateCopy("sample2.tif", ds, 0, null, null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                bmp.UnlockBits(bitmapData);
            }

        }

        public static void ReadOverview(string path)
        {
            int iOverview = -1;

            try
            {
                /* -------------------------------------------------------------------- */
                /*      Register driver(s).                                             */
                /* -------------------------------------------------------------------- */
                Gdal.AllRegister();

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

                /* -------------------------------------------------------------------- */
                /*      Get raster band                                                 */
                /* -------------------------------------------------------------------- */
                for (int iBand = 1; iBand <= ds.RasterCount; iBand++)
                {
                    Band band = ds.GetRasterBand(iBand);
                    Console.WriteLine("Band " + iBand + " :");
                    Console.WriteLine("   DataType: " + band.DataType);
                    Console.WriteLine("   Size (" + band.XSize + "," + band.YSize + ")");
                    Console.WriteLine("   PaletteInterp: " + band.GetRasterColorInterpretation().ToString());

                    for (int iOver = 0; iOver < band.GetOverviewCount(); iOver++)
                    {
                        Band over = band.GetOverview(iOver);
                        Console.WriteLine("      OverView " + iOver + " :");
                        Console.WriteLine("         DataType: " + over.DataType);
                        Console.WriteLine("         Size (" + over.XSize + "," + over.YSize + ")");
                        Console.WriteLine("         PaletteInterp: " + over.GetRasterColorInterpretation().ToString());
                    }
                }

                /* -------------------------------------------------------------------- */
                /*      Processing the raster                                           */
                /* -------------------------------------------------------------------- */
                //SaveBitmapBuffered(ds, args[1], iOverview);

            }
            catch (Exception e)
            {
                Console.WriteLine("Application error: " + e.Message);
            }
        }

        public static void GDALRead(string path, string outputPath, int overview)
        {
            int iOverview = -1;
            iOverview = overview;

            // Using early initialization of System.Console
            //Console.WriteLine("");

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

                /* -------------------------------------------------------------------- */
                /*      Get raster band                                                 */
                /* -------------------------------------------------------------------- */
                for (int iBand = 1; iBand <= ds.RasterCount; iBand++)
                {
                    Band band = ds.GetRasterBand(iBand);
                    Console.WriteLine("Band " + iBand + " :");
                    Console.WriteLine("   DataType: " + band.DataType);
                    Console.WriteLine("   Size (" + band.XSize + "," + band.YSize + ")");
                    Console.WriteLine("   PaletteInterp: " + band.GetRasterColorInterpretation().ToString());

                    for (int iOver = 0; iOver < band.GetOverviewCount(); iOver++)
                    {
                        Band over = band.GetOverview(iOver);
                        Console.WriteLine("      OverView " + iOver + " :");
                        Console.WriteLine("         DataType: " + over.DataType);
                        Console.WriteLine("         Size (" + over.XSize + "," + over.YSize + ")");
                        Console.WriteLine("         PaletteInterp: " + over.GetRasterColorInterpretation().ToString());
                    }
                }

                /* -------------------------------------------------------------------- */
                /*      Processing the raster                                           */
                /* -------------------------------------------------------------------- */
                SaveBitmapBuffered(ds, outputPath, iOverview);

            }
            catch (Exception e)
            {
                Console.WriteLine("Application error: " + e.Message);
            }
        }

        private static void SaveBitmapBuffered(Dataset ds, string filename, int iOverview)
        {
            // Get the GDAL Band objects from the Dataset
            Band redBand = ds.GetRasterBand(1);

            if (redBand.GetRasterColorInterpretation() == ColorInterp.GCI_PaletteIndex)
            {
                SaveBitmapPaletteBuffered(ds, filename, iOverview);
                return;
            }

            if (redBand.GetRasterColorInterpretation() == ColorInterp.GCI_GrayIndex)
            {
                SaveBitmapGrayBuffered(ds, filename, iOverview);
                return;
            }

            if (redBand.GetRasterColorInterpretation() != ColorInterp.GCI_RedBand)
            {
                Console.WriteLine("Non RGB images are not supported by this sample! ColorInterp = " +
                                  redBand.GetRasterColorInterpretation().ToString());
                return;
            }

            if (ds.RasterCount < 3)
            {
                Console.WriteLine("The number of the raster bands is not enough to run this sample");
                return;
                //System.Environment.Exit(-1);
            }

            if (iOverview >= 0 && redBand.GetOverviewCount() > iOverview)
                redBand = redBand.GetOverview(iOverview);

            Band greenBand = ds.GetRasterBand(2);

            if (greenBand.GetRasterColorInterpretation() != ColorInterp.GCI_GreenBand)
            {
                Console.WriteLine("Non RGB images are not supported by this sample! ColorInterp = " +
                                  greenBand.GetRasterColorInterpretation().ToString());
                return;
            }

            if (iOverview >= 0 && greenBand.GetOverviewCount() > iOverview)
                greenBand = greenBand.GetOverview(iOverview);

            Band blueBand = ds.GetRasterBand(3);

            if (blueBand.GetRasterColorInterpretation() != ColorInterp.GCI_BlueBand)
            {
                Console.WriteLine("Non RGB images are not supported by this sample! ColorInterp = " +
                                  blueBand.GetRasterColorInterpretation().ToString());
                return;
            }

            if (iOverview >= 0 && blueBand.GetOverviewCount() > iOverview)
                blueBand = blueBand.GetOverview(iOverview);

            // Get the width and height of the raster
            int width = redBand.XSize;
            int height = redBand.YSize;

            // Create a Bitmap to store the GDAL image in
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            DateTime start = DateTime.Now;

            byte[] r = new byte[width*height];
            byte[] g = new byte[width*height];
            byte[] b = new byte[width*height];

            redBand.ReadRaster(0, 0, width, height, r, width, height, 0, 0);
            greenBand.ReadRaster(0, 0, width, height, g, width, height, 0, 0);
            blueBand.ReadRaster(0, 0, width, height, b, width, height, 0, 0);
            TimeSpan renderTime = DateTime.Now - start;
            Console.WriteLine("SaveBitmapBuffered fetch time: " + renderTime.TotalMilliseconds + " ms");

            int i, j;
            for (i = 0; i < width; i++)
            {
                for (j = 0; j < height; j++)
                {
                    Color newColor = Color.FromArgb(Convert.ToInt32(r[i + j*width]), Convert.ToInt32(g[i + j*width]),
                        Convert.ToInt32(b[i + j*width]));
                    bitmap.SetPixel(i, j, newColor);
                }
            }

            bitmap.Save(filename);
        }

        private static void SaveBitmapPaletteBuffered(Dataset ds, string filename, int iOverview)
        {
            // Get the GDAL Band objects from the Dataset
            Band band = ds.GetRasterBand(1);
            if (iOverview >= 0 && band.GetOverviewCount() > iOverview)
                band = band.GetOverview(iOverview);

            ColorTable ct = band.GetRasterColorTable();
            if (ct == null)
            {
                Console.WriteLine("   Band has no color table!");
                return;
            }

            if (ct.GetPaletteInterpretation() != PaletteInterp.GPI_RGB)
            {
                Console.WriteLine("   Only RGB palette interp is supported by this sample!");
                return;
            }

            // Get the width and height of the Dataset
            int width = band.XSize;
            int height = band.YSize;

            // Create a Bitmap to store the GDAL image in
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            DateTime start = DateTime.Now;

            byte[] r = new byte[width*height];

            band.ReadRaster(0, 0, width, height, r, width, height, 0, 0);
            TimeSpan renderTime = DateTime.Now - start;
            Console.WriteLine("SaveBitmapBuffered fetch time: " + renderTime.TotalMilliseconds + " ms");

            int i, j;
            for (i = 0; i < width; i++)
            {
                for (j = 0; j < height; j++)
                {
                    ColorEntry entry = ct.GetColorEntry(r[i + j*width]);
                    Color newColor = Color.FromArgb(Convert.ToInt32(entry.c1), Convert.ToInt32(entry.c2),
                        Convert.ToInt32(entry.c3));
                    bitmap.SetPixel(i, j, newColor);
                }
            }

            bitmap.Save(filename);
        }

        private static void SaveBitmapGrayBuffered(Dataset ds, string filename, int iOverview)
        {
            // Get the GDAL Band objects from the Dataset
            Band band = ds.GetRasterBand(1);
            if (iOverview >= 0 && band.GetOverviewCount() > iOverview)
                band = band.GetOverview(iOverview);

            // Get the width and height of the Dataset
            int width = band.XSize;
            int height = band.YSize;

            float scaleCoeff = 1;
            float maxSize = 512;
            scaleCoeff = width > height ? maxSize/(float) width : maxSize/(float) height;
            int newWidth = (int) ((float) width*scaleCoeff);
            int newHeight = (int) ((float) height*scaleCoeff);

            // Create a Bitmap to store the GDAL image in
            //Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            Bitmap bitmap = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppRgb);

            DateTime start = DateTime.Now;

            //byte[] r = new byte[width * height];
            byte[] r = new byte[newWidth*newHeight];

            band.ReadRaster(0, 0, width, height, r, newWidth, newHeight, 0, 0);
            //band.ReadRaster(0, 0, width, height, r, width, height, 0, 0);
            TimeSpan renderTime = DateTime.Now - start;
            Console.WriteLine("SaveBitmapBuffered fetch time: " + renderTime.TotalMilliseconds + " ms");

            int i, j;
            //for (i = 0; i < width; i++)
            for (i = 0; i < newWidth; i++)
            {
                //for (j = 0; j < height; j++)
                for (j = 0; j < newHeight; j++)
                {
                    Color newColor = Color.FromArgb(Convert.ToInt32(r[i + j*newWidth]),
                        Convert.ToInt32(r[i + j*newWidth]), Convert.ToInt32(r[i + j*newWidth]));
                    bitmap.SetPixel(i, j, newColor);
                }
            }

            bitmap.Save(filename);
        }

        public static void MakeThumbnail(string path, string outputPath)
        {
            if (!File.Exists(Properties.Settings.Default.UtilitiesDirectory + "gdal_translate.exe"))
            {
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

                Band band = ds.GetRasterBand(1);

                // Get the width and height of the Dataset
                int width = band.XSize;
                int height = band.YSize;

                float scaleCoeff = 1;
                float maxSize = 256;
                scaleCoeff = width > height ? maxSize / (float)width : maxSize / (float)height;
                int newWidth = (int)((float)width * scaleCoeff);
                int newHeight = (int)((float)height * scaleCoeff);

                UseGdalTranslate(path, outputPath, newWidth, newHeight);
                /*
                //string[] saveOptions = {"QUALITY=75"};
                string[] saveOptions = { "EXIF_THUMBNAIL=YES" };
                //Driver jpegDriver = Gdal.GetDriverByName("PNG");
                Driver jpegDriver = Gdal.GetDriverByName("JPEG");
                jpegDriver.CreateCopy(outputPath, ds, 0, saveOptions, new Gdal.GDALProgressFuncDelegate(ProgressFunc),
                    "Sample Data");
                */

            }
            catch (Exception e)
            {
                Console.WriteLine("Application error: " + e.Message);
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

        public static void UseGdalTranslate(string path, string outputPath, int width, int height)
        {
            Process proc = new Process();
            ProcessStartInfo info = new ProcessStartInfo
            {
                Arguments = String.Format("-of GTiff -outsize {0} {1} \"{2}\" \"{3}\"", width, height, path, outputPath),
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

    }
}
