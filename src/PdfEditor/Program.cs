using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Font;
using iText.IO.Font;

class Program
{
    static string pdfPath = @"D:\Projetos\PdfEditor\data\input\Aditivo_Contratual_ORÇAMENTO_v2024_PF.pdf";
    static string outputPath = @"D:\Projetos\PdfEditor\data\output\Aditivo_Contratual_ORÇAMENTO_v2024_PF_Atualizado.pdf";
    static string fontPath = @"D:\Projetos\PdfEditor\data\fonts\Arial.ttf";

    static void Main(string[] args)
    {  
        string nomeConstrutora = "NovaConstrutora";
        string adquirentes = "Nome dos Adquirentes";
        string numeroApto = "101";
        string torre = "A";
        string nomeEmpreendimento = "Empreendimento X";

        var textReplacements = new Dictionary<string, string>
        {
            { "[NomeConstrutora]", nomeConstrutora },
            { "[Adquirentes]", adquirentes },
            { "[NumeroApto]", numeroApto },
            { "[Torre]", torre },
            { "[NomeEmpreendimento]", nomeEmpreendimento }
        };

        using (PdfReader reader = new PdfReader(pdfPath))
        using (PdfWriter writer = new PdfWriter(outputPath))
        using (iText.Kernel.Pdf.PdfDocument pdfDoc = new iText.Kernel.Pdf.PdfDocument(reader, writer))
        {
            var page = pdfDoc.GetPage(1); 
            var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);

            foreach (var texto in textReplacements.Keys)
            {
                var posicaoPalavra = EncontrarPosicaoPalavra(pdfPath, texto);
                if (posicaoPalavra != null)
                {
                    //RemoverTexto(page, posicaoPalavra, texto, pdfDoc);
                    AlterarPalavras(page, font, posicaoPalavra, textReplacements[texto], pdfDoc);
                }
                else
                {
                    Console.WriteLine($"A palavra '{texto}' não foi encontrada no PDF.");
                }
            }
        }
        
    }

    static Tuple<double, double>? EncontrarPosicaoPalavra(string pdfPath, string palavraAlvo)
    {
        using (UglyToad.PdfPig.PdfDocument document = UglyToad.PdfPig.PdfDocument.Open(pdfPath))
        {
            var primeiraPagina = document.GetPage(1);

            foreach (var word in primeiraPagina.GetWords())
            {
                string textoLimpado = word.Text.TrimEnd(',', '.', ';', ':', '!', '?');

                if (textoLimpado.Equals(palavraAlvo, StringComparison.OrdinalIgnoreCase))
                {
                    return Tuple.Create(word.BoundingBox.Left, word.BoundingBox.Bottom);
                }
            }
        }
        return null;
    }

    static void AlterarPalavras(iText.Kernel.Pdf.PdfPage page, PdfFont font, Tuple<double, double> posicao, string textNew, iText.Kernel.Pdf.PdfDocument pdfDoc)
    {
        var canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);
        canvas.BeginText()
              .SetFontAndSize(font, 12)
              .MoveText((float)posicao.Item1, (float)posicao.Item2)
              .ShowText(textNew)
              .EndText();
    }

    static void RemoverTexto(iText.Kernel.Pdf.PdfPage page, Tuple<double, double> posicao, string texto, iText.Kernel.Pdf.PdfDocument pdfDoc)
    {
        float fontSize = 12f; 

        var larguraTexto = texto.Length * fontSize; 
        var alturaTexto = fontSize; 

        var canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);

        canvas.SetFillColorRgb(255, 255, 255);
        canvas.Rectangle((float)posicao.Item1, (float)posicao.Item2, larguraTexto, alturaTexto);
        canvas.Fill();
    }
}