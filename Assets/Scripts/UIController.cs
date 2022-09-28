using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PhotonServerDemo
{
    public class UIController : MonoBehaviour
    {
        public Text player1HealthText, player2HealthText;
        public Button healButton, damageButton;
        public Text timerText;

        public static UIController instance;

        private void Awake()
        {
            instance = this;
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateHealth(int playerID, int value)
        {
            if (playerID == 1)
            {
                player1HealthText.text = "P1: " + value.ToString();
            }
            if (playerID == 2)
            {
                player2HealthText.text = "P2: " + value.ToString();
            }
        }

        public void UpdateTimer(int value)
        {
            timerText.text = "Time: " + value;
        }

        public void AttachToHealButton(UnityAction callback)
        {
            healButton.onClick.AddListener(callback);
        }

        public void AttachToDamageButton(UnityAction callback)
        {
            damageButton.onClick.AddListener(callback);
        }
    }
}
