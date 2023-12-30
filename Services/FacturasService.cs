using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Reflection;
using Document = iTextSharp.text.Document;

namespace Services;

public class Factura
{
    public int NumeroFactura { get; set; }
    public int Importe { get; set; }
    public int IVA { get; set; }
    public int Total { get; set; }
    public bool PendientePago { get; set; }
    public required string DescripcionOperacion { get; set; }
    public DateTime FechaExpedicion { get; set; }
    public DateTime FechaCobro { get; set; }
    public required string ClienteId { get; set; }

    // Método para calcular el total
    public void CalcularTotal() => Total = Importe + (Importe * IVA);

    // Otros métodos y lógica según sea necesario
}

public class FacturasService
{
    public void FacturasWritter()
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

        // Añadimos una fila a la tabla
        tblPrueba.AddCell("Nombre del producto");
        tblPrueba.AddCell("Cantidad");
        tblPrueba.AddCell("Precio");

        // Finalmente, añadimos la tabla al documento PDF y cerramos el documento
        _ = doc.Add(tblPrueba);

        // Cerramos el documento
        doc.Close();
        writer.Close();

        // Abrimos el documento existente
        PdfReader reader = new(@"C:\Dev\.NET\MicroservicesScheme\FactivarAPI\Docs\plantilla.pdf");
        PdfStamper stamper = new(reader, new FileStream(@"C:\Dev\.NET\MicroservicesScheme\FactivarAPI\Docs\prueba.pdf", FileMode.Create));

        // Obtenemos el contenido del PDF
        PdfContentByte content = stamper.GetOverContent(1);

        // Añadimos la tabla al contenido del PDF
        _ = tblPrueba.WriteSelectedRows(0, -1, 10, 500, content);

        // Cerramos el PdfStamper
        stamper.Close();
    }

    public void FacturasStamper()
    {
        // Abrimos el documento existente
        PdfReader reader = new(@"C:\Dev\.NET\MicroservicesScheme\FactivarAPI\Docs\plantilla.pdf");
        PdfStamper stamper = new(reader, new FileStream(@"C:\Dev\.NET\MicroservicesScheme\FactivarAPI\Docs\prueba.pdf", FileMode.Create));

        // Obtenemos los campos del formulario
        AcroFields fields = stamper.AcroFields;

        // Establecemos los valores de los campos
        _ = fields.SetField("Invoice", "Factura");
        _ = fields.SetField("Invoice Date", "29.12.2023"); // Cambia esto por la fecha que desees
        _ = fields.SetField("Description", "Descripción"); // Cambia esto por la descripción que desees
        _ = fields.SetField("Amount", "Cantidad"); // Cambia esto por la cantidad que desees

        // Aseguramos que los campos no puedan ser editados
        stamper.FormFlattening = true;

        // Cerramos el PdfStamper
        stamper.Close();
    }

    // Método para agregar campos de formulario a la plantilla
    public void AgregarCampos(string plantilla, string salida, Factura factura)
    {
        PdfReader? pdfReader = null;
        PdfStamper? pdfStamper = null;
        System.Reflection.PropertyInfo[] facturaProps = factura.GetType().GetProperties();

        try
        {
            // Cargar la plantilla
            pdfReader = new PdfReader(plantilla);

            // Crear el PDF de salida
            pdfStamper = new PdfStamper(pdfReader, new FileStream(salida, FileMode.Create));

            foreach (PropertyInfo prop in facturaProps)
            {
                // Crear un nuevo campo de texto
                PdfFormField campo = PdfFormField.CreateTextField(pdfStamper.Writer, false, false, 0);
                campo.SetWidget(new Rectangle(0, 0, 0, 0), PdfAnnotation.HIGHLIGHT_NONE);
                campo.FieldName = nameof(prop);

                // Agregar el campo al documento
                pdfStamper.AddAnnotation(campo, 1);
            }
        }
        catch (Exception ex)
        {
            // Manejar la excepción
            Console.WriteLine("Error al agregar campos: " + ex.Message);
            throw;
        }
        finally
        {
            // Cerrar el PDF
            pdfStamper?.Close();
            pdfReader?.Close();
        }
    }
}

//public class GeneradorPDF
//{
//    public void CrearPDF(Factura factura)
//    {
//        using (PdfWriter writer = new("factura.pdf"))
//        {
//            using (PdfDocument pdf = new(writer))
//            {
//                Document document = new(pdf);
//                _ = document.Add(new Paragraph($"Número de Factura: {factura.NumeroFactura}"));
//                _ = document.Add(new Paragraph($"Importe: {factura.Importe}"));
//                _ = document.Add(new Paragraph($"IVA: {factura.IVA}%"));
//                _ = document.Add(new Paragraph($"Total: {factura.Total}"));
//                _ = document.Add(new Paragraph($"Descripción Operación: {factura.DescripcionOperacion}"));
//                // Agrega más detalles según sea necesario.
//            }
//        }
//    }
//}