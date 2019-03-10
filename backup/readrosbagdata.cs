using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.UI;

//librosbagreadposeplugin.so
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class readrosbagdata : MonoBehaviour {
	[DllImport("librosbagreadposeplugin.so", CallingConvention = CallingConvention.Cdecl)]
	private static extern void initplugin(string str);
	[DllImport("librosbagreadposeplugin.so")]
	private static extern void getpose(float[] poses);
	[DllImport("librosbagreadposeplugin.so")]
	private static extern void getNewImage (int[] imgBuf);
	[DllImport("librosbagreadposeplugin.so")]
	private static extern void getNewWallQuads (float[] quadBuf, int[] numVertices);
	[DllImport("librosbagreadposeplugin.so")]
	private static extern void getNewBoxQuads(float[] boxBuf, int[] numVertices);
	[DllImport("librosbagreadposeplugin.so")]
	private static extern int getwidth();
	[DllImport("librosbagreadposeplugin.so")]
	private static extern int getheight();

	//rgb vars	
	int[] img;
	byte[] myImg;
	int width;
	int height;
	Texture2D tex;
	float[] quadBuf;
	float[] boxBuf;
	float[] poseBuf;
	int[] numVertices;

	//public GameObject cube;
	//public GameObject[] quads;
	public GameObject showobj;
	public Material objectMat;
	public Material wallMat;
	float []pose;
	private Vector3 campos;
    private Vector3 sp,ep,angles;
	private Quaternion camrot;
	Quaternion camrotation;
	Matrix4x4 mymat;
	Matrix4x4 mymatnew;

	Vector3[] quadVertices; 
	Vector3[] boxVertices; 
	Vector3 centerPos;
	Quaternion rot;

	void Start () {
		width = getwidth ();
		height = getheight ();
		img = new int[width*height*3];
		myImg = new byte[width*height*4];
		quadBuf = new float[3000];
		boxBuf = new float[3000];
		numVertices = new int[2];
		numVertices[0] = numVertices[1] = 0;

		pose=new float[7];//plane equation coefficients and plane position
		pose[0] = 0;
    	pose[1] = 0;
  		pose[2] = 0;
    	pose[3] = 1;
    	pose[4] = 0;
    	pose[5] = 0;
    	pose[6] = 0;
		camrotation = new Quaternion(0,0,0,1);
		mymat=new Matrix4x4();
		sp = new Vector3 (0, 0, 0);
		ep = new Vector3 (1, 1, 1);
		angles = new Vector3 (0, 0, 0);
		string path = Directory.GetCurrentDirectory();
		Debug.Log(path);
		initplugin(path);
		tex = new Texture2D(width,height, TextureFormat.RGBA32, false);
		centerPos.x = 0;
		centerPos.y = 0;
		centerPos.z = 0;
		rot.x = 0;
		rot.y = 0;
		rot.z = 0;
		rot.w = 1;
		quadVertices = new Vector3[100];
		boxVertices = new Vector3[1000];
	}
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.H)){
			transform.LookAt (campos);
			transform.position = centerPos;
		}
		getpose (pose);
		getNewImage (img);
		/*getQuadVertices ();
		getBoxVertices ();
		if (numVertices [0] == 0)
			return;
		if (numVertices [1] == 0)
			return;
		DestroyAllCubeGameObjects ();
		createWallCubes ();
		createObjectCubes ();
		campos.x = pose [0];
		campos.y = pose [1];
		campos.z = pose [2];
		camrot.x = pose [4];
		camrot.y = pose [5];
		camrot.z = pose [6];
		camrot.w = pose [3];
		campos.x= -pose[1];//camposition.x;//whatever the camera x position is
		campos.y= pose[2];//camposition.y; //whatever the camera y position is
		campos.z= pose[0];//camposition.z;//whatever the camera z position is

		camrotation.x = -pose[5];
		camrotation.y = -pose[6];
		camrotation.z = -pose[4];
		camrotation.w = pose[3];
		if(camrotation.w > -0.000001 && camrotation.w<0.000001)
		camrotation.w =1;
		float ll = Mathf.Sqrt((camrotation.x*camrotation.x + camrotation.y*camrotation.y+
		camrotation.z*camrotation.z + camrotation.w*camrotation.w));
		camrotation.x = camrotation.x/ll;
		camrotation.y = camrotation.y/ll;
		camrotation.z = camrotation.z/ll;
		camrotation.w = camrotation.w/ll;
                
		//Matrix4x4 mymat=new Matrix4x4();

		mymat.SetTRS (sp, camrotation, ep);

		mymatnew.m00 = mymat.m02;
		mymatnew.m10 = mymat.m12;
		mymatnew.m20 = mymat.m22;
		mymatnew.m01 = mymat.m00;
		mymatnew.m11 = mymat.m10;
		mymatnew.m21 = mymat.m20;
		mymatnew.m02 = mymat.m01;
		mymatnew.m12 = mymat.m11;
		mymatnew.m22 = mymat.m21;

		//mymatnew.SetTRS(Vector3.up, Quaternion.Euler(Vector3.up * 45.0f), Vector3.one * 2.0f);
		// Extract new local rotation
		Quaternion camrotationnew  = Quaternion.LookRotation(
			mymatnew.GetColumn(2),
			mymatnew.GetColumn(1)
			);
		angles.x = camrotationnew.eulerAngles.x;   // 100t5 add -
		angles.y = camrotationnew.eulerAngles.z-90;
		angles.z = -camrotationnew.eulerAngles.y-90; //100t5 add -
		camrotationnew.eulerAngles = angles;
		camrot.x= camrotationnew.x;//whatever the camera x rotation is
		camrot.y= camrotationnew.y;//whatever the camera y rotation is
		camrot.z= camrotationnew.z;//whatever the camera z rotation is
		camrot.w= camrotationnew.w;//whatever the camera w rotation is
		transform.position = campos;
		transform.rotation = camrot;*/
		convertdata ();
		tex.LoadRawTextureData(myImg);
		tex.Apply();
		showobj.GetComponent<Renderer>().material.mainTexture = tex;
	}

	void getQuadVertices()
	{
		getNewWallQuads (quadBuf, numVertices);
		centerPos.x = 0;
		centerPos.y = 0;
		centerPos.z = 0;
		for (int i=0; i < numVertices[0]; i++) {
			quadVertices [i].x = -quadBuf [3 * i + 1];
			quadVertices [i].y = quadBuf [3 * i + 2];
			quadVertices [i].z = quadBuf [3 * i];
			centerPos += quadVertices [i];
		}
		centerPos = centerPos / numVertices [0];
	}

	void getBoxVertices()
	{
		getNewBoxQuads (boxBuf, numVertices);
		for (int i=0; i < numVertices[1]; i++) {
			boxVertices [i].x = -boxBuf [3 * i + 1];
			boxVertices [i].y = boxBuf [3 * i + 2];
			boxVertices [i].z = boxBuf [3 * i];
		}
	}

	public void DestroyAllCubeGameObjects()
	{
		GameObject[] GameObjects = (FindObjectsOfType<GameObject>() as GameObject[]);
		for (int i = 0; i < GameObjects.Length; i++)
		{
			if (GameObjects [i].name == "Cube")
				Destroy(GameObjects[i]);
		}
	}

	void createWallCubes()
	{
		int i = 0;
		Vector3[] vertices = new Vector3[4];
		while (i < numVertices[0]) 
		{
			vertices [0] = quadVertices [i];
			vertices [1] = quadVertices [i+1];
			vertices [2] = quadVertices [i+2];
			vertices [3] = quadVertices [i+3];
			i+=4;
			Vector3 w = vertices [1] - vertices[0];
			Vector3 h = vertices [3] - vertices[0];
			//quads[c] = helper.CreateQuad (3, 3, wallMat);
			float quad_width = w.magnitude;
			float quad_height = h.magnitude;
			float depth = 0.2f;
			GameObject go = helper.CreateCube (quad_width, quad_height, depth, wallMat);
			w = w.normalized;
			h = h.normalized;
			Vector3 forward = Vector3.Cross (w, h).normalized;
			go.transform.rotation = Quaternion.LookRotation (forward, h);
			go.transform.position = vertices [0];
		}
	}

	void createObjectCubes()
	{
		int i = 0;
		Vector3[] vertices = new Vector3[5];
		while (i < numVertices[1]) 
		{
			vertices [0] = boxVertices [i];
			vertices [1] = boxVertices [i+1];
			vertices [2] = boxVertices [i+2];
			vertices [3] = boxVertices [i+3];
			vertices [4] = boxVertices [i+4];
			i+=8;
			Vector3 w = vertices [1] - vertices[0];
			Vector3 h = vertices [3] - vertices[0];
			Vector3 d = vertices [4] - vertices[0];
			float box_width = w.magnitude;
			float box_height = h.magnitude;
			float depth = d.magnitude;
			GameObject go = helper.CreateCube (box_width, box_height, depth, objectMat);
			w = w.normalized;
			h = h.normalized;
			go.transform.rotation = Quaternion.LookRotation (d.normalized, h);
			//Vector3 forward = Vector3.Cross (w, h).normalized;
			//go.transform.rotation = Quaternion.LookRotation (forward, h);
			go.transform.position = vertices [0];
		}
	}

	void convertdata()
	{
		//rgb
		int fillcounter=0;
		for(int i=0;i<width*height*4;i++)
		{
			if((i+1)%4==0)
			{
				myImg[i]=(byte)255;
				continue;
			}
			myImg[i]=(byte)img[fillcounter++];//0;
		}
	}

}
