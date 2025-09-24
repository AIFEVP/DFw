// Program.cs
using System.CommandLine;
using System.IO;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // Create an option for the file path.
        var fileOption = new Option<FileInfo?>(
             "--file",
             "The path to the file to write.");

        // Create an option for the content to be written.
        var contentOption = new Option<string?>(
             "--content",
             "The content to write to the file.");

        // Create a root command.
        var rootCommand = new RootCommand("A sample command-line program that writes content to a file.");

        // Add the options to the root command.
        rootCommand.Add(fileOption);
        rootCommand.Add(contentOption);

        // Set the handler for the root command.
        rootCommand.SetHandler((file, content) =>
        {
            if (file != null && content != null)
            {
                File.WriteAllText(file.FullName, content);
                Console.WriteLine($"Successfully wrote to {file.FullName}");
            }
            else
            {
                Console.WriteLine("Both --file and --content options are required.");
            }
        }, fileOption, contentOption); // Pass the options to the handler.

        // Parse the incoming arguments and invoke the handler.
        return await rootCommand.InvokeAsync(args);
    }
}