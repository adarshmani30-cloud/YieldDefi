using TMPro;
using UnityEngine;

namespace DD.Web3
{
    public class FakeStakeEngine : MonoBehaviour
    {
        [Header("Texts")]
        public TextMeshProUGUI txtWallet;  // "Wallet : 160 STT"
        public TextMeshProUGUI txtStaked;  // "Staked : 0 STT"
        public string tokenSymbol = "STT";

        // persisted state
        private const string KEY_STAKED = "YQ_STAKED";
        private const string KEY_DELTA = "YQ_WALLET_DELTA"; // applied over real chain balance

        public int Wallet { get; private set; } // computed: chain + delta
        public int Staked { get; private set; }
        private int walletDelta;                // local +/- over chain

        void OnEnable()
        {
            Staked = PlayerPrefs.GetInt(KEY_STAKED, 0);
            walletDelta = PlayerPrefs.GetInt(KEY_DELTA, 0);

            int chainBal = 0;
            if (BlockchainManager.Instance != null &&
                int.TryParse(BlockchainManager.Instance.dropErc20Balance, out var c)) chainBal = c;

            Wallet = chainBal + walletDelta;
            Paint();

            if (BlockchainManager.Instance != null)
                BlockchainManager.Instance.onDropErc20BalanceUpdated += SyncWalletFromChain;
        }

        void OnDisable()
        {
            if (BlockchainManager.Instance != null)
                BlockchainManager.Instance.onDropErc20BalanceUpdated -= SyncWalletFromChain;
        }

        void SyncWalletFromChain(string newBal)
        {
            if (int.TryParse(newBal, out var chain))
            {
                Wallet = chain + walletDelta; // keep our local delta on top
                Paint();
            }
        }

        void Save()
        {
            PlayerPrefs.SetInt(KEY_STAKED, Staked);
            PlayerPrefs.SetInt(KEY_DELTA, walletDelta);
            PlayerPrefs.Save();
        }

        void Paint()
        {
            if (txtWallet) txtWallet.text = $"{Wallet} {tokenSymbol}";
            if (txtStaked) txtStaked.text = $"{Staked} {tokenSymbol}";
        }

        public bool CanStake(int amt) => amt > 0 && Wallet >= amt;
        public bool CanUnstake(int amt) => amt > 0 && Staked >= amt;

        // Apply AFTER tx success (we call from bindings)
        public void ApplyStake(int amt)
        {
            walletDelta -= amt;
            Staked += amt;
            Wallet -= amt;
            Save(); Paint();
        }

        public void ApplyUnstake(int amt)
        {
            walletDelta += amt;
            Staked -= amt;
            Wallet += amt;
            Save(); Paint();
        }

        public void ApplyRewards(int amt)
        {
            walletDelta += Mathf.Max(0, amt);
            Wallet += Mathf.Max(0, amt);
            Save(); Paint();
        }
    }
}
