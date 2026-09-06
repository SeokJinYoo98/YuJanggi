# YuJanggi 서버 현황 요약

작성 기준: `YuJanggi.Server` 현재 소스. 이 문서는 Unity 클라이언트 연동 전에 서버의 실제 동작과 통신 계약을 빠르게 확인하기 위한 참고 자료다.

## 1. 현재 구현 범위

- 서버 프로젝트: `YuJanggiServer` (`net10.0`), TCP 서버
- 콘솔 검증 클라이언트: `YuJanggiClient` (`net10.0`)
- 공용 메시지 계약: `YuJanggiCommon` (`net10.0`, `netstandard2.1`)
- 게임 규칙: 서버의 `Core/YuJanggi.Core`를 사용하며, 게임 상태와 합법 수 판정은 서버가 최종 권한을 가진다.

현재 포트는 `7777`이며, 모든 네트워크 인터페이스(`IPAddress.Any`)에서 TCP 연결을 받는다. 포트·호스트는 아직 설정 파일이나 실행 인자로 분리되어 있지 않다.

현재 콘솔 클라이언트 두 개로 아래 흐름은 구현되어 있다.

```text
TCP 연결 → Join → 자동 매칭 → GameStart 스냅샷 수신
→ 합법 수 조회 또는 MoveRequest → MoveResult 스냅샷 수신 → 게임 내 채팅
```

## 2. 서버 구성과 책임

| 위치 | 책임 |
| --- | --- |
| `YuJanggiServer/YuJanggiServer.cs` | TCP 연결, 메시지 디스패치, 참가, 매칭, 채팅, 연결 종료 처리 |
| `YuJanggiServer/ClientSession.cs` | 연결별 `TcpClient`, 플레이어/게임 정보, 송신 순서 보호용 `SemaphoreSlim` |
| `YuJanggiServer/Game/GameSession.cs` | 게임별 `MatchModel`, 양측 참가자, 수 검증 및 보드 스냅샷 생성 |
| `YuJanggiCommon/YuJanggiCommon` | TCP 프레이밍, 메시지 Envelope, 요청/응답 DTO, 오류 코드 |
| `Core` | 장기판, 턴, 합법 수, 실제 이동 적용 |

게임 하나는 별도 `GameSession`과 `MatchModel`을 가지며, 게임별 `Lock`으로 합법 수 조회와 이동 적용을 직렬화한다. 현재 초기 진형은 양측 모두 `EHHE`, 시간 제한은 `0`으로 고정되어 있다.

## 3. TCP 패킷 규약

모든 패킷은 Length-Prefixed JSON이다.

```text
4-byte big-endian JSON 본문 길이
+ UTF-8 JSON 본문
```

- 최대 JSON 본문: `4096` bytes
- 길이가 `0` 이하이거나 최대값을 초과하면 연결 처리 전에 거부한다.
- JSON Envelope은 `Type`, `RequestId`, `Payload`를 가진 `ChatMessage`다.
- 현재 `System.Text.Json` 기본 설정을 사용하므로 enum 값은 문자열이 아닌 숫자로 직렬화된다.
- 요청자에게 돌아오는 응답은 원칙적으로 같은 `RequestId`를 사용하며, 상대에게 방송되는 이벤트의 `RequestId`는 `null`이다.

Unity 쪽도 동일한 Big Endian 헤더, UTF-8, 최대 4 KiB 제한을 지켜야 한다.

## 4. 실제 메시지 흐름

### 접속, 참가, 매칭

```text
Client → Join { PlayerName }
Server → Join { PlayerId, PlayerName }

Client → MatchmakingStart {}
Server → MatchmakingStatus { Waiting }       # 첫 대기자

Server → MatchFound { GameId, Opponent, Side } # 매칭된 양쪽
Server → GameStart { GameId, Side, CurrentTurn, Pieces } # 매칭된 양쪽
```

- 이름은 공백 불가, 최대 20자이며 대소문자를 구분하지 않고 중복을 거부한다.
- 매칭은 대기열 선착순 두 명을 즉시 연결한다.
- `MatchmakingCancel {}`은 대기 중인 클라이언트만 취소할 수 있다.
- `GameStart.Pieces`는 전체 보드 스냅샷이고, 좌표는 `X: 0..8`, `Z: 0..9`다.
- `Side`는 `Cho` 또는 `Han`이다.

### 기물 선택과 이동

```text
Client → LegalMovesRequest { From }
Server → LegalMovesResult { From, LegalMoves }

Client → MoveRequest { From, To }
Server → MoveResult { GameId, From, To, MovedBy, CurrentTurn, Pieces } # 양쪽
```

`MoveRequest`는 서버에서 다음을 순서대로 검증한다.

1. 요청자가 해당 게임의 참가자인지
2. 요청자 진영이 현재 턴인지
3. 출발·도착 좌표가 보드 안인지
4. 출발지에 기물이 있는지와 해당 기물이 요청자 소유인지
5. `MatchModel.TryMove()` 기준으로 합법 수인지

