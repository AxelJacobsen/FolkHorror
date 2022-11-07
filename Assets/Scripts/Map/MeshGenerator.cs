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

	public bool generateObjects;
	private int wallHeight;
	List<Vector3> vertices;
	List<int> triangles;

	Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
	List<List<int>> outlines = new List<List<int>>();
	HashSet<int> checkedVertices = new HashSet<int>();

	// meshType 0 = standard, 1 = bush, 2 = outerwall , 3 = floor
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
		for (int x = 0; x < squareGrid.squares.GetLength(0); x++) {
			for (int y = 0; y < squareGrid.squares.GetLength(1); y++) {
				TriangulateSquare(squareGrid.squares[x, y]);
			}
		}

		Mesh mesh = new Mesh();

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();

		int tileAmount = 10;
		Vector2[] uvs = new Vector2[vertices.Count];
		for (int i = 0; i < vertices.Count; i++) {
			float percentX = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, vertices[i].x) * tileAmount;
			float percentY = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, vertices[i].z) * tileAmount;
			uvs[i] = new Vector2(percentX, percentY);
		}
		mesh.uv = uvs;

		if (meshType == 3) {
			floor.mesh = mesh;
			MeshCollider floorCollider = floor.gameObject.GetComponent<MeshCollider>();
			floorCollider.sharedMesh = mesh;
		} else {
			CreateWallMesh(meshType);
		}

		if (meshType == 2) {
			outerRoof.mesh = mesh;
        }
	}

	void CreateWallMesh(int meshType) {
		CalculateMeshOutlines();

		List<Vector3> wallVertices = new List<Vector3>();
		List<int> wallTriangles = new List<int>();
		Mesh wallMesh = new Mesh();
		int treeChance = 0;
		foreach (List<int> outline in outlines) {
			if (generateObjects) {
				treeChance = SpawnObjectHandler(outline, treeChance, meshType);
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
						print("Uses default");
						treeWall.mesh = wallMesh;
						MeshCollider wallCollider = treeWall.gameObject.GetComponent<MeshCollider>();
						wallCollider.sharedMesh = wallMesh;
					} break;
		}
	}

	int SpawnObjectHandler(List<int> outline, int treeChance, int spawnObject) {
		List<Vector2> polyList = new List<Vector2>();
		foreach (int vertex in outline) {
			polyList.Add(new Vector2(vertices[vertex].x, vertices[vertex].z));
		}
		Vector2[] poly = polyList.ToArray();
		int[] largestOutline = {0,0};
		if (spawnObject != 1) { spawnObject = 0; }
		//Spawn along outline edge to secure border
		for (int o = 0; o < polyList.Count; o++) {
			if (1000 < polyList.Count) { break; }
			treeChance += Random.Range(1, 3);
			if (treeChance < 10) { continue; }
			objSpawner.SpawnObject(new Vector3(polyList[o].x, 0, polyList[o].y), spawnObject);
			treeChance = 0;
		}
		//Spawn trees randomly inside border
		for (int t = 0; t < outline.Count * 2 / 5; t++) {
			//Stows largest Outline to spawn exit and entrance
			if (largestOutline[0] < outline.Count) {
				largestOutline[0] = outline.Count;
				largestOutline[0] = t;
			}
			if (1000 < outline.Count) { break; }
			Vector2 pos = Funcs.GetRandomPointInPolygon(poly);
			objSpawner.SpawnObject(new Vector3(pos.x, 0, pos.y), spawnObject);
		}

		//Spawns entrance and exit
		if (1000 < polyList.Count) { //JANKY JEG VET :^) SKAL FIKSE og legge til limiter på spawn location
			Vector2 pos;
			pos = Funcs.GetRandomPointInPolygon(poly);
			objSpawner.SpawnObject(new Vector3(pos.x, 0, pos.y), 5);
			pos = Funcs.GetRandomPointInPolygon(poly);
			objSpawner.SpawnObject(new Vector3(pos.x, 0, pos.y), 6);
		}
		return treeChance;
	}

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

	void AssignVertices(Node[] points) {
		for (int i = 0; i < points.Length; i++) {
			if (points[i].vertexIndex == -1) {
				points[i].vertexIndex = vertices.Count;
				vertices.Add(points[i].position);
			}
		}
	}

	void CreateTriangle(Node a, Node b, Node c) {
		triangles.Add(a.vertexIndex);
		triangles.Add(b.vertexIndex);
		triangles.Add(c.vertexIndex);

		Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
		AddTriangleToDictionary(triangle.vertexIndexA, triangle);
		AddTriangleToDictionary(triangle.vertexIndexB, triangle);
		AddTriangleToDictionary(triangle.vertexIndexC, triangle);
	}

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

	void FollowOutline(int vertexIndex, int outlineIndex) {
		outlines[outlineIndex].Add(vertexIndex);
		checkedVertices.Add(vertexIndex);
		int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

		if (nextVertexIndex != -1) {
			FollowOutline(nextVertexIndex, outlineIndex);
		}
	}

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

	public class Node {
		public Vector3 position;
		public int vertexIndex = -1;

		public Node(Vector3 _pos) {
			position = _pos;
		}
	}

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
