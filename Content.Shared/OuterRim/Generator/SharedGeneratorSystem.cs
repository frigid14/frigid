namespace Content.Shared.OuterRim.Generator;

/// <summary>
/// This handles small, portable generators that run off a material fuel.
/// </summary>
public abstract class SharedGeneratorSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {

    }

    public static float CalcFuelEfficiency(float targetPower)
    {
        return (float)(targetPower/2 + 0.2);
    }
}
