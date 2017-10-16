﻿using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf.Tables;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wibci.LogicCommand;

namespace Samples.XamarinForms.AzureBlobStorage
{
    public interface IGenerateInvoiceCommand : IAsyncLogicCommand<GenerateInvoiceContext, GenerateDefaultInvoiceResult>
    {
    }

    public class GenerateDefaultInvoiceResult : TaskCommandResult
    {
        public string FileName { get; set; }
        public byte[] PdfResult { get; set; }
    }

    public class GenerateInvoiceCommand : AsyncLogicCommand<GenerateInvoiceContext, GenerateDefaultInvoiceResult>, IGenerateInvoiceCommand
    {
        private const float FOOTER_HEIGHT = 20;

        public override Task<GenerateDefaultInvoiceResult> ExecuteAsync(GenerateInvoiceContext request)
        {
            GenerateDefaultInvoiceResult retResult = new GenerateDefaultInvoiceResult();

            //https://help.syncfusion.com/file-formats/pdf/getting-started
            PdfGenerator pdf = new PdfGenerator();
            pdf.Setup("Invoice");

            float y = GenerateBodyHeader(request, pdf);

            y = GenerateBody(request, pdf, y, !request.SimpleFormat);

            //if the next bit (total + signature) might be split - rather create new page
            y = pdf.IncrementY(y, 0, FOOTER_HEIGHT, 180); //110 + 70 = estimated height of total + signature

            y = GenerateTotal(request, pdf, y);

            y = GenerateSignature(request, pdf, y);

            y = GenerateTermsBody(request, pdf, y);

            GenerateFooter(request, pdf); //BUG: Generate footer before body to cater for bug where paginatebounds not taken into account for pdfLightTable
            //Save the document.
            //await pdf.SaveAsync(request.FileName, launchFile: false);
            retResult.PdfResult = pdf.SaveDocumentAsByteArray();

            return Task.FromResult(retResult);
        }

        private float GenerateBody(GenerateInvoiceContext request, PdfGenerator pdf, float currentY, bool generateItems)
        {
            float y = currentY;

            y = pdf.IncrementY(y, 10, FOOTER_HEIGHT);
            PdfLayoutResult result = pdf.AddText(request.Invoice.Description, 10, y);

            y = result.Bounds.Bottom;
            y = pdf.IncrementY(y, 0, FOOTER_HEIGHT);

            if (generateItems)
            {
                if (request.SimpleTableItems)
                {
                    y = GenerateItemizedBodyWithLightTable(request, pdf, y);
                }
                else
                {
                    y = GenerateItemizedBodyWithGrid(request, pdf, y);
                }
            }

            return y;
        }

