using System.IO;

namespace AxelotTask
{
    public class Reporter
    {
        public void ReportSuccess(byte[] data, int id)
        {
            File.WriteAllBytes($"Report_[{id}].txt", data);
        }

        public void ReportError(int id)
        {
            File.WriteAllText($"Error_[{id}].txt", "Report error");
        }

        public void ReportTimeout(int id)
        {
            File.WriteAllText($"Timeout_[{id}].txt", "Report error");
        }
    }
}
