using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using TreeViewWithCheckBoxes;


namespace SPGenerator
{
    class UpdateSPGenerator : BaseSPGenerator
    {
        public UpdateSPGenerator()
        {
            InitSettings();
        }

        protected override string GetSpName(string tableName)
        {
            return prefixUpdateSp + tableName;
        }

        protected override void GenerateStatement(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {
            StringBuilder sbFields = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();
            sb.Append(Environment.NewLine + "\tUPDATE " + tableName + " SET ");

            foreach (DBTableColumnInfo colInf in selectedFields)
            {
                if (colInf.IsIdentity)
                    continue;
                sb.Append(colInf.ColumnName + "=" + prefixInputParameter + colInf.ColumnName);
                sb.Append(",");
            }
            //Remove Commma from end
            sb[sb.Length - 1] = ' ';
            GenerateWhereStatement(whereConditionFields, sb);
        }
    }
}
