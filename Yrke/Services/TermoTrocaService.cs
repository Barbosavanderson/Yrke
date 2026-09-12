using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace Yrke.Services;

public class TermoTrocaService
{
    private readonly IWebHostEnvironment _environment;
    private const string TemplateFileName = "termo_santa_rita.pdf";

    public TermoTrocaService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public byte[] GerarTermo(string nome, DateTime data)
    {
        var templatePath = Path.Combine(_environment.ContentRootPath, "Documents", TemplateFileName);
        if (!File.Exists(templatePath))
            throw new FileNotFoundException("Template do termo não encontrado.", templatePath);

        using var document = PdfReader.Open(templatePath, PdfDocumentOpenMode.Modify);
        var page = document.Pages[0];
        using var gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);

        var font = new XFont("Arial", 11, XFontStyle.Regular);
        var dataFormatada = data.ToString("dd/MM/yyyy");

        gfx.DrawString($"Nome: {nome}", font, XBrushes.Black, new XPoint(120, 680));
        gfx.DrawString($"Data: {dataFormatada}", font, XBrushes.Black, new XPoint(120, 700));

        using var stream = new MemoryStream();
        document.Save(stream, false);
        return stream.ToArray();
    }
}
