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
        Console.WriteLine("Digite a Construtora: ");
        string nomeConstrutora = Console.ReadLine();

        Console.WriteLine("Digite o Adquirente: ");
        string adquirentes = Console.ReadLine();

        Console.WriteLine("Digite o Número do Apartamento: ");
        string numeroApto = Console.ReadLine();

        Console.WriteLine("Digite a Torre: ");
        string torre = Console.ReadLine();

        Console.WriteLine("Digite o Nome do Empreendimento: ");
        string nomeEmpreendimento = Console.ReadLine();

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
                    RemoverPalavra(page, texto, pdfDoc);
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
              .SetFillColorRgb(0, 0, 0)
              .MoveText((float)posicao.Item1, (float)posicao.Item2)
              .ShowText(textNew)
              .EndText();
    }

    static void RemoverPalavra(iText.Kernel.Pdf.PdfPage page, string palavraAlvo, iText.Kernel.Pdf.PdfDocument pdfDoc)
    {
        using (var document = UglyToad.PdfPig.PdfDocument.Open(pdfPath))
        {
            var primeiraPagina = document.GetPage(1);

            foreach (var word in primeiraPagina.GetWords())
            {
                string textoLimpado = word.Text.TrimEnd(',', '.', ';', ':', '!', '?');

                if (textoLimpado.Equals(palavraAlvo, StringComparison.OrdinalIgnoreCase))
                {
                    double x = word.BoundingBox.Left;
                    double y = word.BoundingBox.Bottom;
                    double largura = word.BoundingBox.Width;
                    double altura = word.BoundingBox.Height;

                    var canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);
                    canvas.SetFillColorRgb(255, 255, 255);
                    canvas.Rectangle((float)x, (float)y - 3, (float)largura, (float)altura + 3);
                    canvas.Fill();

                    Console.WriteLine($"Palavra '{palavraAlvo}' removida na posição ({x}, {y}).");
                }
            }
        }
        
    }

}