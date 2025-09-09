using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DD.Web3
{
    public class ClaimButtonHandler : MonoBehaviour
    {
        public Button[] claimButtons;  // All claim buttons
        [SerializeField] private Text scoreText; // Single score text
        [SerializeField] private int claimAmount = 0;
        public GameObject panel;
        private void Start()
        {
            foreach (var claimButton in claimButtons)
            {
                claimButton.onClick.AddListener(OnClaimButtonClicked);
            }
        }

        private void OnClaimButtonClicked()
        {
            
            panel.SetActive(true);
            // Get the score from UI
            if (scoreText != null && int.TryParse(scoreText.text, out int score))
            {
                claimAmount = score;
            }
            else
            {
                Debug.LogWarning("Score text is invalid or empty!");
                return;
            }

            // Blockchain flow
            if (BlockchainManager.Instance != null)
            {
                HandleClaimFlow();
            }
            else
            {
                Debug.Log("Blockchain or Wallet is not being used!");
            }
        }

        private void HandleClaimFlow()
        {
            BlockchainManager.Instance.connectionManager.ClaimDropERC20(claimAmount, (result) =>
            {
                if (result)
                    OnTransactionSuccessful();
                else
                    OnTransactionFailed();
            });
        }

        private void OnTransactionSuccessful()
        {
            Debug.Log("Transaction successful.");
            BlockchainManager.Instance.connectionManager.ShowLoadingScreen(false);
            panel.SetActive(false);
            foreach (var btn in claimButtons)
                btn.gameObject.SetActive(false);
        }

        private void OnTransactionFailed()
        {
            Debug.Log("Transaction failed.");
            panel.SetActive(false);
            BlockchainManager.Instance.connectionManager.ShowLoadingScreen(false);
        }

        private void OnDestroy()
        {
            foreach (var claimButton in claimButtons)
            {
                claimButton.onClick.RemoveAllListeners();
            }
        }
    }
}
