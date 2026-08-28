using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace RightClickTools
{
    class Setup
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // Get the directory where Setup.exe is located
                string setupDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                // Look for RightClickTools.exe in the same directory
                string rightClickToolsPath = Path.Combine(setupDirectory, "RightClickTools.exe");

                if (!File.Exists(rightClickToolsPath))
                {
                    MessageBox.Show(
                        "Error: RightClickTools.exe not found in the same directory as Setup.exe\n\n" +
                        "Expected path: " + rightClickToolsPath,
                        "Setup Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Launch RightClickTools.exe with /Setup argument
                Process p = new Process();
                p.StartInfo.FileName = rightClickToolsPath;
                p.StartInfo.Arguments = "/Setup";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = false;
                p.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error launching RightClickTools.exe:\n\n" + ex.Message,
                    "Setup Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
