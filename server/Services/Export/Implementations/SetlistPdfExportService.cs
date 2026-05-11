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
                page.Margin(36);
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(12).FontColor(Colors.Black));

                page.Header().Element(ComposeHeader);

                page.Content().PaddingTop(16).Element(c =>
                    ComposeBody(c, data));
            });
        });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container)
    {
        container
            .AlignLeft()
            .Text("RITUAL")
            .FontSize(16)
            .Bold()
            .LetterSpacing(0.2f)
            .FontColor(Colors.Black);
    }

    private static void ComposeBody(IContainer container, SetlistExportData data)
    {
        container.Column(col =>
        {
            col.Item()
                .Text(data.SetlistName)
                .FontSize(40)
                .Bold()
                .FontColor(Colors.Black);

            col.Item().PaddingTop(6)
                .Text(data.BandName)
                .FontSize(16)
                .FontColor("#333333");

            if (!string.IsNullOrWhiteSpace(data.SetlistDescription))
            {
                col.Item().PaddingTop(10)
                    .Text(data.SetlistDescription)
                    .FontSize(14)
                    .Italic()
                    .FontColor("#444444");
            }

            col.Item().PaddingTop(22);

            if (data.Songs.Count == 0)
            {
                col.Item()
                    .PaddingTop(20)
                    .Text("This ritual has no songs yet.")
                    .FontSize(22)
                    .FontColor("#444444")
                    .Italic();
                return;
            }

            col.Item().Column(list =>
            {
                for (var i = 0; i < data.Songs.Count; i++)
                {
                    var song = data.Songs[i];
                    var hasNotes = !string.IsNullOrWhiteSpace(song.Notes)
                        || !string.IsNullOrWhiteSpace(song.TransitionNotes)
                        || !string.IsNullOrWhiteSpace(song.PerformanceNotes);

                    list.Item().PaddingBottom(12).Column(songCol =>
                    {
                        songCol.Item().Text($"{i + 1}. {song.Title}")
                            .FontSize(30)
                            .Bold()
                            .FontColor(Colors.Black);

                        if (hasNotes)
                        {
                            songCol.Item().PaddingTop(3).Column(notes =>
                            {
                                if (!string.IsNullOrWhiteSpace(song.TransitionNotes))
                                {
                                    notes.Item().Text($"Transition: {song.TransitionNotes}")
                                        .FontSize(13)
                                        .FontColor("#333333");
                                }
                                if (!string.IsNullOrWhiteSpace(song.PerformanceNotes))
                                {
                                    notes.Item().PaddingTop(1).Text($"Performance: {song.PerformanceNotes}")
                                        .FontSize(13)
                                        .FontColor("#333333");
                                }
                                if (!string.IsNullOrWhiteSpace(song.Notes))
                                {
                                    notes.Item().PaddingTop(1).Text($"Notes: {song.Notes}")
                                        .FontSize(13)
                                        .FontColor("#333333");
                                }
                            });
                        }
                    });
                }
            });
        });
    }
}
