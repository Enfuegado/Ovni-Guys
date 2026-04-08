using System.Collections.Generic;
public class EventProcessor
{
    private HashSet<int> processedOrbs = new();

    public bool IsGameEnd(float z) => z >= 9000f;

    public int? GetOrbId(float z)
    {
        if (z >= 1000f && z < 9000f)
        {
            int id = (int)(z - 1000f);

            if (processedOrbs.Contains(id))
                return null;

            processedOrbs.Add(id);
            return id;
        }

        return null;
    }
}