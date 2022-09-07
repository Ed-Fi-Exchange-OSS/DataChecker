// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace MSDF.DataChecker.Services
{
    public static class Utils
    {
        public static List<Dictionary<string, object>> Serialize(List<string> columnsName, List<DataRow> rows)
        {
            var results = new List<Dictionary<string, object>>();
            foreach (DataRow dr in rows)
            {
                results.Add(SerializeRow(columnsName, dr));
            }
            return results;
        }

        public static string GenerateSqlWithTop(string diagnosticSql, string maxNumberResults, string Engine)
        {
            string result = diagnosticSql;
            result = result.ToLower().Trim();
            if (result.EndsWith(";"))
            {
                result = result.Remove(result.Length-1,1);
            }


            if (result.StartsWith("select"))
            {
                if (!result.StartsWith("select top"))
                {
                    if (result.StartsWith("select distinct"))
                    {
                        if (Engine == "SqlServer")
                        {
                            var regex = new Regex(Regex.Escape("select distinct"));
                            result = regex.Replace(result, "select distinct top " + maxNumberResults + " ", 1);
                        }
                        else
                        {
                            string pattern = @"limit\s\d+";
                            Match m = Regex.Match(result, pattern, RegexOptions.IgnoreCase);
                            if (! m.Success)
                                result = result + $" Limit {maxNumberResults} ";
                        }
                    }
                    else
                    {
                        if (Engine == "SqlServer")
                        {
                            var regex = new Regex(Regex.Escape("select"));
                            result = regex.Replace(result, "select top " + maxNumberResults + " ", 1);
                        }
                        else
                        {
                            string pattern = @"limit\s\d+";
                            Match m = Regex.Match(result, pattern, RegexOptions.IgnoreCase);
                            if (!m.Success)
                                result = result + $" Limit {maxNumberResults} ";
                        }
                    }
                }
            }
            else if (result.StartsWith("with"))
            {
                if (!result.Contains(") select top"))
                {
                    if (Engine == "SqlServer")
                    {
                        if (result.Contains(") select distinct"))
                        {
                            var regex = new Regex(Regex.Escape(") select distinct"));
                            result = regex.Replace(result, ") select distinct top " + maxNumberResults + " ", 1);
                        }
                        else
                        {
                            var regex = new Regex(Regex.Escape(") select"));
                            result = regex.Replace(result, ") select top " + maxNumberResults + " ", 1);
                        }
                    }
                }
            }

            return result;
        }

        public static string GenerateSqlWithCount(string diagnosticSql)
        {
            string result = diagnosticSql;
            result = result.ToLower().Trim().Replace(";", "");

            if (result.StartsWith("select"))
            {
                return string.Format("SELECT COUNT(*) FROM ( \n {0} \n) as TBL", result);
            }

            return string.Empty;
        }

        public static DataTable GetTableForSqlBulk(int ruleExecutionLogId, DataTable reader, Dictionary<string, string> columns, out string columnsSchema)
        {
            Dictionary<string, string> listColumnsSchema = new Dictionary<string, string>();
            DataTable table = new DataTable();
            List<string> classColumns = columns.Select(rec => rec.Key).ToList();
            columnsSchema = string.Empty;

            foreach (var column in columns)
            {
                if (column.Value.Contains("int"))
                    table.Columns.Add(column.Key, typeof(int));
                else if (column.Value.Contains("varchar"))
                    table.Columns.Add(column.Key, typeof(string));
                else if (column.Value.Contains("date"))
                    table.Columns.Add(column.Key, typeof(DateTime));
                else if (column.Value.Contains("decimal") || column.Value.Contains("numeric"))
                    table.Columns.Add(column.Key, typeof(decimal));
                else if (column.Value.Contains("float"))
                    table.Columns.Add(column.Key, typeof(double));
                else if (column.Value.Contains("text"))
                    table.Columns.Add(column.Key, typeof(string));
            }

            if (reader.Rows.Count > 0)
            {
                List<string> staticColumns = new List<string>();
                List<string> jsonColumns = new List<string>();

                var columnsSource = reader.Columns;
                foreach (DataColumn column in columnsSource)
                {
                    string columnName = column.ColumnName.ToLower();
                    if (classColumns.Contains(columnName))
                    {
                        staticColumns.Add(columnName);
                    }
                    else
                    {
                        jsonColumns.Add(columnName);
                    }

                    if (column.DataType.Name.ToLower().Contains("date"))
                        listColumnsSchema.Add(column.ColumnName, "datetime");
                    else
                        listColumnsSchema.Add(column.ColumnName, "string");
                }

                foreach (DataRow row in reader.Rows)
                {
                    DataRow dr = table.NewRow();
                    dr["ruleexecutionlogid"] = ruleExecutionLogId;

                    foreach (string column in staticColumns)
                    {
                        dr[column] = row[column].ToString();
                    }

                    JObject jsonObject = new JObject();
                    foreach (string column in jsonColumns)
                    {
                        string strNewValue = row[column].ToString();
                        jsonObject.Add(column, strNewValue);
                    }

                    dr["otherdetails"] = jsonObject.ToString();
                    table.Rows.Add(dr);
                }

            }

            columnsSchema = Newtonsoft.Json.JsonConvert.SerializeObject(listColumnsSchema);
            return table;
        }

        public static DataTable GetTableForSqlBulk(int ruleExecutionLogId, DbDataReader reader, Dictionary<string, string> columns, out string columnsSchema)
        {
            Dictionary<string, string> listColumnsSchema = new Dictionary<string, string>();
            DataTable table = new DataTable();
            List<string> classColumns = columns.Select(rec => rec.Key).ToList();
            columnsSchema = string.Empty;

            foreach (var column in columns)
            {
                if (column.Value.Contains("int"))
                    table.Columns.Add(column.Key, typeof(int));
                else if (column.Value.Contains("varchar"))
                    table.Columns.Add(column.Key, typeof(string));
                else if (column.Value.Contains("date"))
                    table.Columns.Add(column.Key, typeof(DateTime));
                else if (column.Value.Contains("decimal") || column.Value.Contains("numeric"))
                    table.Columns.Add(column.Key, typeof(decimal));
                else if (column.Value.Contains("float"))
                    table.Columns.Add(column.Key, typeof(double));
                else if (column.Value.Contains("text"))
                    table.Columns.Add(column.Key, typeof(string));
            }

            if (reader.HasRows)
            {
                List<string> staticColumns = new List<string>();
                List<string> jsonColumns = new List<string>();

                var columnsSource = reader.GetColumnSchema();
                foreach (var column in columnsSource)
                {
                    string columnName = column.ColumnName.ToLower();
                    if (classColumns.Contains(columnName))
                    {
                        staticColumns.Add(columnName);
                    }
                    else
                    {
                        jsonColumns.Add(columnName);
                    }

                    if (column.DataTypeName.ToLower().Contains("date"))
                        listColumnsSchema.Add(column.ColumnName, "datetime");
                    else
                        listColumnsSchema.Add(column.ColumnName, "string");
                }

                while (reader.Read())
                {
                    DataRow dr = table.NewRow();
                    dr["ruleexecutionlogid"] = ruleExecutionLogId;

                    foreach (string column in staticColumns)
                    {
                        dr[column] = reader.GetValue(column).ToString();
                    }

                    JObject jsonObject = new JObject();
                    foreach (string column in jsonColumns)
                    {
                        string strNewValue = reader.GetValue(column).ToString();
                        jsonObject.Add(column, strNewValue);
                    }

                    dr["otherdetails"] = jsonObject.ToString();
                    table.Rows.Add(dr);
                }
            }

            columnsSchema = Newtonsoft.Json.JsonConvert.SerializeObject(listColumnsSchema);
            reader.Close(); // <- too easy to forget
            reader.Dispose(); // <- too easy to forget
            return table;
        }

        public static DataTable GetTableForSqlBulk(List<Dictionary<string, string>> rows, Dictionary<string, string> columns)
        {
            DataTable table = new DataTable();

            foreach (var column in columns)
            {
                if (column.Value.Contains("date"))
                    table.Columns.Add(column.Key, typeof(DateTime));
                else
                    table.Columns.Add(column.Key, typeof(string));
            }

            if (rows != null && rows.Count > 0)
            {
                foreach (var row in rows)
                {
                    DataRow dr = table.NewRow();
                    foreach (var column in columns)
                    {
                        dr[column.Key] = row[column.Key];
                    }
                    table.Rows.Add(dr);
                }
            }

            return table;
        }

        private static Dictionary<string, object> SerializeRow(IEnumerable<string> cols, DataRow dr)
        {
            var result = new Dictionary<string, object>();
            DateTime dtAux;
            foreach (var col in cols)
            {
                if (!dr.IsNull(col))
                {
                    var columnValue = dr[col];
                    if (columnValue.GetType() == typeof(DateTime))
                    {
                        if (DateTime.TryParse(columnValue.ToString(), out dtAux))
                            result.Add(col, dtAux.ToString("MM/dd/yyyy"));
                        else
                            result.Add(col, columnValue);
                    }
                    else
                    {
                        result.Add(col, columnValue);
                    }
                }
                else
                {
                    result.Add(col, "NULL");
                }
            }
            return result;
        }
    }
}
