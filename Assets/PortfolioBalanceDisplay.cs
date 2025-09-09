using TMPro;
using UnityEngine;

namespace DD.Web3
{
    public class PortfolioBalanceDisplay : MonoBehaviour
    {
        public TextMeshProUGUI txtStaked;      // "Staked: 250 TEST"
        public TextMeshProUGUI txtWallet;      // "Wallet: 750 TEST"
        public string tokenSymbol = "TEST";

        void OnEnable()
        {
            if (BlockchainManager.Instance)
            {
                BlockchainManager.Instance.onDropErc20BalanceUpdated += OnWalletBal;
                // initial paint
                OnWalletBal(BlockchainManager.Instance.dropErc20Balance);
            }
        }

        void OnDisable()
        {
            if (BlockchainManager.Instance)
                BlockchainManager.Instance.onDropErc20BalanceUpdated -= OnWalletBal;
        }

        void OnWalletBal(string bal)
        {
            if (txtWallet) txtWallet.text = $"{bal} {tokenSymbol}";
            // remove or comment out txtStaked overwrite
            // if (txtStaked) txtStaked.text = $"{tokenSymbol}";
        }

    }
}
