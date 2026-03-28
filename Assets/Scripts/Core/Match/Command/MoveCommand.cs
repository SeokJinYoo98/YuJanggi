using Yujanggi.Core.Domain;

namespace Yujanggi.Core.Match
{
    public class MoveCommand : ICommand
    {
        private readonly Pos _from;
        private readonly Pos _to;
        
        public MoveCommand(Pos from, Pos to)
        {
            _from = from;
            _to = to;
        }
        public bool Execute(MatchManager match)
        {
            throw new System.NotImplementedException();
        }

        public void Undo(MatchManager match)
        {
            throw new System.NotImplementedException();
        }
    }
}