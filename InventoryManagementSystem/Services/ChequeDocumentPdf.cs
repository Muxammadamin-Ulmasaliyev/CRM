using InventoryManagementSystem.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

public class ChequeDocument : IDocument
{
    public Order Order { get; }

    public ChequeDocument(Order order)
    {
        Order = order;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4.Landscape());

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);

                page.Footer().Element(ComposeFooter);


            });
    }

    void ComposeFooter(IContainer container)
    {
        var textStyle = TextStyle.Default.FontSize(16).SemiBold().FontColor(Colors.Blue.Medium).FontFamily("Cambria");

        container.Row(row =>
        {
            row.RelativeItem().AlignLeft().Column(column =>
            {
                column.Item().Text($"Asadbek : +998916034105").Style(textStyle);
                column.Item().Text($"Zohidjon : +998902560976").Style(textStyle);

            });

            row.RelativeItem().AlignBottom().AlignRight().Text(x =>
            {
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });
        });
    }


    void ComposeHeader(IContainer container)
    {
        var titleStyle = TextStyle.Default.FontSize(18).SemiBold().FontColor(Colors.Blue.Medium).FontFamily("Cambria");

        container.Row(row =>
        {
            row.RelativeItem().AlignLeft().Column(column =>
            {
                column.Item().Text($"Invoice #{Order.Id}").Style(titleStyle);

                column.Item().Text(text =>
                {
                    text.Span("Issue date: ").SemiBold();
                    text.Span($"{Order.OrderDate.ToString("dd-MM-yyyy")}");
                });


            });

            row.RelativeItem().AlignRight().Column(column =>
            {
                column.Item().Text($"Customer : {Order.Customer.Name}").Style(titleStyle);
                column.Item().Text(text =>
                {
                    text.Span("Phone : ").SemiBold();
                    text.Span($"{Order.Customer.Phone}");
                });
            });


        });
    }



    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(40).Column(column =>
        {
            column.Spacing(3);

            column.Item().Element(ComposeTable);

            column.Item().AlignRight().DefaultTextStyle(x => x.FontFamily("Cambria")).Text(text =>
            {
                text.Span($"Total Sum : {Order.TotalAmount.ToString("C0", new CultureInfo("uz-UZ"))}").SemiBold();


            });
            column.Item().AlignRight().DefaultTextStyle(x => x.FontFamily("Cambria")).Text(text =>
            {


                // ################################################################################
                text.Span($"To`langan summa : ").SemiBold();

            });
            column.Item().AlignRight().DefaultTextStyle(x => x.FontFamily("Cambria")).Text(text =>
            {
                // ################################################################################

                text.Span($"Qarzdorlik : ").SemiBold();
            });
        });

       
    }


    void ComposeTable(IContainer container)
    {
        container
            .Border(1)
            .Table(table =>
        {

            // step 1
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(25); // Id
                columns.RelativeColumn(); // productname
                columns.RelativeColumn(); //// product car
                columns.RelativeColumn(); // product country
                columns.RelativeColumn(); // product company
                columns.RelativeColumn(); // price
                columns.RelativeColumn(); // quantity
                columns.RelativeColumn(); // subtotal
            });

            // step 2
            table.Header(header =>
            {
                header.Cell().Element(CellStyle).AlignCenter().Text("#");
                header.Cell().Element(CellStyle).Text("Product");
                header.Cell().Element(CellStyle).Text("Car");
                header.Cell().Element(CellStyle).Text("Country");
                header.Cell().Element(CellStyle).Text("Company");
                header.Cell().Element(CellStyle).AlignRight().Text("Unit price");
                header.Cell().Element(CellStyle).AlignRight().Text("Quantity");
                header.Cell().Element(CellStyle).AlignRight().Text("Total");

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold().FontFamily("Cambria").FontSize(12))
                                    .PaddingVertical(1)
                                    .Border(1)
                                    .Background(Colors.Grey.Lighten3)
                                    .BorderColor(Colors.Grey.Lighten3);
                }
            });

            // step 3
            foreach (var item in Order.OrderDetails)
            {
                table.Cell().Element(CellStyle).AlignCenter().Text(Order.OrderDetails.IndexOf(item) + 1);
                table.Cell().Element(CellStyle).Text(item.Product.Name);
                table.Cell().Element(CellStyle).Text(item.Product.CarType.Name);
                table.Cell().Element(CellStyle).Text(item.Product.Country.Name);
                table.Cell().Element(CellStyle).Text(item.Product.Company.Name);
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price.ToString("C0", new CultureInfo("uz-UZ"))}");
                table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity);
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.SubTotal.ToString("C0", new CultureInfo("uz-UZ"))}");

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.FontSize(11).FontFamily("Cambria").Light())
                                    .Border(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(1).PaddingHorizontal(1);

                }
            }
        });
    }
}