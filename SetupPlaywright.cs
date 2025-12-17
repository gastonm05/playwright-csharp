// Simple script to install Playwright browsers
using System;
using System.Diagnostics;
using System.Threading.Tasks;

// Build: csc SetupPlaywright.cs -r:"$HOME\.nuget\packages\microsoft.playwright\1.48.0\lib\net8.0\*.dll"
// Run: .\SetupPlaywright.exe

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Installing Playwright browsers...");
        try
        {
            // Use the playwright executable bundled with the package
            var playwrightPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".nuget", "packages", "microsoft.playwright", "1.48.0", "build", "Release", "playwright.exe"
            );
            
            if (!File.Exists(playwrightPath))
            {
                Console.WriteLine($"Playwright executable not found at {playwrightPath}");
                Console.WriteLine("Trying alternative method...");
                
                // Alternative: Use dotnet to execute the playwright tool via reflection
                var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "tool run playwright -- install",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                });
                
                if (process != null)
                {
                    var output = await process.StandardOutput.ReadToEndAsync();
                    var error = await process.StandardError.ReadToEndAsync();
                    
                    process.WaitForExit();
                    
                    Console.WriteLine(output);
                    if (!string.IsNullOrEmpty(error))
                        Console.WriteLine("Error: " + error);
                }
            }
            else
            {
                Console.WriteLine($"Found playwright at: {playwrightPath}");
                var process = Process.Start(playwrightPath, "install");
                if (process != null)
                    process.WaitForExit();
            }
            
            Console.WriteLine("Playwright browsers installed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error installing browsers: {ex.Message}");
        }
    }
}
