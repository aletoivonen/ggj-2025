using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSegment : MonoBehaviour
{
    public List<Platform> GetPlatforms()
    {
        return GetComponentsInChildren<Platform>().ToList();
    }

    public float GetLowestPoint()
    {
        var plats = GetComponentsInChildren<Platform>().ToList();
        return plats.Min(p => p.transform.position.y);
    }
    
    public float GetHighestPoint()
    {
        var plats = GetComponentsInChildren<Platform>().ToList();
        return plats.Max(p => p.transform.position.y);
    }

    public float GetSegmentYSize()
    {
        return GetHighestPoint() - GetLowestPoint();
    }
}
