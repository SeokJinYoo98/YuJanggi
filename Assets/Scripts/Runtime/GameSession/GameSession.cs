using System.Collections.Generic;
using Yujanggi.Core.Domain;
using Yujanggi.Core.Match;

namespace Yujanggi.Runtime.GameSession
{
    public class        GameSession
    {
        public GameSession(
            GameSessionInfo  sessionInfo,
            GameSessionView  sessionView,
            MatchManager     sessionMatch,
            IPlayerController cho,
            IPlayerController han)
        {
            _sessionView        = sessionView;
            _sessionInfo        = sessionInfo;
            _sessionMatch       = sessionMatch;
            _playerCho          = cho;
            _playerHan          = han;
        }

        public void BindEvents()
        {
            _playerCho.BindEvents(); 
            _playerHan.BindEvents();
            _sessionMatch.BindEvents();
            _sessionView.BindUI(_sessionMatch);

            var events = _sessionMatch.MatchEvent;
            events.OnGameEnded        += HandleGameEnded;
            events.OnTurnChanged      += HandleTurnChanged;
            events.OnPieceMoved       += HandlePieceMoved;
            events.OnCheckOccurred    += HandleCheckOccured;
            events.OnCheckReleased    += HandleCheckReleased;

            _playerCho.OnMoveRequest  += HandleTryMove;
            _playerHan.OnMoveRequest  += HandleTryMove;
            if (_playerCho is ILocalPlayer local1)
                local1.OnSelectionChanged += HandleSelectionChanged;
            if (_playerHan is ILocalPlayer local2)
                local2.OnSelectionChanged += HandleSelectionChanged;
        }
        public void UnBindEvents()
        {
            _playerCho.UnBindEvents(); 
            _playerHan.UnBindEvents();
            _sessionMatch.UnBindEvents();
            _sessionView.UnBindUI(_sessionMatch);

            var events  = _sessionMatch.MatchEvent;
            events.OnTurnChanged      -= HandleTurnChanged;
            events.OnGameEnded        -= HandleGameEnded;
            events.OnPieceMoved       -= HandlePieceMoved;
            events.OnCheckOccurred    -= HandleCheckOccured;
            events.OnCheckReleased    -= HandleCheckReleased;

            _playerCho.OnMoveRequest -= HandleTryMove;
            _playerHan.OnMoveRequest -= HandleTryMove;
            if (_playerCho is ILocalPlayer local1)
                local1.OnSelectionChanged -= HandleSelectionChanged;
            if (_playerHan is ILocalPlayer local2)
                local2.OnSelectionChanged -= HandleSelectionChanged;
        }
        public void Handicap()
            => _sessionMatch.Handicap();
        public void GiveUp()
        {
            DisableAllControllers();
            var info = _sessionMatch.GiveUp();
            HandleGameEnded(info);
        }
        public void StartGame()
        {
            _sessionMatch.StartGame(_sessionInfo.ChoFormation, _sessionInfo.HanFormation);
            _playerCho.BeginTurn();
            _playerHan.EndTurn();
            _sessionView.StartGame(_sessionMatch.Board);
        }
        public void ResetGame()
        {
            StartGame();
            var board = _sessionMatch.Board;
            _sessionView.ResetGame(board);
        }
        public void UnDo()
        {
            if (!_sessionMatch.TryUnDo(out var ctx)) return;
            if (ctx.IsHandicap)
                return;
            _sessionView.UnDo(in ctx);
        }
        public void Update(float deltaTime)
            => _sessionMatch.Update(deltaTime);

        #region 멤버변수
        private readonly IPlayerController  _playerCho;
        private readonly IPlayerController  _playerHan;
        private readonly GameSessionInfo    _sessionInfo;
        private readonly GameSessionView    _sessionView;
        private readonly MatchManager       _sessionMatch;
        private SessionDisplayMode _mode = SessionDisplayMode.Live;
        #endregion

        private void              HandleTryMove(Pos from, Pos to)
        {
            _sessionMatch.TryMove(from, to);
        }
        private void              HandleCheckReleased()
        {
            _sessionView.CheckReleased();
        }
        private void              HandleCheckOccured(PlayerTeam team)
        {
            _sessionView.CheckOccured(team);
        }
        private void              HandlePieceMoved(MoveRecord record)
        {
            _sessionView.PieceMoved(in record);
        }
        private void              HandleSelectionChanged(int? pieceId, IReadOnlyList<Pos> legalCells, IReadOnlyList<Pos> illegalCells)
        {
            _sessionView.SelectionChanged(pieceId, legalCells, illegalCells);
        }
        private void              HandleTurnChanged(PlayerTeam next)
        {
            var nextPlayer = BeginNextTurn(next);
            bool isLocal = nextPlayer is ILocalPlayer;
            _sessionView.OnTurnChanged(next, isLocal);
        }
        private void              HandleGameEnded(GameResultInfo info)
        {
            DisableAllControllers();
            var winnerIsLocal = GetPlayer(info.Winner) is ILocalPlayer;
            _sessionView.OnGameEnded(winnerIsLocal, in info);
        }
        private void              DisableAllControllers()
        {
            _playerCho.EndTurn(); _playerHan.EndTurn();
        }
        private IPlayerController BeginNextTurn(PlayerTeam turn)
        {
            DisableAllControllers();
            if (turn == PlayerTeam.Cho)
            {
                _playerCho.BeginTurn();
                return _playerCho;
            }
            _playerHan.BeginTurn();
            return _playerHan;
        }
        private IPlayerController GetPlayer(PlayerTeam team)
            => team == PlayerTeam.Cho ? _playerCho : _playerHan;

    }
}
