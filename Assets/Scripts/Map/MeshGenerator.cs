using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class MeshGenerator : MonoBehaviour {

	public SquareGrid squareGrid;
	public MeshFilter treeWall;
	public MeshFilter floor;
	public MeshFilter bushCollide;
	public MeshFilter outerWall;
	public MeshFilter outerRoof;

	private ObjectSpawner objSpawner;

	[Header("Map Objects")]
	public GameObject Tree;
	public GameObject Bush;
	public List<GameObject> Generic;

	[Header("Enemies")]
	public List<GameObject> Enemies;
	public GameObject EnemyWeapon;

	[Header("Portals")]
	public GameObject Entrance;
	public GameObject Exit;

	public int PortalRadius = 10;
	public int NumberOfEnemies = 10;
	public bool GenerateObjects = true;
	public bool GenerateEnemies = true;

	private int wallHeight;
	List<Vector3> vertices;
	List<int> triangles;
	Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
	List<List<int>> outlines = new List<List<int>>();
	HashSet<int> checkedVertices = new HashSet<int>();

	/// <summary>
	/// Handles all mesh and object generation using lower functions
	/// </summary>
	/// <param name="map">Input map to create mesh of</param>
	/// <param name="squareSize">size of a single square tile</param>
	/// <param name="depth">Height of the tree or bush being generated</param>
	/// <param name="meshType">meshType 0 = standard, 1 = bush, 2 = outerwall , 3 = floor</param>
	public void GenerateMesh(int[,] map, float squareSize, int depth, int meshType) {
		wallHeight = depth;
		triangleDictionary.Clear();
		outlines.Clear();
		checkedVertices.Clear();
		int useFloorHeight = depth;
		switch (meshType) {
			case 1: map = IsolateMapObjects(map, 10); break;
			case 2: map = IsolateMapObjects(map, 20); break;
			case 3: useFloorHeight = 0; break;
			default: break;
		}

		objSpawner = GetComponent<ObjectSpawner>();
		if (objSpawner == null) Debug.LogError("MeshGen couldnt find Object spawner!");

		squareGrid = new SquareGrid(map, squareSize, useFloorHeight);

		vertices = new List<Vector3>();
		triangles = new List<int>();
		//Fills vertices and triangles lists with data
		for (int x = 0; x < squareGrid.squares.GetLength(0); x++) {
			for (int y = 0; y < squareGrid.squares.GetLength(1); y++) {
				TriangulateSquare(squareGrid.squares[x, y]);
			}
		}

		Mesh mesh = new Mesh();

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();

		int tileAmountX = map.GetLength(0)/10,
			tileAmountY = map.GetLength(1)/10;
		Vector2[] uvs = new Vector2[vertices.Count];
		
		//Creates floor & roof UVs
		for (int i = 0; i < vertices.Count; i++) {
			float percentX = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, vertices[i].x) * tileAmountX;
			float percentY = Mathf.InverseLerp(-map.GetLength(1) / 2 * squareSize, map.GetLength(1) / 2 * squareSize, vertices[i].z) * tileAmountY;
			uvs[i] = new Vector2(percentX, percentY);
		}
		mesh.uv = uvs;

		switch (meshType) {
			case 2: 
				{
					outerRoof.mesh = mesh;
					//Necessary to save the mesh
					MeshCollider outerRoofCollide  = outerRoof.gameObject.GetComponent<MeshCollider>();
					outerRoofCollide.sharedMesh = mesh;
					CreateWallMesh(map, squareSize, meshType); 
				} break;
			case 3: 
				{
					// If floor is being generated, sets collider and 
					floor.mesh = mesh;
					MeshCollider floorCollider = floor.gameObject.GetComponent<MeshCollider>();
					floorCollider.sharedMesh = mesh;
				} break;
			default: CreateWallMesh(map, squareSize, meshType); break;
		}
		if (meshType == 3) {
			grabTextureFromPlayer();
		}
	}

	/// <summary>
    /// Creates a wall
    /// </summary>
    /// <param name="meshType"></param>
	void CreateWallMesh(int[,] map, float squareSize, int meshType) {
		CalculateMeshOutlines();

		List<Vector3> wallVertices = new List<Vector3>();
		List<int> wallTriangles = new List<int>();
		Mesh wallMesh = new Mesh();
		int treeChance = 0;
		bool first = false;
		if (meshType == 2) { first = true; }

		foreach (List<int> outline in outlines) {
			if (GenerateObjects) {
				treeChance = SpawnObjectHandler(outline, treeChance, meshType, first);
				if (first) { first = false; }
			}
			for (int i = 0; i < outline.Count - 1; i++) {
				int startIndex = wallVertices.Count;
				wallVertices.Add(vertices[outline[i]]); // left
				wallVertices.Add(vertices[outline[i + 1]]); // right
				wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); // bottom left
				wallVertices.Add(vertices[outline[i + 1]] - Vector3.up * wallHeight); // bottom right

				wallTriangles.Add(startIndex + 0);
				wallTriangles.Add(startIndex + 2);
				wallTriangles.Add(startIndex + 3);

				wallTriangles.Add(startIndex + 3);
				wallTriangles.Add(startIndex + 1);
				wallTriangles.Add(startIndex + 0);
			}
		}
		wallMesh.vertices = wallVertices.ToArray();
		wallMesh.triangles = wallTriangles.ToArray();
		wallMesh.RecalculateNormals();

		switch (meshType) {
			case 0: {	//treewalls
						treeWall.mesh = wallMesh;
						MeshCollider wallCollider = treeWall.gameObject.GetComponent<MeshCollider>();
						wallCollider.sharedMesh = wallMesh;
					} break;
			case 1: {	//spawnbushes
						bushCollide.mesh = wallMesh;
						MeshCollider bushCrash = bushCollide.gameObject.GetComponent<MeshCollider>();
						bushCrash.sharedMesh = wallMesh;
					} break;
			case 2: {   //spawn outerwall 
						outerWall.mesh = wallMesh;
						MeshCollider outerWallCollider = outerWall.gameObject.GetComponent<MeshCollider>();
						outerWallCollider.sharedMesh = wallMesh;
					} break;
			default: {
						//Shouldnt occur
						print("Uses default");
						treeWall.mesh = wallMesh;
						MeshCollider wallCollider = treeWall.gameObject.GetComponent<MeshCollider>();
						wallCollider.sharedMesh = wallMesh;
					} break;
		}
	}

	/// <summary>
    /// Spawns objects within the bounds of diffrent polygons on the map
    /// </summary>
    /// <param name="outline"></param>
    /// <param name="treeChance"></param>
    /// <param name="spawnObject"></param>
    /// <param name="isFirst"></param>
    /// <returns></returns>
	int SpawnObjectHandler(List<int> outline, int treeChance, int spawnObject, bool isFirst) {
		List<Vector2> polyList = new List<Vector2>();
		foreach (int vertex in outline) {
			polyList.Add(new Vector2(vertices[vertex].x, vertices[vertex].z));
		}
		GameObject mapObject = Tree;
		Vector2[] poly = polyList.ToArray();

		//Spawns entrance and exit, but only in the main outline
		if (isFirst) {
			Vector2 pos1 = new Vector2(0, 0),
					pos2 = new Vector2(0, 0),
					pLowBound = new Vector2(0, 0),
					pUpBound = new Vector2(0, 0);
			(pos1, pos2, pLowBound, pUpBound) = Funcs.ForceFarSpawn(poly);
			GameObject[] trees = GameObject.FindGameObjectsWithTag("Solid Object");

			SpawnPortal(poly);
			if (!GenerateEnemies) { return treeChance; }

			PlayerController pCon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
			//Default to center to avoid dumb issues
			Vector2 eSpawnPoint = new Vector2(0, 0);
			for (int e = 0; e < NumberOfEnemies + pCon.currentStage; e++) {
				eSpawnPoint = Funcs.FindPointOutsidePlayerSpawn(pLowBound, pUpBound, poly);
				int enemyType = Random.Range(0, Enemies.Count);

				GameObject Enemy = objSpawner.SpawnObject(new Vector3(eSpawnPoint.x, 0, eSpawnPoint.y), Enemies[enemyType]);
				MoveObjectOutOfTrees(trees, Enemy, poly);
				GameObject EnemyWep = objSpawner.SpawnObject(new Vector3(eSpawnPoint.x, 0, eSpawnPoint.y), EnemyWeapon);
				MoveObjectOutOfTrees(trees, EnemyWep, poly);
			}
			return 0;
		}

		if (spawnObject == 1) { mapObject = Bush; }
		//Spawn along outline edge to secure border
		for (int o = 0; o < polyList.Count; o++) {
			treeChance += Random.Range(1, 3);
			if (treeChance < 10) { continue; }
			objSpawner.SpawnObject(new Vector3(polyList[o].x, 0, polyList[o].y), mapObject);
			treeChance = 0;
		}

		//Spawn trees randomly inside border
		for (int t = 0; t < outline.Count * 2 / 5; t++) {
			Vector2 pos = Funcs.GetRandomPointInPolygon(poly);
			objSpawner.SpawnObject(new Vector3(pos.x, 0, pos.y), mapObject);
		}

		
		return treeChance;
	}

	/// <summary>
    /// Handles spawning portals
    /// </summary>
    /// <param name="poly"></param>
	private void SpawnPortal(Vector2[] poly) {
		Vector2 pos1 = new Vector2(0, 0),
		pos2 = new Vector2(0, 0),
		pLowBound = new Vector2(0, 0),
		pUpBound = new Vector2(0, 0);
		(pos1, pos2, pLowBound, pUpBound) = Funcs.ForceFarSpawn(poly);
		GameObject[] trees = GameObject.FindGameObjectsWithTag("Solid Object");

		//Spawns exit
		objSpawner.SpawnObject(new Vector3(pos1.x, 0, pos1.y), Exit);
		//Moves a portal away if its stuck in trees
		MoveObjectOutOfTrees(trees, Exit, poly);
		//Spawns entrance
		objSpawner.SpawnObject(new Vector3(pos2.x, 0, pos2.y), Entrance);
		MoveObjectOutOfTrees(trees, Entrance, poly);
	}

	/// <summary>
    /// Checks for the nearest tree and moves the object away
    /// </summary>
    /// <param name="trees">List of all trees</param>
    /// <param name="newName">The portals new name</param>
	private void MoveObjectOutOfTrees(GameObject[] trees, GameObject interObject, Vector2[] poly) {
		//GameObject portal = GameObject.Find("Portal(Clone)");
		Vector2 moveAwayVec = new Vector2(0,0);
		bool intersects = true;
		bool notInMap = false;
		int timeOut = 100;
		do {
			(moveAwayVec, intersects) = Funcs.CheckForIntersect(trees, interObject.transform.position, PortalRadius);
			if (intersects) {
				//Extend vector from nearest tree to be demanded radius
				moveAwayVec = moveAwayVec.normalized * PortalRadius;
				//Apply the new vector to the portal excluding the Y axis
				interObject.transform.position = new Vector3(interObject.transform.position.x + moveAwayVec.x*2, interObject.transform.position.y, interObject.transform.position.z + moveAwayVec.y*2);
			}
			if (timeOut%5 == 0) {
				if (!Funcs.IsInPolygon(poly, interObject.transform.position)) {
					notInMap = true; 
					interObject.transform.position -= (interObject.transform.position / -10.0f); 
				} else { notInMap = false; } 
			}
			timeOut--;
		} while (intersects || notInMap && 0<timeOut);
	}

	/// <summary>
    /// Creates a meshTriangle from squares corners depending on a squares neighbours
    /// </summary>
    /// <param name="square"></param>
	void TriangulateSquare(Square square) {
		switch (square.configuration) {
			case 0:
				break;
			// 1 points:
			case 1:
				MeshFromPoints(square.centreLeft, square.centreBottom, square.bottomLeft);
				break;
			case 2:
				MeshFromPoints(square.bottomRight, square.centreBottom, square.centreRight);
				break;
			case 4:
				MeshFromPoints(square.topRight, square.centreRight, square.centreTop);
				break;
			case 8:
				MeshFromPoints(square.topLeft, square.centreTop, square.centreLeft);
				break;

			// 2 points:
			case 3:
				MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
				break;
			case 6:
				MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
				break;
			case 9:
				MeshFromPoints(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
				break;
			case 12:
				MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreLeft);
				break;
			case 5:
				MeshFromPoints(square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
				break;
			case 10:
				MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
				break;

			// 3 point:
			case 7:
				MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
				break;
			case 11:
				MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
				break;
			case 13:
				MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
				break;
			case 14:
				MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
				break;

			// 4 point:
			case 15:
				MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
				checkedVertices.Add(square.topLeft.vertexIndex);
				checkedVertices.Add(square.topRight.vertexIndex);
				checkedVertices.Add(square.bottomRight.vertexIndex);
				checkedVertices.Add(square.bottomLeft.vertexIndex);
				break;
			default:
				break;
		}

	}

	/// <summary>
    /// Creates Triangles with vertexIndex points depending on the amount of supplied nodes
    /// </summary>
    /// <param name="points"></param>
	void MeshFromPoints(params Node[] points) {
		AssignVertices(points);

		if (points.Length >= 3)
			CreateTriangle(points[0], points[1], points[2]);
		if (points.Length >= 4)
			CreateTriangle(points[0], points[2], points[3]);
		if (points.Length >= 5)
			CreateTriangle(points[0], points[3], points[4]);
		if (points.Length >= 6)
			CreateTriangle(points[0], points[4], points[5]);

	}

	/// <summary>
    /// 
    /// </summary>
    /// <param name="points"></param>
	void AssignVertices(Node[] points) {
		for (int i = 0; i < points.Length; i++) {
			if (points[i].vertexIndex == -1) {
				points[i].vertexIndex = vertices.Count;
				vertices.Add(points[i].position);
			}
		}
	}

	/// <summary>
    /// Creates a triangle using its vertex Indexes, then adds it to the global dictionary
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
	void CreateTriangle(Node a, Node b, Node c) {
		triangles.Add(a.vertexIndex);
		triangles.Add(b.vertexIndex);
		triangles.Add(c.vertexIndex);

		Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
		AddTriangleToDictionary(triangle.vertexIndexA, triangle);
		AddTriangleToDictionary(triangle.vertexIndexB, triangle);
		AddTriangleToDictionary(triangle.vertexIndexC, triangle);
	}

	/// <summary>
    /// Adds a given triangle to a dictionary where all triangles are bound to a vertexIndex
    /// </summary>
    /// <param name="vertexIndexKey"></param>
    /// <param name="triangle"></param>
	void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle) {
		if (triangleDictionary.ContainsKey(vertexIndexKey)) {
			triangleDictionary[vertexIndexKey].Add(triangle);
		}
		else {
			List<Triangle> triangleList = new List<Triangle>();
			triangleList.Add(triangle);
			triangleDictionary.Add(vertexIndexKey, triangleList);
		}
	}

	/// <summary>
    /// Creates a list containing all mesh outlines in the whole map
    /// </summary>
	void CalculateMeshOutlines() {
		for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex++) {
			if (!checkedVertices.Contains(vertexIndex)) {
				int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
				if (newOutlineVertex != -1) {
					checkedVertices.Add(vertexIndex);

					List<int> newOutline = new List<int>();
					newOutline.Add(vertexIndex);
					outlines.Add(newOutline);
					FollowOutline(newOutlineVertex, outlines.Count - 1);
					outlines[outlines.Count - 1].Add(vertexIndex);
				}
			}
		}
	}

	/// <summary>
    /// Initiates iterating through an outline
    /// </summary>
    /// <param name="vertexIndex"></param>
    /// <param name="outlineIndex"></param>
	void FollowOutline(int vertexIndex, int outlineIndex) {
		outlines[outlineIndex].Add(vertexIndex);
		checkedVertices.Add(vertexIndex);
		int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

		if (nextVertexIndex != -1) {
			FollowOutline(nextVertexIndex, outlineIndex);
		}
	}

	/// <summary>
    /// Finds vertexes that border the same outline 
    /// </summary>
    /// <param name="vertexIndex"></param>
    /// <returns></returns>
	int GetConnectedOutlineVertex(int vertexIndex) {
		List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

		for (int i = 0; i < trianglesContainingVertex.Count; i++) {
			Triangle triangle = trianglesContainingVertex[i];

			for (int j = 0; j < 3; j++) {
				int vertexB = triangle[j];
				if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB)) {
					if (IsOutlineEdge(vertexIndex, vertexB)) {
						return vertexB;
					}
				}
			}
		}

		return -1;
	}

	/// <summary>
	/// Used to check wether a given vertex "B" coordinate has neighbors
	/// </summary>
	/// <param name="vertexA"></param>
	/// <param name="vertexB"></param>
	/// <returns></returns>
	bool IsOutlineEdge(int vertexA, int vertexB) {
		List<Triangle> trianglesContainingVertexA = triangleDictionary[vertexA];
		int sharedTriangleCount = 0;

		for (int i = 0; i < trianglesContainingVertexA.Count; i++) {
			if (trianglesContainingVertexA[i].Contains(vertexB)) {
				sharedTriangleCount++;
				if (sharedTriangleCount > 1) {
					break;
				}
			}
		}
		return sharedTriangleCount == 1;
	}

	/// <summary>
    /// Finds objects using their predetermined identifier then returns a map of 1s & 0s where the object lies
    /// </summary>
    /// <param name="inMap"></param>
    /// <param name="objectIdentifier"></param>
    /// <returns></returns>
	int[,] IsolateMapObjects(int[,] inMap, int objectIdentifier) {
		int[,] outMap = new int[inMap.GetLength(0), inMap.GetLength(1)];
		for (int x = 0; x < inMap.GetLength(0); x++) {
			for (int y = 0; y < inMap.GetLength(1); y++) {
				if (inMap[x, y] == objectIdentifier) {
					outMap[x, y] = 1;
					
				} else { outMap[x, y] = 0; }
			}
		}
		return outMap;
	}
	
	// maybe call with playerrefrence
	void grabTextureFromPlayer() {
		Material	roofMat,
					floorMat,
					wallMat;

		GameObject player = GameObject.FindGameObjectWithTag("Player");
		//Requests material data from
		(roofMat, floorMat, wallMat) =	player.GetComponent<MapTextureHandler>().RequestMaterialFromPlayer(
										player.GetComponent<PlayerController>().currentStage, 
										transform.GetComponent<MapGenerator>().bossLevel);

		if (floorMat == null || roofMat == null || wallMat == null) {
			return;
        }

		//Assigns the texture to respective gameobject
		floor.transform.GetComponent<Renderer>().material		= floorMat;
		outerRoof.transform.GetComponent<Renderer>().material	= roofMat;
		outerWall.transform.GetComponent<Renderer>().material	= wallMat;

	}

	/// <summary>
    /// Struct containing custom Triangle data
    /// </summary>
	struct Triangle {
		public int vertexIndexA;
		public int vertexIndexB;
		public int vertexIndexC;
		int[] vertices;

		public Triangle(int a, int b, int c) {
			vertexIndexA = a;
			vertexIndexB = b;
			vertexIndexC = c;

			vertices = new int[3];
			vertices[0] = a;
			vertices[1] = b;
			vertices[2] = c;
		}

		public int this[int i] {
			get {
				return vertices[i];
			}
		}


		public bool Contains(int vertexIndex) {
			return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
		}
	}

	/// <summary>
    /// Square grid manages the coordinates of the whole mesh grid
    /// </summary>
	public class SquareGrid {
		public Square[,] squares;

		public SquareGrid(int[,] map, float squareSize, int depth) {
			int nodeCountX = map.GetLength(0);
			int nodeCountY = map.GetLength(1);
			float mapWidth = nodeCountX * squareSize;
			float mapHeight = nodeCountY * squareSize;
			ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];
			for (int x = 0; x < nodeCountX; x++) {
				for (int y = 0; y < nodeCountY; y++) {
					Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, depth, -mapHeight / 2 + y * squareSize + squareSize / 2);
					controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
				}
			}

			squares = new Square[nodeCountX - 1, nodeCountY - 1];
			for (int x = 0; x < nodeCountX - 1; x++) {
				for (int y = 0; y < nodeCountY - 1; y++) {
					squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
				}
			}

		}
	}

	/// <summary>
    /// Square hold individual information for each square in the grid
    /// </summary>
	public class Square {

		public ControlNode topLeft, topRight, bottomRight, bottomLeft;
		public Node centreTop, centreRight, centreBottom, centreLeft;
		public int configuration;

		public Square(ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft) {
			topLeft = _topLeft;
			topRight = _topRight;
			bottomRight = _bottomRight;
			bottomLeft = _bottomLeft;

			centreTop = topLeft.right;
			centreRight = bottomRight.above;
			centreBottom = bottomLeft.right;
			centreLeft = bottomLeft.above;

			if (topLeft.active)
				configuration += 8;
			if (topRight.active)
				configuration += 4;
			if (bottomRight.active)
				configuration += 2;
			if (bottomLeft.active)
				configuration += 1;
		}

	}

	/// <summary>
    /// Node is the baseclass for Control Node, used to contain vertexIndex and xyz coordinate of a corner
    /// </summary>
	public class Node {
		public Vector3 position;
		public int vertexIndex = -1;

		public Node(Vector3 _pos) {
			position = _pos;
		}
	}

	/// <summary>
    /// Subclass of Node, holds xyz for a "square" side, also wether or not its connected to another square
    /// </summary>
	public class ControlNode : Node {

		public bool active;
		public Node above, right;

		public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
			active = _active;
			above = new Node(position + Vector3.forward * squareSize / 2f);
			right = new Node(position + Vector3.right * squareSize / 2f);
		}

	}
}
