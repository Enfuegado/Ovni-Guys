using System.IO;
using UnityEngine;

public class FileLock : MonoBehaviour
{
    private FileStream stream;

    public void Init(FileStream fs)
    {
        stream = fs;
    }

    void OnApplicationQuit()
    {
        if (stream != null)
        {
            stream.Close();
            stream.Dispose();
        }
    }
}