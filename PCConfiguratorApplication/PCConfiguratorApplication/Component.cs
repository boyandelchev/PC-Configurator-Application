namespace PCConfiguratorApplication
{
    public abstract class Component
    {
        public string ComponentType { get; init; }

        public string PartNumber { get; init; }

        public string Name { get; init; }

        public decimal Price { get; init; }

        public override string ToString()
            => $"{this.ComponentType} - {this.Name}";
    }
}
