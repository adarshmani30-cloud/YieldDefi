mergeInto(LibraryManager.library, {
  AddAndSwitchChain: function(chainIdPtr, rpcUrlPtr, chainNamePtr, nativeCurrencyPtr, blockExplorerPtr) {
    const chainId = UTF8ToString(chainIdPtr); // Example: "0x4f588"
    const rpcUrl = UTF8ToString(rpcUrlPtr);   // Example: "https://325000.rpc.thirdweb.com/your-client-id"
    const chainName = UTF8ToString(chainNamePtr); // Example: "Camp Network Testnet V2"
    const nativeCurrencyStr = UTF8ToString(nativeCurrencyPtr); // JSON string like {"name":"Ethereum","symbol":"ETH","decimals":18}
    const blockExplorer = UTF8ToString(blockExplorerPtr); // Example: "https://camp-network-testnet.blockscout.com/"

    console.log("Switch Chain Called");
    console.log("Chain ID: " + chainId);
    console.log("RPC URL: " + rpcUrl);
    console.log("Chain Name: " + chainName);
    console.log("Block Explorer: " + blockExplorer);
    console.log("Native Currency JSON (raw): " + nativeCurrencyStr);

    let nativeCurrency;
    try {
      nativeCurrency = JSON.parse(nativeCurrencyStr);
    } catch (e) {
      console.error("Failed to parse nativeCurrency JSON: " + nativeCurrencyStr);
      console.error(e);
      return;
    }

    if (typeof window.ethereum !== "undefined") {
      console.log("MetaMask detected");

      window.ethereum.request({
        method: 'wallet_switchEthereumChain',
        params: [{ chainId: chainId }]
      }).then(function () {
        console.log("Switched to existing chain " + chainId);
      }).catch(function (switchError) {
        console.warn("Switch failed, trying to add chain. Error:");
        console.warn(switchError);

        if (switchError.code === 4902) {
          window.ethereum.request({
            method: 'wallet_addEthereumChain',
            params: [{
              chainId: chainId,
              chainName: chainName,
              rpcUrls: [rpcUrl],
              nativeCurrency: nativeCurrency,
              blockExplorerUrls: [blockExplorer]
            }]
          }).then(function () {
            console.log("Chain added, switching now...");
            return window.ethereum.request({
              method: 'wallet_switchEthereumChain',
              params: [{ chainId: chainId }]
            });
          }).then(() => {
            console.log("Successfully switched to new chain after adding.");
          }).catch(function (addError) {
            console.error("Add chain failed:");
            console.error(addError);
          });
        } else {
          console.error("Switch chain failed (non-4902):");
          console.error(switchError);
        }
      });
    } else {
      console.error("MetaMask not available");
    }
  }
});
