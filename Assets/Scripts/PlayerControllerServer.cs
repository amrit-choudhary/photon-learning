using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace PhotonServerDemo
{
    public class PlayerControllerServer : MonoBehaviourPunCallbacks, IPunObservable, IOnEventCallback
    {
        public static GameObject localPlayerInstance;
        private Transform m_transform;
        private Material m_material;
        private float m_h, m_v;
        private float m_speed;

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
                m_h = Input.GetAxis("Horizontal");
                m_v = Input.GetAxis("Vertical");

                m_transform.position += (Vector3)(m_speed * Time.deltaTime * new Vector2(m_h, m_v));
            }
        }

        private void HealCallback()
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonEventCodes.HealEventCode, null,
                raiseEventOptions, SendOptions.SendReliable);
        }

        private void DamageCallback()
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PhotonEventCodes.DamageEventCode, null,
                raiseEventOptions, SendOptions.SendReliable);
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

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == PhotonEventCodes.InitPlayerEventCode)
            {
                var data = photonEvent.Parameters;
                if ((int)data[0] == photonView.ControllerActorNr)
                {
                    m_material.color = new Color((float)data[1], (float)data[2], (float)data[3]);
                    m_speed = (float)data[4];
                }
            }

            if (photonEvent.Code == PhotonEventCodes.HealthUpdateEventCode)
            {
                var data = photonEvent.Parameters;
                UIController.instance.UpdateHealth(1, (int)data[0]);
                UIController.instance.UpdateHealth(2, (int)data[1]);
            }

            if(photonEvent.Code == PhotonEventCodes.TimerUpdateEventCode)
            {
                var data = photonEvent.Parameters;
                UIController.instance.UpdateTimer((int)data[0]);
            }
        }
    }
}
