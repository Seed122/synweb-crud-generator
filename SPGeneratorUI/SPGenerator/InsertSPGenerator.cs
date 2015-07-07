using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using TreeViewWithCheckBoxes;


namespace SPGenerator
{
    class InsertSPGenerator : BaseSPGenerator
    {
        public InsertSPGenerator()
        {
            InitSettings();
        }

        protected override string GetSpName(string tableName)
        {

            return prefixInsertSp + tableName;
        }

        protected override void GenerateStatement(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {
            StringBuilder sbFields = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();
            foreach (DBTableColumnInfo colInf in selectedFields)
            {
                if (colInf.IsIdentity)
                    continue;
                sbValues.Append(prefixInputParameter + colInf.ColumnName + ",");
                sbFields.Append(colInf.ColumnName + ",");
            }
            sb.Append(Environment.NewLine + "\tInsert into " + tableName + " (" + sbFields.ToString().TrimEnd(',') + ")");
            sb.Append(Environment.NewLine + "\tvalues(" + sbValues.ToString().TrimEnd(',') + ")");
        }

    }
}
