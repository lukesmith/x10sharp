namespace x10sharp
{
    public enum FunctionCode
    {
        AllUnitsOff = 0x00,
        AllLightsOn = 0x01,
        On = 0x02,
        Off = 0x03,
        Dim = 0x04,
        Bright = 0x05,
        AllLightsOff = 0x06,
        ExtendedCode = 0x07,
        HailRequest = 0x08,
        HailAcknowledge = 0x09,
        PreSetDimHigh = 0x0A,
        PreSetDimLow = 0x0B,
        ExtendedDataAnalog = 0x0C,
        StatusOn = 0x0D,
        StatusOff = 0x0E,
        StatusRequest = 0x0F
    }
}
