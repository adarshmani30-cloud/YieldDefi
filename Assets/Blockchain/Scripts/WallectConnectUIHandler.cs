using Thirdweb.Unity;
using UnityEngine;


namespace DD.Web3
{
    public class WallectConnectUIHandler : MonoBehaviour
    {
        private void OnEnable()
        {
            if(BlockchainManager.Instance != null && ThirdwebManager.Instance != null)
            {
                bool isWalletConnected = !string.IsNullOrEmpty(BlockchainManager.Instance.walletAddress);

                if (isWalletConnected)
                {
                    BlockchainManager.Instance.walletConnected.OnWalletConnected();
                }
                else
                {

                }
            }
        }
    }
}

