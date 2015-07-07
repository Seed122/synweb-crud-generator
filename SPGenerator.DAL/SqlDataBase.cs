﻿using SPGenerator.DataModel;
using SPGenerator.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPGenerator.DAL
{
   public  partial class SqlDataBase : IDataBase
    {
        public SqlDataBase(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public List<DBTableInfo> GetDataBaseTables()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                string sql = "SELECT name FROM sys.Tables";
                sqlTableList = new List<DBTableInfo>();

                DataTable dt = ExecuteDataTable(sql, connection);
                identityColumns = LoadIdentityColumns(connection);
                primaryColumns = LoadPrimaryColumns(connection);
                foreach (DataRow dr in dt.Rows)
                {
                    var tbinfo = GetTableInformation(dr["name"].ToString(), connection);
                    sqlTableList.Add(tbinfo);
                }
            }

            return sqlTableList;
        }
        private DBTableInfo GetTableInformation(string tableName, SqlConnection connection)
        {
            string sql = "Select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='" + tableName + "'";
            var dt = ExecuteDataTable(sql, connection);
            var sqlTableInfo = new DBTableInfo();
            List<DBTableColumnInfo> colList = new List<DBTableColumnInfo>();

            foreach (DataRow dr in dt.Rows)
            {
                var colinfo = new DBTableColumnInfo();
                colinfo.ColumnName = dr["COLUMN_NAME"].ToString();
                colinfo.DataType = dr["DATA_TYPE"].ToString();
                if (dr["CHARACTER_MAXIMUM_LENGTH"] != null && dr["CHARACTER_MAXIMUM_LENGTH"].ToString().Trim() != "")
                    colinfo.CharacterMaximumLength = int.Parse(dr["CHARACTER_MAXIMUM_LENGTH"].ToString());

                if (dr["NUMERIC_PRECISION"] != null && dr["NUMERIC_PRECISION"].ToString().Trim() != "")
                    colinfo.NumericPrecision = int.Parse(dr["NUMERIC_PRECISION"].ToString());

                if (dr["NUMERIC_PRECISION_RADIX"] != null && dr["NUMERIC_PRECISION_RADIX"].ToString().Trim() != "")
                    colinfo.NumericPrecisionRadix = int.Parse(dr["NUMERIC_PRECISION_RADIX"].ToString());

                if (dr["NUMERIC_SCALE"] != null && dr["NUMERIC_SCALE"].ToString().Trim() != "")
                    colinfo.NumericScale = int.Parse(dr["NUMERIC_SCALE"].ToString());

                if (string.IsNullOrEmpty(sqlTableInfo.Schema) && dr["TABLE_SCHEMA"].ToString().Trim() != "")
                    sqlTableInfo.Schema = dr["TABLE_SCHEMA"].ToString();


                colinfo.IsIdentity = IsIdentityColumn(dr["COLUMN_NAME"].ToString(), tableName, dr["TABLE_SCHEMA"].ToString());
                colinfo.IsPrimaryKey = IsPrimaryColumn(dr["COLUMN_NAME"].ToString(), tableName, dr["TABLE_SCHEMA"].ToString());
                colinfo.Exclude = IsExcludeColumn(colinfo);

                colList.Add(colinfo);
            }

            sqlTableInfo.TableName = tableName;
            sqlTableInfo.Columns = colList;
            return sqlTableInfo;

        }
        public string ConnectionString { get; set; }

        Dictionary<string, bool> identityColumns = new Dictionary<string, bool>();
        Dictionary<string, bool> primaryColumns = new Dictionary<string, bool>();
        List<DBTableInfo> sqlTableList = null;

        private DataTable ExecuteDataTable(string sql, SqlConnection connection)
        {
            var dataTable = new DataTable();
            var command = new SqlCommand(sql);
            command.Connection = connection;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            dataAdapter.Fill(dataTable);
            return dataTable;
        }

        private bool IsExcludeColumn(DBTableColumnInfo colInfo)
        {
            return colInfo.IsIdentity
                   || colInfo.DataType.ToUpperInvariant() == "TIMESTAMP"
                   || colInfo.DataType.ToUpperInvariant() == "DATETIME" && colInfo.ColumnName.ToUpperInvariant() == "CREATIONDATE"; //autogenerated by convention
        }

       private bool IsIdentityColumn(string columnName, string tableName, string schema)
        {
            string key = (schema + tableName + columnName).ToUpperInvariant();
            return identityColumns.ContainsKey(key);
        }

       private bool IsPrimaryColumn(string columnName, string tableName, string schema)
        {
            string key = (schema + tableName + columnName).ToUpperInvariant();
            return primaryColumns.ContainsKey(key);
        }

        private Dictionary<string, bool> LoadIdentityColumns(SqlConnection connection)
        {
            string sql = "select COLUMN_NAME, TABLE_NAME, TABLE_SCHEMA from INFORMATION_SCHEMA.COLUMNS where COLUMNPROPERTY(object_id(TABLE_SCHEMA+'.'+TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1 order by TABLE_NAME";
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            var dt = ExecuteDataTable(sql, connection);
            foreach (DataRow dr in dt.Rows)
            {
                dic[(dr["TABLE_SCHEMA"].ToString() + dr["TABLE_NAME"].ToString() + dr["COLUMN_NAME"].ToString()).ToUpperInvariant()] = true;
            }
            return dic;
        }

        private Dictionary<string, bool> LoadPrimaryColumns(SqlConnection connection)
        {
            //string sql = "select COLUMN_NAME, TABLE_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA = '" + SchemaName + "' and COLUMNPROPERTY(object_id(TABLE_NAME), COLUMN_NAME, 'IsPrimaryKey') = 1 order by TABLE_NAME";
            string sql =
                "SELECT K.TABLE_NAME,K.TABLE_SCHEMA,K.COLUMN_NAME,K.CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS C JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS K ON C.TABLE_NAME = K.TABLE_NAME AND C.CONSTRAINT_CATALOG = K.CONSTRAINT_CATALOG AND C.CONSTRAINT_SCHEMA = K.CONSTRAINT_SCHEMA AND C.CONSTRAINT_NAME = K.CONSTRAINT_NAME WHERE C.CONSTRAINT_TYPE = 'PRIMARY KEY'";
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            var dt = ExecuteDataTable(sql, connection);
            foreach (DataRow dr in dt.Rows)
            {
                dic[(dr["TABLE_SCHEMA"].ToString() + dr["TABLE_NAME"].ToString() + dr["COLUMN_NAME"].ToString()).ToUpperInvariant()] = true;
            }
            return dic;
        }
    }
}