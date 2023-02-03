namespace PCConfiguratorApplication
{
    using System;

    public class Combination
    {
        public string CPU { get; init; }

        public string Motherboard { get; init; }

        public string Memory { get; init; }

        public decimal Price { get; init; }

        public override string ToString()
            => $"    {this.CPU}{Environment.NewLine}" +
               $"    {this.Motherboard}{Environment.NewLine}" +
               $"    {this.Memory}{Environment.NewLine}" +
               $"    Price: {this.Price:F0}";
    }
}
