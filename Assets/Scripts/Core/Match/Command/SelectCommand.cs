

namespace Yujanggi.Core.Match
{
    public interface ICommand
    {
        bool Execute(MatchManager match);
        void Undo(MatchManager match);
    }

    public class SelectCommand : ICommand
    {
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