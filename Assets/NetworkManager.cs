using System.Collections;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    public Camera stanbyCamera;
    private SpawnSpot[] spawnSpots;

    void Start () {
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        Connect();

    }
	
	void Connect () {
        PhotonNetwork.ConnectUsingSettings("0.0");
	}

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinedFailed");
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom()
    {
        Debug.Log("OnJoinRoom");
        SpawnMyPlayer();
    }

    void SpawnMyPlayer()
    {
        //disable standby camera
        stanbyCamera.enabled = false;
        //place player in spawn spot
        SpawnSpot mySpawnSpot = spawnSpots[0];
        GameObject myPlayerGO = (GameObject)PhotonNetwork.Instantiate("Player", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
        //activate player scripts and camera
        ((MonoBehaviour)myPlayerGO.GetComponent("RigidbodyFirstPersonController")).enabled = true;
        myPlayerGO.transform.FindChild("PlayerFirstPersonCam").gameObject.SetActive(true);
    }
}
