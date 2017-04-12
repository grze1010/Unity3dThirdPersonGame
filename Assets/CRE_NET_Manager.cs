using System.Collections;
using UnityEngine;

public class CRE_NET_Manager : MonoBehaviour {

    public GameObject stanbyCamera;
    private CRE_SpawnSpot[] spawnSpots;
    private int mySpotIndex = 0;

    void Start () {
        spawnSpots = GameObject.FindObjectsOfType<CRE_SpawnSpot>();
        Connect();
    }

    void Connect () {
        PhotonNetwork.ConnectUsingSettings("0.1");
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
        //cre_todo wait for second player
        SpawnMyPlayer();
    }

    void SpawnMyPlayer()
    {
        //disable standby camera
        stanbyCamera.SetActive(false);
        //place player in spawn spot
        CRE_SpawnSpot mySpawnSpot = spawnSpots[mySpotIndex];
        GameObject myPlayerGO = (GameObject)PhotonNetwork.Instantiate("PlayerController", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
        //activate player scripts and camera
        ((MonoBehaviour)myPlayerGO.GetComponent("CRE_CTRL_PlayerMovement")).enabled = true;
        //((MonoBehaviour)myPlayerGO.GetComponent("PlayerShooting")).enabled = true;
        myPlayerGO.transform.FindChild("CameraAnchor").gameObject.SetActive(true);
        myPlayerGO.transform.FindChild("CameraTarget").gameObject.SetActive(true);
    }

}
