using server.Dao.Interfaces;
using server.Logic.Exceptions;
using server.Logic.Interfaces;
using server.Models.Export;
using server.Services.Export.Interfaces;

namespace server.Logic.Implementations;

public class SetlistExportLogic : ISetlistExportLogic
{
    private readonly ISetlistExportDao _exportDao;
    private readonly ISetlistPdfExportService _pdfService;
    private readonly ISetlistDocxExportService _docxService;

    public SetlistExportLogic(
        ISetlistExportDao exportDao,
        ISetlistPdfExportService pdfService,
        ISetlistDocxExportService docxService)
    {
        _exportDao = exportDao;
        _pdfService = pdfService;
        _docxService = docxService;
    }

    public async Task<ExportFileResponse> ExportSetlistPdfAsync(string bandId, string setlistId)
    {
        var data = await LoadAndValidateAsync(bandId, setlistId);
        var content = _pdfService.Generate(data);

        return new ExportFileResponse
        {
            Content = content,
            ContentType = "application/pdf",
            FileName = BuildFileName(data.BandName, data.SetlistName, "pdf")
        };
    }

    public async Task<ExportFileResponse> ExportSetlistDocxAsync(string bandId, string setlistId)
    {
        var data = await LoadAndValidateAsync(bandId, setlistId);
        var content = _docxService.Generate(data);

        return new ExportFileResponse
        {
            Content = content,
            ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            FileName = BuildFileName(data.BandName, data.SetlistName, "docx")
        };
    }

    private async Task<SetlistExportData> LoadAndValidateAsync(string bandId, string setlistId)
    {
        if (string.IsNullOrWhiteSpace(bandId))
            throw new ValidationException("Band ID is required.");

        if (string.IsNullOrWhiteSpace(setlistId))
            throw new ValidationException("Setlist ID is required.");

        var data = await _exportDao.GetSetlistExportDataAsync(bandId, setlistId);

        if (data is null)
            throw new NotFoundException("Setlist was not found or does not belong to this band.");

        return data;
    }

    private static string BuildFileName(string bandName, string setlistName, string extension)
    {
        var safe = $"ritual-{Sanitize(bandName)}-{Sanitize(setlistName)}.{extension}";
        return safe;
    }

    private static string Sanitize(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "unknown";

        var lower = input.ToLowerInvariant();

        // Replace spaces and underscores with hyphens
        var result = System.Text.RegularExpressions.Regex.Replace(lower, @"[\s_]+", "-");

        // Remove chars that are not alphanumeric or hyphens
        result = System.Text.RegularExpressions.Regex.Replace(result, @"[^a-z0-9\-]", "");

        // Collapse repeated hyphens and trim
        result = System.Text.RegularExpressions.Regex.Replace(result, @"-{2,}", "-");
        result = result.Trim('-');

        return string.IsNullOrEmpty(result) ? "unknown" : result;
    }
}
