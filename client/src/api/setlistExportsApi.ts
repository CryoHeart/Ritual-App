const BASE_URL = 'http://localhost:5000';

type ExportFormat = 'pdf' | 'docx';

async function downloadExport(
  bandId: string,
  setlistId: string,
  format: ExportFormat
): Promise<void> {
  const url = `${BASE_URL}/api/bands/${bandId}/setlists/${setlistId}/exports/${format}`;

  const response = await fetch(url);

  if (!response.ok) {
    if (response.status === 404) {
      throw new Error('This ritual no longer exists.');
    }
    if (response.status === 403) {
      throw new Error('You do not have permission to export this ritual.');
    }
    if (response.status === 401) {
      throw new Error('Session expired. Please log in again.');
    }
    throw new Error('Export failed. Please try again.');
  }

  const blob = await response.blob();
  const objectUrl = URL.createObjectURL(blob);

  const disposition = response.headers.get('Content-Disposition') ?? '';
  const filenameMatch = disposition.match(/filename\*?=(?:UTF-8'')?["']?([^"';\r\n]+)["']?/i);
  const filename = filenameMatch
    ? decodeURIComponent(filenameMatch[1].trim())
    : `ritual-export.${format}`;

  const anchor = document.createElement('a');
  anchor.href = objectUrl;
  anchor.download = filename;
  anchor.style.display = 'none';
  document.body.appendChild(anchor);
  anchor.click();
  document.body.removeChild(anchor);

  setTimeout(() => URL.revokeObjectURL(objectUrl), 10_000);
}

export function exportSetlistPdf(bandId: string, setlistId: string): Promise<void> {
  return downloadExport(bandId, setlistId, 'pdf');
}

export function exportSetlistDocx(bandId: string, setlistId: string): Promise<void> {
  return downloadExport(bandId, setlistId, 'docx');
}
