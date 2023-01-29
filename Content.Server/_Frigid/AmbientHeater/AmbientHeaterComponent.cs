namespace Content.Server._Frigid.AmbientHeater;

[RegisterComponent]
public sealed class AmbientHeaterComponent : Component
{
    [DataField("targetTemperature")]
    [ViewVariables(VVAccess.ReadWrite)]
    public float TargetTemperature = 293.15f;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("heatPerSecond")]
     public float HeatPerSecond = 100f;

     public bool Powered = false;
}
