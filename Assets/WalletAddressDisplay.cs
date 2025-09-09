using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DD.Web3
{
    public class WalletAddressDisplay : MonoBehaviour
    {
        public TextMeshProUGUI addressText;     // header right
        public Button connectHint;              // button itself
        public Button Connected;              // button itself
        public TextMeshProUGUI connectHintText; // button ke andar ka TMP_Text

        void OnEnable() { Refresh(); InvokeRepeating(nameof(Refresh), 0.5f, 0.5f); }
        void OnDisable() { CancelInvoke(nameof(Refresh)); }

        void Refresh()
        {
            string addr = BlockchainManager.Instance ? BlockchainManager.Instance.walletAddress : "";
            bool connected = !string.IsNullOrEmpty(addr);

            // address text update
            if (addressText)
                addressText.text = connected ? Shorten(addr) : "Not Connected";

            // connect button state
            if (connectHint)
            {
                connectHint.interactable = !connected; // disable after connect
            }

            if (connectHintText)
            {
                connectHintText.text = connected ? "Connected" : "Connect";
                Connected.gameObject.SetActive(true);
            }
        }

        string Shorten(string a)
        {
            if (string.IsNullOrEmpty(a) || a.Length < 10) return a;
            return a.Substring(0, 6) + ".." + a.Substring(a.Length - 4);
        }
    }
}
