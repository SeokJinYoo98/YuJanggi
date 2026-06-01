using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Yujanggi.Runtime.Input
{
    using Core.Domain;

    public class PcInputHandler : MonoBehaviour, IInputHandler
    {
        [SerializeField] private Camera     _camera;
        [SerializeField] private LayerMask  _clickableLayer;
        private bool _isActivate = true;
        public event Action<Pos> OnBoardClicked;
        public event Action      OnEmptyClicked;

        private PlayerInputs _input;
        private PlayerInputs.PlayerActions _actions;
        private void OnPressPerformed(InputAction.CallbackContext context)
        {
            if (!_isActivate) 
                return;

            if (!TryRaycastToBoard(out var pos))
            {
                OnEmptyClicked?.Invoke();
                return;
            }    
      
            OnBoardClicked?.Invoke((pos));
        }

        void Awake()
        {
            _input = new PlayerInputs();
            _actions = _input.Player;
        }    
        private void OnEnable()
        {
            _actions.Mouse.Enable();
            _actions.MousePos.Enable();
            _actions.Mouse.performed += OnPressPerformed;

        }
        private void OnDisable()
        {
            _actions.Mouse.performed -= OnPressPerformed;
            _actions.MousePos.Disable();
            _actions.Mouse.Disable();
        }

        public void RotateCamera(PlayerTeam team)
        {
            if (team == PlayerTeam.Han)
            {
                _camera.transform.position = new Vector3(4, 9, 6);
                _camera.transform.eulerAngles = new Vector3(90, 0, 180);
            }
            else
            {
                _camera.transform.position = new Vector3(4, 9, 3);
                _camera.transform.eulerAngles = new Vector3(90, 0, 0);
            }
        }
        public void Activate()   => _isActivate = true;
        public void Deactivate() => _isActivate = false;

        private bool TryRaycastToBoard(out Pos pos)
        {
            pos = default;

            if (_camera == null)
                return false;

            Vector2 mousePos = _actions.MousePos.ReadValue<Vector2>();

            Ray ray = _camera.ScreenPointToRay(mousePos);

            if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, _clickableLayer))
                return false;

            if (!hit.collider.TryGetComponent(out IBoardClickable clickable))
                return false;
            
            pos = clickable.BoardPos;

            return true;
        }
    }
}