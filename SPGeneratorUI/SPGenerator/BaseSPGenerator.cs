using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeViewWithCheckBoxes;


namespace SPGenerator
{
    public abstract class BaseSPGenerator
    {
        protected string prefixWhereParameter = "@w_";
        protected string prefixInputParameter = "@p_";
        protected string prefixInsertSp = "sp_insert_";
        protected string prefixUpdateSp = "sp_update_";
        protected string WhereNodekey = "Where Caluse Columns";

        protected void InitSettings()
        {
            //prefixWhereParameter = Setting.prefixWhereParameter;
            //prefixInputParameter = Setting.prefixInputParameter;
            //prefixInsertSp = Setting.prefixInsertSp;
            //prefixUpdateSp = Setting.prefixUpdateSp;
        }

        #region GenerateSP

        protected abstract string GetSpName(string tableName);

        public void GenerateSp(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields)
        {
            string spName = GetSpName(tableName);
            GenerateDropScript(spName, sb);
            sb.Append(Environment.NewLine + " CREATE PROCEDURE " + spName);
            GenerateInputParameters(selectedFields, sb);
            GenerateWhereParameters(whereConditionFields, sb);
            sb.Append(Environment.NewLine + "AS" + Environment.NewLine + "BEGIN");
            sb.Append(Environment.NewLine + "BEGIN TRY");
            GenerateStatement(tableName, sb, selectedFields, whereConditionFields);
            sb.Append(Environment.NewLine + "END TRY");
            GenerateCatchBlock(sb);
            sb.Append(Environment.NewLine + "END");
            sb.Append(Environment.NewLine + "GO");
        }

        protected virtual void GenerateDropScript(string spName, StringBuilder sb)
        {
            sb.Append(Environment.NewLine + "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'");
            sb.Append(spName);
            sb.Append("')AND type in (N'P', N'PC'))");
            sb.Append(Environment.NewLine + "DROP PROCEDURE ");
            sb.Append(spName);
            sb.Append(Environment.NewLine + "GO" + Environment.NewLine);
        }

        private void GenerateCatchBlock(StringBuilder sb)
        {
            sb.Append(Environment.NewLine + "BEGIN CATCH");
            sb.Append(Environment.NewLine + "\tSELECT @out_error_number=ERROR_NUMBER()");
            sb.Append(Environment.NewLine + "END CATCH");
        }

        protected virtual void GenerateInputParameters(List<DBTableColumnInfo> tableFields, StringBuilder sb)
        {
            sb.Append(Environment.NewLine + "@out_error_number INT = 0 OUTPUT,");
            foreach (DBTableColumnInfo colInf in tableFields)
            {
                if (colInf.IsIdentity)
                    continue;
                sb.Append(Environment.NewLine + prefixInputParameter + colInf.ColumnName);
                sb.Append(" " + colInf.DataType);
                if (colInf.CharacterMaximumLength > 0)
                {
                    sb.Append("(" + colInf.CharacterMaximumLength.ToString() + ")");
                }
                sb.Append(",");
            }
            //Remove Commma from end
            sb[sb.Length - 1] = ' ';
        }

        protected void GenerateWhereParameters(List<DBTableColumnInfo> whereConditionFields, StringBuilder sb)
        {
            foreach (DBTableColumnInfo colInf in whereConditionFields)
            {
                sb.Append(Environment.NewLine + prefixWhereParameter + colInf.ColumnName);
                sb.Append(" " + colInf.DataType);
                if (colInf.CharacterMaximumLength > 0)
                {
                    sb.Append("(" + colInf.CharacterMaximumLength.ToString() + ")");
                }
                sb.Append(",");
            }
            //Remove Commma from end
            sb[sb.Length - 1] = ' ';
        }
        protected abstract void GenerateStatement(string tableName, StringBuilder sb, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields);

        protected void GenerateWhereStatement(List<DBTableColumnInfo> whereConditionFields, StringBuilder sb)
        {
            sb.Append(Environment.NewLine + "\tWHERE ");

            foreach (DBTableColumnInfo colInf in whereConditionFields)
            {
                sb.Append(colInf.ColumnName + "=" + prefixWhereParameter + colInf.ColumnName);
                sb.Append(" AND ");
            }
            sb.Remove(sb.Length - 5, 5);
        }

        #endregion
    }
}
