using System;
using UnityEngine;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.Input
{
    public abstract class InputHandlerBehaviour : MonoBehaviour, IInputHandler
    {
        public abstract event Action<Pos> OnBoardClicked;
        public abstract event Action OnEmptyClicked;

        public abstract void Activate();
        public abstract void Deactivate();
        public abstract void RotateCamera(PlayerTeam team);
    }

    public interface IBoardClickable
    {
        public Pos BoardPos { get; }
    }
}