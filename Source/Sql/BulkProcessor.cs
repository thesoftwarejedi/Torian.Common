using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Collections.Generic;

/// as seen at http://bryanoconnell.blogspot.com/2013/06/using-linq-to-sql-for-etl.html
namespace Torian.Common.Sql
{
    public static class BulkProcessor
    {
        public static void Cram<T>(List<T> ObjectsToInsert, string TableName, string ConnectionString) where T : class
        {
            using (DataTable TableToInsert = BuildDataTable(ObjectsToInsert))
            {
                using (SqlBulkCopy BulkCopy = new SqlBulkCopy(ConnectionString, SqlBulkCopyOptions.KeepIdentity & SqlBulkCopyOptions.KeepNulls))
                {
                    foreach (DataColumn DC in TableToInsert.Columns)
                    {
                        BulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(DC.ColumnName, FixLinqFieldNaming(DC.ColumnName)));
                    }

                    // LINQ will sometimes change a table name from singular to 
                    // plural or plural to singular. Forcing the user to 
                    // provide the table name they want to use is safer.
                    BulkCopy.DestinationTableName = "[" + TableName + "]";
                    BulkCopy.WriteToServer(TableToInsert);
                }
            }
        }

        /// <summary>Converts a list of LINQ objects to a DataTable.</summary>
        private static DataTable BuildDataTable<T>(List<T> ObjectList)
        {
            Type ObjectType = typeof(T);
            DataTable TableToBuild = new DataTable(ObjectType.Name);
            List<PropertyInfo> DataFieldProperties = new List<PropertyInfo>();

            TableToBuild.BeginLoadData();

            // Add Columns to the Table:
            foreach (PropertyInfo PropInfo in ObjectType.GetProperties())
            {
                Type CurrentType = PropInfo.PropertyType;

                if (CurrentType.IsGenericType && (CurrentType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    CurrentType = Nullable.GetUnderlyingType(CurrentType);
                }

                if (CurrentType.IsPrimitive || CurrentType.IsValueType || (CurrentType == typeof(string)))
                {
                    TableToBuild.Columns.Add(PropInfo.Name, CurrentType);
                    DataFieldProperties.Add(PropInfo);
                }
            }

            // Fill Table with data:
            foreach (var DataObject in ObjectList)
            {
                List<object> FieldValues = new List<object>();
                foreach (PropertyInfo PropInfo in DataFieldProperties)
                {
                    FieldValues.Add(PropInfo.GetValue(DataObject, null));
                }
                TableToBuild.LoadDataRow(FieldValues.Cast<object>().ToArray(), true);
            }

            TableToBuild.EndLoadData();
            return TableToBuild;
        }

        /// <summary>LINQ alters a field's name when it's the same or very 
        /// similar to the table name by adding a '1'. SqlBulkInsert will not
        /// work if this naming change isn't corrected.</summary>
        private static string FixLinqFieldNaming(string FieldName)
        {
            string NameToUse = FieldName;
            const char ONE = '1';

            if (FieldName.EndsWith(ONE.ToString()))
            {
                NameToUse = FieldName.TrimEnd(ONE);
            }

            return NameToUse;
        }
    }
}
