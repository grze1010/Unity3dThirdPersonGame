using System.Collections;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    public GameObject stanbyCamera;
    private SpawnSpot[] spawnSpots;
    private int mySpotIndex = 0;

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
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        mySpotIndex = 1;
        PhotonNetwork.CreateRoom(null,options,null);
    }

    void OnJoinedRoom()
    {
        Debug.Log("OnJoinRoom");
        SpawnMyPlayer();
    }

    void SpawnMyPlayer()
    {
        //disable standby camera
        stanbyCamera.SetActive(false);
        //place player in spawn spot
        SpawnSpot mySpawnSpot = spawnSpots[mySpotIndex];
        GameObject myPlayerGO = (GameObject)PhotonNetwork.Instantiate("PlayerController", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
        //activate player scripts and camera
        ((MonoBehaviour)myPlayerGO.GetComponent("PlayerMovementController")).enabled = true;
        myPlayerGO.transform.FindChild("PlayerCamera").gameObject.SetActive(true);
    }

}
