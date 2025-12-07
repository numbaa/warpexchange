using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common;
using Microsoft.Extensions.Logging;

namespace trading_engine
{
    public class OrderService
    {
        private readonly ILogger<OrderService> log;
        private readonly ConcurrentDictionary<long, OrderEntity> activeOrders = new();
        private readonly ConcurrentDictionary<long, ConcurrentDictionary<long, OrderEntity>> userOrders = new();
        private readonly AssetService assetService;

        public OrderService(ILogger<OrderService> logger, AssetService assetService)
        {
            this.log = logger;
            this.assetService = assetService;
        }

        public OrderEntity? createOrder(long sequenceId, long ts, long orderId, long userId, Direction direction, decimal price, decimal quantity)
        {
            switch (direction)
            {
                case Direction.BUY:
                    if (!assetService.TryFreeze(userId, AssetEnum.USD, price * quantity))
                    {
                        log.LogDebug($"Insufficient USD balance for user {userId} to create BUY order {orderId}");
                        return null;
                    }
                    break;
                case Direction.SELL:
                    if (!assetService.TryFreeze(userId, AssetEnum.BTC, quantity))
                    {
                        log.LogDebug($"Insufficient BTC balance for user {userId} to create SELL order {orderId}");
                        return null;
                    }
                    break;
                default:
                    throw new ArgumentException($"Invalid direction: {direction}");
            }
            var order = new OrderEntity
            {
                id = orderId,
                sequenceId = sequenceId,
                userId = userId,
                price = price,
                quantity = quantity,
                unfilledQuantity = quantity,
                createdAt = ts,
                updatedAt = ts
            };
            activeOrders[orderId] = order;
            var userOrders = this.userOrders.GetOrAdd(userId, _ => new ConcurrentDictionary<long, OrderEntity>());
            userOrders[orderId] = order;
            return order;
        }

        public void removeOrder(long orderId)
        {
            if (!activeOrders.TryRemove(orderId, out var order))
            {
                log.LogWarning($"Attempted to remove non-existent order {orderId}");
                throw new Exception($"Order {orderId} does not exist");
            }
            if (userOrders.TryGetValue(order.userId, out var uOrders))
            {
                if (!uOrders.TryRemove(orderId, out _))
                {
                    throw new Exception($"Order {orderId} not found in user {order.userId}'s orders when removing");
                }
            }
            else
            {
                throw new Exception($"User orders for user {order.userId} not found when removing order {orderId}");
            }
        }

        public OrderEntity? GetOrder(long orderId)
        {
            if (activeOrders.TryGetValue(orderId, out var order))
            {
                return order;
            }
            else
            {
                return null;
            }
        }

        public ConcurrentDictionary<long, OrderEntity>? GetUerOrders(long userId)
        {
            if (userOrders.TryGetValue(userId, out var orders))
            {
                return orders;
            }
            else
            {
                return null;
            }
        }
    }
}
