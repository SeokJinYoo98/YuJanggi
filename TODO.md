# Mobile Expansion TODO

## 1. PC와 모바일을 함께 지원하는 포인터 입력 계층 구성
- 대상: `Assets/Scripts/Runtime/Input/PcInputHandler.cs`, `Assets/Scripts/Runtime/Input/IInput.cs`, `Assets/Scripts/Core/Domain/Domain.cs`의 `IInputHandler`, `Assets/Scripts/Runtime/Game/GameManager.cs`
- 해야 할 일: 기존 마우스 클릭 동작을 유지하면서 터치 위치를 동일한 보드 좌표 이벤트로 변환하고, `GameManager`의 `PcInputHandler` 구체 타입 의존을 모바일 입력 구현도 주입할 수 있는 형태로 정리한다. `PlayerInputs`의 Touch 바인딩 및 Android에서의 Input System 설정은 추가 확인 필요.
- 필요한 이유: 현재 입력은 `Mouse`와 `MousePos` 액션만 읽으며 조립 지점도 `PcInputHandler`에 고정되어 있어 Android 터치 입력을 세션 흐름으로 전달할 경로가 없다.
- 완료 조건: PC 클릭과 Android 단일 터치가 모두 `IInputHandler` 이벤트를 통해 같은 `LocalController` 선택/이동 흐름을 사용하고, 빈 공간 터치 및 입력 활성화/비활성화가 두 플랫폼에서 동일하게 동작한다.

## 2. 터치 드래그 및 드롭 상호작용 추가
- 대상: `Assets/Scripts/Runtime/Input/PcInputHandler.cs`, `Assets/Scripts/Runtime/Controller/LocalController.cs`, `Assets/Scripts/Runtime/GameSession/GameSession.cs`, `Assets/Scripts/Runtime/GameSession/State/SessionLiveState.cs`, `Assets/Scripts/Runtime/GameSession/MatchView.cs`, `Assets/Scripts/Runtime/Board/BoardView.cs`, `Assets/Scripts/Runtime/Piece/PieceView.cs`
- 해야 할 일: Runtime 입력 계층에서 press/move/release와 드래그 임계값을 처리하고, 드래그 중에는 선택된 말의 화면 위치만 바꾼다. 드롭 시에는 보드 좌표의 이동 요청을 기존 `LocalController` -> `GameSession` -> `SessionLiveState` -> `MatchModel.TryMove` 경로로 전달하며, 취소되거나 불법인 드롭은 View를 Core 상태에 맞게 복구한다.
- 필요한 이유: 작은 모바일 화면에서는 탭 두 번만으로 말을 이동하는 방식보다 직접 조작 가능한 드래그가 필요하지만, 현재 입력 계약에는 클릭 이벤트만 있고 `PieceView`는 완료된 이동 Tween만 처리한다.
- 완료 조건: 유효한 드롭만 Core 이동으로 확정되고, 드래그 중에는 `BoardModel`이 변경되지 않으며, 불법 드롭·보드 밖 드롭·멀티터치·입력 비활성 상태의 동작이 정의되고 검증된다. 멀티터치 정책과 드래그 감도 기준은 추가 확인 필요.

## 3. 모바일 화면 비율, Safe Area, 터치 UI 대응
- 대상: `Assets/Scripts/Runtime/UI`, `Assets/Scripts/Runtime/Game/GameManager.cs`, `Assets/Scripts/Runtime/Game/LobbyManager.cs`, `Assets/Scripts/Runtime/Input/PcInputHandler.cs`, `Assets/Scripts/Runtime/Board`, 관련 Scene/Prefab은 추가 확인 필요
- 해야 할 일: 세로/가로 방향 정책을 정한 뒤 노치와 시스템 바 Safe Area를 반영하고, 로비·매치·결과·리플레이 UI의 앵커, Canvas Scaler, 터치 영역과 텍스트 가독성을 주요 Android 해상도에서 조정한다. 카메라의 고정 위치/회전과 `MatchUI`의 장군 텍스트 고정 좌표가 화면 비율별로 올바른지도 검증한다.
- 필요한 이유: 스크립트에는 Safe Area 또는 화면 크기 대응 코드가 없고, 카메라 위치 및 UI 애니메이션 좌표가 고정값이어서 기기 비율에 따라 보드나 조작 UI가 잘리거나 겹칠 수 있다.
- 완료 조건: 지원할 최소/최대 화면 비율에서 보드 전체와 필수 조작이 안전 영역 안에 표시되고, 모든 버튼과 드롭다운이 터치 가능한 크기를 가지며, Cho/Han 카메라 방향 모두에서 입력 Raycast와 화면 표시가 일치한다. 현재 Canvas 설정과 지원 방향은 추가 확인 필요.

