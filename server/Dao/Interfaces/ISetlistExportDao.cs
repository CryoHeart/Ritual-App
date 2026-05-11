using server.Models.Export;

namespace server.Dao.Interfaces;

public interface ISetlistExportDao
{
    Task<SetlistExportData?> GetSetlistExportDataAsync(string bandId, string setlistId);
}
