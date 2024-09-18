using FileScopedNamespace.Helpers;
using System.IO;
using System.Linq;

namespace FileScopedNamespace
{
    [Command(PackageIds.FileScopedNamespaceCommand)]
    internal sealed class FileScopedNamespaceCommand : BaseCommand<FileScopedNamespaceCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
            {
                var activeProject = await VS.Solutions.GetActiveProjectAsync();

                if (activeProject != null)
                {
                    var projectDirectory = Directory.GetParent(activeProject.FullPath).FullName;

                    var csFiles = Directory.GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories);

                    foreach (var filePath in csFiles)
                    {
                        try
                        {
                            var lines = File.ReadAllLines(filePath).ToList();

                            var formattedResult = SamFormater.Format(lines);

                            File.WriteAllText(filePath, formattedResult.Trim());

                            Console.WriteLine($"File '{filePath}' has been formatted successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error formatting file '{filePath}': {ex.Message}");
                        }
                    }

                    await VS.MessageBox.ShowAsync("All C# files in the project have been formatted successfully.");
                }
                else
                {
                    await VS.MessageBox.ShowErrorAsync("No active project found.");
                }
            }
            catch (Exception ex)
            {
                await VS.MessageBox.ShowErrorAsync($"An error occurred: {ex.Message}");
            }
        }

    }
}
