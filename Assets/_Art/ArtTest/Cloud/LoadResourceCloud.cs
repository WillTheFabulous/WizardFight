using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadResourceCloud : MonoBehaviour
{
	class MeshData
	{
	    public List<int> VTX;
	    public List<int> IDX;
	    public List<Vector3> Position;
	    public List<Color> VertexColor;
	    public List<Vector2> Texcoord0;
	    public List<Vector2> Texcoord1;
	    public List<Vector2> Texcoord2;
	    public List<Vector2> Texcoord3;
	    public MeshData()
	    {
	        this.VTX = new List<int>();
	        this.IDX = new List<int>();
	        this.Position = new List<Vector3>();
	        this.VertexColor = new List<Color>();
	        this.Texcoord0 = new List<Vector2>();
	        this.Texcoord1 = new List<Vector2>();
	        this.Texcoord2 = new List<Vector2>();
	        this.Texcoord3 = new List<Vector2>();
	    }
	}

	public Material material0;
	public Material material1;
	public Material material2;
	public GameObject go0;
	public GameObject go1;
	public GameObject go2;
	void Parse(string path, GameObject go, Material mat)
	{
	    string[] lines = File.ReadAllLines(path);
	    //print(lines[0].Length);

	    MeshData meshData = new MeshData();

	    for (int i = 1; i < lines.Length; i++)
	    {
	        var line = lines[i];
	        string[] words = line.Split(',');

	        int index = 0;
	        meshData.VTX.Add(int.Parse(words[index++]));
	        meshData.IDX.Add(int.Parse(words[index++]));
	        meshData.Position.Add(new Vector3(float.Parse(words[index++]), float.Parse(words[index++]), float.Parse(words[index++])));
	        meshData.VertexColor.Add(new Color(float.Parse(words[index++]), float.Parse(words[index++]), float.Parse(words[index++]), float.Parse(words[index++])));
	        meshData.Texcoord0.Add(new Vector2(float.Parse(words[index++]), float.Parse(words[index++])));
	        meshData.Texcoord1.Add(new Vector2(float.Parse(words[index++]), float.Parse(words[index++])));
	        meshData.Texcoord2.Add(new Vector2(float.Parse(words[index++]), float.Parse(words[index++])));
	        meshData.Texcoord3.Add(new Vector2(float.Parse(words[index++]), float.Parse(words[index++])));
	    }
	    // var go = new GameObject(path);
	    // go.transform.position = Vector3.zero;
	    MeshFilter mf = go.AddComponent<MeshFilter>();
	    MeshRenderer mr = go.AddComponent<MeshRenderer>();
	    Mesh mesh = new Mesh();
	    mesh.vertices = meshData.Position.ToArray();
	    mesh.colors = meshData.VertexColor.ToArray();
	    mesh.uv = meshData.Texcoord0.ToArray();
	    mesh.uv2 = meshData.Texcoord1.ToArray();
	    mesh.uv3 = meshData.Texcoord2.ToArray();
	    mesh.uv4 = meshData.Texcoord3.ToArray();
	    mesh.triangles = meshData.VTX.ToArray();
	    mf.mesh = mesh;
	    mr.material = mat;
	}
    // Start is called before the first frame update
    void Start()
    {
        Parse("Assets/_Art/ArtTest/Cloud/CloudLow.csv", go0, material0);
        Parse("Assets/_Art/ArtTest/Cloud/CloudMiddle.csv", go1, material1);
        Parse("Assets/_Art/ArtTest/Cloud/CloudTop.csv", go2, material2);
    }
}

