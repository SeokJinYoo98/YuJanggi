using UnityEngine;
using UnityEngine.InputSystem;

namespace Yujanggi.Runtime.Input
{
    using System;
    using Yujanggi.Core.Domain;
    using Yujanggi.Core.Match;

    public class LocalPlayerController : MonoBehaviour, IParticipantController, ILocalPlayer
    {
        [SerializeField] private Camera _camera;

        private PlayerInputs _input;
        private PlayerInputs.PlayerActions _actions;

        public bool CanInput => _canInput;
        private bool _canInput = true;


        public void SetInputEnabled(bool enabled)
            => _canInput = enabled;
       
        private void Awake()
        {
            _input = new PlayerInputs();
            _actions = _input.Player;
        }

        // 인풋
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


        public event Action<Pos> OnBoardClicked;
        private void OnPressPerformed(InputAction.CallbackContext context)
        {
            if (!_canInput) return;
            OnBoardClicked?.Invoke(Clicked());
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
                // z 6
                // y 180
                _camera.transform.position = new Vector3(4, 9, 6);
                _camera.transform.eulerAngles = new Vector3(90, 0, 180);
            }
            else
            {
                _camera.transform.position = new Vector3(4, 9, 3);
                _camera.transform.eulerAngles = new Vector3(90, 0, 0);
            }
        }
    }
}


