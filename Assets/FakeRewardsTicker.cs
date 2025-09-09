using TMPro;
using UnityEngine;

namespace DD.Web3
{
    public class FakeRewardsTicker : MonoBehaviour
    {
        public enum RewardMode { APRBased, FixedPerSecond }

        [Header("Refs")]
        public FakeStakeEngine stakeEngine;      // <-- assign in Inspector

        [Header("UI")]
        public TextMeshProUGUI rewardsText;      // "1.2345 STT"
        public TextMeshProUGUI apyText;          // "APY: 24%" (optional)

        [Header("Mode & Settings")]
        public RewardMode mode = RewardMode.APRBased;
        [Tooltip("Target APR % when APRBased mode is selected")]
        public float targetAPRPercent = 24f;     // e.g., 24% per year
        [Tooltip("Used only in FixedPerSecond mode (STT per second)")]
        public float fixedGrowthPerSecond = 0.01f;
        public string tokenSymbol = "STT";

        // ---- persistence keys ----
        private const string KEY_CUR = "YQ_REWARDS_CUR";
        private const string KEY_TS = "YQ_REWARDS_LAST_TS";

        // ---- runtime ----
        private float current;                   // unclaimed rewards (STT)
        private float growthPerSecond;           // computed each frame (depends on staked in APR mode)
        private float autosaveT;
        private int lastStaked;                // to detect staked changes
        private const double SECONDS_PER_YEAR = 365.0 * 24.0 * 60.0 * 60.0;

        void OnEnable()
        {
            // Load persisted amount
            current = PlayerPrefs.GetFloat(KEY_CUR, 0f);

            double lastTs = 0;
            double.TryParse(PlayerPrefs.GetString(KEY_TS, "0"), out lastTs);

            // Resume growth for time-away gap (based on last known staked & mode)
            int stakedNow = (stakeEngine ? stakeEngine.Staked : 0);
            growthPerSecond = ComputeGrowthPerSecond(stakedNow);
            if (lastTs > 0)
            {
                double elapsed = (System.DateTime.UtcNow - UnixToDateTime(lastTs)).TotalSeconds;
                if (elapsed > 0 && growthPerSecond > 0)
                    current += (float)(elapsed * growthPerSecond);
            }

            lastStaked = stakedNow;
            Paint();
        }
        public float GetCurrentRewards()
        {
            return current;
        }
        void Update()
        {
            // Recompute if staked changed (APR mode)
            int stakedNow = (stakeEngine ? stakeEngine.Staked : 0);
            if (mode == RewardMode.APRBased && stakedNow != lastStaked)
            {
                growthPerSecond = ComputeGrowthPerSecond(stakedNow);
                lastStaked = stakedNow;
            }

            // Accrue
            if (growthPerSecond > 0f)
                current += growthPerSecond * Time.deltaTime;

            // Autosave snapshot every few seconds
            autosaveT += Time.deltaTime;
            if (autosaveT >= 5f) { SaveSnapshot(); autosaveT = 0f; }

            Paint();
        }

        /// <summary>Call this on successful Claim TX → returns amount claimed, resets counter.</summary>
        public float ClaimAndReset()
        {
            float claimed = current;
            current = 0f;
            SaveSnapshot();
            Paint();
            return claimed;
        }

        void OnDisable() => SaveSnapshot();
        void OnApplicationPause(bool pause) { if (pause) SaveSnapshot(); }
        void OnApplicationQuit() => SaveSnapshot();

        // ---------------- helpers ----------------
        float ComputeGrowthPerSecond(int staked)
        {
            if (mode == RewardMode.FixedPerSecond)
                return Mathf.Max(0f, fixedGrowthPerSecond);

            // APRBased
            if (staked <= 0 || targetAPRPercent <= 0f) return 0f;

            // APR% per year on 'staked' → per-second rewards:
            // rewards_per_sec = (APR% / 100) * staked / seconds_per_year
            double perSec = (targetAPRPercent / 100.0) * (double)staked / SECONDS_PER_YEAR;
            return (float)perSec;
        }

        void Paint()
        {
            if (rewardsText) rewardsText.text = $"{current:F4} {tokenSymbol}";

            if (apyText)
            {
                if (mode == RewardMode.APRBased)
                {
                    // Show the configured APR
                    apyText.text = $"APY: {targetAPRPercent:0.#}%";
                }
                else
                {
                    // Show implied APR from fixed per-second growth (if staked > 0)
                    int staked = stakeEngine ? stakeEngine.Staked : 0;
                    if (staked > 0)
                    {
                        double perYear = fixedGrowthPerSecond * SECONDS_PER_YEAR;
                        double aprPct = (perYear / Mathf.Max(1, staked)) * 100.0;
                        apyText.text = $"APY: {aprPct:0.#}%";
                    }
                    else
                    {
                        apyText.text = "APY: —";
                    }
                }
            }
        }

        void SaveSnapshot()
        {
            PlayerPrefs.SetFloat(KEY_CUR, current);
            PlayerPrefs.SetString(KEY_TS, DateTimeToUnix(System.DateTime.UtcNow).ToString());
            PlayerPrefs.Save();
        }

        static double DateTimeToUnix(System.DateTime dt)
            => (dt - new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;

        static System.DateTime UnixToDateTime(double unix)
            => new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(unix);
    }

    

}
