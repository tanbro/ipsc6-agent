namespace ipsc6.agent.client
{
    public enum WorkType
    {
        Unknown = 0,
        CallIn = 1,
        Transfer = 2,
        SysCall = 3,
        HandCall = 4,
        Consult = 5,
        Flow = 6,
        ForceInsert = 7,
        Listen = 8,
        Hold = 9,
        PauseBusy = 10,
        PauseLeave = 11,
        PauseTyping = 12,
        PauseForce = 13,
        PauseDisconnect = 14,
        PauseSnooze = 15,
        PauseDinner = 16,
        PauseTrain = 17,
    }
}
