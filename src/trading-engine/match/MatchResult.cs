using common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trading_engine.match
{
    public record struct MatchDetailRecord(decimal Price, decimal Quantity, OrderEntity takerOrder, OrderEntity makerOrder);


    public class MatchResult
    {
        public OrderEntity TakerOrder { get; set; }
        public  List<MatchDetailRecord> MatchDetails { get; set; } = [];

        public MatchResult(OrderEntity takerOrder)
        {
            TakerOrder = takerOrder;
        }

        public override string ToString()
        {
            var details = string.Join(", ", MatchDetails.Select(md => $"(Price: {md.Price}, Quantity: {md.Quantity}, MakerOrderId: {md.makerOrder.id})"));
            return $"MatchResult(TakerOrderId: {TakerOrder.id}, MatchDetails: [{details}])";
        }
    }
}
