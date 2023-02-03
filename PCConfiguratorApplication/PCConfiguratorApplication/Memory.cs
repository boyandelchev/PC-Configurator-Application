namespace PCConfiguratorApplication
{
    public class Memory : Component
    {
        public string Type { get; init; }

        public override string ToString()
            => base.ToString() + $" - {this.Type}";
    }
}
