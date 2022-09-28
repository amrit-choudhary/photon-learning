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
        public string pluginName;

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
            PhotonNetwork.SerializationRate = 60;
            PhotonNetwork.SendRate = 60;
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.Plugins = new string[] { pluginName };
            PhotonNetwork.JoinOrCreateRoom("GameRoom", roomOptions, TypedLobby.Default);
        }

        public override void OnJoinedLobby()
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.Plugins = new string[] { pluginName };
            PhotonNetwork.JoinOrCreateRoom("GameRoom", roomOptions, TypedLobby.Default);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
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
