using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace GDAL_GUI_New
{
    public sealed class MyDataRow
    {
        private DataRow m_Row;
        public MyDataRow(DataRow row)
        {
            m_Row = row;
        }

        public DataRow GetDataRow
        {
            get { return m_Row; }
        }

        public override string ToString()
        {
            return this.m_Row["NameOfTheParameter"].ToString();
        }
    }
}
