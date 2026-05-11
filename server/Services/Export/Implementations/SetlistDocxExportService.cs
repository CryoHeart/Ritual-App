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

            // Title heading
            body.AppendChild(CreateHeading("RITUAL — Setlist Export", 1));

            // Metadata block
            body.AppendChild(CreateMetaParagraph("Band", data.BandName));
            body.AppendChild(CreateMetaParagraph("Setlist", data.SetlistName));
            if (!string.IsNullOrWhiteSpace(data.SetlistDescription))
                body.AppendChild(CreateMetaParagraph("Description", data.SetlistDescription!));
            body.AppendChild(CreateMetaParagraph("Total Songs", data.TotalSongs.ToString()));
            body.AppendChild(CreateMetaParagraph("Total Duration", FormatDuration(data.TotalDurationSeconds)));
            body.AppendChild(CreateMetaParagraph("Exported", data.ExportedAt.ToString("dd MMM yyyy HH:mm UTC")));

            body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.TextWrapping })));

            if (data.Songs.Count == 0)
            {
                body.AppendChild(CreatePlainParagraph("This ritual has no songs yet."));
            }
            else
            {
                // Track table
                body.AppendChild(CreateHeading("Track Listing", 2));
                body.AppendChild(BuildSongTable(data.Songs));

                body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.TextWrapping })));

                // Performance Notes section — only if any song has notes
                var songsWithNotes = data.Songs.Where(s =>
                    !string.IsNullOrWhiteSpace(s.Notes)
                    || !string.IsNullOrWhiteSpace(s.TransitionNotes)
                    || !string.IsNullOrWhiteSpace(s.PerformanceNotes)).ToList();

                if (songsWithNotes.Count > 0)
                {
                    body.AppendChild(CreateHeading("Performance Notes", 2));

                    foreach (var song in songsWithNotes)
                    {
                        body.AppendChild(CreateHeading($"{song.PositionIndex + 1}. {song.Title}", 3));

                        if (!string.IsNullOrWhiteSpace(song.TransitionNotes))
                            body.AppendChild(CreateBulletParagraph("Transition", song.TransitionNotes!));
                        if (!string.IsNullOrWhiteSpace(song.PerformanceNotes))
                            body.AppendChild(CreateBulletParagraph("Performance", song.PerformanceNotes!));
                        if (!string.IsNullOrWhiteSpace(song.Notes))
                            body.AppendChild(CreateBulletParagraph("Notes", song.Notes!));
                    }
                }
            }

            mainPart.Document.Save();
        }

        return ms.ToArray();
    }

    private static Table BuildSongTable(List<SetlistExportSong> songs)
    {
        var table = new Table();

        // Table properties
        var tableProps = new TableProperties(
            new TableBorders(
                new TopBorder { Val = BorderValues.Single, Size = 4, Color = "CCCCCC" },
                new BottomBorder { Val = BorderValues.Single, Size = 4, Color = "CCCCCC" },
                new LeftBorder { Val = BorderValues.Single, Size = 4, Color = "CCCCCC" },
                new RightBorder { Val = BorderValues.Single, Size = 4, Color = "CCCCCC" },
                new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Color = "CCCCCC" },
                new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Color = "CCCCCC" }
            ),
            new TableWidth { Width = "9360", Type = TableWidthUnitValues.Dxa }
        );
        table.AppendChild(tableProps);

        // Header row
        var headers = new[] { "#", "Song", "Album", "Duration", "BPM", "Tuning", "Key" };
        var headerRow = new TableRow();
        foreach (var h in headers)
        {
            headerRow.AppendChild(CreateTableCell(h, bold: true, shading: "1C1C1F", textColor: "A1A1AA", isHeader: true));
        }
        table.AppendChild(headerRow);

        // Data rows
        for (var i = 0; i < songs.Count; i++)
        {
            var song = songs[i];
            var shade = i % 2 == 0 ? "18181B" : "1C1C1F";
            var dataRow = new TableRow();

            dataRow.AppendChild(CreateTableCell($"{i + 1}", shading: shade, textColor: "71717A"));
            dataRow.AppendChild(CreateTableCell(song.Title, bold: true, shading: shade, textColor: "F4F4F5"));
            dataRow.AppendChild(CreateTableCell(song.AlbumTitle ?? "—", shading: shade, textColor: "A1A1AA"));
            dataRow.AppendChild(CreateTableCell(FormatDuration(song.DurationSeconds), shading: shade, textColor: "A1A1AA"));
            dataRow.AppendChild(CreateTableCell(song.Bpm.HasValue ? song.Bpm.Value.ToString() : "—", shading: shade, textColor: "A1A1AA"));
            dataRow.AppendChild(CreateTableCell(song.Tuning ?? "—", shading: shade, textColor: "A1A1AA"));
            dataRow.AppendChild(CreateTableCell(song.SongKey ?? "—", shading: shade, textColor: "A1A1AA"));

            table.AppendChild(dataRow);
        }

        return table;
    }

    private static TableCell CreateTableCell(
        string text,
        bool bold = false,
        string shading = "FFFFFF",
        string textColor = "000000",
        bool isHeader = false)
    {
        var cell = new TableCell();

        cell.AppendChild(new TableCellProperties(
            new Shading
            {
                Val = ShadingPatternValues.Clear,
                Fill = shading,
                Color = "auto"
            },
            new TableCellMargin(
                new TopMargin { Width = "80", Type = TableWidthUnitValues.Dxa },
                new BottomMargin { Width = "80", Type = TableWidthUnitValues.Dxa },
                new LeftMargin { Width = "108", Type = TableWidthUnitValues.Dxa },
                new RightMargin { Width = "108", Type = TableWidthUnitValues.Dxa }
            )
        ));

        var run = new Run(new Text(text));
        var runProps = new RunProperties();
        if (bold) runProps.AppendChild(new Bold());
        runProps.AppendChild(new Color { Val = textColor });
        runProps.AppendChild(new FontSize { Val = isHeader ? "18" : "20" });
        run.PrependChild(runProps);

        cell.AppendChild(new Paragraph(run));
        return cell;
    }

    private static Paragraph CreateHeading(string text, int level)
    {
        var para = new Paragraph();
        var props = new ParagraphProperties(new ParagraphStyleId { Val = $"Heading{level}" });
        para.AppendChild(props);
        para.AppendChild(new Run(new Text(text)));
        return para;
    }

    private static Paragraph CreateMetaParagraph(string label, string value)
    {
        var para = new Paragraph();
        var run1 = new Run(new Text($"{label}: "));
        run1.PrependChild(new RunProperties(new Bold()));
        var run2 = new Run(new Text(value));
        para.AppendChild(run1);
        para.AppendChild(run2);
        return para;
    }

    private static Paragraph CreateBulletParagraph(string label, string value)
    {
        var para = new Paragraph();
        var props = new ParagraphProperties(
            new ParagraphStyleId { Val = "ListParagraph" },
            new NumberingProperties(
                new NumberingLevelReference { Val = 0 },
                new NumberingId { Val = 1 }
            )
        );
        para.AppendChild(props);

        var run1 = new Run(new Text($"{label}: "));
        run1.PrependChild(new RunProperties(new Bold()));
        var run2 = new Run(new Text(value) { Space = SpaceProcessingModeValues.Preserve });
        para.AppendChild(run1);
        para.AppendChild(run2);
        return para;
    }

    private static Paragraph CreatePlainParagraph(string text)
    {
        return new Paragraph(new Run(new Text(text)));
    }

    private static void AddStyles(MainDocumentPart mainPart)
    {
        var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
        stylesPart.Styles = new Styles(
            BuildStyle("Normal", "Normal", false, "202020", 20),
            BuildStyle("Heading1", "Heading 1", true, "DC2626", 28),
            BuildStyle("Heading2", "Heading 2", true, "F4F4F5", 24),
            BuildStyle("Heading3", "Heading 3", true, "A1A1AA", 22),
            BuildStyle("ListParagraph", "List Paragraph", false, "E4E4E7", 20)
        );
    }

    private static Style BuildStyle(string styleId, string name, bool bold, string color, int size)
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

        if (styleId.StartsWith("Heading"))
        {
            style.AppendChild(new StyleParagraphProperties(
                new SpacingBetweenLines { Before = "240", After = "80" }
            ));
        }

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

    private static string FormatDuration(int? seconds)
    {
        if (!seconds.HasValue || seconds.Value <= 0) return "—";
        var h = seconds.Value / 3600;
        var m = (seconds.Value % 3600) / 60;
        var s = seconds.Value % 60;
        if (h > 0) return $"{h}:{m:D2}:{s:D2}";
        return $"{m}:{s:D2}";
    }
}
