using SDSL.Prototypes;

namespace SDSL;

public class SealNumber
{
    [FunctionExport("floor() -> Number")]
    public static SealValue Floor(SealValue self, ReadOnlySpan<SealValue> args)
    {
        return Math.Floor(self.AsNumber());
    }
}