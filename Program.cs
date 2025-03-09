using System.Diagnostics;
using System.Security.Principal;
using Microsoft.Win32;

const string PROTOCOL_NAME = "open";
const bool DEBUG_MODE = true; // Set to false for production

if (args.Length > 0)
{
    HandleProtocol(args[0]);
}
else if (IsAdministrator())
{
    RegisterProtocolHandler();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Protocol handler registered successfully!");
    Console.ResetColor();
    PauseIfDebug();
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Please run as administrator to register the protocol handler.");
    Console.ResetColor();
    PauseIfDebug();
    return 1;
}

return 0;

void HandleProtocol(string url)
{
    try
    {
        Console.WriteLine($"Received URL: {url}");

        // Skip the protocol part manually instead of using Uri class
        string path = url;

        // Remove the protocol prefix (open://)
        if (path.StartsWith($"{PROTOCOL_NAME}://", StringComparison.OrdinalIgnoreCase))
        {
            path = path.Substring($"{PROTOCOL_NAME}://".Length);
        }

        // URL decode the path
        path = Uri.UnescapeDataString(path);

        Console.WriteLine($"Decoded path: {path}");

        // Remove any trailing slashes
        path = path.TrimEnd('/', '\\');

        // Fix potential C:/ vs C|/ issues
        if (path.Length >= 2 && char.IsLetter(path[0]) && (path[1] == '|' || path[1] == ':'))
        {
            path = path[0] + ":" + path.Substring(2);
        }

        // Also check for the case where the colon was encoded as %3A
        if (path.Length >= 3 && char.IsLetter(path[0]) && path.Substring(1, 3) == "%3A")
        {
            path = path[0] + ":" + path.Substring(4);
        }

        // Convert forward slashes to backslashes for Windows
        path = path.Replace('/', '\\');

        Console.WriteLine($"Final path: {path}");

        // Determine if it's a file or folder and act accordingly
        if (File.Exists(path))
        {
            Console.WriteLine($"Detected as file, opening: {path}");
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        else if (Directory.Exists(path))
        {
            Console.WriteLine($"Detected as folder, opening: {path}");
            var startInfo = new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"\"{path}\"",
                UseShellExecute = false
            };

            Process.Start(startInfo);
        }
        else
        {
            var error = $"Path not found: {path}";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ResetColor();
            PauseIfDebug();
        }
    }
    catch (Exception ex)
    {
        var error = $"Error handling protocol: {ex.Message}";
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(error);
        Console.ResetColor();
        PauseIfDebug();
    }
}

void RegisterProtocolHandler()
{
    var exePath = $"\"{Environment.ProcessPath}\" \"%1\"";

    try
    {
        using var key = Registry.ClassesRoot.CreateSubKey(PROTOCOL_NAME);
        key.SetValue("", $"URL:{PROTOCOL_NAME} Protocol");
        key.SetValue("URL Protocol", "");

        using (var defaultIcon = key.CreateSubKey("DefaultIcon"))
        {
            defaultIcon.SetValue("", exePath);
        }

        using (var commandKey = key.CreateSubKey(@"shell\open\command"))
        {
            commandKey.SetValue("", exePath);
        }

        var success = $"Registered protocol handler: {PROTOCOL_NAME}://";
        Console.WriteLine(success);
        Console.WriteLine($"Handler path: {exePath}");
    }
    catch (Exception ex)
    {
        var error = $"Failed to register protocol handler: {ex.Message}";
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(error);
        Console.ResetColor();
        PauseIfDebug();
    }
}

bool IsAdministrator()
{
    var identity = WindowsIdentity.GetCurrent();
    var principal = new WindowsPrincipal(identity);
    return principal.IsInRole(WindowsBuiltInRole.Administrator);
}

void PauseIfDebug()
{
    if (DEBUG_MODE)
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}