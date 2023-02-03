namespace PCConfiguratorApplication
{
    using System.Collections.Generic;

    public class Inventory
    {
        public IEnumerable<CPU> CPUs { get; init; }

        public IEnumerable<Memory> Memory { get; init; }

        public IEnumerable<Motherboard> Motherboards { get; init; }
    }
}
