using UnityEngine;

namespace Yujanggi.Runtime.Player
{
    using Input;
    using UnityEngine.InputSystem;
    using Core.Domain;

    namespace Yujanggi.Runtime.Player
    {
        using Board;
        public class PlayerController : MonoBehaviour, IPlayer
        {
            [SerializeField] private Camera _camera;
            [SerializeField] private Board _board;
            private PlayerInputs _input;
            private PlayerInputs.PlayerActions _actions;

            private PlayerType _type = PlayerType.Cho;
            public PlayerType Type => _type;
            public void Init(PlayerType type)
                => _type = type;
            private void Awake()
            {
                _input = new PlayerInputs();
                _actions = _input.Player;
            }

            private void OnEnable()
            {
                _actions.Mouse.started   += OnPressStarted;
                _actions.Mouse.performed += OnPressPerformed;
                _actions.Mouse.canceled  += OnPressCanceled;

                _actions.Mouse.Enable();


                _actions.MousePos.Enable();
            }
            private void OnDisable()
            {
                _actions.Mouse.Disable();

                _actions.Mouse.started   -= OnPressStarted;
                _actions.Mouse.performed -= OnPressPerformed;
                _actions.Mouse.canceled  -= OnPressCanceled;

                _actions.MousePos.Disable();
            }
            private void OnPressStarted(InputAction.CallbackContext context)
            {
                // 버튼 누름 시작
                Debug.Log("마우스 눌림");
                Clicked();
            }
            private void OnPressPerformed(InputAction.CallbackContext context)
            {
                // 버튼 뗐을 때 이벤트 발생
                Debug.Log("마우스 떨어짐");
                Clicked();
            }
            private void OnPressCanceled(InputAction.CallbackContext context)
            {
                // 드래그 등으로 취소될 경우
                Debug.Log("마우스 취소");
            }

            private void Clicked()
            {
                var mousePos = _actions.MousePos.ReadValue<Vector2>();
                Ray ray = _camera.ScreenPointToRay(mousePos);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    var pos = hit.point;

                    int x = Mathf.RoundToInt(pos.x);
                    int z = Mathf.RoundToInt(pos.z);

                    _board.OnClickCell(x, z, Type);
                }
            }

        }
        // 클릭, 땠을때 명령을 전송한다.
        // 플레이어 팀, 위치
        // 그리드 좌표 변환
        // 클릭 시스템 추가
    }

}
