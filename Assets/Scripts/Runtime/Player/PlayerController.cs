using UnityEngine;

namespace Yujanggi.Runtime.Player
{
    using Core.Domain;
    using Input;
    using UnityEngine.InputSystem;

    namespace Yujanggi.Runtime.Player
    {
        using Board;
        using global::Yujanggi.Core.Board;

        public class PlayerController : MonoBehaviour, IPlayer
        {
            [SerializeField] private Camera _camera;
            [SerializeField] private Board  _board;

            private PlayerInputs               _input;
            private PlayerInputs.PlayerActions _actions;

            private PlayerType _type = PlayerType.Cho;
            public PlayerType  Type => _type;
            public void Init(PlayerType type)
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
                if (!BoundaryCheck(pos.x, pos.z)) return;
                _board.HandleClick(pos.x, pos.z, Type);
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
            bool BoundaryCheck(int x, int z)
            {
                bool isValid = false;
                if (BoardData.WIDTH - BoardData.WIDTH <= x && x < BoardData.WIDTH)
                    if (BoardData.HEIGHT - BoardData.HEIGHT <= z && z < BoardData.HEIGHT)
                        isValid = true;
                return isValid;
            }
            
        }
        // 클릭, 땠을때 명령을 전송한다.
        // 플레이어 팀, 위치
        // 그리드 좌표 변환
        // 클릭 시스템 추가
    }

}
