using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Xml.Linq;

public class FileIconChanger
{
    public static void ChangeFileIcon(string fileExtension, string iconPath)
    {
        try
        {
            // Open or create the file extension registry key
            using (RegistryKey extensionKey = Registry.ClassesRoot.OpenSubKey(fileExtension, true) ?? Registry.ClassesRoot.CreateSubKey(fileExtension))
            {
                if (extensionKey == null)
                {
                    Console.WriteLine($"Error: Could not create or open registry key for {fileExtension}");
                    return;
                }

                // Force association with a custom ProgID
                string progId = $"{fileExtension.Trim('.')}_customFile";
                extensionKey.SetValue(string.Empty, progId); // Override any existing ProgID

                // Open or create the ProgID registry key
                using (RegistryKey progIdKey = Registry.ClassesRoot.OpenSubKey(progId, true) ?? Registry.ClassesRoot.CreateSubKey(progId))
                {
                    if (progIdKey == null)
                    {
                        Console.WriteLine($"Error: Could not create or open ProgID registry key for {progId}");
                        return;
                    }

                    // Open or create the DefaultIcon subkey for the ProgID
                    using (RegistryKey defaultIconKey = progIdKey.OpenSubKey("DefaultIcon", true) ?? progIdKey.CreateSubKey("DefaultIcon"))
                    {
                        if (defaultIconKey == null)
                        {
                            Console.WriteLine("Error: Could not create or open DefaultIcon registry key");
                            return;
                        }

                        // Set or overwrite the icon path
                        defaultIconKey.SetValue(string.Empty, iconPath);
                    }
                }
            }

            // Restart Explorer to apply changes
            Console.WriteLine($"Successfully changed icon for {fileExtension} to {iconPath}");
            Console.WriteLine("Restarting Windows Explorer...");
            System.Diagnostics.Process.Start("cmd.exe", "/C taskkill /f /im explorer.exe && start explorer.exe");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }


    public static void DeleteFileIcon(string fileExtension)
    {
        try
        {
            using (RegistryKey extensionKey = Registry.ClassesRoot.OpenSubKey(fileExtension, true))
            {
                if (extensionKey == null)
                {
                    Console.WriteLine($"Error: Registry key for {fileExtension} not found");
                    return;
                }

                string progId = extensionKey.GetValue(string.Empty)?.ToString();
                if (!string.IsNullOrEmpty(progId))
                {
                    using (RegistryKey progIdKey = Registry.ClassesRoot.OpenSubKey(progId, true))
                    {
                        if (progIdKey != null)
                        {
                            progIdKey.DeleteSubKey("DefaultIcon", false);
                            Console.WriteLine($"Successfully deleted custom icon for {fileExtension}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Make sure this application has been run as an adminstrator. If not, you will see a permission denied error later.");
        Console.Write("File extension: ");
        string fileExtension = Console.ReadLine();
        if (fileExtension == null || fileExtension == "")
        {
            Console.WriteLine("Error: File Extension is null");
        }
        else
        {
            Console.WriteLine("Delete file icon or Create file icon? (d/c)");
            string go = Console.ReadLine();
            if (go == "c")
            {
                Console.Write("Path to icon: ");
                string iconPath = Console.ReadLine();
                if (iconPath == null)
                {
                    Console.WriteLine("Error: Path to icon is null");
                }
                else
                {
                    ChangeFileIcon(fileExtension, iconPath);
                }
            }
            else if (go == "d")
            {
                DeleteFileIcon(fileExtension);
            }
        }
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}

/*

Copyright 2025 Rihaan Meher

   Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

 */