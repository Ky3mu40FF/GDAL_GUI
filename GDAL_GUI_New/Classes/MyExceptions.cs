using System;

namespace GDAL_GUI_New
{
    class NotEditingException : Exception
    {
        public NotEditingException() :
            base("Не включен режим редактирования процесса.")
        { }
    }
}