        private float GenerateBodyHeader(GenerateInvoiceContext request, PdfGenerator pdf)
        {
            PdfLayoutResult result = null;
            float y = 10;
            //Add IMAGE
            pdf.AddImage(request.Invoice.Logo, 0, y, request.LogoWidth, request.LogoHeight);

            var font = pdf.NormalFontBold;
            float x = request.LogoWidth > 0 ? request.LogoWidth + 10 : 0;
            result = pdf.AddText(request.Invoice.BusinessName, x, y, font);
            result = pdf.AddText(request.Invoice.BusinessInfo, x, result.Bounds.Bottom + 2);
            float businessY = result.Bounds.Bottom + 10;

            string twitterLink = "http://twitter.com/syncfusion";
            string webLink = "https://www.syncfusion.com/";
            string facebook = "https://www.facebook.com/Syncfusion";
            string youtube = "https://www.youtube.com/syncfusioninc";
            string linkedIn = "https://www.linkedin.com/company/syncfusion?trk=top_nav_home";
            string instagram = "http://www.instagram.com";

            string[] socialLinks = new string[] { "twitter", "website", "facebook", "youtube", "linkedIn", "instagram" };

            //left align all the social links
            float xOffsetTextSize = pdf.LongestText(socialLinks);

            result = pdf.AddWebLink(webLink, 0, y, "website", null, true, xOffsetTextSize);
            result = pdf.AddWebLink(facebook, 0, result.Bounds.Bottom, "facebook", null, true, xOffsetTextSize);
            result = pdf.AddWebLink(twitterLink, 0, result.Bounds.Bottom, "twitter", null, true, xOffsetTextSize);
            result = pdf.AddWebLink(youtube, 0, result.Bounds.Bottom, "youtube", null, true, xOffsetTextSize);
            result = pdf.AddWebLink(linkedIn, 0, result.Bounds.Bottom, "linkedin", null, true, xOffsetTextSize);
            result = pdf.AddWebLink(instagram, 0, result.Bounds.Bottom, "instagram", null, true, xOffsetTextSize);

            float socialLinksY = result.Bounds.Bottom + 10;
            float imageY = request.LogoHeight + 15;

            float actualY = Enumerable.Max(new float[] { businessY, socialLinksY, imageY });
            y = actualY;

            string customer = request.Invoice.Customer + Environment.NewLine + request.Invoice.Address;
            result = pdf.AddText(customer, 0, y, font);

            y = result.Bounds.Bottom;
            y = pdf.IncrementY(y, 10, FOOTER_HEIGHT, 30);

            string leftText = "INVOICE #" + request.Invoice.Number.ToString();
            string rightText = DateTime.Now.ToString("dd MMM yyyy");
            result = pdf.AddRectangleText(leftText, rightText, y, 30);

            y = result.Bounds.Bottom;
            y = pdf.IncrementY(y, 15, FOOTER_HEIGHT);

            //Creates text elements to add the address and draw it to the page.
            result = pdf.AddText(request.Invoice.Heading, 10, y, pdf.SubHeadingFont);
            y = result.Bounds.Bottom;
            y = pdf.IncrementY(y, 3, FOOTER_HEIGHT);

            pdf.DrawHorizontalLine(0, pdf.PageWidth, y, 0.7f, pdf.AccentColor);

            y = pdf.IncrementY(y, 1, FOOTER_HEIGHT);

            return y;
        }

        private void GenerateFooter(GenerateInvoiceContext request, PdfGenerator pdf)
        {
            //https://help.syncfusion.com/file-formats/pdf/working-with-headers-and-footers

            RectangleF bounds = new RectangleF(0, 0, pdf.PageWidth, FOOTER_HEIGHT);

            PdfPageTemplateElement footer = new PdfPageTemplateElement(bounds);

            //BUG: NullReferenceException when trying to put a web link into the footer - drawing web links using footer graphics not supported yet by syncfusion
            //pdf.DrawWebLink(0, 35, "http://www.syncfusion.com", "Awesome control library for your mobile cross platform needs", pdf.NormalFont, footer.Graphics);
            //pdf.DrawWebLinkPageBottom(0, 35, "http://www.syncfusion.com", "Awesome control library for your mobile cross platform needs", pdf.NormalFont);

            PdfCompositeField compositeField = new PdfCompositeField(pdf.NormalFont, pdf.AccentBrush, "http://www.syncfusion.com Awesome control library for your mobile cross platform needs");
            compositeField.Bounds = footer.Bounds;

            //Draw the composite field in footer.

            compositeField.Draw(footer.Graphics, new PointF(0, 0));

            pdf.DrawHorizontalLine(0, pdf.PageWidth, 0, 0.7f, pdf.AccentColor, footer.Graphics);

            pdf.Document.Template.Bottom = footer;
        }

        private void GenerateHeader(GenerateInvoiceContext request, PdfGenerator pdf)
        {
            RectangleF bounds = new RectangleF(0, 0, pdf.PageWidth, 50);

            PdfPageTemplateElement header = new PdfPageTemplateElement(bounds);

            pdf.DrawText("Header Text", 10, 0, pdf.NormalFont, pdf.AccentBrush, header.Graphics);

            pdf.Document.Template.Top = header;
        }

