using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace PipePressureDrop.Utility
{
    internal class ExcelExporter
    {
        public static void ListToExcel<T>(string fileName, IList<T> objects)
            where T : class
        {
            using (var ep = new ExcelPackage(new FileInfo(fileName)))
            {
                var worksheet = ep.Workbook.Worksheets.Add("Sheet1");
                var properties = typeof(T).GetProperties();
                for (var i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }
                for (var i = 0; i < objects.Count; i++)
                {
                    for (var j = 0; j < properties.Length; j++)
                    {
                        worksheet.Cells[i + 2, j + 1].Value = properties[j].GetValue(objects[i], null);
                    }
                }
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells.AutoFitColumns();
                ep.Save();
            }
        }
    }
}
