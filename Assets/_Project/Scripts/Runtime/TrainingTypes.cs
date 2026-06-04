namespace DailyEmergencyResponseVR
{
    public enum ZoneType
    {
        Accident,
        Fire,
        Evacuation
    }

    public enum InteractionType
    {
        ButtonPress,
        LeverPull,
        CoverOpen,
        GrabUse,
        ConnectionConfirm
    }

    public enum DeviceType
    {
        EmergencyCall,
        FireExtinguisher,
        AED,
        EmergencyDoorRelease,
        GuideLight,
        FireHydrant,
        EscalatorStopButton,
        TrainEmergencyDevice
    }

    public enum ScenarioType
    {
        MedicalPatient,
        EscalatorAccident,
        TrainInteriorAccident,
        SmallFire,
        LargeFire,
        SmokeEvacuation
    }

    public enum TrainingFlowState
    {
        Start,
        ModeSelect,
        LearningSelect,
        DeviceLearning,
        ScenarioLearning,
        Summary
    }
}
