<div align="center">

# YuJanggi.Unity

**장기 규칙과 Unity 표현을 분리한 Unity 6 기반 한국 장기 게임 클라이언트**

![Unity](https://img.shields.io/badge/Unity-6000.3.1f1-000000?logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-Unity_Runtime-512BD4?logo=dotnet&logoColor=white)
![Core](https://img.shields.io/badge/Core-Git_UPM-2ea44f)
![Status](https://img.shields.io/badge/status-active_development-f59e0b)

[포트폴리오](https://app.notion.com/p/3b28a299d1c480ed867fef02568ca410) ·
[실행 파일](https://app.notion.com/p/38e8a299d1c48043b6a8f045695abf57) ·
[Core](https://github.com/SeokJinYoo98/YuJanggi.Core) ·
[Server](https://github.com/SeokJinYoo98/YuJanggi.Server)

</div>

## 프로젝트 소개

YuJanggi.Unity는 Local 대국, AI 대국, 진행 중 기보 탐색과 종료 후 Replay를 지원하는 개인 장기 프로젝트입니다.

장기 규칙과 대국 상태는 Unity API에 의존하지 않는 [YuJanggi.Core](https://github.com/SeokJinYoo98/YuJanggi.Core)가 담당합니다. 이 저장소는 입력, 세션 흐름, 보드와 기물 표현, UI, 오디오처럼 Unity에서만 필요한 책임에 집중합니다.

> **현재 상태**
>
> Local·AI·Replay 흐름은 구현되어 있습니다. Android 화면 대응과 실제 단말 검증, 서버 권위형 온라인 대전 연결은 진행 중입니다.

## 주요 기능

| 영역 | 구현 내용 |
| --- | --- |
| 대국 | Local 대국, AI 대국, 턴 진행, 한 수 쉼, 무르기, 기권 |
| AI | Random·Greedy·Minimax 전략, 선택한 수의 Core 검증 |
| 장기 규칙 | 기물 이동, 궁성 규칙, 합법 수 필터링, 장군, 외통수, 점수와 결과 판정 |
| Replay | 진행 중 이전·다음 수 탐색, 종료 후 전체 기보 탐색, Live 복귀 시 화면 동기화 |
| 입력 | Unity Input System 기반 포인터 입력, 마우스와 Primary Touch 공통 처리 |
| 표현 | 기물 이동 Tween, 이동 가이드, 캡처 파티클, 사운드, 결과 UI |
| 데이터 | ScriptableObject 기반 기물 종류·진영·Mesh 관리 |

## 아키텍처

~~~mermaid
flowchart LR
    I["Mouse / Touch"] --> IN["Runtime Input"]
    IN --> C["Local / AI Controller"]
    C --> S["GameSession"]
    S --> ST["SessionState"]
    ST --> CORE["YuJanggi.Core"]
    CORE --> EV["Match Events"]
    EV --> ST
    ST --> V["MatchView / ReplayView"]
~~~

- **Controller**는 입력을 출발 좌표와 도착 좌표를 가진 이동 요청으로 변환합니다.
- **GameSession**은 현재 세션 상태에 요청과 Core 이벤트를 위임합니다.
- **SessionState**는 Live, Replay, End 상태별로 허용할 입력과 화면 갱신을 결정합니다.
- **Core**는 실제 대국의 보드, 턴, 점수, 기보와 승패 처리를 담당하며, 대국 요청은 <code>MatchModel</code>을 통해 적용합니다.
- **View**는 실제 대국 보드를 변경하지 않고 결과를 화면에 표현합니다. <code>ReplayView</code>는 Core의 <code>Record</code> API를 통해 리플레이 모드와 기보 탐색 커서를 갱신합니다.

<details>
<summary>기존 설계 이미지 보기</summary>

<img width="1452" height="993" alt="YuJanggi 전체 흐름" src="https://github.com/user-attachments/assets/06d0137e-378d-46d4-843b-3f4b44bd31bf" />

<img width="821" height="672" alt="SessionState 흐름" src="https://github.com/user-attachments/assets/c266e36d-b741-47f4-b2a2-c2f5fe034da6" />

</details>

## 핵심 설계

### Core와 Unity Runtime 분리

공용 규칙은 Git UPM 패키지로 가져오며 <code>Packages/manifest.json</code>에서 사용할 Core 커밋을 고정합니다.

| 저장소 | 책임 |
| --- | --- |
| [YuJanggi.Unity](https://github.com/SeokJinYoo98/YuJanggi.Unity) | 입력, 세션 조율, View, UI, 오디오와 Unity 생명주기 |
| [YuJanggi.Core](https://github.com/SeokJinYoo98/YuJanggi.Core) | 보드, 기물, 이동 규칙, 턴, 점수, 기보와 대국 상태 |
| [YuJanggi.Server](https://github.com/SeokJinYoo98/YuJanggi.Server) | TCP 세션, 자동 매칭, 서버 권위형 이동 판정과 결과 전파 |

Unity와 .NET 서버는 Core를 공유하지만 참조 커밋은 각각 관리됩니다. 같은 규칙 버전을 유지하려면 Unity 패키지와 Server submodule을 동일한 검증된 커밋으로 맞춰야 합니다. 로컬·AI 대국의 실제 이동은 <code>MatchModel.TryMove()</code>에서 검증합니다.

### Live와 Replay 상태 분리

Replay 화면과 계속 진행되는 Live 모델이 서로 다른 시점을 가리킬 수 있기 때문에 <code>GameSession</code>이 다음 상태를 명시적으로 관리합니다.

- <code>SessionLiveState</code>: 선택, 이동, 무르기, 한 수 쉼과 최신 모델 반영
- <code>SessionReplayState</code>: Live 모델을 유지한 채 과거 기보를 화면에 표시
- <code>SessionEndState</code>: 입력 중지와 대국 결과 표시
- <code>SessionEndReplayState</code>: 종료된 대국의 기보 탐색

Live로 복귀할 때 <code>MatchView.SyncBoardState()</code>로 화면을 최신 모델 상태에 다시 맞춥니다.

### 입력과 표현의 책임 분리

<code>PointerInputHandler</code>가 Raycast 결과에서 보드 좌표를 얻어 입력 계층으로 전달하고, <code>LocalController</code>가 선택과 이동 요청을 생성합니다. 드래그나 애니메이션 중 화면 표현은 Core 상태를 변경하지 않습니다.

반복 생성되는 이동 가이드와 파티클은 Object Pool로 재사용하며, 기물 이동은 DOTween, AI 턴의 비동기 지연과 취소는 UniTask로 처리합니다.

### 온라인 연결 상태

<code>TcpGameClient</code>는 TCP 접속·송수신과 수신 이벤트 큐를 제공하고, <code>TcpGameClientBehaviour</code>는 <code>Update()</code>에서 수신 이벤트를 Unity 메인 스레드로 전달합니다. <code>ServerMessageFactory</code>는 참가·매칭·이동 등의 요청 메시지를 생성합니다.

공용 메시지 계약은 <code>Assets/Plugins/YuJanggiCommon</code>의 <code>YuJanggiCommon.dll</code>과 JSON 관련 DLL로 포함되어 있습니다. 서버 프로토콜을 변경할 때 이 DLL의 호환성도 함께 확인해야 합니다.

현재 작업 중인 로비 코드에는 참가·매칭 응답과 게임 시작 메시지 처리가 있습니다. 온라인 대국 씬 진입, 이동 요청 전송, 서버 이동 결과의 대국 상태 반영은 아직 연결되지 않았으며, <code>NetworkController</code>는 빈 구현입니다. 온라인 대국 전체 흐름은 완성되지 않았습니다.

## 기술 스택

| 구분 | 기술 |
| --- | --- |
| Engine | Unity 6000.3.1f1, URP 17.3 |
| Language | C# |
| Input | Unity Input System 1.17 |
| Async | UniTask |
| Animation | DOTween |
| Data | ScriptableObject |
| Shared rules | YuJanggi.Core 0.1.0, Git UPM commit pinning |

## 프로젝트 구조

~~~text
Packages
└── com.seokjinyoo.yujanggi.core  # YuJanggi.Core

Assets/Scripts
├── Data                           # Piece ScriptableObject
└── Runtime
    ├── Audio
    ├── Board
    ├── Controller
    ├── Game
    ├── GameSession
    │   └── State
    ├── Network                       # TCP 전송과 메시지 생성
    ├── Input
    ├── Particle
    ├── Piece
    └── UI
~~~

## 시작하기

### 요구 사항

- Unity Hub
- Unity <code>6000.3.1f1</code>
- Git

### 실행

~~~powershell
git clone https://github.com/SeokJinYoo98/YuJanggi.Unity.git
~~~

1. Unity Hub에서 Clone한 폴더를 엽니다.
2. Package Manager가 <code>YuJanggi.Core</code>와 다른 패키지를 복원할 때까지 기다립니다.
3. <code>Assets/Scenes/LobbyScene.unity</code>를 열고 Play Mode를 실행합니다. 빌드에서도 이 씬이 첫 씬으로 등록되어 있습니다.

Core는 <code>Packages/manifest.json</code>에 기록된 커밋 SHA로 고정되므로, Core를 갱신할 때는 검증된 SHA를 명시적으로 변경해야 합니다.

## 검증과 개발 상태

Core에는 이동 규칙과 상태 무결성을 확인하는 MSTest 20개 실행 케이스가 있습니다. Unity Editor, Android 빌드와 실제 단말 동작은 별도 확인이 필요합니다.

## 사용 에셋

- 장기말: [장기 Janggi KOREA Ver 접이식 장기판 버전](https://www.acon3d.com/ko/product/1000013872)
- UI와 배경: Aseprite로 직접 제작
- 사운드: [Pixabay Chess Sound Effects](https://pixabay.com/sound-effects/search/chess/)
