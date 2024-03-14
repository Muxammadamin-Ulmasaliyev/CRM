using InventoryManagementSystem.Model;
using Notification.Wpf;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace InventoryManagementSystem.Services
{
    public class ChequeDocumentXlsx
    {


        public static void ExportToExcel(List<Product> products, string filePath)
        {
            XSSFWorkbook workbook = new();

            ISheet sheet = workbook.CreateSheet("Products");
            
            // Header cell font styling
            IRow headerRow = sheet.CreateRow(1);
            ICellStyle borderedHeaderStyle = workbook.CreateCellStyle();

            IFont headerFontStyle = workbook.CreateFont();
            headerFontStyle.FontName = "Times New Roman";
            headerFontStyle.FontHeightInPoints = 16;
            headerFontStyle.Boldweight = (short)FontBoldWeight.Bold;
            borderedHeaderStyle.SetFont(headerFontStyle);

           


            borderedHeaderStyle.BorderBottom = BorderStyle.Thin;
            borderedHeaderStyle.BorderTop = BorderStyle.Thin;
            borderedHeaderStyle.BorderLeft = BorderStyle.Thin;
            borderedHeaderStyle.BorderRight = BorderStyle.Thin;
            borderedHeaderStyle.Alignment = HorizontalAlignment.Center;


            // Cell borders styling
            ICellStyle borderedUpBottomCellStyle = workbook.CreateCellStyle();
            borderedUpBottomCellStyle.BorderTop = BorderStyle.Thin;
            borderedUpBottomCellStyle.BorderBottom = BorderStyle.Thin;
            borderedUpBottomCellStyle.BorderLeft = BorderStyle.None;
            borderedUpBottomCellStyle.BorderRight = BorderStyle.None;
            borderedUpBottomCellStyle.Alignment = HorizontalAlignment.Center;

            ICellStyle borderedLeftUpBottomCellStyle = workbook.CreateCellStyle();
            borderedLeftUpBottomCellStyle.BorderBottom = BorderStyle.Thin;
            borderedLeftUpBottomCellStyle.BorderLeft = BorderStyle.Thin;
            borderedLeftUpBottomCellStyle.BorderTop = BorderStyle.Thin;
            borderedLeftUpBottomCellStyle.Alignment = HorizontalAlignment.Center;


            ICellStyle borderedRightUpBottomCellStyle = workbook.CreateCellStyle();
            borderedRightUpBottomCellStyle.BorderBottom = BorderStyle.Thin;
            borderedRightUpBottomCellStyle.BorderRight = BorderStyle.Thin;
            borderedRightUpBottomCellStyle.BorderTop = BorderStyle.Thin;
            borderedRightUpBottomCellStyle.Alignment = HorizontalAlignment.Center;



            IFont fontStyle = workbook.CreateFont();
            fontStyle.FontName = "Times New Roman";
            fontStyle.FontHeightInPoints = 15;

            borderedUpBottomCellStyle.SetFont(fontStyle);
            borderedLeftUpBottomCellStyle.SetFont(fontStyle);
            borderedRightUpBottomCellStyle.SetFont(fontStyle);




            // Createing header cells
            headerRow.CreateCell(1).SetCellValue("Id");
            headerRow.CreateCell(2).SetCellValue("Товар");
            headerRow.CreateCell(3).SetCellValue("Kodi");
            headerRow.CreateCell(4).SetCellValue("Нарх");
            headerRow.CreateCell(5).SetCellValue("$ Нарх");
            headerRow.CreateCell(6).SetCellValue("Сони");
            headerRow.CreateCell(7).SetCellValue("Завод");
            headerRow.CreateCell(8).SetCellValue("Давлат");
            headerRow.CreateCell(9).SetCellValue("Машина");
            headerRow.CreateCell(10).SetCellValue("Улчов");

            // Styling header cells
            for (int i = 1; i <= 10; i++)
            {
                headerRow.GetCell(i).CellStyle = borderedHeaderStyle;
            }






            for (int i = 0; i < products.Count; i++)
            {
                IRow dataRow = sheet.CreateRow(i + 2);

                ICell idCell = dataRow.CreateCell(1);
                idCell.SetCellValue(products[i].Id);
                idCell.CellStyle = borderedLeftUpBottomCellStyle;

                ICell nameCell = dataRow.CreateCell(2);
                nameCell.SetCellValue(products[i].Name);
                nameCell.CellStyle = borderedUpBottomCellStyle;

                ICell codeCell = dataRow.CreateCell(3);
                codeCell.SetCellValue(products[i].Code);
                codeCell.CellStyle = borderedUpBottomCellStyle;

                ICell priceCell = dataRow.CreateCell(4);
                priceCell.SetCellValue(products[i].Price.ToString());
                priceCell.CellStyle = borderedUpBottomCellStyle;

                ICell usdPriceCell = dataRow.CreateCell(5);
                usdPriceCell.SetCellValue(products[i].USDPriceForCustomer.ToString());
                usdPriceCell.CellStyle = borderedUpBottomCellStyle;

                ICell quantityCell = dataRow.CreateCell(6);
                quantityCell.SetCellValue(products[i].Quantity);
                quantityCell.CellStyle = borderedUpBottomCellStyle;

                ICell companyCell = dataRow.CreateCell(7);
                companyCell.SetCellValue(products[i].Company.Name);
                companyCell.CellStyle = borderedUpBottomCellStyle;

                ICell countryCell = dataRow.CreateCell(8);
                countryCell.SetCellValue(products[i].Country.Name);
                countryCell.CellStyle = borderedUpBottomCellStyle;

                ICell carTypeCell = dataRow.CreateCell(9);
                carTypeCell.SetCellValue(products[i].CarType.Name);
                carTypeCell.CellStyle = borderedUpBottomCellStyle;

                ICell setTypeCell = dataRow.CreateCell(10);
                setTypeCell.SetCellValue(products[i].SetType.Name);
                setTypeCell.CellStyle = borderedRightUpBottomCellStyle;

            }


            sheet = MakeColumnsAutoresizable(sheet, 10);



            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fileStream);
            }

            NotificationManager notificationManager = new();
            notificationManager.Show("Success", "Excel file created successfully", NotificationType.Success);


        }

        private static ISheet MakeColumnsAutoresizable(ISheet sheet, int noOfColumns)
        {
            for (int i = 1; i <= noOfColumns; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            return sheet;
        }
    }
}
