using L5Sharp.Core;
using System.Data;

namespace VikingLogixUtility.L5xApp.Models
{
    internal sealed class ExportTable(DataTable table)
    {
        public static ExportTable Create()
        {
            var exportTable = new DataTable();

            exportTable.Columns.Add(new DataColumn("Scope", typeof(string)));
            exportTable.Columns.Add(new DataColumn("Name", typeof(string)));
            exportTable.Columns.Add(new DataColumn("TagType", typeof(string)));
            exportTable.Columns.Add(new DataColumn("AliasFor", typeof(string)));
            exportTable.Columns.Add(new DataColumn("DataType", typeof(string)));
            exportTable.Columns.Add(new DataColumn("MemberDataType", typeof(string)));
            exportTable.Columns.Add(new DataColumn("Description", typeof(string)));
            exportTable.Columns.Add(new DataColumn("Value", typeof(string)));
            exportTable.Columns.Add(new DataColumn("Address", typeof(string)));

            return new ExportTable(exportTable);
        }

        public DataTable Table => table;

        public DataRowCollection Rows => Table.Rows;

        public void AddRow(string scope, string name, TagType? tagType, TagName? aliasFor, string dataType, string memberDataType,
            string? description, LogixData value, string address)
        {
            if (dataType == "NULL")
                dataType = string.Empty;

            if (value.ToString() == "NULL")
                value = string.Empty;

            var row = table.NewRow();

            row["Scope"] = scope;
            row["Name"] = name;
            row["TagType"] = tagType?.ToString() ?? string.Empty;
            row["AliasFor"] = aliasFor?.ToString() ?? string.Empty;
            row["DataType"] = dataType;
            row["MemberDataType"] = memberDataType;
            row["Description"] = description ?? string.Empty;
            row["Value"] = value;
            row["Address"] = address;

            table.Rows.Add(row);
        }
    }
}
