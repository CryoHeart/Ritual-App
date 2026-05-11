using server.Models.Export;

namespace server.Logic.Interfaces;

public interface ISetlistExportLogic
{
    Task<ExportFileResponse> ExportSetlistPdfAsync(string bandId, string setlistId);

    Task<ExportFileResponse> ExportSetlistDocxAsync(string bandId, string setlistId);
}
