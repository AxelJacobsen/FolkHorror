using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TerrainGenerator : MonoBehaviour
{
    private int[,] MapContainer;
   // private List<List<Coord>> WallRegions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateTerrain(int[,] inMap) {
        MapContainer = inMap;
    }

    /*public void RecieveRegions(List<List<Coord>> inRegions) {
        WallRegions = inRegions;
    }

    struct Coord {
        public int tileX;
        public int tileY;

        public Coord(int x, int y) {
            tileX = x;
            tileY = y;
        }
    }*/
}