## 4. Android 생명주기와 시스템 뒤로가기 처리
- 대상: `Assets/Scripts/Runtime/Game/LobbyManager.cs`, `Assets/Scripts/Runtime/Game/GameManager.cs`, `Assets/Scripts/Runtime/GameSession/GameSession.cs`, `Assets/Scripts/Runtime/Controller/AIController.cs`, `Assets/Scripts/Core/Domain/Domain.cs`의 `GameSessionStore`
- 해야 할 일: 앱 일시정지/복귀, 포커스 상실, 시스템 뒤로가기 시의 입력·턴 타이머·AI 비동기 작업·Scene 전환 정책을 정의하고 Runtime 생명주기에서 일관되게 처리한다. 프로세스 종료 후 진행 중 대국 복구가 제품 요구사항인지와 저장 형식은 추가 확인 필요.
- 필요한 이유: 현재는 `OnApplicationQuit` 외에 모바일 생명주기 처리가 없고 세션 시작 정보는 정적 `GameSessionStore`에만 저장된다. Android 백그라운드 전환이나 OS 종료 시 타이머, AI 작업, 세션 정보가 예상과 다르게 동작할 수 있다.
- 완료 조건: 홈 이동과 복귀, 화면 잠금, 앱 전환, 시스템 뒤로가기를 반복해도 입력 중복·AI 중복 실행·비정상 턴 경과가 없고, 로비와 대국 화면의 뒤로가기 결과 및 필요한 세션 보존 범위가 명세대로 동작한다.

## 5. Android 빌드 호환성, 성능, 실제 단말 검증
- 대상: `Assets/Scripts/Runtime/Particle`, `Assets/Scripts/Runtime/Piece/PieceView.cs`, `Assets/Scripts/Runtime/GameSession/ReplayView.cs`, `Assets/Scripts/Runtime/Controller/AIController.cs`, `Assets/Scripts/Runtime/Game/LobbyManager.cs`, `Assets/Scripts/Runtime/Audio`, Android `ProjectSettings`와 패키지 구성은 추가 확인 필요
- 해야 할 일: Input System, DOTween, UniTask, Adaptive Performance 참조의 Android/IL2CPP 호환성을 확인하고, 60 FPS 설정에서 파티클 풀·Tween·AI 계산·리플레이·오디오의 CPU/GPU/메모리 및 발열을 프로파일링한다. Android SDK/API 레벨, ARM64, 그래픽 API, 권한, 품질 단계와 빌드 백엔드 설정은 추가 확인 필요.
- 필요한 이유: Runtime은 여러 외부 패키지와 Unity 기능에 의존하며 `Application.targetFrameRate = 60`을 고정하지만, 현재 스크립트만으로 Android 빌드 성공 여부나 저사양 단말의 지속 성능을 확인할 수 없다.
- 완료 조건: 개발 빌드와 릴리스 빌드가 목표 Android 설정으로 성공하고, 최소 사양 실제 단말에서 로비부터 대국·AI·리플레이·결과 화면까지 크래시나 누락 자산 없이 실행되며, 합의된 프레임 시간·메모리·발열 기준을 충족한다. 최소 지원 OS/단말과 성능 기준은 추가 확인 필요.
