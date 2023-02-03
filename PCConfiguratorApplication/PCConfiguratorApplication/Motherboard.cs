namespace PCConfiguratorApplication
{
    public class Motherboard : Component, ISocket
    {
        public string Socket { get; init; }

        public override string ToString()
            => base.ToString() + $" - {this.Socket}";
    }
}
