using common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trading_engine
{
    public enum TransferType
    {
        AVAILABLE_TOAVAILABLE,
        AVAILABLE_TO_FROZEN,
        FROZEN_TO_AVAILABLE,
    }
    public class AssetService
    {
        private readonly Dictionary<long, Dictionary<AssetEnum, Asset>> userAssets = [];

        private readonly ILogger<AssetService> log;

        public AssetService(ILogger<AssetService> logger)
        {
            this.log = logger;
        }

        public Asset? GetAsset(long userId, AssetEnum assetType)
        {
            if (userAssets.TryGetValue(userId, out var assets) && assets.TryGetValue(assetType, out var asset))
            {
                return asset;
            }
            else
            {
                return null;
            }
        }

        public Dictionary<AssetEnum, Asset> GetAssets(long userId)
        {
            if (userAssets.TryGetValue(userId, out var assets))
            {
                return assets;
            }
            else
            {
                return [];
            }
        }

        public Dictionary<long, Dictionary<AssetEnum, Asset>> GetAllAssets()
        {
            return userAssets;
        }

        public bool TryFreeze(long userId, AssetEnum assetType, decimal amount)
        {
            bool ok = TryTransfer(TransferType.AVAILABLE_TO_FROZEN, userId, userId, assetType, amount, true);
            if (ok)
            {
                log.LogDebug($"User {userId} froze {amount} of {assetType}");
            }
            return ok;
        }

        public void UnFreeze(long userId, AssetEnum assetType, decimal amount)
        {
            bool ok = TryTransfer(TransferType.FROZEN_TO_AVAILABLE, userId, userId, assetType, amount, true);
            if (!ok)
            {
                throw new Exception($"Failed to unfreeze {amount} of {assetType} for user {userId}");
            }
            log.LogDebug($"User {userId} unfroze {amount} of {assetType}");
        }

        public void Transfer(TransferType transferType, long fromUser, long toUser, AssetEnum assetType, decimal amount)
        {
            bool ok = TryTransfer(transferType, fromUser, toUser, assetType, amount, true);
            if (!ok)
            {
                throw new Exception($"Failed to transfer {amount} of {assetType} from user {fromUser} to user {toUser} with transfer type {transferType}");
            }
            log.LogDebug($"Transferred {amount} of {assetType} from user {fromUser} to user {toUser} with transfer type {transferType}");
        }

        public bool TryTransfer(TransferType transferType, long fromUser, long toUser, AssetEnum assetId, decimal amount, bool checkBalance)
        {
            if (amount == 0)
            {
                return true;
            }
            if (amount < 0)
            {
                throw new ArgumentException("Amount must be non-negative", nameof(amount));
            }
            Asset? fromAsset = GetAsset(fromUser, assetId);
            if (fromAsset == null)
            {
                fromAsset = initAsset(fromUser, assetId);
            }
            Asset? toAsset = GetAsset(toUser, assetId);
            if (toAsset == null)
            {
                toAsset = initAsset(toUser, assetId);
            }
            switch (transferType)
            {
                case TransferType.AVAILABLE_TOAVAILABLE:
                    if (checkBalance && fromAsset.Available < amount)
                    {
                        return false;
                    }
                    fromAsset.Available -= amount;
                    toAsset.Available += amount;
                    return true;
                case TransferType.AVAILABLE_TO_FROZEN:
                    if (checkBalance && fromAsset.Available < amount)
                    {
                        return false;
                    }
                    fromAsset.Available -= amount;
                    toAsset.Frozen += amount;
                    return true;
                case TransferType.FROZEN_TO_AVAILABLE:
                    if (checkBalance && fromAsset.Frozen < amount)
                    {
                        return false;
                    }
                    fromAsset.Frozen -= amount;
                    toAsset.Available += amount;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(transferType), "Invalid transfer type");
            }

        }

        private Asset initAsset(long userId, AssetEnum assetId)
        {
            if (!userAssets.ContainsKey(userId))
            {
                userAssets[userId] = [];
            }
            Asset asset = new(0, 0);
            userAssets[userId][assetId] = asset;
            return asset;
        }
    }
}
