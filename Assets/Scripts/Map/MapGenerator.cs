using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine.SceneManagement;


public class MapGenerator : MonoBehaviour {
	[Header("Map dimentions")]
	public int width;
	public int height;
	public int depth = 5;
	public int bushSize = 3;

	[Header("Map seed settings")]
	public string seed;
	public bool useRandomSeed;

	[Header("Map generation settings")]
	public int smoothing = 5;
	public int smoothingStrictness = 4;
	public int borderSize = 2;
	public int roomSizeThreshold = 100;
	public int wallSizeThreshold = 50;

	[Header("Load specific binary map")]
	public bool loadSpecificMapFromFile = false;

	[Header("Save current map settings, or load from file")]
	public bool saveAsPremade = false;

	[Header("Map settings file")]
	public string saveFileName;

	[Header("Map density")]
	[Range(0, 100)]
	public int randomFillPercent = 42;

	int[,] map;

	void Start() {
		//SceneManager.SetActiveScene(SceneManager.GetSceneByName("MapGenScene"));
		//New entrypoint for map generation, handles loading data from file
		PreMapGen();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.F5)) {
			PreMapGen();
		}
	}

	/*
		Handles loading and/or saving of the current map settings
		Finished project will always load from file
	 */
	void PreMapGen() {
		if (saveAsPremade) {
			SaveMapSettings();
		} else { LoadMapSettings(); }
		GenerateMap();
	}
	/*
		Saves the current map settings to .txt file
	 */
	void SaveMapSettings() {
		StringBuilder mapData = new StringBuilder();
		mapData.Append(width + "\n" + height + "\n" + depth + "\n" + bushSize + "\n");
		mapData.Append(seed + "\n");
		if (useRandomSeed) {
			mapData.Append(1 + "\n");
		} else { mapData.Append(0 + "\n"); }
		mapData.Append(smoothing + "\n" + smoothingStrictness + "\n" + borderSize + "\n");
		mapData.Append(roomSizeThreshold + "\n" + wallSizeThreshold + "\n" + randomFillPercent);

		System.IO.File.WriteAllText(string.Format("Assets/Resources/MapTemplates/{0}.txt", saveFileName), mapData.ToString());
	}
	/*
		Loads settings from desired file
	 */
	void LoadMapSettings() {
		//Will grab current stage depth and mapType from player
		string currentMap = "testForrest1";
		var mapData = Resources.Load<TextAsset>(string.Format("MapTemplates/{0}", currentMap));
		string[] splitData = mapData.text.Split("\n");
		int[] parsedData = new int[splitData.Length];
		for (int i = 0; i < splitData.Length; i++) {
			if (splitData[i] != "") {
				parsedData[i] = Int32.Parse(splitData[i]);
			} else { parsedData[i] = 0; }
			
		}
		//Sets all data after reading it
		int cur = 0;
		width				= parsedData[cur];	cur++;
		height				= parsedData[cur];	cur++;
		depth				= parsedData[cur];	cur++;
		bushSize			= parsedData[cur];	cur++;
		seed				= splitData[cur];	cur++;
		if (parsedData[cur] == 0) { useRandomSeed = false; cur++; } 
		else { useRandomSeed = true; cur++; }
		smoothing			= parsedData[cur];	cur++;
		smoothingStrictness = parsedData[cur];	cur++;
		borderSize			= parsedData[cur];	cur++;
		roomSizeThreshold	= parsedData[cur];	cur++;
		wallSizeThreshold	= parsedData[cur];	cur++;
		randomFillPercent	= parsedData[cur];	cur++;
		/*width = parsedData[11]; //Spares incase we add more variables
		width = parsedData[12];
		width = parsedData[13];*/
	}

	/// <summary>
    /// Handles all map generation
    /// </summary>
	void GenerateMap() {
		SceneManager.SetActiveScene(SceneManager.GetSceneByName("MapGenScene"));
		if (loadSpecificMapFromFile) {
			//Loads the map from file
			LoadMapFromFile();
		} else {
			//Original map initialized here
			map = new int[width, height];
			//Create initial noisemap (Big ol mess, unsusable)
			RandomFillMap();
		}
		//Initialize secondary maps
		int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];
		int[,] invertedMap = new int[width + borderSize * 2, height + borderSize * 2];

		//Groups the map using neighbour algorythm, (Usable almost immediately)
		for (int i = 0; i < smoothing; i++) {
			SmoothMap();
		}

		//Performs more advanced grouping and item generation
		ProcessMap();

		//Add border on borderedMap and create inveted map
		for (int x = 0; x < borderedMap.GetLength(0); x++) {
			for (int y = 0; y < borderedMap.GetLength(1); y++) {
				if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize) {
					borderedMap[x, y] = map[x - borderSize, y - borderSize];
					if (borderedMap[x, y] != 0 && borderedMap[x, y] != 1) {
						invertedMap[x, y] = 1;
					}
					else {
						invertedMap[x, y] = 1 - borderedMap[x, y];
					}
				}
				else {
					borderedMap[x, y] = 20;
					invertedMap[x, y] = 1;
				}
			}
		}
		//Generate all meshes
		//Trees
		MeshGenerator treeGen = GetComponent<MeshGenerator>();
		treeGen.GenerateMesh(borderedMap, 1, depth, 0);
		//Bushes
		MeshGenerator bushGen = GetComponent<MeshGenerator>();
		bushGen.GenerateMesh(borderedMap, 1, bushSize, 1);
		//Outer wall
		MeshGenerator outerGen = GetComponent<MeshGenerator>();
		outerGen.GenerateMesh(borderedMap, 1, depth, 2);
		//Map Floor
		MeshGenerator genFloor = GetComponent<MeshGenerator>();
		genFloor.GenerateMesh(invertedMap, 1, depth, 3);
	}
	/*
		Process map marks trees bushes as well as defining rooms and ensuring they are connected 
	 */
	void ProcessMap() {
		List<List<Coord>> wallRegions = GetRegions(1);
		bool first = true;
		foreach (List<Coord> wallRegion in wallRegions) {
			if (first) {
				first = false;
				foreach (Coord tile in wallRegion) {
					map[tile.tileX, tile.tileY] = 20;
				}
			}
			if (wallRegion.Count < wallSizeThreshold) {
				foreach (Coord tile in wallRegion) {
					map[tile.tileX, tile.tileY] = 10;
				}
			}
		}

		List<List<Coord>> roomRegions = GetRegions(0);
		List<Room> survivingRooms = new List<Room>();

		foreach (List<Coord> roomRegion in roomRegions) {
			if (roomRegion.Count < roomSizeThreshold) {
				foreach (Coord tile in roomRegion) {
					map[tile.tileX, tile.tileY] = 1;
				}
			}
			else {
				survivingRooms.Add(new Room(roomRegion, map));
			}
		}
		survivingRooms.Sort();
		survivingRooms[0].isMainRoom = true;
		survivingRooms[0].isAccessibleFromMainRoom = true;

		ConnectClosestRooms(survivingRooms);
	}
	/*
		Connects a room to its closest neighbour
	 */
	void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false) {

		List<Room> roomListA = new List<Room>();
		List<Room> roomListB = new List<Room>();

		if (forceAccessibilityFromMainRoom) {
			foreach (Room room in allRooms) {
				if (room.isAccessibleFromMainRoom) {
					roomListB.Add(room);
				}
				else {
					roomListA.Add(room);
				}
			}
		}
		else {
			roomListA = allRooms;
			roomListB = allRooms;
		}

		int bestDistance = 0;
		Coord bestTileA = new Coord();
		Coord bestTileB = new Coord();
		Room bestRoomA = new Room();
		Room bestRoomB = new Room();
		bool possibleConnectionFound = false;

		foreach (Room roomA in roomListA) {
			if (!forceAccessibilityFromMainRoom) {
				possibleConnectionFound = false;
				if (roomA.connectedRooms.Count > 0) {
					continue;
				}
			}

			foreach (Room roomB in roomListB) {
				if (roomA == roomB || roomA.IsConnected(roomB)) {
					continue;
				}

				for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA++) {
					for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB++) {
						Coord tileA = roomA.edgeTiles[tileIndexA];
						Coord tileB = roomB.edgeTiles[tileIndexB];
						int distanceBetweenRooms = (int)(Mathf.Pow(tileA.tileX - tileB.tileX, 2) + Mathf.Pow(tileA.tileY - tileB.tileY, 2));

						if (distanceBetweenRooms < bestDistance || !possibleConnectionFound) {
							bestDistance = distanceBetweenRooms;
							possibleConnectionFound = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = roomA;
							bestRoomB = roomB;
						}
					}
				}
			}
			if (possibleConnectionFound && !forceAccessibilityFromMainRoom) {
				CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			}
		}

		if (possibleConnectionFound && forceAccessibilityFromMainRoom) {
			CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			ConnectClosestRooms(allRooms, true);
		}

		if (!forceAccessibilityFromMainRoom) {
			ConnectClosestRooms(allRooms, true);
		}
	}
	/*
		Creates a path from one room to another using a straight line and drawing circles along it
	 
	 */
	void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {
		Room.ConnectRooms(roomA, roomB);
		//Debug.DrawLine (CoordToWorldPoint (tileA), CoordToWorldPoint (tileB), Color.green, 100);

		List<Coord> line = GetLine(tileA, tileB);
		foreach (Coord c in line) {
			DrawCircle(c, 5);
		}
	}
	/*
		Draws circles along a line to create a path
	 */
	void DrawCircle(Coord c, int r) {
		for (int x = -r; x <= r; x++) {
			for (int y = -r; y <= r; y++) {
				if (x * x + y * y <= r * r) {
					int drawX = c.tileX + x;
					int drawY = c.tileY + y;
					if (IsInMapRange(drawX, drawY)) {
						map[drawX, drawY] = 0;
					}
				}
			}
		}
	}
	/*
		 Creates a list of coordinates that draw a line from one point to another
		 Used to connect rooms
	 */
	List<Coord> GetLine(Coord from, Coord to) {
		List<Coord> line = new List<Coord>();

		int x = from.tileX;
		int y = from.tileY;

		int dx = to.tileX - from.tileX;
		int dy = to.tileY - from.tileY;

		bool inverted = false;
		int step = Math.Sign(dx);
		int gradientStep = Math.Sign(dy);

		int longest = Mathf.Abs(dx);
		int shortest = Mathf.Abs(dy);

		if (longest < shortest) {
			inverted = true;
			longest = Mathf.Abs(dy);
			shortest = Mathf.Abs(dx);

			step = Math.Sign(dy);
			gradientStep = Math.Sign(dx);
		}

		int gradientAccumulation = longest / 2;
		for (int i = 0; i < longest; i++) {
			line.Add(new Coord(x, y));

			if (inverted) {
				y += step;
			}
			else {
				x += step;
			}

			gradientAccumulation += shortest;
			if (gradientAccumulation >= longest) {
				if (inverted) {
					x += gradientStep;
				}
				else {
					y += gradientStep;
				}
				gradientAccumulation -= longest;
			}
		}

		return line;
	}
	/*
	 Converts a Coord object to in game coordinate
	 */
	Vector3 CoordToWorldPoint(Coord tile) {
		return new Vector3(-width / 2 + .5f + tile.tileX, 2, -height / 2 + .5f + tile.tileY);
	}
	/*
		Gets a list of all regions of a given type
		@see	GetRegionTiles(int,int);
	 */
	List<List<Coord>> GetRegions(int tileType) {
		List<List<Coord>> regions = new List<List<Coord>>();
		int[,] mapFlags = new int[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (mapFlags[x, y] == 0 && map[x, y] == tileType) {
					List<Coord> newRegion = GetRegionTiles(x, y);
					regions.Add(newRegion);

					foreach (Coord tile in newRegion) {
						mapFlags[tile.tileX, tile.tileY] = 1;
					}
				}
			}
		}

		return regions;
	}
	/*
		Gets all tiles within a "region" in a given type,
		For example a section of connected wall
	 */
	List<Coord> GetRegionTiles(int startX, int startY) {
		List<Coord> tiles = new List<Coord>();
		int[,] mapFlags = new int[width, height];
		int tileType = map[startX, startY];

		Queue<Coord> queue = new Queue<Coord>();
		queue.Enqueue(new Coord(startX, startY));
		mapFlags[startX, startY] = 1;

		while (queue.Count > 0) {
			Coord tile = queue.Dequeue();
			tiles.Add(tile);

			for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
				for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
					if (IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX)) {
						if (mapFlags[x, y] == 0 && map[x, y] == tileType) {
							mapFlags[x, y] = 1;
							queue.Enqueue(new Coord(x, y));
						}
					}
				}
			}
		}
		return tiles;
	}
	/*
		Ensures a given coordinate is on the map
	*/
	bool IsInMapRange(int x, int y) {
		return x >= 0 && x < width && y >= 0 && y < height;
	}

	/*
		Creates a random noisemap in the map variable
	 */
	void RandomFillMap() {
		//Uses current time as seed
		if (useRandomSeed) {
			seed = Time.time.ToString();
		}

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
					map[x, y] = 1;
				}
				else {
					//If the rolled number is smaller than the randomFillPercent variable, then its a "wall"
					map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
				}
			}
		}
	}
	/*
		SmoothMap groups walls depending on the ammount of neighbouring walls
	 */
	void SmoothMap() {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				int neighbourWallTiles = GetSurroundingWallCount(x, y);

				if (neighbourWallTiles > smoothingStrictness)
					map[x, y] = 1;
				else if (neighbourWallTiles < smoothingStrictness)
					map[x, y] = 0;
			}
		}
	}
	/*
	 Checks how many walls are in a 3x3 grid around any given point,
	 Map boundaries er automatically walls
	 */
	int GetSurroundingWallCount(int gridX, int gridY) {
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {
				if (IsInMapRange(neighbourX , neighbourY)) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += map[neighbourX ,  neighbourY];
					}
				}
				else {
					wallCount++;
				}
			}
		}
		return wallCount;
	}

	/*
		Loads the map from a custom binary file, only used to create then export specific meshes
	 */
	void LoadMapFromFile() {
		var inMap = Resources.Load<TextAsset>("Text/roundRoomMap");
		string[] processedMap = inMap.text.Split("\n");
		int yCount = 0;
		int xCount = 0;
		map = new int[processedMap[0].Length, processedMap.Length];
		foreach (string yCoord in processedMap) {
			xCount = 0;
			foreach (char binaryMapData in yCoord) {
				map[xCount, yCount] = Mathf.RoundToInt((float)Char.GetNumericValue(binaryMapData));
				xCount++;
			}
			yCount++;
		}
		width = xCount;
		height = yCount;
	}
	/*
		Coord struct is effectively an int variation of a Vec2 
	 */
	struct Coord {
		public int tileX;
		public int tileY;

		public Coord(int x, int y) {
			tileX = x;
			tileY = y;
		}
	}

	/*
		The room class is used to store information about all rooms,
		as well as comparing rooms using internal functions
	 */
	class Room : IComparable<Room> {
		public List<Coord> tiles;
		public List<Coord> edgeTiles;
		public List<Room> connectedRooms;
		public int roomSize;
		public bool isAccessibleFromMainRoom;
		public bool isMainRoom;

		public Room() {
		}
		//creates a list defining all edgetiles of a room as well as finding a rooms size
		public Room(List<Coord> roomTiles, int[,] map) {
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room>();

			edgeTiles = new List<Coord>();
			foreach (Coord tile in tiles) {
				for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
					for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
						if (x == tile.tileX || y == tile.tileY) {
							if (map[x, y] == 1) {
								edgeTiles.Add(tile);
							}
						}
					}
				}
			}
		}
		/*
			Iterates all connected rooms and sets that they are connected to the main room if they arent allready
		 */
		public void SetAccessibleFromMainRoom() {
			if (!isAccessibleFromMainRoom) {
				isAccessibleFromMainRoom = true;
				foreach (Room connectedRoom in connectedRooms) {
					connectedRoom.SetAccessibleFromMainRoom();
				}
			}
		}
		/*
			Sets both provided rooms as connected to eachother
		 */
		public static void ConnectRooms(Room roomA, Room roomB) {
			if (roomA.isAccessibleFromMainRoom) {
				roomB.SetAccessibleFromMainRoom();
			}
			else if (roomB.isAccessibleFromMainRoom) {
				roomA.SetAccessibleFromMainRoom();
			}
			roomA.connectedRooms.Add(roomB);
			roomB.connectedRooms.Add(roomA);
		}
		/*
			Checks if a room is connected to a given room
		 */
		public bool IsConnected(Room otherRoom) {
			return connectedRooms.Contains(otherRoom);
		}
		/*
			Compares the size of two rooms
		 */
		public int CompareTo(Room otherRoom) {
			return otherRoom.roomSize.CompareTo(roomSize);
		}
	}
}
