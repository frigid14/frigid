using Content.Server.Materials;
using Content.Server.Power.Components;
using Content.Server.Stack;
using Content.Shared.Construction.Components;
using Content.Shared.Interaction;
using Content.Shared.Materials;
using Content.Shared.OuterRim.Generator;
using Robust.Server.GameObjects;

namespace Content.Server.OuterRim.Generator;

/// <inheritdoc/>
public sealed class GeneratorSystem : SharedGeneratorSystem
{
    [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<SharedGeneratorComponent, InteractUsingEvent>(OnInteractUsing);
        SubscribeLocalEvent<SharedGeneratorComponent, SetTargetPowerMessage>(OnTargetPowerSet);
    }

    private void OnTargetPowerSet(EntityUid uid, SharedGeneratorComponent component, SetTargetPowerMessage args)
    {
        component.TargetPower = args.TargetPower;
    }

    private void OnInteractUsing(EntityUid uid, SharedGeneratorComponent component, InteractUsingEvent args)
    {
        if (!TryComp(args.Used, out MaterialComponent? mat) || !TryComp(args.Used, out StackComponent? stack))
            return;

        if (!mat.MaterialIds.Contains(component.FuelMaterial))
            return;

        component.RemainingFuel += stack.Count;
        QueueDel(args.Used);
        return;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        foreach (var (gen, supplier, xform) in EntityQuery<SharedGeneratorComponent, PowerSupplierComponent, TransformComponent>())
        {
            supplier.Enabled = !(gen.RemainingFuel <= 0.0f || xform.Anchored == false);

            float maxFuelRate = 90f/3600; // 90 sheets in 60 minutes = 0.025 sheets/sec
            var fuelRate = gen.TargetPower * maxFuelRate;
            gen.RemainingFuel = MathF.Max(gen.RemainingFuel - (fuelRate * frameTime), 0.0f);

            // Plasma: 400 kJ/sheet
            supplier.MaxSupply = fuelRate * 400000f * CalcFuelEfficiency(gen.TargetPower);

            UpdateUi(gen);
        }
    }

    private void UpdateUi(SharedGeneratorComponent comp)
    {
        if (!_uiSystem.IsUiOpen(comp.Owner, GeneratorComponentUiKey.Key))
            return;

        _uiSystem.TrySetUiState(comp.Owner, GeneratorComponentUiKey.Key, new GeneratorComponentBuiState(comp));
    }
}
