
# YuJanggi

Unity 기반 장기 게임 프로젝트입니다.

단순히 장기 게임을 구현하는 것보다,  
입력·로직·표현의 책임을 분리하고  
Live 게임과 Replay 화면 흐름이 서로 간섭하지 않는 구조 설계에 중점을 두었습니다.

## 프로젝트 목표

- Core 로직의 Unity 의존 제거
- Input / Logic / Presentation 분리
- Local / AI / Network 환경 확장 고려
- Live / Replay 흐름 분리
- Rule Pipeline 기반 규칙 처리 구조 설계

## 기술 스택

- Unity
- C#
- Coroutine
- Event Driven Architecture

## 핵심 구조
<img width="1452" height="993" alt="흐름도" src="https://github.com/user-attachments/assets/06d0137e-378d-46d4-843b-3f4b44bd31bf" /><img width="821" height="672" alt="StateFlow" src="https://github.com/user-attachments/assets/c266e36d-b741-47f4-b2a2-c2f5fe034da6" />

Input
→ Controller
→ GameSession
→ MatchManager
→ MatchEvents
→ GameSession
→ View

GameSession이 이벤트 흐름을 중재하며,
View와 Model이 직접 상태를 변경하지 않도록 구조를 구성했습니다.

## 주요 구현 내용

### Session 중심 흐름 제어

초기에는 MatchManager와 View가 직접 연결되어 있었지만,
Replay 기능 추가 과정에서 이벤트 충돌과 상태 동기화 문제가 발생했습니다.

이를 해결하기 위해 GameSession을 중심으로 이벤트 흐름을 통합하고,
게임 상태별 동작을 SessionState로 분리했습니다.

### Live / Replay 흐름 분리

Replay가 단순히 게임을 멈춘 뒤 과거 상태를 보여주는 방식이 아니라,
사용자가 과거 수를 탐색하는 동안에도 실제 게임 진행은 계속 유지되도록 설계했습니다.

- Live: 이벤트 기반 상태 갱신
- Replay: Record 기반 화면 재구성

두 흐름의 책임을 분리해 UI 충돌과 상태 꼬임 문제를 해결했습니다.

### Rule Pipeline

장기 규칙을 단일 조건문으로 처리하지 않고,
다음 단계로 분리했습니다.

1. 이동 후보 생성
2. 궁성 이동 제한 적용
3. 장군 회피 가능 여부 검사
4. 최종 이동 가능 위치 결정

이를 통해 규칙 추가와 유지보수가 쉬운 구조를 구성했습니다.

### Controller 추상화

입력 처리를 Local / AI / Network 환경과 독립적으로 동작할 수 있도록 구성했습니다.

- LocalController
- AIController
- (확장 예정) NetworkController

모든 입력은 동일한 Move Request 흐름으로 처리됩니다.

## 프로젝트 구조

Core
- Board
- Rule
- Match
- Session
- Controller

Runtime
- Input
- View
- UI
- Audio

## 주요 기능

- 장기 이동 규칙 구현
- 장군 / 체크 판정
- AI 턴 처리
- 리플레이 전진 / 후진
- 기물 이동 애니메이션
- 사운드 처리
- 결과 UI

## 트러블슈팅

### Replay와 Live UI 충돌 문제

Replay 도중에도 실제 게임 상태는 계속 변경되면서,
View 이벤트가 중복 갱신되는 문제가 발생했습니다.

이를 해결하기 위해:
- Replay 렌더링 흐름과
- Live 이벤트 흐름을 분리하고,
- SessionState 기반으로 UI 갱신 책임을 제어했습니다.

## 향후 개선 예정

- Network 대전
- AI 고도화
- Replay 저장 / 불러오기
- Undo / Record 관리 개선
