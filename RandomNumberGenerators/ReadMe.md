
# Random Number Generators Library

## Overview

This C# project provides a comprehensive library of random number generators (RNGs) tailored for various applications, including cryptographic, deterministic, and non-uniform number generation. The library is modular, allowing users to select specific RNGs or combine them as needed.

You can find additional info in the original [article](https://github.com/Hawkynt/Randomizer) about it.

## Project Structure

The project is organized into the following directories:

- **Composites**: Contains classes that combine multiple RNGs to create more complex or resilient generators.
- **Cryptographic**: Includes RNGs designed for cryptographic applications, focusing on security and unpredictability.
- **Deterministic**: Contains deterministic RNGs, which generate reproducible sequences of numbers given the same initial seed.
- **Interfaces**: Defines interfaces that all RNGs in this library implement, ensuring consistency and interoperability.
- **NonUniform**: Includes RNGs that produce non-uniform distributions, such as Gaussian or Poisson distributions.

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.

### Building the Project

To build the project, navigate to the project directory and run:

```bash
dotnet build
```

### Running Tests

If the project includes unit tests (typically in a `Tests` or similar directory), you can run them using:

```bash
dotnet test
```

### Using the Library

You can reference this library in your projects by adding the `RandomNumberGenerators` project as a dependency or by including the compiled DLL in your project.

### Example Usage

```csharp
using RandomNumberGenerators.Interfaces;

public class Example
{
    public static void Main()
    {
        IRandomNumberGenerator rng = new SomeSpecificRNG();
        ulong randomNumber = rng.Next();
        Console.WriteLine(randomNumber);
    }
}
```

Replace `SomeSpecificRNG` with the actual class name of the RNG you wish to use.

## Contributing

Contributions are welcome! Please feel free to submit issues, fork the repository, and send pull requests.

## License

This project is licensed under the MIT License. See the `LICENSE` file for more details.

## Contact

For any questions or issues, please open an issue on the GitHub repository or contact the maintainers.