        private float GenerateItemizedBodyWithGrid(GenerateInvoiceContext request, PdfGenerator pdf, float y)
        {
            y = pdf.IncrementY(y, 10, FOOTER_HEIGHT);
            //Create a new PdfGrid.

            PdfGrid pdfGrid = new PdfGrid();

            //Add four columns.

            pdfGrid.Columns.Add(4);
            var columnFormat = new PdfStringFormat
            {
                Alignment = PdfTextAlignment.Center,
                LineAlignment = PdfVerticalAlignment.Middle
            };
            pdfGrid.Columns[1].Format = columnFormat;
            pdfGrid.Columns[2].Format = columnFormat;
            pdfGrid.Columns[3].Format = columnFormat;

            //Add header.

            pdfGrid.Headers.Add(1);

            PdfGridRow pdfGridHeader = pdfGrid.Headers[0];

            pdfGridHeader.Cells[0].Value = "Item";
            pdfGridHeader.Cells[1].Value = "Cost";
            pdfGridHeader.Cells[2].Value = "Qty";
            pdfGridHeader.Cells[3].Value = "Total";

            //Add rows.
            foreach (var item in request.Invoice.Items)
            {
                PdfGridRow pdfGridRow = pdfGrid.Rows.Add();

                //NOTE: It seems that values MUST be string values

                pdfGridRow.Cells[0].Value = item.Name;
                pdfGridRow.Cells[1].Value = item.ItemAmount.ToString("n2");
                pdfGridRow.Cells[2].Value = item.Quantity.ToString("n0");
                pdfGridRow.Cells[3].Value = item.Amount.ToString("n2");
            }

            var data = request.Invoice.Items.Select(x => x.ItemAmount.ToString("n2"));
            pdfGrid.Columns[1].SizeColumnToContent(data, pdf.PageWidth, pdf.NormalFont);
            data = request.Invoice.Items.Select(x => x.Quantity.ToString("n2"));
            pdfGrid.Columns[2].SizeColumnToContent(data, pdf.PageWidth, pdf.NormalFont);
            data = request.Invoice.Items.Select(x => x.Amount.ToString("n2"));
            pdfGrid.Columns[3].SizeColumnToContent(data, pdf.PageWidth, pdf.NormalFont);

            //Apply built-in table style
            //NOTE: that the accent2 color of #FFED7D31 is used in generating the total rectangle as well
            pdfGrid.ApplyBuiltinStyle(PdfGridBuiltinStyle.GridTable4Accent2);

            //Apply Custom Style
            //pdfGrid.Style = new PdfGridStyle
            //{
            //    //BackgroundBrush = pdf.AccentBrush,
            //    TextBrush = pdf.AccentBrush,
            //    //TextPen = new PdfPen(pdf.AccentBrush)
            //};

            PdfGridLayoutFormat format = new PdfGridLayoutFormat();
            format.Layout = PdfLayoutType.Paginate;
            format.PaginateBounds = new RectangleF(0, 0, pdf.CurrentPage.Graphics.ClientSize.Width, pdf.CurrentPage.Graphics.ClientSize.Height - FOOTER_HEIGHT);

            //Draw the PdfGrid.
            var result = pdfGrid.Draw(pdf.CurrentPage, new PointF(10, y), format);

            return result.Bounds.Bottom;
        }

        private float GenerateItemizedBodyWithLightTable(GenerateInvoiceContext request, PdfGenerator pdf, float currentY)
        {
            float y = currentY + 10;

            PdfLightTable pdfLightTable = new PdfLightTable();

            pdfLightTable.ApplyLightTableStyle(pdf.PdfGridStyle2Color, pdf.AccentColor, pdf.PdfGridStyle2AltColor, new PdfColor(Color.White));

            pdfLightTable.DataSourceType = PdfLightTableDataSourceType.TableDirect;

            //Add columns to the DataTable
            var columnFormat = new PdfStringFormat
            {
                Alignment = PdfTextAlignment.Center,
                LineAlignment = PdfVerticalAlignment.Middle
            };
            pdfLightTable.Columns.Add(new PdfColumn("Title") { StringFormat = columnFormat });
            pdfLightTable.Columns.Add(new PdfColumn("Cost") { StringFormat = columnFormat });
            pdfLightTable.Columns.Add(new PdfColumn("Qty") { StringFormat = columnFormat });
            pdfLightTable.Columns.Add(new PdfColumn("Total") { StringFormat = columnFormat });

            foreach (var item in request.Invoice.Items)
            {
                pdfLightTable.Rows.Add(new object[] { item.Name, item.ItemAmount.ToString("n2"), item.Quantity.ToString(), item.Amount.ToString("n2") });
            }

            //resize columns to content width - current a BUG in PdfLightTable implementation (works for PdfGrid)
            //ref: https://www.syncfusion.com/forums/131302/pdfgrid-size-grid-to-content

            //var data = request.Invoice.Items.Select(x => x.ItemAmount.ToString("n2"));
            //pdfLightTable.Columns[1].SizeColumnToContent(data, pdf.PageWidth, pdf.NormalFont);
            //data = request.Invoice.Items.Select(x => x.Quantity.ToString("n2"));
            //pdfLightTable.Columns[2].SizeColumnToContent(data, pdf.PageWidth, pdf.NormalFont);
            //data = request.Invoice.Items.Select(x => x.Amount.ToString("n2"));
            //pdfLightTable.Columns[3].SizeColumnToContent(data, pdf.PageWidth, pdf.NormalFont);

            PdfLightTableLayoutFormat layoutFormat = new PdfLightTableLayoutFormat();

            layoutFormat.Break = PdfLayoutBreakType.FitPage;
            layoutFormat.Layout = PdfLayoutType.Paginate;
            layoutFormat.PaginateBounds = new RectangleF(0, 0, pdf.CurrentPage.Graphics.ClientSize.Width, pdf.CurrentPage.Graphics.ClientSize.Height - FOOTER_HEIGHT);

            var result = pdfLightTable.Draw(pdf.CurrentPage, new PointF(10, y), layoutFormat);

            y = result.Bounds.Bottom;
            y = pdf.IncrementY(y, 0, FOOTER_HEIGHT);

            return y;
        }

