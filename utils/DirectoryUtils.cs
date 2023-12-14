using System.Collections.Generic;
using Godot;

namespace ProjectNostalgia.utils;

public static class DirectoryUtils
{

    public static List<string> GetFilesInDirectory(string path)
    {
        List<string> files = new();
        using DirAccess dir = DirAccess.Open(path);
        string[] fileNames =DirAccess.GetFilesAt(path);
        foreach (string fileName in fileNames)
        {
            files.Add(path + "/" + fileName);
        }
        return files;
    } 
    
}