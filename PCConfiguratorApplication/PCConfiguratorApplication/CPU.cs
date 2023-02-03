namespace PCConfiguratorApplication
{
    public class CPU : Component, ISocket
    {
        public string SupportedMemory { get; init; }

        public string Socket { get; init; }

        public override string ToString()
            => base.ToString() + $" - {this.Socket}, {this.SupportedMemory}";
    }
}
