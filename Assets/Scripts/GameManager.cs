using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace PhotonServerDemo
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public GameObject playerPrefab;
        public bool autoSpawnObjects = true;
        public Stack<GameObject> spawnedObjects = new Stack<GameObject>();
        protected int spawnedAsActorId;
        public int playerTTL = -1;
        public byte MaxPlayers = 4;

        public override void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        void Start()
        {
            Debug.Log("GameManager.Start() will now call: PhotonNetwork.ConnectUsingSettings().");

            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster() was called by PUN. This client is now connected to Master Server in region [" + PhotonNetwork.CloudRegion +
                "] and can join a room. Calling: PhotonNetwork.JoinRandomRoom();");

            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("OnJoinedLobby(). This client is now connected to Relay in region [" + PhotonNetwork.CloudRegion + "]. This script now calls: PhotonNetwork.JoinRandomRoom();");

            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available in region [" + PhotonNetwork.CloudRegion + "], so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");

            RoomOptions roomOptions = new RoomOptions() { MaxPlayers = this.MaxPlayers };
            if (playerTTL >= 0)
                roomOptions.PlayerTtl = playerTTL;

            PhotonNetwork.CreateRoom(null, roomOptions, null);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("OnDisconnected(" + cause + ")");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room in region [" + PhotonNetwork.CloudRegion + "]. Game is now running.");

            if (autoSpawnObjects && !PhotonNetwork.LocalPlayer.HasRejoined)
            {
                SpawnObjects();
            }
        }

        public void SpawnObjects()
        {
            Vector3 spawnPos = Random.insideUnitCircle * 3;
            int id = PhotonNetwork.LocalPlayer.ActorNumber;
            var newObj = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity, 0);
            spawnedObjects.Push(newObj);
        }

        public void DespawnObjects(bool localOnly)
        {
            while (spawnedObjects.Count > 0)
            {
                var go = spawnedObjects.Pop();
                if (go)
                {
                    if (localOnly)
                        Object.Destroy(go);
                    else
                        PhotonNetwork.Destroy(go);
                }
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message) { }

        public override void OnLeftRoom() { }

        public override void OnCreatedRoom() { }

        public override void OnCreateRoomFailed(short returnCode, string message) { }

        public override void OnFriendListUpdate(List<FriendInfo> friendList) { }

    }
}
