using SPGenerator.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SPGenerator.Common;

namespace SPGenerator.Core
{
    public abstract class BaseSPGenerator
    {
        #region Abstract Method
        protected abstract string GetSpName(string tableName);

        protected abstract string GenerateStatement(DBTableInfo tableInfo, List<DBTableColumnInfo> selectedFields, List<DBTableColumnInfo> whereConditionFields);
        #endregion

        #region Static Members
        internal static string PrefixWhereParameter = "@";
        internal static string PrefixInputParameter = "@";
        internal static bool errorHandling = false;
        //internal static string[] sqlKeyWords;
        static BaseSPGenerator()
        {
           // sqlKeyWords = File.ReadAllLines("SqlKeyWords.txt").Select(p => p.Trim().ToUpperInvariant()).ToArray();
        }
        //public static void SetSettings(Settings setting)
        //{
            //errorHandling =  setting.errorHandling == "Yes";
        //}
        #endregion

        #region GenerateSP

        public StoredProcedure GenerateSp(DBTableInfo tableInfo, List<DBTableColumnInfo> selectedFields,
            List<DBTableColumnInfo> whereConditionFields)
        {
            var name = GetSpName(tableInfo.TableName);
            var statementBuilder = new StringBuilder();
            statementBuilder.AppendLine("CREATE PROCEDURE " + string.Format("[{0}].[{1}]", tableInfo.Schema, name));
            //GenerateErrorNumberOutParameter(statementBuilder);
            var inputs = GenerateInputParameters(selectedFields);
            var wheres = GenerateWhereParameters(whereConditionFields);
            if (!string.IsNullOrWhiteSpace(inputs))
            {
                statementBuilder.Append(inputs);
                if (!string.IsNullOrWhiteSpace(wheres))
                {
                    statementBuilder.Append(',');
                }
                statementBuilder.AppendLine();
            }
            
            if (!string.IsNullOrWhiteSpace(wheres))
            {
                statementBuilder.AppendLine(wheres);
            }
            statementBuilder.AppendLine("AS");
            //GenerateStartTryBlock(statementBuilder);
            statementBuilder.AppendLine(GenerateStatement(tableInfo, selectedFields, whereConditionFields));
            //GenerateEndTryBlock(statementBuilder);
            //GenerateCatchBlock(statementBuilder);
            var res = new StoredProcedure
            {
                Name = name,
                DropScript = GenerateDropScript(name),
                Script = statementBuilder.ToString()
            };
            return res;
        }

        protected virtual string GenerateDropScript(string spName)
        {
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine + "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'");
            sb.Append(spName);
            sb.Append("')AND type in (N'P', N'PC'))");
            sb.Append(Environment.NewLine + "DROP PROCEDURE ");
            sb.Append(Wrap(spName));
            //sb.Append(Environment.NewLine + "GO" + Environment.NewLine);
            return sb.ToString();
        }

        protected virtual string GenerateInputParameters(List<DBTableColumnInfo> fields)
        {
            var inputParams = new List<string>();
            foreach (DBTableColumnInfo colInf in fields)
            {
                string par = PrefixInputParameter + colInf.ColumnName + " " + colInf.DataType;
                if (colInf.DataType.Contains("char"))
                {
                    string charMaxLength = colInf.CharacterMaximumLength > 0
                        ? colInf.CharacterMaximumLength.ToString()
                        : "MAX";
                    par += "(" + charMaxLength + ")";
                }
                inputParams.Add(par);
            }
            return string.Join("," + Environment.NewLine, inputParams);
        }

        protected string GenerateWhereParameters(List<DBTableColumnInfo> whereConditionFields)
        {
            var whereParams = new List<string>();
            foreach (DBTableColumnInfo colInf in whereConditionFields)
            {
                string par = PrefixInputParameter + colInf.ColumnName + " " + colInf.DataType;
                if (colInf.DataType.Contains("char"))
                {
                    string charMaxLength = colInf.CharacterMaximumLength > 0
                        ? colInf.CharacterMaximumLength.ToString()
                        : "MAX";
                    par += "(" + charMaxLength + ")";
                }
                whereParams.Add(par);
            }
            return string.Join("," + Environment.NewLine, whereParams);
        }

        protected string GenerateWhereStatement(List<DBTableColumnInfo> whereConditionFields)
        {
            var sb = new StringBuilder();
            sb.Append("\tWHERE ");
            for (int i = 0; i < whereConditionFields.Count; i++)
            {
                if (i != 0)
                {
                    sb.Append(Environment.NewLine + "\t\t AND ");
                }
                DBTableColumnInfo colInf = whereConditionFields[i];
                sb.Append(Wrap(colInf.ColumnName) + "=" + PrefixWhereParameter + colInf.ColumnName);
            }
            return sb.ToString();
        }

        #region ErrorHandling
        private void GenerateStartTryBlock(StringBuilder sb)
        {
            if (errorHandling)
            {
                sb.Append(Environment.NewLine + "BEGIN TRY");
            }
        }

        private void GenerateEndTryBlock(StringBuilder sb)
        {
            if (errorHandling)
            {
                sb.Append(Environment.NewLine + "END TRY");
            }
        }

        private void GenerateErrorNumberOutParameter(StringBuilder sb)
        {
            if (errorHandling)
            {
                sb.Append(Environment.NewLine + "@out_error_number INT = 0 OUTPUT,");
            }
        }

        private void GenerateCatchBlock(StringBuilder sb)
        {
            if (!errorHandling) return;
            sb.Append(Environment.NewLine + "BEGIN CATCH");
            sb.Append(Environment.NewLine + "\tSELECT @out_error_number=ERROR_NUMBER()");
            sb.Append(Environment.NewLine + "END CATCH");
        }
        #endregion

        protected string Wrap(string name)
        {
            //if (sqlKeyWords.Contains(name.Trim().ToUpperInvariant()))
            //{
                name = "[" + name + "]";
            //}
            return name;
        }
        #endregion
    }
}
