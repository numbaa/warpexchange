namespace common;

public enum AssetEnum
{
    BTC,
    USD
}

public class Asset
{
    public decimal Available { get; set; }
    public decimal Frozen { get; set; }

    public Asset(decimal available = 0, decimal frozen = 0)
    {
        Available = available;
        Frozen = frozen;
    }

    public override string ToString()
    {
        return $"Available: {Available:F2}, Frozen: {Frozen:F2}";
    }
}