        private float GenerateSignature(GenerateInvoiceContext request, PdfGenerator pdf, float y)
        {
            y = pdf.IncrementY(y, 55, FOOTER_HEIGHT); //gap to place signature

            pdf.DrawHorizontalLine(15, 85, y, 0.7f, pdf.AccentColor);
            pdf.DrawHorizontalLine(85, 15, y, 0.7f, pdf.AccentColor, null, true);

            y = pdf.IncrementY(y, 2, FOOTER_HEIGHT);
            var result = pdf.AddText("customer", 15, y);
            result = pdf.AddText("business", 15, y, null, null, true);
            y = result.Bounds.Bottom;
            y = pdf.IncrementY(y, 0, FOOTER_HEIGHT);

            return y;
        }

        private float GenerateTermsBody(GenerateInvoiceContext request, PdfGenerator pdf, float y)
        {
            var check = pdf.MeasureTextHeight(request.Invoice.Terms);
            y = pdf.IncrementY(y, 10, FOOTER_HEIGHT, check); //don't split terms over the footer

            PdfLayoutResult result = pdf.AddText(request.Invoice.Terms, 10, y, pdf.NormalFontBold);
            y = result.Bounds.Bottom;
            //NOTE: last item - don't check for increment

            return y;
        }

        private float GenerateTotal(GenerateInvoiceContext request, PdfGenerator pdf, float currentY)
        {
            float y = currentY;

            double vatAmount = request.Invoice.Amount * (request.Invoice.VatPercentage / 100d);
            double total = request.Invoice.Amount + vatAmount;
            total = Math.Round(total, 2);

            y = pdf.IncrementY(y, 10, FOOTER_HEIGHT);
            string leftText = "TOTAL";
            string rightText = request.Invoice.Currency + " " + request.Invoice.Amount.ToString();
            PdfLayoutResult result = pdf.AddRectangleText(leftText, rightText, y, 30, PdfBrushes.White, pdf.AccentBrush, pdf.NormalFontBold);
            y = result.Bounds.Bottom;
            y = pdf.IncrementY(y, 0, FOOTER_HEIGHT, 30);

            leftText = "VAT" + request.Invoice.VatPercentage + "%";
            rightText = request.Invoice.Currency + " " + vatAmount.ToString();
            result = pdf.AddRectangleText(leftText, rightText, y, 30, PdfBrushes.White, pdf.AccentBrush, pdf.NormalFontBold);
            y = result.Bounds.Bottom;
            y = pdf.IncrementY(y, 5, FOOTER_HEIGHT);

            leftText = "Total Due";
            rightText = request.Invoice.Currency + " " + total.ToString();
            result = pdf.AddRectangleText(leftText, rightText, y, 30, pdf.PdfGridStyle2Brush);

            y = result.Bounds.Bottom;
            y = pdf.IncrementY(y, 10, FOOTER_HEIGHT);

            return y;
        }
    }

    public class GenerateInvoiceContext
    {
        public string FileName { get; set; }

        public Invoice Invoice { get; set; }

        public uint LogoHeight { get; set; }
        public uint LogoWidth { get; set; }
        public bool SimpleFormat { get; set; }

        public bool SimpleTableItems { get; set; }
    }
}