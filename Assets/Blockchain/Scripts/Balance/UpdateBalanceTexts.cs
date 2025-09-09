using TMPro;
using UnityEngine;

namespace DD.Web3
{
    public class UpdateBalanceTexts : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _dropErc20BalTxt;
        private void OnEnable()
        {
            if(BlockchainManager.Instance != null)
            {
                BlockchainManager.Instance.onDropErc20BalanceUpdated += DropErc20BalanceUpdated;
                

                    if (_dropErc20BalTxt.gameObject)
                {
                    _dropErc20BalTxt.text = "Score : " + BlockchainManager.Instance.dropErc20Balance;
                }
            }
        }

        private void OnDisable()
        {
            if (BlockchainManager.Instance != null)
            {
                BlockchainManager.Instance.onDropErc20BalanceUpdated -= DropErc20BalanceUpdated;
            }
        }

        private void DropErc20BalanceUpdated(string _bal)
        {
            if (_dropErc20BalTxt.gameObject)
            {
                _dropErc20BalTxt.text = "Score : "+_bal;
            }
        }
    }
}

