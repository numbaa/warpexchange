using common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trading_engine.match
{
    public record struct OrderKey(long SequenceId, decimal Price);
    public class OrderBook
    {
        public Direction Direction { get; private set; }
        public SortedDictionary<OrderKey, OrderEntity> Book { get; set; }

        private readonly IComparer<OrderKey> _BuyComparer = Comparer<OrderKey>.Create((left, right) => {
            int cmp = left.Price.CompareTo(right.Price);
            if (cmp == 0)
            {
                return left.SequenceId.CompareTo(right.SequenceId);
            }
            else
            {
                return cmp;
            }
        });

        private readonly IComparer<OrderKey> _SellComparer = Comparer<OrderKey>.Create((left, right) => {
            int cmp = right.Price.CompareTo(left.Price);
            if (cmp == 0)
            {
                return left.SequenceId.CompareTo(right.SequenceId);
            }
            else
            {
                return cmp;
            }
        });

        public OrderBook(Direction direction)
        {
            Direction = direction;
            Book = new SortedDictionary<OrderKey, OrderEntity>(direction == Direction.BUY ? _BuyComparer : _SellComparer);
        }

        public OrderEntity? GetFirst()
        {
            return Book.Count > 0 ? Book.First().Value : null;
        }

        public bool Remove(OrderEntity order)
        {
            return Book.Remove(new OrderKey(order.sequenceId, order.price));
        }

        public bool Add(OrderEntity order)
        {
            return Book.TryAdd(new OrderKey(order.sequenceId, order.price), order);
        }
    }
}
