using System;
using UnityEngine;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.Input
{
    public abstract class InputHandlerBehaviour : MonoBehaviour, IInputHandler
    {
        public event Action<Pos> OnBoardClicked;
        public event Action OnEmptyClicked;

        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }

        public void RotateCamera(PlayerTeam team)
        {
            throw new NotImplementedException();
        }
    }

    public interface IBoardClickable
    {
        public Pos BoardPos { get; }
    }
}