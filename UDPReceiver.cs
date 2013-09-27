using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceiver : MonoBehaviour {
	Thread receiveThread;
	UdpClient client;
	public const int port = 9090;
	
	public string lastReceivedUPDpacket = "";
	public string allReceivedUPDpacket = "";
	
	private GameObject marker;
	
	public static int posX = 0;
	public static int posY = 0;

	private System.Object thisLock = new System.Object();

	// Use this for initialization
	void Start () {
		receiveThread = new Thread(new ThreadStart(this.ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
		
		marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		marker.name = "MidPtSph";
		marker.tag = "HandMarker";
		marker.AddComponent("TestObjCollider");
		marker.AddComponent<Rigidbody>();
		marker.collider.isTrigger = false;
		marker.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
	}
	
	private void ReceiveData()
	{
		client = new UdpClient(port);
		Debug.Log("thread");
		
		while(true)
		{
			try
			{
				IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
				byte[] data = client.Receive(ref ip);
				
				string content = Encoding.UTF8.GetString(data);
				lock(thisLock){
				//Debug.Log(content);
				lastReceivedUPDpacket = content;
				allReceivedUPDpacket += content;
				
				string[] array = content.Split(',');
				//Debug.Log(array[0] + "   **  " + array[1]);
				posX = (int)Convert.ToDouble(array[0]);
				posY = (int)Convert.ToDouble(array[1]);
				}
				
			}
			catch(Exception e)
			{
				Debug.Log(e.ToString());
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		//marker.transform.position = new Vector3(posX, posY, 0);
		//Debug.Log(posX + "////" + posY);
		//Debug.Log(content);
		marker.transform.position = new Vector3(posX, posY, 0);
	}
	
	void OnDisable()
	{
		if(receiveThread != null)
			receiveThread.Abort();
		client.Close();
		Debug.Log("Connection Lost.");
	}
}

