# YuJanggi.Unity

Unity 6 기반 한국 장기 게임입니다.

로컬 대국, AI 대국, 기보 리플레이를 지원합니다.

[포트폴리오](https://app.notion.com/p/3b28a299d1c480ed867fef02568ca410) / [실행 파일](https://app.notion.com/p/38e8a299d1c48043b6a8f045695abf57) / [Core](https://github.com/SeokJinYoo98/YuJanggi.Core) / [Server](https://github.com/SeokJinYoo98/YuJanggi.Server)

## 주요 기능

- **대국:** 로컬 대국과 AI 대국, 한 수 쉼, 무르기, 기권

- **AI:** Random, Greedy, Minimax 전략

- **리플레이:** 진행 중과 종료 후 기보 탐색, Live 복귀

- **입력과 표현:** 마우스와 터치, 이동 가이드, 애니메이션, 사운드

## 구조

입력 → Controller → GameSession / State → Core → View

- **Core:** 이동 검증, 보드, 턴, 점수, 기보, 승패 처리

- **Unity:** 입력, 세션 흐름, 화면과 오디오

- **ReplayView:** 실제 대국 보드는 유지하고 기보 커서와 화면을 갱신

Core는 Git UPM으로 참조합니다. Unity와 Server의 Core 커밋은 함께 맞춰야 합니다.

## 기술

- 엔진: Unity **6000.3.1f1**, URP **17.3**

- 입력: Input System **1.17**

- 비동기와 애니메이션: UniTask, DOTween

기물 데이터는 ScriptableObject, 이동 가이드와 파티클은 Object Pool을 사용합니다.

## 실행

1. 저장소를 Clone하고 Unity Hub에서 엽니다.

2. Unity **6000.3.1f1**에서 패키지 복원을 기다립니다.

3. Assets/Scenes/LobbyScene.unity를 열고 Play를 누릅니다.

## 현재 상태

- **구현:** 로컬, AI, 리플레이, TCP 송수신, 로비 참가, 매칭 응답 처리

- **온라인 미완성:** 대국 씬 진입, 이동 요청과 서버 결과 반영

  NetworkController는 빈 구현입니다.


통신 코드: Assets/Scripts/Runtime/Network

공용 DLL: Assets/Plugins/YuJanggiCommon

## 사용 에셋

- 장기말: [장기 Janggi KOREA Ver](https://www.acon3d.com/ko/product/1000013872)

- UI와 배경: Aseprite로 직접 제작

- 사운드: [Pixabay](https://pixabay.com/sound-effects/search/chess/)
