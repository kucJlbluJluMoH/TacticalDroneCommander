using System.Threading;

namespace TacticalDroneCommander.Core.States
{
    public interface IGameState
    {
        GameState StateType { get; }
        void Enter(CancellationToken cancellationToken);
        void Exit();
        void Update();
        bool CanTransitionTo(GameState newState);
    }
}

