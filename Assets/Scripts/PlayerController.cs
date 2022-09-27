using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace PhotonServerDemo
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable, IOnEventCallback
    {
        public float speed;
        public static GameObject localPlayerInstance;
        private Transform m_transform;
        private Material m_material;
        private int m_health = 100;
        private const byte DamageEventCode = 1;


        private void Awake()
        {
            if (photonView.IsMine)
            {
                localPlayerInstance = gameObject;
            }

            m_transform = transform;
            m_material = m_transform.GetChild(0).GetComponent<Renderer>().material;
        }

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
            switch (photonView.ControllerActorNr)
            {
                case 1: m_material.color = Color.red; break;
                case 2: m_material.color = Color.blue; break;
            }

            UIController.instance.UpdateHealth(photonView.ControllerActorNr, m_health);

            if (photonView.IsMine)
            {
                UIController.instance.AttachToHealButton(HealCallback);
                UIController.instance.AttachToDamageButton(DamageCallback);
            }
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");

                m_transform.position += (Vector3)(speed * Time.deltaTime * new Vector2(h, v));
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                Vector3 pos = m_transform.position;
                stream.SendNext(pos);
            }
            else
            {
                Vector3 pos = (Vector3)stream.ReceiveNext();
                m_transform.position = pos;
            }
        }

        [PunRPC]
        public void Heal()
        {
            m_health += 5;
            UIController.instance.UpdateHealth(photonView.ControllerActorNr, m_health);
        }

        public void Damage()
        {
            m_health -= 5;
            UIController.instance.UpdateHealth(photonView.ControllerActorNr, m_health);
        }
        public void HealCallback()
        {
            photonView.RPC("Heal", RpcTarget.AllViaServer);
        }

        public void DamageCallback()
        {
            object[] content = new object[] { photonView.ControllerActorNr };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(DamageEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        }

        public void OnEvent(EventData photonEvent)
        {
            if(photonEvent.Code == DamageEventCode)
            {
                object[] data = (object[])photonEvent.CustomData;

                if ((int)data[0] != photonView.ControllerActorNr)
                {
                    Damage();
                }
            }
        }
    }
}
