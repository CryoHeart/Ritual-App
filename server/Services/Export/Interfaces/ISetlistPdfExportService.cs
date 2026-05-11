using server.Models.Export;

namespace server.Services.Export.Interfaces;

public interface ISetlistPdfExportService
{
    byte[] Generate(SetlistExportData data);
}
