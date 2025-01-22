using System.Diagnostics;

public class TabulaJarService
{
    private readonly string _tabulaJarPath;

    public TabulaJarService()
    {
        _tabulaJarPath = Path.Combine(Directory.GetCurrentDirectory(), "AppData", "tabula-1.0.5-jar-with-dependencies.jar");
    }

    public string ExtractTables(string pdfPath)
    {
        string output = string.Empty;
        string arguments = $"-jar \"{_tabulaJarPath}\" --silent --lattice --pages all \"{pdfPath}\"";

        try
        {
            // Configure the process
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "java",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            // Start the process
            process.Start();

            // Capture the output
            output = process.StandardOutput.ReadToEnd();

            // Wait for the process to exit
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                string error = process.StandardError.ReadToEnd();
                throw new Exception($"Tabula JAR execution failed: {error}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error extracting tables: {ex.Message}");
        }

        return output;
    }
}
