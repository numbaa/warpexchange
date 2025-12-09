using common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trading_engine.match
{
    public class MatchEngine
    {
        public OrderBook BuyBook { get; } = new OrderBook(Direction.BUY);
        public OrderBook SellBook { get; } = new OrderBook(Direction.SELL);

        public decimal MarketPrice { get; private set; } = 0m;

        private long _SequenceId = 0;

        public MatchResult ProcessOrder(long sequenceId, OrderEntity order)
        {
            switch (order.Direction)
            {
                case Direction.BUY:
                    return ProcessBuyOrder(sequenceId, order);
                case Direction.SELL:
                    return ProcessSellOrder(sequenceId, order);
                default:
                    throw new ArgumentException($"Invalid order direction: {order.Direction}");
            }
        }
    }
}
