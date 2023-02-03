namespace PCConfiguratorApplication
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;

    internal class Program
    {
        private const string InputEntry = "Please enter part number(s): ";
        private const string InputSeparator = ", ";

        private const string CombinationsCount = "There are {0} possible combinations:";
        private const string CombinationAppendCount = "Combination {0}";

        private const string ErrorEmptyInput = "ERROR: Please provide valid part numbers.";
        private const string ErrorInputLength = "ERROR: Please provide no more than 3 part numbers.";

        private const string ErrorNoValidCombinations = "ERROR: No valid combinations found.";
        private const string ErrorSameComponentTypes = "ERROR: Please choose different component types.";

        private const string ErrorConfigurationNotValid = "ERROR: The selected configuration is not valid.";
        private const string ErrorAppendCount = "{0}. ";
        private const string ErrorMotherboardIncompatible = "Motherboard with socket {0} is not compatible with the CPU.";
        private const string ErrorMemoryIncompatible = "Memory of type {0} is not compatible with the CPU.";
        private const string ErrorMemoryAndMotherboardIncompatibleWithCPU = "Memory of type {0} and motherboard with socket {1} are not compatible with any CPU.";

        static void Main(string[] args)
        {
            var fileName = "pc-store-inventory.json";
            var path = Path.Combine("..", "..", "..", fileName);
            var jsonString = File.ReadAllText(path);
            var inventory = JsonSerializer.Deserialize<Inventory>(jsonString);

            var CPUsByPartNumber = inventory.CPUs
                .ToDictionary(cpu => cpu.PartNumber.ToLower(), cpu => cpu)
                .AsReadOnly();
            var memoryByPartNumber = inventory.Memory
                .ToDictionary(m => m.PartNumber.ToLower(), m => m)
                .AsReadOnly();
            var motherboardsByPartNumber = inventory.Motherboards
                .ToDictionary(m => m.PartNumber.ToLower(), m => m)
                .AsReadOnly();

            Console.Write(InputEntry);
            var input = Console.ReadLine()
                .Split(InputSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (input.Length == 0)
            {
                Console.WriteLine(ErrorEmptyInput);
                return;
            }

            if (input.Length > 3)
            {
                Console.WriteLine(ErrorInputLength);
                return;
            }

            var combinations = new List<Combination>();

            if (input.Length == 1)
            {
                var key = input[0].ToLower();

                if (CPUsByPartNumber.TryGetValue(key, out var CPU))
                {
                    CPUOnlyInputConfigurations(CPU, motherboardsByPartNumber, memoryByPartNumber, combinations);
                }
                else if (motherboardsByPartNumber.TryGetValue(key, out var motherboard))
                {
                    MotherboardOnlyInputConfigurations(motherboard, CPUsByPartNumber, memoryByPartNumber, combinations);
                }
                else if (memoryByPartNumber.TryGetValue(key, out var memory))
                {
                    MemoryOnlyInputConfigurations(memory, CPUsByPartNumber, motherboardsByPartNumber, combinations);
                }
                else
                {
                    Console.WriteLine(ErrorNoValidCombinations);
                    return;
                }
            }
            else if (input.Length == 2)
            {
                var keyOne = input[0].ToLower();
                var keyTwo = input[1].ToLower();

                if (!CPUsByPartNumber.ContainsKey(keyOne) &&
                    !motherboardsByPartNumber.ContainsKey(keyOne) &&
                    !memoryByPartNumber.ContainsKey(keyOne)
                    ||
                    !CPUsByPartNumber.ContainsKey(keyTwo) &&
                    !motherboardsByPartNumber.ContainsKey(keyTwo) &&
                    !memoryByPartNumber.ContainsKey(keyTwo))
                {
                    Console.WriteLine(ErrorNoValidCombinations);
                    return;
                }

                if (CPUsByPartNumber.TryGetValue(keyOne, out var CPU) &&
                    motherboardsByPartNumber.TryGetValue(keyTwo, out var motherboard))
                {
                    MotherboardCompatibilityConfigurations(CPU, motherboard, memoryByPartNumber, combinations);
                }
                else if (CPUsByPartNumber.TryGetValue(keyOne, out CPU) &&
                    memoryByPartNumber.TryGetValue(keyTwo, out var memory))
                {
                    MemoryCompatibilityConfigurations(CPU, memory, motherboardsByPartNumber, combinations);
                }
                else if (motherboardsByPartNumber.TryGetValue(keyOne, out motherboard) &&
                    CPUsByPartNumber.TryGetValue(keyTwo, out CPU))
                {
                    MotherboardCompatibilityConfigurations(CPU, motherboard, memoryByPartNumber, combinations);
                }
                else if (motherboardsByPartNumber.TryGetValue(keyOne, out motherboard) &&
                    memoryByPartNumber.TryGetValue(keyTwo, out memory))
                {
                    CPUCompatibilityConfigurations(motherboard, memory, CPUsByPartNumber, combinations);
                }
                else if (memoryByPartNumber.TryGetValue(keyOne, out memory) &&
                    CPUsByPartNumber.TryGetValue(keyTwo, out CPU))
                {
                    MemoryCompatibilityConfigurations(CPU, memory, motherboardsByPartNumber, combinations);
                }
                else if (memoryByPartNumber.TryGetValue(keyOne, out memory) &&
                    motherboardsByPartNumber.TryGetValue(keyTwo, out motherboard))
                {
                    CPUCompatibilityConfigurations(motherboard, memory, CPUsByPartNumber, combinations);
                }
                else
                {
                    Console.WriteLine(ErrorSameComponentTypes);
                    return;
                }
            }
            else if (input.Length == 3)
            {
                var keyOne = input[0].ToLower();
                var keyTwo = input[1].ToLower();
                var keyThree = input[2].ToLower();

                if (!CPUsByPartNumber.ContainsKey(keyOne) &&
                    !motherboardsByPartNumber.ContainsKey(keyOne) &&
                    !memoryByPartNumber.ContainsKey(keyOne)
                    ||
                    !CPUsByPartNumber.ContainsKey(keyTwo) &&
                    !motherboardsByPartNumber.ContainsKey(keyTwo) &&
                    !memoryByPartNumber.ContainsKey(keyTwo)
                    ||
                    !CPUsByPartNumber.ContainsKey(keyThree) &&
                    !motherboardsByPartNumber.ContainsKey(keyThree) &&
                    !memoryByPartNumber.ContainsKey(keyThree))
                {
                    Console.WriteLine(ErrorNoValidCombinations);
                    return;
                }

                if (CPUsByPartNumber.TryGetValue(keyOne, out var CPU) &&
                    motherboardsByPartNumber.TryGetValue(keyTwo, out var motherboard) &&
                    memoryByPartNumber.TryGetValue(keyThree, out var memory))
                {
                    ValidateConfiguration(CPU, motherboard, memory, combinations);
                }
                else if (CPUsByPartNumber.TryGetValue(keyOne, out CPU) &&
                    memoryByPartNumber.TryGetValue(keyTwo, out memory) &&
                    motherboardsByPartNumber.TryGetValue(keyThree, out motherboard))
                {
                    ValidateConfiguration(CPU, motherboard, memory, combinations);
                }
                else if (motherboardsByPartNumber.TryGetValue(keyOne, out motherboard) &&
                    CPUsByPartNumber.TryGetValue(keyTwo, out CPU) &&
                    memoryByPartNumber.TryGetValue(keyThree, out memory))
                {
                    ValidateConfiguration(CPU, motherboard, memory, combinations);
                }
                else if (motherboardsByPartNumber.TryGetValue(keyOne, out motherboard) &&
                    memoryByPartNumber.TryGetValue(keyTwo, out memory) &&
                    CPUsByPartNumber.TryGetValue(keyThree, out CPU))
                {
                    ValidateConfiguration(CPU, motherboard, memory, combinations);
                }
                else if (memoryByPartNumber.TryGetValue(keyOne, out memory) &&
                    CPUsByPartNumber.TryGetValue(keyTwo, out CPU) &&
                    motherboardsByPartNumber.TryGetValue(keyThree, out motherboard))
                {
                    ValidateConfiguration(CPU, motherboard, memory, combinations);
                }
                else if (memoryByPartNumber.TryGetValue(keyOne, out memory) &&
                    motherboardsByPartNumber.TryGetValue(keyTwo, out motherboard) &&
                    CPUsByPartNumber.TryGetValue(keyThree, out CPU))
                {
                    ValidateConfiguration(CPU, motherboard, memory, combinations);
                }
                else
                {
                    Console.WriteLine(ErrorSameComponentTypes);
                    return;
                }
            }

            if (combinations.Count > 0)
            {
                PrintCombinations(combinations);
            }
        }

        private static void ValidateConfiguration(
            CPU CPU,
            Motherboard motherboard,
            Memory memory,
            ICollection<Combination> combinations)
        {
            var isCPUValid = CPU.Socket == motherboard.Socket && CPU.SupportedMemory == memory.Type;

            if (!isCPUValid)
            {
                var sb = new StringBuilder();
                sb.AppendLine(ErrorConfigurationNotValid);
                var count = 1;

                if (CPU.SupportedMemory != memory.Type)
                {
                    sb.AppendFormat(ErrorAppendCount, count++)
                      .AppendFormat(ErrorMemoryIncompatible, memory.Type)
                      .AppendLine();
                }

                if (CPU.Socket != motherboard.Socket)
                {
                    sb.AppendFormat(ErrorAppendCount, count++)
                      .AppendFormat(ErrorMotherboardIncompatible, motherboard.Socket)
                      .AppendLine();
                }

                Console.WriteLine(sb.ToString().TrimEnd());
                return;
            }

            CreateCombination(combinations, CPU, motherboard, memory);
        }

        private static void CPUCompatibilityConfigurations(
            Motherboard motherboard,
            Memory memory,
            IReadOnlyDictionary<string, CPU> CPUsByPartNumber,
            ICollection<Combination> combinations)
        {
            var validCPUs = CPUsByPartNumber.Values
                        .Where(cpu => cpu.Socket == motherboard.Socket
                                   && cpu.SupportedMemory == memory.Type)
                        .ToList();

            if (validCPUs.Count == 0)
            {
                var errorMessage = ErrorConfigurationNotValid +
                    Environment.NewLine +
                    ErrorMemoryAndMotherboardIncompatibleWithCPU;
                Console.WriteLine(errorMessage, memory.Type, motherboard.Socket);
                return;
            }

            foreach (var CPU in validCPUs)
            {
                CreateCombination(combinations, CPU, motherboard, memory);
            }
        }

        private static void MemoryCompatibilityConfigurations(
            CPU CPU,
            Memory memory,
            IReadOnlyDictionary<string, Motherboard> motherboardsByPartNumber,
            ICollection<Combination> combinations)
        {
            if (CPU.SupportedMemory != memory.Type)
            {
                var errorMessage = ErrorConfigurationNotValid +
                    Environment.NewLine +
                    ErrorMemoryIncompatible;
                Console.WriteLine(errorMessage, memory.Type);
                return;
            }

            var validMotherboards = motherboardsByPartNumber.Values
                .Where(m => m.Socket == CPU.Socket);

            foreach (var motherboard in validMotherboards)
            {
                CreateCombination(combinations, CPU, motherboard, memory);
            }
        }

        private static void MotherboardCompatibilityConfigurations(
            CPU CPU,
            Motherboard motherboard,
            IReadOnlyDictionary<string, Memory> memoryByPartNumber,
            ICollection<Combination> combinations)
        {
            if (CPU.Socket != motherboard.Socket)
            {
                var errorMessage = ErrorConfigurationNotValid +
                    Environment.NewLine +
                    ErrorMotherboardIncompatible;
                Console.WriteLine(errorMessage, motherboard.Socket);
                return;
            }

            var validMemory = memoryByPartNumber.Values
                .Where(m => m.Type == CPU.SupportedMemory);

            foreach (var memory in validMemory)
            {
                CreateCombination(combinations, CPU, motherboard, memory);
            }
        }

        private static void CPUOnlyInputConfigurations(
            CPU CPU,
            IReadOnlyDictionary<string, Motherboard> motherboardsByPartNumber,
            IReadOnlyDictionary<string, Memory> memoryByPartNumber,
            ICollection<Combination> combinations)
        {
            var motherboards = motherboardsByPartNumber.Values
                .Where(m => m.Socket == CPU.Socket);

            var memoryData = memoryByPartNumber.Values
                .Where(m => m.Type == CPU.SupportedMemory);

            foreach (var motherboard in motherboards)
            {
                foreach (var memory in memoryData)
                {
                    CreateCombination(combinations, CPU, motherboard, memory);
                }
            }
        }

        private static void MotherboardOnlyInputConfigurations(
            Motherboard motherboard,
            IReadOnlyDictionary<string, CPU> CPUsByPartNumber,
            IReadOnlyDictionary<string, Memory> memoryByPartNumber,
            ICollection<Combination> combinations)
        {
            var CPUs = CPUsByPartNumber.Values
                        .Where(cpu => cpu.Socket == motherboard.Socket);

            foreach (var CPU in CPUs)
            {
                var memoryData = memoryByPartNumber.Values
                    .Where(m => m.Type == CPU.SupportedMemory);

                foreach (var memory in memoryData)
                {
                    CreateCombination(combinations, CPU, motherboard, memory);
                }
            }
        }

        private static void MemoryOnlyInputConfigurations(
            Memory memory,
            IReadOnlyDictionary<string, CPU> CPUsByPartNumber,
            IReadOnlyDictionary<string, Motherboard> motherboardsByPartNumber,
            ICollection<Combination> combinations)
        {
            var CPUs = CPUsByPartNumber.Values
                        .Where(cpu => cpu.SupportedMemory == memory.Type);

            foreach (var CPU in CPUs)
            {
                var motherboards = motherboardsByPartNumber.Values
                    .Where(m => m.Socket == CPU.Socket);

                foreach (var motherboard in motherboards)
                {
                    CreateCombination(combinations, CPU, motherboard, memory);
                }
            }
        }

        private static void CreateCombination(
            ICollection<Combination> combinations,
            CPU CPU,
            Motherboard motherboard,
            Memory memory)
        {
            var combination = new Combination
            {
                CPU = CPU.ToString(),
                Motherboard = motherboard.ToString(),
                Memory = memory.ToString(),
                Price = CPU.Price + motherboard.Price + memory.Price,
            };

            combinations.Add(combination);
        }

        private static void PrintCombinations(IList<Combination> combinations)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(CombinationsCount, combinations.Count)
              .AppendLine();

            for (int i = 0; i < combinations.Count; i++)
            {
                sb.AppendFormat(CombinationAppendCount, i + 1)
                  .AppendLine()
                  .AppendLine(combinations[i].ToString());
            }

            Console.WriteLine(sb.ToString().TrimEnd());
        }
    }
}