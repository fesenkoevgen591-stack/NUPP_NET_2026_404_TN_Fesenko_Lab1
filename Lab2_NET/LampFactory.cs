
using SmartHome.Common;

public static class LampFactory
{
    public static Lamp CreateNew()
    {
        return new Lamp(
            $"Lamp-{Guid.NewGuid():N}",
            Random.Shared.Next(1, 101),
            new[] { "White", "Yellow", "Blue", "Red" }
                [Random.Shared.Next(4)],
            Math.Round(Random.Shared.NextDouble() * 95 + 5, 2)
        );
    }
}
