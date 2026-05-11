using server.Models.Export;

namespace server.Services.Export.Interfaces;

public interface ISetlistDocxExportService
{
    byte[] Generate(SetlistExportData data);
}
