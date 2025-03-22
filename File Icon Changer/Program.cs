using Microsoft.Win32;
using System;

public class FileIconChanger
{
    public static void ChangeFileIcon(string fileExtension, string iconPath)
    {
        try
        {
            // Check if the file extension registry key exists
            using (RegistryKey extensionKey = Registry.ClassesRoot.OpenSubKey(fileExtension, true) ?? Registry.ClassesRoot.CreateSubKey(fileExtension))
            {
                if (extensionKey == null)
                {
                    Console.WriteLine($"Error: Could not create or open registry key for {fileExtension}");
                    return;
                }

                // Retrieve or set the ProgID for the file extension
                string progId = extensionKey.GetValue(string.Empty)?.ToString();
                if (string.IsNullOrEmpty(progId))
                {
                    progId = $"{fileExtension.Trim('.')}_file"; // Default ProgID if none exists
                    extensionKey.SetValue(string.Empty, progId);
                }

                // Open or create the ProgID registry key
                using (RegistryKey progIdKey = Registry.ClassesRoot.CreateSubKey(progId))
                {
                    if (progIdKey == null)
                    {
                        Console.WriteLine($"Error: Could not create or open ProgID registry key for {progId}");
                        return;
                    }

                    // Set the DefaultIcon subkey for the ProgID
                    using (RegistryKey defaultIconKey = progIdKey.CreateSubKey("DefaultIcon"))
                    {
                        if (defaultIconKey == null)
                        {
                            Console.WriteLine("Error: Could not create or open DefaultIcon registry key");
                            return;
                        }

                        defaultIconKey.SetValue(string.Empty, iconPath); // Set the icon path
                    }
                }
            }

            Console.WriteLine($"Successfully changed icon for {fileExtension} to {iconPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public static void Main(string[] args)
    {
        Console.Write("File extension: ");
        string fileExtension = Console.ReadLine();
        Console.Write("Path to icon: ");
        string iconPath = Console.ReadLine();
        ChangeFileIcon(fileExtension, iconPath);
    }
}
