using UnityEngine;

namespace Yujanggi.Runtime.Player
{
    using Core.Domain;
    using Input;
    using UnityEngine.InputSystem;

    namespace Yujanggi.Runtime.Controller
    {
        using Board;
        using Manager;

        public class JanggiController : MonoBehaviour, IPlayerController
        {
            [SerializeField] private Camera     _camera;
            [SerializeField] private GameManager _gm;

            private PlayerInputs               _input;
            private PlayerInputs.PlayerActions _actions;

            private PlayerTeam _type = PlayerTeam.Cho;
            public PlayerTeam   Type => _type;
            public void Init(PlayerTeam type)
                => _type = type;
            private void Awake()
            {
                _input = new PlayerInputs();
                _actions = _input.Player;
            }


            // 인풋
            private void OnEnable()
            {
                //_actions.Mouse.started   += OnPressStarted;
                _actions.Mouse.performed += OnPressPerformed;
               // _actions.Mouse.canceled  += OnPressCanceled;

                _actions.Mouse.Enable();


                _actions.MousePos.Enable();
            }
            private void OnDisable()
            {
                _actions.Mouse.Disable();

                //_actions.Mouse.started   -= OnPressStarted;
                _actions.Mouse.performed -= OnPressPerformed;
                //_actions.Mouse.canceled  -= OnPressCanceled;

                _actions.MousePos.Disable();
            }
            private void OnPressPerformed(InputAction.CallbackContext context)
            {
                var pos = Clicked();
                _gm.HandleClick(pos.x, pos.z);
            }
            private (int x, int z) Clicked()
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
                return (x, z);
            }
        }
    }

}
