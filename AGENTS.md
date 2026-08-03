0. 최우선 규칙: codex 브랜치만 사용

저장소에 영향을 주는 모든 작업은 반드시 정확히 codex 브랜치에서만 수행한다.

작업 시작 전:

git branch --show-current
git status --short --branch

처리 규칙:

현재 브랜치가 codex이고 작업 트리가 깨끗하면 진행한다.

현재 브랜치가 codex이지만 기존 변경 사항이 있으면 수정하지 말고 보고한다. 사용자가 계속 진행하라고 명시한 경우에만 기존 변경을 보존하며 작업한다.

detached HEAD이거나 현재 브랜치가 codex가 아닌 상태에서 기존 변경 사항이 있으면 중단하고 보고한다.

다른 브랜치이며 작업 트리가 깨끗한 경우, 로컬 codex 브랜치가 있을 때만 전환한다.

git show-ref --verify --quiet refs/heads/codex
git switch codex
git branch --show-current
git status --short --branch

로컬 codex 브랜치가 없거나 전환 후 브랜치가 정확히 codex가 아니면 중단하고 보고한다. 브랜치를 임의로 생성하지 않는다.

codex 외 브랜치에서는 다음을 금지한다.

파일 생성, 수정, 삭제, 이름 변경

패키지 설치, 코드 생성, 자동 포맷

Scene, Prefab, Asset, 설정 변경

저장소 파일을 생성하거나 갱신하는 빌드 및 도구 실행

커밋과 푸시

명시적인 요청 없이는 브랜치 생성·삭제, merge, rebase, cherry-pick, stash, reset, clean, restore, force push, 커밋, 푸시를 수행하지 않는다. 기존 변경 사항을 되돌리거나 덮어쓰지 않는다.

1. 저장소 구조

실제 저장소 구조를 먼저 확인하며, 존재하지 않는 경로나 클래스를 추측하여 만들지 않는다.

Assets/Scripts/Core: Unity 비의존 규칙, 모델, 기록, 매치 로직

Assets/Scripts/Runtime: Unity 입력, 표현, UI, 오디오, 생명주기

Assets/Scripts/Runtime/Input: 포인터 및 보드 입력

Assets/Scripts/Runtime/View: 보드와 장기말 표현

Assets/Scripts/Runtime/Session: GameSession과 세션 상태

의존 방향:

Runtime → Core
Core -X→ Runtime
Core -X→ UnityEngine

주요 책임:

PcInputHandler: 포인터 입력과 보드 클릭 감지

PieceView: 장기말 표현과 시각적 이동

BoardView: 보드와 장기말 표현 관리

GameSession: 매치 흐름과 세션 상태 전환 조율

SessionLiveState: 라이브 플레이 요청 처리

ReplayView: 기록 기반 리플레이 화면 반영

LiveView: 이벤트 기반 라이브 결과 화면 반영

아키텍처 불변 조건:

PieceView는 입력을 직접 읽거나 Core 매치 로직을 직접 호출하지 않는다.

View와 시각적 연출은 Core 상태를 변경하지 않는다.

플레이 요청은 기존 GameSession과 세션 상태 흐름으로 전달한다.

Core에 UnityEngine, MonoBehaviour 또는 Unity 생명주기 의존성을 추가하지 않는다.

2. 모바일 입력 확장

기존 PC 마우스 입력을 유지하면서 모바일 포인터·터치를 추가한다.

포인터 감지는 Runtime 입력 계층에서 처리한다.

데스크톱과 모바일이 공유하는 포인터 흐름을 우선한다.

드래그 중에는 화면 표현만 바꾼다.

드롭 요청은 기존 세션 흐름으로 전달한다.

입력 핸들러가 좌표를 전달할 수 있으면 View에 Update를 추가하지 않는다.

Runtime 입력 핸들러의 드래그 처리를 위한 Update 사용은 허용한다.

별도 요청이 없으면 새로운 입력 프레임워크, 패키지, 플러그인을 추가하지 않는다.

플랫폼 조건부 컴파일은 공통 처리로 해결할 수 없을 때만 사용한다.

Pointer Input
→ Runtime Input
→ GameSession / Session State
→ Core Match Logic
→ LiveView / ReplayView

3. 변경 원칙

수정 전:

관련 파일, 호출부, 이벤트, 직렬화 참조를 확인한다.

요청에 필요한 최소 변경 범위를 정한다.

수정 중:

관련 없는 리팩터링을 하지 않는다.

C#, Input Actions, JSON, Markdown, Unity YAML 파일은 LF 줄바꿈을 유지한다. 줄바꿈 정규화 작업에서는 코드와 직렬화 데이터의 내용은 변경하지 않는다.

public API와 직렬화 필드 변경을 최소화한다.

Scene, Prefab, ProjectSettings, Packages는 요청에 포함되거나 반드시 필요한 경우에만 수정한다.

Library, Temp, Logs, Obj, 빌드 결과물을 수정하거나 커밋하지 않는다.

기존 .meta 파일을 삭제하거나 임의로 재생성하지 않는다.

확인되지 않은 구조를 추측하여 구현하지 않는다.

계획이나 문서 작성만 요청받으면 지정된 문서 외에는 변경하지 않는다.

수정 후:

git diff --check
git diff --stat
git status --short --branch

다음을 확인한다.

현재 브랜치가 codex인지

컴파일 오류 가능성과 호출부 영향

직렬화 필드와 Inspector 참조 영향

Core의 Unity 비의존성이 유지되는지

View가 Core 상태를 직접 변경하지 않는지

기존 PC 입력 경로가 유지되는지

실제로 실행하지 않은 Unity Editor, 테스트, 빌드, 실기기 동작을 검증했다고 표현하지 않는다.

4. 변경 보고

첫 줄:

작업 브랜치: codex

아래 순서로 간결하게 보고하며 해당 사항이 없으면 없음으로 작성한다.

변경된 파일

추가·삭제·수정된 클래스와 메서드

Core 동작 변경 여부

직렬화 필드 변경 여부

Unity Inspector 수동 작업

수행한 정적 검증과 테스트

Unity Editor 추가 확인 사항

확인되지 않은 가정과 위험 요소