namespace Backrooms.Core
{
    public interface IState
    {
        void Enter();
        void Tick();
        void Exit();
    }
}