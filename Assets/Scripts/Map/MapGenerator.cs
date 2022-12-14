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
	public int bushSizeThreshold = 50;
	public int bossLevel = 10;

	[Header("Overrides automatic template loading")]
	public bool dontLoadMap = false;
	protected bool interruptMapGen = false;

	[Header("Have checked to build")]
	public bool buildControl = false;
	
	[Header("Map settings filename")]
	public string saveFileName;

	[Header("Map density")]
	[Range(0, 100)]
	public int randomFillPercent = 42;
	

	int[,] map;

	void Start() {
		SceneManager.SetActiveScene(SceneManager.GetSceneByName("MapGenScene"));
		//New entrypoint for map generation, handles loading data from file
		PreMapGen();
	}

	void Update() {
		if (!buildControl) { 
			if (Input.GetKeyDown(KeyCode.F5)) {
				PreMapGen();
			}
			if (Input.GetKeyDown(KeyCode.F8)) {
				SaveMapSettings();
			}
		}
	}

	/*
		Handles loading and/or saving of the current map settings
		Finished project will always load from file
	 */
	void PreMapGen() {
		if (!dontLoadMap || buildControl) {
			LoadMapSettings();
		}
		if (interruptMapGen) { return; } 
		GenerateMap();
	}
	
	/// <summary>
    /// DEVELOPER FUNCTION
    /// Stores current map settings to txt file with publicly set variable
    /// </summary>
	void SaveMapSettings() {
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		PlayerController playerData = player.GetComponent<PlayerController>();
		int curStage = playerData.currentStage;
		StringBuilder mapData = new StringBuilder();
		mapData.Append(width + "\n" + height + "\n" + depth + "\n" + bushSize + "\n");
		mapData.Append(seed + "\n");
		if (useRandomSeed) {
			mapData.Append(1 + "\n");
		} else { mapData.Append(0 + "\n"); }
		mapData.Append(smoothing + "\n" + smoothingStrictness + "\n" + borderSize + "\n");
		mapData.Append(roomSizeThreshold + "\n" + bushSizeThreshold + "\n" + randomFillPercent);
		System.IO.File.WriteAllText(string.Format("Assets/Resources/MapTemplates/{0}{1}.txt", saveFileName, curStage), mapData.ToString());
	}
	
	/// <summary>
    /// Loads a map based on what template is currently wanted
    /// </summary>
	void LoadMapSettings() {
		//Grabs current stage depth and mapType from player
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		PlayerController playerData = player.GetComponent<PlayerController>();
		
		int curStage = playerData.currentStage;
		string curBiome = playerData.currentBiome;
		string currentMap = curBiome + curStage;

		//Checks if player has arrived at the boss
		if (bossLevel <= curStage) {
			interruptMapGen = true;
			//Swap to boss scene
			GameObject sceneLoaderObject = GameObject.FindGameObjectWithTag("SceneLoader");
			SceneLoader sceneLoader = sceneLoaderObject.GetComponent<SceneLoader>();
			sceneLoader.ChangeScene(curBiome + "BossArena");
			return;
		}

		var mapData = Resources.Load<TextAsset>(string.Format("MapTemplates/{0}", currentMap));
		//If there isnt a template with the desired name, just use whatever is default atm
		if (mapData == null) { return; }
		string[] splitData = mapData.text.Split("\n");
		int[] parsedData = new int[splitData.Length];
		for (int i = 0; i < splitData.Length; i++) {
			if (splitData[i] != "") {
				if (int.TryParse(splitData[i], out parsedData[i])) {
					parsedData[i] = Int32.Parse(splitData[i]);
				}
				

			} else { parsedData[i] = 0; }
		}
		//Sets all data after reading it
		int cur = 0;
		width				= parsedData[cur];	cur++;
		height				= parsedData[cur];	cur++;
		depth				= parsedData[cur];	cur++;
		bushSize			= parsedData[cur];	cur++;
		seed				= splitData[cur];	cur++;
		if (splitData[cur] == "0") { useRandomSeed = false; cur++; } 
		else { useRandomSeed = true; cur++; }
		smoothing			= parsedData[cur];	cur++;
		smoothingStrictness = parsedData[cur];	cur++;
		borderSize			= parsedData[cur];	cur++;
		roomSizeThreshold	= parsedData[cur];	cur++;
		bushSizeThreshold	= parsedData[cur];	cur++;
		randomFillPercent	= parsedData[cur];	cur++;
	}

	/// <summary>
    /// Handles all map generation
    /// </summary>
	void GenerateMap() {
		//Original map initialized here
		map = new int[width, height];
		//Create initial noisemap (Big ol mess, unsusable)
		RandomFillMap();        
		//Initialize secondary maps
		int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];
		int[,] invertedMap = new int[width + borderSize * 2, height + borderSize * 2];
		//Groups the map using neighbour algorithm, (Usable almost immediately)
		for (int i = 0; i < smoothing; i++) {
			SmoothMap();
		}

		//Performs more advanced grouping and item generation
		ProcessMap();

		//Add border on borderedMap and create invreted map
		for (int x = 0; x < borderedMap.GetLength(0); x++) {
			for (int y = 0; y < borderedMap.GetLength(1); y++) {
				invertedMap[x, y] = 1;
				if (x >= borderSize && x < width+ borderSize && y >= borderSize && y < height+borderSize) {
					borderedMap[x, y] = map[x - borderSize, y - borderSize];
				}
				else {
					borderedMap[x, y] = 20;
					//Including this would remove all mesh underneeth the outer roof,
					//However, raycasting demands there to be a floor to cast to, so the mesh has been kept
					//invertedMap[x, y] = 0;
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

	/// <summary>
	/// Process map marks trees bushes as well as defining rooms and ensuring they are connected 
	/// </summary>
	void ProcessMap() {
		List<List<Coord>> roomRegions = GetRegions(0);
		List<Room> survivingRooms = new List<Room>();

		foreach (List<Coord> roomRegion in roomRegions) {
			if (roomRegion.Count < roomSizeThreshold) {
				foreach (Coord tile in roomRegion) {
					map[tile.tileX, tile.tileY] = 1;
				}
			}
		}

		List<List<Coord>> wallRegions = GetRegions(1);
		int buffer = (width + height) / 12;
		// Defines outer wall to ensure it doesnt get turned into objects
		foreach (Coord tile in wallRegions[0]) {
			if (tile.tileX <= buffer || (width - buffer) <= tile.tileX || tile.tileY <= buffer || (height - buffer) <= tile.tileY) {
				map[tile.tileX, tile.tileY] = 20;
			}
		}
		//Refresh region list, since there might be residual wall segments
		wallRegions = GetRegions(1);
		wallRegions.AddRange(GetRegions(20));
		foreach (List<Coord> wallRegion in wallRegions) {
			//Replaces everything smaller than the threshold with bushes
			if (wallRegion.Count < bushSizeThreshold) {
				foreach (Coord tile in wallRegion) {
					map[tile.tileX, tile.tileY] = 10;
				}
			}
		}

		roomRegions = GetRegions(0);
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

	/// <summary>
	/// Connects a room to its closest neighbour
	/// </summary>
	/// <param name="allRooms"></param>
	/// <param name="forceAccessibilityFromMainRoom"></param>
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


	/// <summary>
	/// Creates a path from one room to another using a straight line and drawing circles along it
	/// </summary>
	/// <param name="roomA"></param>
	/// <param name="roomB"></param>
	/// <param name="tileA"></param>
	/// <param name="tileB"></param>
	void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {
		Room.ConnectRooms(roomA, roomB);
		//Debug.DrawLine (CoordToWorldPoint (tileA), CoordToWorldPoint (tileB), Color.green, 100);

		List<Coord> line = GetLine(tileA, tileB);
		foreach (Coord c in line) {
			DrawCircle(c, 10);
		}
	}

	/// <summary>
	/// Draws circles along a line to create a path
	/// </summary>
	/// <param name="c"></param>
	/// <param name="r"></param>
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

	/// <summary>
	/// Creates a list of coordinates that draw a line from one point to another
	/// Used to connect rooms
	/// </summary>
	/// <param name="from"></param>
	/// <param name="to"></param>
	/// <returns></returns>
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

	/// <summary>
	/// Converts a Coord object to in game coordinate
	/// </summary>
	/// <param name="tile"></param>
	/// <returns></returns>
	Vector3 CoordToWorldPoint(Coord tile) {
		return new Vector3(-width / 2 + .5f + tile.tileX, 2, -height / 2 + .5f + tile.tileY);
	}

	/// <summary>
	/// Gets a list of all regions of a given type
	/// @see GetRegionTiles(int, int);
	/// </summary>
	/// <param name="tileType"></param>
	/// <returns></returns>
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


	/// <summary>
	/// Gets all tiles within a "region" in a given type,
	/// For example a section of connected wall
	/// </summary>
	/// <param name="startX"></param>
	/// <param name="startY"></param>
	/// <returns></returns>
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

	/// <summary>
	/// Ensures a given coordinate is on the map
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	bool IsInMapRange(int x, int y) {
		return x >= 0 && x < width && y >= 0 && y < height;
	}

	/// <summary>
	/// Creates a random noisemap in the map variable
	/// </summary>
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


	/// <summary>
	/// SmoothMap groups walls depending on the ammount of neighbouring walls
	/// </summary>
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


	/// <summary>
	/// Checks how many walls are in a 3x3 grid around any given point,
	/// Map boundaries er automatically walls
	/// </summary>
	/// <param name="gridX"></param>
	/// <param name="gridY"></param>
	/// <returns></returns>
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


	/// <summary>
	/// Loads the map from a custom binary file, only used to create then export specific meshes
	/// </summary>
	[Obsolete("This is used to load a specific binary textfile as a map, no longer used, instead generate map and save mesh")]
	void LoadMapFromFile() {
		var inMap = Resources.Load<TextAsset>("Text/"+"BINARYFILENAME");
		if (inMap == null) return;
		string[] processedMap = inMap.text.Split("\n");
		int yCount = 0;
		int xCount = 0;
		map = new int[processedMap[0].Length, processedMap.Length];
		for (int y = 0; y < processedMap.Length; y++) {
			xCount = 0;
			foreach (var binaryMapData in processedMap[y]) {
				map[xCount, yCount] = Mathf.RoundToInt((float)Char.GetNumericValue(binaryMapData));
				xCount++;
			}
			yCount++;
		}
		width = processedMap[0].Length;
		height = processedMap.Length;
	}

	/// <summary>
	/// Coord struct is effectively an int variation of a Vec2 
	/// </summary>
	struct Coord {
		public int tileX;
		public int tileY;

		public Coord(int x, int y) {
			tileX = x;
			tileY = y;
		}
	}

	
	/// <summary>
    /// The room class is used to store information about all rooms,
	///	as well as comparing rooms using internal functions
	/// </summary>
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

		/// <summary>
		/// Iterates all connected rooms and sets that they are connected to the main room if they arent allready
		/// </summary>
		public void SetAccessibleFromMainRoom() {
			if (!isAccessibleFromMainRoom) {
				isAccessibleFromMainRoom = true;
				foreach (Room connectedRoom in connectedRooms) {
					connectedRoom.SetAccessibleFromMainRoom();
				}
			}
		}

		/// <summary>
		/// Sets both provided rooms as connected to eachother
		/// </summary>
		/// <param name="roomA"></param>
		/// <param name="roomB"></param>
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

		/// <summary>
		/// Checks if a room is connected to a given room
		/// </summary>
		/// <param name="otherRoom"></param>
		/// <returns></returns>
		public bool IsConnected(Room otherRoom) {
			return connectedRooms.Contains(otherRoom);
		}

		/// <summary>
		/// Compares the size of two rooms
		/// </summary>
		/// <param name="otherRoom"></param>
		/// <returns></returns>
		public int CompareTo(Room otherRoom) {
			return otherRoom.roomSize.CompareTo(roomSize);
		}
	}
}
