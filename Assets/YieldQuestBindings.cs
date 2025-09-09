using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DD.Web3
{
    public class YieldQuestBindings : MonoBehaviour
    {
        [Header("Refs")]
        public ConnectionManager connection;     // Thirdweb wallet + claim
        public FakeStakeEngine fake;             // Wallet/Staked math
        public FakeRewardsTicker ticker;         // Rewards auto-grow

        [Header("Amount UI")]
        public TMP_InputField amountInput;
        public Slider amountSlider;
        public int fallbackAmount = 10;
        public int minAmount = 1;
        public int maxAmount = 1000;
        public bool sliderWholeNumbers = true;

        [Header("Buttons")]
        public Button btnStake;
        public Button btnUnstake;
        public Button btnClaimSmall;
        public Button btnClaimBig;

        [Header("UX")]
        public GameObject loadingPanel;
        //public TextMeshProUGUI statusLabel;

        private bool _updating; // guard for slider/input sync

        void Awake()
        {
            // button binds
            if (btnStake) btnStake.onClick.AddListener(OnStake);
            if (btnUnstake) btnUnstake.onClick.AddListener(OnUnstake);
            if (btnClaimSmall) btnClaimSmall.onClick.AddListener(OnClaimRewards);
            if (btnClaimBig) btnClaimBig.onClick.AddListener(OnClaimRewards);

            // slider/input sync
            if (amountSlider)
            {
                amountSlider.minValue = minAmount;
                amountSlider.maxValue = maxAmount;
                amountSlider.wholeNumbers = sliderWholeNumbers;
                amountSlider.onValueChanged.AddListener(OnSliderChanged);
            }
            if (amountInput)
            {
                amountInput.onValueChanged.AddListener(OnInputChanged);
                amountInput.contentType = TMP_InputField.ContentType.IntegerNumber;
            }

            HarmonizeInitial();
        }

        void HarmonizeInitial()
        {
            if (!amountInput && !amountSlider) return;

            if (amountInput && int.TryParse(amountInput.text.Trim(), out var v) && v > 0)
            {
                var c = Clamp(v);
                if (amountSlider) { _updating = true; amountSlider.value = c; _updating = false; }
                amountInput.text = c.ToString();
            }
            else if (amountSlider)
            {
                var c = Clamp(Mathf.RoundToInt(amountSlider.value));
                if (amountInput) { _updating = true; amountInput.text = c.ToString(); _updating = false; }
                if (amountSlider.value != c) amountSlider.value = c;
            }
            else if (amountInput)
            {
                var c = Clamp(fallbackAmount > 0 ? fallbackAmount : minAmount);
                amountInput.text = c.ToString();
            }
        }

        void OnSliderChanged(float f)
        {
            if (_updating) return;
            var v = Clamp(Mathf.RoundToInt(f));
            if (amountInput)
            {
                _updating = true;
                amountInput.text = v.ToString();
                _updating = false;
            }
        }

        void OnInputChanged(string s)
        {
            if (_updating) return;
            if (!int.TryParse(s.Trim(), out var v)) v = fallbackAmount > 0 ? fallbackAmount : minAmount;
            v = Clamp(v);
            if (amountSlider)
            {
                _updating = true;
                amountSlider.value = v;
                _updating = false;
            }
            if (amountInput && amountInput.text != v.ToString())
            {
                _updating = true;
                amountInput.text = v.ToString();
                _updating = false;
            }
        }

        int Clamp(int v) => Mathf.Clamp(v, minAmount, maxAmount);

        int CurrentAmount()
        {
            if (amountInput && int.TryParse(amountInput.text.Trim(), out var v) && v > 0) return v;
            if (amountSlider && amountSlider.value > 0) return Mathf.RoundToInt(amountSlider.value);
            return Mathf.Max(fallbackAmount, minAmount);
        }

        void ShowStatus(string s, bool showPanel = true)
        {
            if (loadingPanel) loadingPanel.SetActive(showPanel);
            //if (statusLabel) statusLabel.text = s;
            Debug.Log("[YQ] " + s);
        }

        // ---------------- Stake / Unstake ----------------
        void OnStake()
        {
            int amt = CurrentAmount();
            if (fake == null || connection == null) { Debug.LogError("[YQ] Refs missing"); return; }
            if (!fake.CanStake(amt)) { ShowStatus("Not enough wallet", false); return; }

            ShowStatus("Staking (tx)…");
            connection.ClaimDropERC20(1, success =>
            {
                if (success)
                {
                    fake.ApplyStake(amt);
                    fake.ApplyRewards(-1); // rollback that +1 from claim
                    ShowStatus("Stake successful!", false);
                }
                else ShowStatus("Stake failed", false);
            });
        }

        void OnUnstake()
        {
            int amt = CurrentAmount();
            if (fake == null || connection == null) { Debug.LogError("[YQ] Refs missing"); return; }
            if (!fake.CanUnstake(amt)) { ShowStatus("Not enough staked", false); return; }

            ShowStatus("Unstaking (tx)…");
            connection.ClaimDropERC20(1, success =>
            {
                if (success)
                {
                    fake.ApplyUnstake(amt);
                    fake.ApplyRewards(-1); // rollback that +1
                    ShowStatus("Unstake successful!", false);
                }
                else ShowStatus("Unstake failed", false);
            });
        }

        // ---------------- Rewards Claim ----------------
        void OnClaimRewards()
        {
            if (connection == null) { Debug.LogError("[YQ] Connection missing"); return; }
            ShowStatus("Claiming rewards (tx)…");
            connection.ClaimDropERC20(1, success =>
            {
                if (success)
                {
                    float claimed = ticker ? ticker.ClaimAndReset() : 0f;
                    if (fake && claimed > 0f) fake.ApplyRewards(Mathf.RoundToInt(claimed - 1));
                    // 👆 claimed -1 rollback because tx itself minted +1
                    ShowStatus("Claim successful!", false);
                }
                else ShowStatus("Claim failed", false);
            });
        }
    }
}
