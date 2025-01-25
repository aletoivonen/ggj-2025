using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<LevelSegment> _levelSegments;
    [SerializeField] private float _segmentSpacing = 2f;
    [SerializeField] private int _segmentCount = 10;
    private int _seed;
    
    private void Start()
    {
        SetSeed();
        GenerateLevel();
    }

    private void SetSeed()
    {
        DateTime utcNow = DateTime.UtcNow;
        _seed = int.Parse(utcNow.ToString("yyyyMMdd"));
    }

    public void GenerateLevel()
    {
        float y = 0;
        Random.InitState(_seed);
        for (int i = 0; i < _segmentCount; i++)
        {
            LevelSegment segment = Instantiate(_levelSegments[Random.Range(0, _levelSegments.Count)]);
            segment.transform.position = new Vector3(0, y, 0);
            y += segment.GetSegmentYSize() + _segmentSpacing;
        }
    }
    
    
    
}
