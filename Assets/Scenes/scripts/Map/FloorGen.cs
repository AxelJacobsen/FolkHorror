using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGen : MonoBehaviour {

	public Material floorTexture;

	private int localWidth;
	private int localHeight;

	public void GenerateFloor(int width, int height) {
		localWidth = width;
		localHeight = height;
		MeshFilter floorFilter = GetComponent<MeshFilter>();
		floorFilter.mesh = GenerateFloorMesh();
		MeshCollider wallCollider = floorFilter.gameObject.AddComponent<MeshCollider>();
		wallCollider.sharedMesh = floorFilter.mesh;
		print("Function called");
	}

	private Mesh GenerateFloorMesh() {
		Mesh m = new Mesh();
		m.name = "Floor_Plane";
		m.vertices = new Vector3[] {
			new Vector3(-localWidth, 0.01f, -localHeight),
			new Vector3(localWidth, 0.01f, -localHeight),
			new Vector3(localWidth, 0.01f, localHeight),
			new Vector3(-localWidth, 0.01f, localHeight)
		};
		m.uv = new Vector2[] {
			new Vector2 (0, 0),
			new Vector2 (0, 1),
			new Vector2(1, 1),
			new Vector2 (1, 0)
		};
		m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
		m.RecalculateNormals();

		return m;
	}
}
