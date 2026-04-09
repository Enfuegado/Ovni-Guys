using System;
using System.IO;
using UnityEngine;

public class PlayerIdAssigner
{
    private FileStream lockStream;

    public int AssignPlayerId()
    {
        string lock0 = Path.Combine(Application.persistentDataPath, "lock0");
        string lock1 = Path.Combine(Application.persistentDataPath, "lock1");

        try
        {
            lockStream = new FileStream(lock0, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            return 0;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Lock0 ocupado, intentando lock1: " + e.Message);

            try
            {
                lockStream = new FileStream(lock1, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                return 1;
            }
            catch (Exception ex)
            {
                Debug.LogError("No se pudo asignar playerId: " + ex.Message);
                return 0;
            }
        }
    }

    public void Cleanup()
    {
        if (lockStream != null)
        {
            lockStream.Close();
            lockStream.Dispose();
            lockStream = null;
        }
    }
}