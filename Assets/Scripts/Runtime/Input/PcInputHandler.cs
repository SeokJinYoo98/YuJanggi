

using UnityEngine;

namespace Yujanggi.Runtime.Input
{
    using Core.Domain;
    using System;
    using UnityEngine.InputSystem;

    public class PcInputHandler : MonoBehaviour, IInputHandler
    {
        [SerializeField] private Camera _camera;
        private bool _isActivate = true;
        public event Action<Pos> OnBoardClicked;


        private PlayerInputs _input;
        private PlayerInputs.PlayerActions _actions;
        private void OnPressPerformed(InputAction.CallbackContext context)
        {
            if (!_isActivate) return;
            OnBoardClicked?.Invoke(Clicked());
        }

        void Awake()
        {
            _input = new PlayerInputs();
            _actions = _input.Player;
        }    
        private void OnEnable()
        {
            _actions.Mouse.performed += OnPressPerformed;
            _actions.Mouse.Enable();
            _actions.MousePos.Enable();
        }
        private void OnDisable()
        {
            _actions.Mouse.Disable();
            _actions.Mouse.performed -= OnPressPerformed;
            _actions.MousePos.Disable();
        }
        private Pos Clicked()
        {
            int x = -10, z = -10;
            var mousePos = _actions.MousePos.ReadValue<Vector2>();
            Ray ray = _camera.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var pos = hit.point;

                x = Mathf.RoundToInt(pos.x);
                z = Mathf.RoundToInt(pos.z);
            }
            return new Pos(x, z);
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

    }
}