using Microsoft.Win32;
using System;

public class FileIconChanger
{
    public static void ChangeFileIcon(string fileExtension, string iconPath)
    {
        try
        {
            // Construct the registry key path
            string keyPath = $@"HKEY_CLASSES_ROOT\{fileExtension}";

            // Open the registry key for the file extension
            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(fileExtension, false))
            {
                if (key == null)
                {
                    Console.WriteLine($"Error: Could not find registry key for {fileExtension}");
                    return;
                }

                // Construct the DefaultIcon subkey path
                string defaultIconKeyPath = keyPath + @"\DefaultIcon";

                // Create the DefaultIcon subkey if it doesn't exist
                using (RegistryKey defaultIconKey = Registry.ClassesRoot.OpenSubKey(defaultIconKeyPath, true) ?? Registry.ClassesRoot.CreateSubKey(defaultIconKeyPath))
                {
                    // Set the default value to the icon path
                    defaultIconKey.SetValue(string.Empty, iconPath);
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
        Console.Write("File extension:");
        string inputext = Console.ReadLine();
        Console.WriteLine("Path to icon");
        string inputpath = Console.ReadLine();
        ChangeFileIcon(inputext, inputpath);
    }
}