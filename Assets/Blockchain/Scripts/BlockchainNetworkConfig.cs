using UnityEngine;

namespace DD.Web3
{

    public enum BlockchainNetwork
    {
        Camp,
        Soneium,
        Somnia,
        Sei
    }

    [CreateAssetMenu(fileName = "BlockchainNetworkConfig", menuName = "Scriptable Objects/Blockchain/NetworkConfig")]
    public class NetworkConfigSO : ScriptableObject
    {
        public BlockchainNetwork network;

        public string networkName;

        [Tooltip("Use string so Unity can serialize it. Example: '1868'")]
        public string chainIdString;

        public string rpcUrl;
        //public string chainDisplayName;

        [TextArea]
        public string nativeCurrencyJson;

        public string blockExplorerUrl;
        public string dropERC20ContractAddress;
        public string purchasingTokenContractAddress;


        public System.Numerics.BigInteger ChainId => System.Numerics.BigInteger.Parse(chainIdString);
    }
}


