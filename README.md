# Unity Web3 Mint + Trigger-Gated UI + DOTween + Teleport

![Unity](https://img.shields.io/badge/Unity-2022%2B-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-10-blue)
![DOTween](https://img.shields.io/badge/DOTween-Required-green)
![License](https://img.shields.io/badge/license-MIT-lightgrey)

A clean Unity template that shows how to:

- ✅ **Mint NFT only after on-chain success** (via `BlockchainManager` → `ClaimDropERC20`)
- 🎯 **Trigger-gated mint buttons** (button shows inside a zone, hides outside)
- 💫 **DOTween pop animations** for buttons (0→1 pop-in, 1→0 pop-out)
- 🌀 **Teleport on trigger enter** (instant or with fade; works with `NavMeshAgent`, `CharacterController`, or `Rigidbody`)
- 💾 **Persist minted items** with `PlayerPrefs` and rebuild UI on load

> Perfect for quick WebGL demos, kiosks, training sims, and prototypes.

---

## ✨ Features

- **Blockchain-first UX:** UI add happens only if the on-chain claim succeeds.
- **Index-linked mint:** `Buttons[i]` ↔ `Sprites[i]` ↔ `TriggerPoints[i]`.
- **Juicy UI:** DOTween-based show/hide with configurable durations & easings.
- **Plug-and-play teleporters:** Enter a trigger → instantly move to destination (optional fade).
- **Simple persistence:** Stores sprite names; rebuilds NFT cards on startup.

---

## 🧱 Scripts

