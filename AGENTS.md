## 저장소 구조

아래 경로는 실제 저장소 구조에 맞게 수정한다.

- `Assets/Scripts/Core`: Unity에 의존하지 않는 규칙, 모델, 기록, 매치 로직
- `Assets/Scripts/Runtime`: Unity 입력, 표현, UI, 오디오, 생명주기 처리
- `Assets/Scripts/Runtime/Input`: 포인터 및 보드 입력 처리
- `Assets/Scripts/Runtime/View`: 보드와 장기말 표현
- `Assets/Scripts/Runtime/Session`: `GameSession`과 세션 상태

## 주요 Runtime 클래스의 책임

- `PcInputHandler`: 포인터 입력과 보드 클릭을 감지한다.
- `PieceView`: 장기말을 화면에 표현하며, 시각적인 이동만 처리한다.
- `BoardView`: 보드와 장기말 표현을 관리한다.
- `GameSession`: 매치 흐름과 세션 상태 전환을 조율한다.
- `SessionLiveState`: 라이브 플레이 요청을 처리한다.
- `ReplayView`: 기록 기반 리플레이 단계를 화면에 반영한다.
- `LiveView`: 이벤트 기반 라이브 플레이 결과를 화면에 반영한다.

`PieceView`에서 Core의 매치 로직을 직접 호출하지 않는다.
시각적 연출은 Core 상태를 변경하지 않는다.

## 입력 대응 규칙

- 기존 PC 입력 동작을 유지하면서 모바일 친화적인 상호작용을 추가한다.
- 포인터 감지는 Runtime 입력 계층에서 처리한다.
- `PieceView` 내부에서 포인터 입력을 직접 읽지 않는다.
- 드래그 중에는 화면 표현만 변경한다.
- 드롭 요청은 기존 세션 흐름을 통해 명시적으로 전달한다.
- 별도 요청이 없다면 새로운 입력 프레임워크나 패키지를 추가하지 않는다.
- 포인터 드래그 처리는 Runtime 입력 계층에서 `Update`를 사용할 수 있는 정당한 사유다.
- 입력 핸들러가 좌표를 전달할 수 있다면 View 클래스에 `Update`를 추가하지 않는다.

## 변경 보고 규칙

수정 전:

- `git status --short --branch`를 실행한다.
- 작업 트리에 기존 변경 사항이 있다면 파일을 수정하기 전에 보고한다.

코드 수정 후 다음 항목을 보고한다.

1. 변경된 파일
2. 추가, 삭제 또는 수정된 메서드
3. Core 동작 변경 여부
4. 직렬화 필드 추가 또는 변경 여부
5. Unity Inspector에서 수동으로 연결해야 하는 항목
6. 수행한 정적 검증
7. Unity Editor에서 추가로 확인해야 하는 항목
8. 확인되지 않은 가정 또는 알려진 위험 요소