성공 시에만 서버 상태를 바꾸고, 이동 전후 정보와 **전체 보드 스냅샷**을 양쪽에 전송한다. 클라이언트는 로컬에서 이동을 확정하지 말고 `MoveResult`를 받은 뒤 보드 표시를 갱신해야 한다.

`TurnChanged` 타입은 계약에 선언되어 있지만 현재 서버는 별도로 전송하지 않는다. 턴 표시는 `GameStart.CurrentTurn`과 `MoveResult.CurrentTurn`을 사용해야 한다.

### 게임 채팅과 오류

```text
Client → GameChatSend { Message }
Server → GameChatReceived { GameId, SenderPlayerId, SenderPlayerName, Message, SentAt } # 양쪽

Server → Error { Code, Message }
```

- 채팅은 매칭된 게임 참가자만 보낼 수 있고, 공백 불가·최대 200자다.
- 오류 코드에는 `NotYourTurn`, `InvalidPosition`, `NotYourPiece`, `IllegalMove`, `NotMatched` 등이 있다.
- 상대 연결이 종료되면 남은 플레이어에게 `GameEnd`와 `OpponentLeft` 사유를 보낸다.

## 5. Unity 연동 시 권장 경계

```text
Unity Input / UI
  → 네트워크 요청 (Join, MatchmakingStart, LegalMovesRequest, MoveRequest)
  → TCP 송수신 계층
  → YuJanggi.Server
  → YuJanggi.Core로 최종 검증·상태 변경
  → 서버 이벤트 (GameStart, MoveResult, Error, GameEnd)
  → Unity GameSession / View 반영
```

Unity가 해야 할 일은 입력을 요청으로 바꾸고 서버 이벤트를 화면에 반영하는 것이다. 승패·턴·합법 수·포획 같은 최종 상태를 Unity가 독자적으로 확정하면 안 된다.

권장 구현 순서:

1. `YuJanggiCommon` DTO와 `MessageProtocol`을 Unity에서 사용할 수 있게 참조하거나 Unity 호환 패키지로 제공한다.
2. Unity에 TCP 연결, 패킷 송수신, 메인 스레드 이벤트 전달을 담당하는 네트워크 계층을 만든다.
3. `Join → MatchmakingStart → MatchFound → GameStart`를 연결해 서버 스냅샷으로 보드를 표시한다.
4. 기존 선택 흐름에서 `LegalMovesRequest`를 보내고, 응답 `LegalMovesResult`로 이동 가이드를 표시한다.
5. 이동 입력은 `MoveRequest`만 보내고, `MoveResult`의 전체 스냅샷을 기준으로 `GameSession`과 View를 갱신한다.
6. `Error`, `GameEnd`, 연결 종료를 UI와 세션 상태에 반영한다.

## 6. Unity 연동 전에 알아둘 제한 사항

- 인증, TLS 암호화, 계정·전적 저장, 랭킹은 없다.
- 재접속과 상태 재동기화 메시지가 없다. 연결이 끊기면 현재 게임은 `OpponentLeft`로 끝난다.
- 정상 게임 종료(체크메이트, 기권, 시간패) 네트워크 이벤트는 아직 완성되지 않았다. 현재 `GameEndReason`은 `OpponentLeft`만 있다.
- 관전, 방 목록/비공개 방, 매칭 조건, 진형 선택, 서버 설정은 없다.
- 상태 버전이나 명령 중복 방지 규칙이 없어, 재시도와 패킷 지연을 다루는 기능은 추가 설계가 필요하다.
- `YuJanggiCommon`은 `netstandard2.1`을 대상으로도 빌드되지만, 서버의 `Core` 프로젝트는 `net10.0`이다. Unity는 서버용 Core DLL을 직접 참조하지 말고, Unity와 서버가 합의한 Core/DTO 배포 방식을 별도로 정해야 한다.

## 7. Unity에서 바로 필요한 DTO

다음 DTO는 `YuJanggiCommon`에 이미 정의되어 있다.

| 흐름 | DTO |
| --- | --- |
| 참가 | `JoinRequest`, `JoinResponse` |
| 매칭 | `MatchmakingStartRequest`, `MatchmakingCancelRequest`, `MatchmakingStatusResponse`, `MatchFoundResponse` |
| 시작/보드 | `GameStartEvent`, `BoardPieceState`, `BoardPosition` |
| 이동 | `LegalMovesRequest`, `LegalMovesResult`, `MoveRequest`, `MoveResultEvent` |
| 종료/오류 | `GameEndEvent`, `ErrorResponse`, `ErrorCode` |
| 채팅 | `GameChatSendRequest`, `GameChatReceivedEvent` |

콘솔 프로토콜 참고 구현은 `YuJanggiClient/Program.cs`에 있다. Unity 네트워크 계층을 만들 때 이 파일의 `SendAsync`, `ReceiveAsync`, 매칭·게임 시작·수신 처리 순서를 기준으로 삼을 수 있다.
