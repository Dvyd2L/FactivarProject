using iTextSharp.text;
using iTextSharp.text.pdf;
using Document = iTextSharp.text.Document;

namespace Services;
public class FacturasService
{
    public static void FacturasWritter()
    {
        // Creamos el documento con el tamaño de página tradicional
        Document doc = new(PageSize.A4);

        // Indicamos donde vamos a guardar el documento
        PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(@"C:\Dev\.NET\MicroservicesScheme\FactivarAPI\Docs\prueba.pdf", FileMode.Create));

        // Le colocamos el título y el autor
        // **Nota: Esto no será visible en el documento
        _ = doc.AddTitle("Mi primer PDF");
        _ = doc.AddCreator("Tu nombre");

        // Abrimos el archivo
        doc.Open();

        // Creamos el tipo de Font que vamos utilizar
        Font _standardFont = new(Font.FontFamily.HELVETICA, 8, Font.NORMAL, BaseColor.BLACK);

        // Escribimos el encabezamiento en el documento
        _ = doc.Add(new Paragraph("Factura"));

        // Creamos una tabla que contendrá el nombre, apellido y país
        // de nuestros visitante.
        PdfPTable tblPrueba = new(3)
        {
            WidthPercentage = 100
        };

        // Configuramos el título de las columnas de la tabla
        PdfPCell clNombre = new(new Phrase("Nombre", _standardFont))
        {
            BorderWidth = 0,
            BorderWidthBottom = 0.75f
        };

        // Añadimos las celdas a la tabla
        _ = tblPrueba.AddCell(clNombre);

        // Finalmente, añadimos la tabla al documento PDF y cerramos el documento
        _ = doc.Add(tblPrueba);
        doc.Close();
        writer.Close();
    }
}
