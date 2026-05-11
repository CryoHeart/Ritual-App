using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using server.Models.Export;
using server.Services.Export.Interfaces;

namespace server.Services.Export.Implementations;

public class SetlistDocxExportService : ISetlistDocxExportService
{
    public byte[] Generate(SetlistExportData data)
    {
        using var ms = new MemoryStream();
        using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document();
            var body = mainPart.Document.AppendChild(new Body());

            AddStyles(mainPart);
            AddPageMargins(body);

            body.AppendChild(CreateHeading("RITUAL", 1));
            body.AppendChild(CreateHeading(data.SetlistName, 2));
            body.AppendChild(CreateMetaLine(data.BandName));

            if (!string.IsNullOrWhiteSpace(data.SetlistDescription))
            {
                body.AppendChild(CreateMetaLine(data.SetlistDescription!));
            }

            body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.TextWrapping })));

            if (data.Songs.Count == 0)
            {
                body.AppendChild(CreatePlainParagraph("This ritual has no songs yet."));
            }
            else
            {
                for (var i = 0; i < data.Songs.Count; i++)
                {
                    var song = data.Songs[i];
                    body.AppendChild(CreateHeading($"{i + 1}. {song.Title}", 3));

                    if (!string.IsNullOrWhiteSpace(song.TransitionNotes))
                    {
                        body.AppendChild(CreateNoteLine($"Transition: {song.TransitionNotes}"));
                    }

                    if (!string.IsNullOrWhiteSpace(song.PerformanceNotes))
                    {
                        body.AppendChild(CreateNoteLine($"Performance: {song.PerformanceNotes}"));
                    }

                    if (!string.IsNullOrWhiteSpace(song.Notes))
                    {
                        body.AppendChild(CreateNoteLine($"Notes: {song.Notes}"));
                    }
                }
            }

            mainPart.Document.Save();
        }

        return ms.ToArray();
    }

    private static Paragraph CreateHeading(string text, int level)
    {
        var para = new Paragraph();
        var props = new ParagraphProperties(new ParagraphStyleId { Val = $"Heading{level}" });
        para.AppendChild(props);
        para.AppendChild(new Run(new Text(text)));
        return para;
    }

    private static Paragraph CreateMetaLine(string value)
    {
        var para = new Paragraph();
        para.AppendChild(new ParagraphProperties(new ParagraphStyleId { Val = "Meta" }));
        para.AppendChild(new Run(new Text(value)));
        return para;
    }

    private static Paragraph CreateNoteLine(string value)
    {
        var para = new Paragraph();
        para.AppendChild(new ParagraphProperties(new ParagraphStyleId { Val = "SongNote" }));
        para.AppendChild(new Run(new Text(value) { Space = SpaceProcessingModeValues.Preserve }));
        return para;
    }

    private static Paragraph CreatePlainParagraph(string text)
    {
        var para = new Paragraph();
        para.AppendChild(new ParagraphProperties(new ParagraphStyleId { Val = "SongNote" }));
        para.AppendChild(new Run(new Text(text)));
        return para;
    }

    private static void AddStyles(MainDocumentPart mainPart)
    {
        var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
        stylesPart.Styles = new Styles(
            BuildStyle("Normal", "Normal", false, "000000", 26, "120", "80"),
            BuildStyle("Heading1", "Heading 1", true, "000000", 44, "0", "140"),
            BuildStyle("Heading2", "Heading 2", true, "000000", 72, "80", "180"),
            BuildStyle("Heading3", "Heading 3", true, "000000", 52, "120", "60"),
            BuildStyle("Meta", "Meta", false, "333333", 26, "20", "20"),
            BuildStyle("SongNote", "Song Note", false, "333333", 24, "20", "80")
        );
    }

    private static Style BuildStyle(string styleId, string name, bool bold, string color, int size, string before, string after)
    {
        var style = new Style
        {
            Type = StyleValues.Paragraph,
            StyleId = styleId,
            StyleName = new StyleName { Val = name }
        };

        var runProps = new StyleRunProperties();
        if (bold) runProps.AppendChild(new Bold());
        runProps.AppendChild(new Color { Val = color });
        runProps.AppendChild(new FontSize { Val = size.ToString() });
        style.AppendChild(runProps);

        style.AppendChild(new StyleParagraphProperties(
            new SpacingBetweenLines { Before = before, After = after }
        ));

        return style;
    }

    private static void AddPageMargins(Body body)
    {
        var sectProps = new SectionProperties(
            new PageMargin
            {
                Top = 1080,
                Bottom = 1080,
                Left = 1080,
                Right = 1080
            }
        );
        body.AppendChild(sectProps);
    }
}
