using System.Threading.Tasks;
using System;
using Thirdweb;
using UnityEngine;
using System.Numerics;

public static class ClaimERC20
{
    public static async Task<ThirdwebTransactionReceipt> DropERC20_Claim_Custom(this ThirdwebContract contract, IThirdwebWallet wallet, string receiverAddress, string amount)
    {
        if (contract == null)
        {
            throw new ArgumentNullException("contract");
        }

        if (wallet == null)
        {
            throw new ArgumentNullException("wallet");
        }

        if (string.IsNullOrEmpty(receiverAddress))
        {
            throw new ArgumentException("Receiver address must be provided");
        }

        if (string.IsNullOrEmpty(amount))
        {
            throw new ArgumentException("Amount must be provided");
        }

        Drop_ClaimCondition activeClaimCondition = await contract.DropERC20_GetActiveClaimCondition();
        int toDecimals = await contract.ERC20_Decimals();
        BigInteger amountInBigInt = BigInteger.Parse(amount);
        BigInteger bigInteger = BigInteger.Parse(amount.ToWei()).AdjustDecimals(18, toDecimals);
        BigInteger weiValue = ((activeClaimCondition.Currency == "0xEeeeeEeeeEeEeeEeEeEeeEEEeeeeEeeeeeeeEEeE") ? (amountInBigInt * activeClaimCondition.PricePerToken) : BigInteger.Zero);
        object[] array = new object[4]
        {
            Array.Empty<byte>(),
            BigInteger.Zero,
            BigInteger.Zero,
            "0x0000000000000000000000000000000000000000"
        };
        object[] parameters = new object[6]
        {
            receiverAddress,
            bigInteger,
            activeClaimCondition.Currency,
            activeClaimCondition.PricePerToken,
            array,
            Array.Empty<byte>()
        };
        return await ThirdwebContract.Write(wallet, contract, "claim", weiValue, parameters);
    }
}
