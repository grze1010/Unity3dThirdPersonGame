using System.Collections;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    public Camera stanbyCamera;
    private Vector3 spawnLocation;

    void Start () {
        Connect();
        spawnLocation = new Vector3(1, 1);

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
        PhotonNetwork.Instantiate("Player", spawnLocation, Quaternion.identity, 0);
        stanbyCamera.enabled = false;
    }
}
