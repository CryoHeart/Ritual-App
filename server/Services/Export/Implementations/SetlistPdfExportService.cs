using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using server.Models.Export;
using server.Services.Export.Interfaces;

namespace server.Services.Export.Implementations;

public class SetlistPdfExportService : ISetlistPdfExportService
{
    public byte[] Generate(SetlistExportData data)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(0);
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(10).FontColor("#e4e4e7"));

                page.Header().Element(ComposeHeader);

                page.Content().PaddingHorizontal(36).PaddingBottom(20).Element(c =>
                    ComposeBody(c, data));

                page.Footer()
                    .PaddingHorizontal(36)
                    .PaddingBottom(16)
                    .Row(row =>
                    {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("RITUAL").FontSize(8).FontColor("#71717a").Bold();
                            text.Span($"  ·  Generated {data.ExportedAt:yyyy-MM-dd HH:mm} UTC").FontSize(8).FontColor("#52525b");
                        });
                        row.ConstantItem(60).AlignRight().Text(text =>
                        {
                            text.CurrentPageNumber().FontSize(8).FontColor("#52525b");
                            text.Span(" / ").FontSize(8).FontColor("#52525b");
                            text.TotalPages().FontSize(8).FontColor("#52525b");
                        });
                    });
            });
        });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container)
    {
        container
            .Background("#18181b")
            .Padding(36)
            .Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(inner =>
                    {
                        inner.Item()
                            .Text("RITUAL")
                            .FontSize(28)
                            .Bold()
                            .FontColor("#ffffff")
                            .LetterSpacing(0.08f);

                        inner.Item()
                            .PaddingTop(2)
                            .Text("Setlist Export")
                            .FontSize(10)
                            .FontColor("#a1a1aa")
                            .LetterSpacing(0.12f);
                    });
                });

                col.Item()
                    .PaddingTop(18)
                    .BorderBottom(2)
                    .BorderColor("#dc2626")
                    .ExtendHorizontal();
            });
    }

    private static void ComposeBody(IContainer container, SetlistExportData data)
    {
        container.PaddingTop(24).Column(col =>
        {
            // Setlist metadata block
            col.Item().Background("#27272a").Padding(16).Column(meta =>
            {
                meta.Item().Row(row =>
                {
                    row.RelativeItem().Column(left =>
                    {
                        left.Item().Text(data.SetlistName)
                            .FontSize(18).Bold().FontColor("#f4f4f5");

                        left.Item().PaddingTop(4).Text(data.BandName)
                            .FontSize(11).FontColor("#a1a1aa");

                        if (!string.IsNullOrWhiteSpace(data.SetlistDescription))
                        {
                            left.Item().PaddingTop(6).Text(data.SetlistDescription)
                                .FontSize(9).FontColor("#71717a").Italic();
                        }
                    });

                    row.ConstantItem(140).Column(right =>
                    {
                        right.Item().AlignRight().Text($"Songs: {data.TotalSongs}")
                            .FontSize(9).FontColor("#a1a1aa");

                        right.Item().AlignRight().PaddingTop(3)
                            .Text($"Duration: {FormatDuration(data.TotalDurationSeconds)}")
                            .FontSize(9).FontColor("#a1a1aa");

                        right.Item().AlignRight().PaddingTop(3)
                            .Text($"Exported: {data.ExportedAt:dd MMM yyyy}")
                            .FontSize(9).FontColor("#52525b");
                    });
                });
            });

            col.Item().PaddingTop(24);

            if (data.Songs.Count == 0)
            {
                col.Item()
                    .PaddingTop(20)
                    .AlignCenter()
                    .Text("This ritual has no songs yet.")
                    .FontSize(11)
                    .FontColor("#71717a")
                    .Italic();
                return;
            }

            // Section label
            col.Item().PaddingBottom(8).Text("Track Listing")
                .FontSize(8)
                .Bold()
                .FontColor("#dc2626")
                .LetterSpacing(0.18f);

            // Song table
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(28);   // #
                    cols.RelativeColumn(3);     // Song
                    cols.RelativeColumn(2);     // Album
                    cols.ConstantColumn(52);    // Duration
                    cols.ConstantColumn(40);    // BPM
                    cols.ConstantColumn(52);    // Tuning
                    cols.ConstantColumn(40);    // Key
                });

                // Header row
                static IContainer HeaderCell(IContainer c) =>
                    c.Background("#27272a")
                     .PaddingVertical(7)
                     .PaddingHorizontal(6);

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("#")
                        .FontSize(8).Bold().FontColor("#a1a1aa").LetterSpacing(0.1f);
                    header.Cell().Element(HeaderCell).Text("SONG")
                        .FontSize(8).Bold().FontColor("#a1a1aa").LetterSpacing(0.1f);
                    header.Cell().Element(HeaderCell).Text("ALBUM")
                        .FontSize(8).Bold().FontColor("#a1a1aa").LetterSpacing(0.1f);
                    header.Cell().Element(HeaderCell).Text("DURATION")
                        .FontSize(8).Bold().FontColor("#a1a1aa").LetterSpacing(0.1f);
                    header.Cell().Element(HeaderCell).Text("BPM")
                        .FontSize(8).Bold().FontColor("#a1a1aa").LetterSpacing(0.1f);
                    header.Cell().Element(HeaderCell).Text("TUNING")
                        .FontSize(8).Bold().FontColor("#a1a1aa").LetterSpacing(0.1f);
                    header.Cell().Element(HeaderCell).Text("KEY")
                        .FontSize(8).Bold().FontColor("#a1a1aa").LetterSpacing(0.1f);
                });

                for (var i = 0; i < data.Songs.Count; i++)
                {
                    var song = data.Songs[i];
                    var rowBg = i % 2 == 0 ? "#18181b" : "#1c1c1f";
                    var isLast = i == data.Songs.Count - 1;
                    var hasNotes = !string.IsNullOrWhiteSpace(song.Notes)
                        || !string.IsNullOrWhiteSpace(song.TransitionNotes)
                        || !string.IsNullOrWhiteSpace(song.PerformanceNotes);

                    IContainer DataCell(IContainer c) =>
                        c.Background(rowBg)
                         .PaddingVertical(hasNotes ? 6 : 9)
                         .PaddingHorizontal(6);

                    table.Cell().Element(DataCell)
                        .Text($"{i + 1}")
                        .FontSize(9).FontColor("#71717a");

                    // Song cell — title + optional notes block
                    table.Cell().Element(c =>
                        c.Background(rowBg).PaddingVertical(hasNotes ? 6 : 9).PaddingHorizontal(6)
                    ).Column(songCol =>
                    {
                        songCol.Item().Text(song.Title).FontSize(10).Bold().FontColor("#f4f4f5");

                        if (hasNotes)
                        {
                            songCol.Item().PaddingTop(5).Column(notes =>
                            {
                                if (!string.IsNullOrWhiteSpace(song.TransitionNotes))
                                {
                                    notes.Item().Row(r =>
                                    {
                                        r.ConstantItem(70).Text("Transition:").FontSize(7.5f).FontColor("#dc2626").Bold();
                                        r.RelativeItem().Text(song.TransitionNotes!).FontSize(7.5f).FontColor("#a1a1aa");
                                    });
                                }
                                if (!string.IsNullOrWhiteSpace(song.PerformanceNotes))
                                {
                                    notes.Item().PaddingTop(2).Row(r =>
                                    {
                                        r.ConstantItem(70).Text("Performance:").FontSize(7.5f).FontColor("#dc2626").Bold();
                                        r.RelativeItem().Text(song.PerformanceNotes!).FontSize(7.5f).FontColor("#a1a1aa");
                                    });
                                }
                                if (!string.IsNullOrWhiteSpace(song.Notes))
                                {
                                    notes.Item().PaddingTop(2).Row(r =>
                                    {
                                        r.ConstantItem(70).Text("Notes:").FontSize(7.5f).FontColor("#71717a").Bold();
                                        r.RelativeItem().Text(song.Notes!).FontSize(7.5f).FontColor("#71717a");
                                    });
                                }
                            });
                        }
                    });

                    table.Cell().Element(DataCell)
                        .Text(song.AlbumTitle ?? "—")
                        .FontSize(9).FontColor("#a1a1aa");

                    table.Cell().Element(DataCell)
                        .Text(FormatDuration(song.DurationSeconds))
                        .FontSize(9).FontColor("#a1a1aa");

                    table.Cell().Element(DataCell)
                        .Text(song.Bpm.HasValue ? song.Bpm.Value.ToString() : "—")
                        .FontSize(9).FontColor("#a1a1aa");

                    table.Cell().Element(DataCell)
                        .Text(song.Tuning ?? "—")
                        .FontSize(9).FontColor("#a1a1aa");

                    table.Cell().Element(DataCell)
                        .Text(song.SongKey ?? "—")
                        .FontSize(9).FontColor("#a1a1aa");
                }
            });

            // Summary footer row
            col.Item()
                .Background("#27272a")
                .Padding(10)
                .Row(row =>
                {
                    row.RelativeItem().Text($"{data.TotalSongs} songs total")
                        .FontSize(8).FontColor("#71717a");
                    row.ConstantItem(120).AlignRight()
                        .Text($"Total: {FormatDuration(data.TotalDurationSeconds)}")
                        .FontSize(8).FontColor("#71717a");
                });
        });
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
