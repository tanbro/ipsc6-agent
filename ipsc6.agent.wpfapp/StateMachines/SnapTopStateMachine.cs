using Stateless;

namespace ipsc6.agent.wpfapp.StateMachines
{
    public class SnapTopStateMachine : StateMachine<SnapTopState, SnapTopTrigger>
    {
        public SnapTopStateMachine() : base(SnapTopState.Initial)
        {
            Configure(SnapTopState.Initial)
                .Permit(SnapTopTrigger.Timer, SnapTopState.Snapped)
                .Permit(SnapTopTrigger.MoveOut, SnapTopState.Final);

            Configure(SnapTopState.Snapped)
                .Permit(SnapTopTrigger.MouseEnter, SnapTopState.SnappedWithMouseEnter);

            Configure(SnapTopState.SnappedWithMouseEnter)
                .Permit(SnapTopTrigger.MouseLeave, SnapTopState.Snapped)
                .Permit(SnapTopTrigger.Timer, SnapTopState.Unsnapped)
                .Permit(SnapTopTrigger.MoveOut, SnapTopState.Final);

            Configure(SnapTopState.Unsnapped)
                .Permit(SnapTopTrigger.MouseLeave, SnapTopState.UnsnappedWithMouseLeave)
                .Permit(SnapTopTrigger.MoveOut, SnapTopState.Final);

            Configure(SnapTopState.UnsnappedWithMouseLeave)
                .Permit(SnapTopTrigger.MouseEnter, SnapTopState.Unsnapped)
                .Permit(SnapTopTrigger.Timer, SnapTopState.Snapped)
                .Permit(SnapTopTrigger.MoveOut, SnapTopState.Final);
        }

    }
}
