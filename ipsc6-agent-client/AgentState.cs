namespace ipsc6.agent.client
{
    public enum AgentState
    {
        NotExist = -2,
        OffLine = -1,
        OnLine = 0,
        Block = 1,
        Pause = 2,
        Leave = 3,
        Idle = 4,
        Ring = 5,
        Work = 6,
        WorkPause = 7,
    }
}
