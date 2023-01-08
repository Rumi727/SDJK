# SDJK
현재 개발중인 리듬게임입니다

요약하자면, [osu!](https://osu.ppy.sh/)에서 ADOFAI와 같은 시야 방해 요소가 포함된 여러 이펙트가 추가된 단순하기 짝이없는 리듬게임에 불과합니다  
내장 맵은 없으며, 1인 개발이고 심지어 유니티, C#을 다루는 실력 또한 허접하기 때문에 디자인과 최적화 등등 여러 요소 또한 ~~개~~허접 할 수 있습니다

대부분의 UI는 [osu!lazer](https://github.com/ppy/osu)에서 영감을 받았습니다 ~~(라기엔 사실상 복사)~~  
이 부분이 문제가 된다면 잔말 없이 바로 수정하갰습니다

\+ 누가 얼불춤 로더 좀 제대로 되게 수정해줘요... ~~진짜 얼불춤 개망겜이네~~  
[(ADOFAIMapLoader.cs)](Assets/Map/Ruleset/ADOFAI/ADOFAIMapLoader.cs)

## 라이선스
TEAM Bucket 블랙리스트에 등록된 사람은 등록된 기간의 모든 커밋 또는 릴리즈 등 제 모든 창작물을 이용, 변경 등을 할 수 없습니다 (2022-07-28 0:53 시간 이후의 만들어진 창작물만 해당)  
(예: a라는 사람이 2022-8-01부터 2022-8-23까지 블랙리스트에 등록되어있으면 그 기간 동안 생성된 커밋과 릴리즈를 등 올라온 모든 창작물을 사용할 수 없다는 뜻)

위에 해당하지 않는다면, 파일에 작성되어있는 라이선스를 따르게 됩니다

[TEAM Bucket 블랙리스트에 등록된 사용자 목록](https://docs.google.com/document/d/1A8kz4DJOdLEtf-kybrKnGR51XDNZVHmojCU86KaDgKg)

이러한 라이선스가 [오픈 소스 정의](https://opensource.org/osd)와 일치하지 않다는것을 알고있습니다  
하지만 상관 없습니다  
블랙리스트에 등록될려면 적어도 테러 정돈 해야 등록될 정도로 매우 빡빡하며 등록된 사람들 전부 대부분의 사람들이 납득 가능한 이유가 있습니다

## 사용된 패키지와 DLL, 오픈 소스
- [SC KRM 1.0](https://github.com/SimsimhanChobo/SC-KRM-1.0)
- [UI Soft Mask](https://github.com/mob-sakai/SoftMaskForUGUI)
- [Super Blur](https://github.com/PavelDoGreat/Super-Blur)

제가 멍청해서 빼먹은 출처가 있을 수 있습니다...  
만약 그럴 경우, 이슈에 올려주세요

## 사용한 아이콘
- 제가 직접 만들었거나, [여기](https://www.iconfinder.com/search?q=&price=free&family=bootstrap)에서 가져왔습니다

## 사양
### 최소 사양
CPU: ?  
GPU: ?  
저장공간: HDD 400MB (맵 포함: 10GB)  
메모리: 3GB  
스마트폰: Samsung Galaxy A31 보단 훨씬 좋아야함

### 권장 사양
CPU: ?  
GPU: ?  
저장공간: SSD 600MB (맵 포함: 50GB ~ ∞)  
메모리: 4GB ~ ∞  
스마트폰: Samsung Galaxy A31 보단 훨씬 좋아야함

### 개발 당시 사양
CPU: i5-9600KF  
GPU: GTX 1660 Super  
저장공간: SSD 1TB  
메모리: 24GB  
스마트폰: Samsung Galaxy A31  

### 비고
최소 사양과 권장 사양은 오로지 **추측일 뿐입니다**

이 게임은 컴퓨터에서 플레이하는 걸 상정하고 만든 게임이지만, **그렇다고 대부분의 컴퓨터에서 정상적으로 플레이할 수 있다고 보장하지 않습니다**  
이게 무슨 개소리냐 하실 수 있는데  
그냥 제작자라는 놈이 코딩을 더럽게 ~~병신~~같이 한다고 봐주시면 될 것 같네요

일단 스팀덱에선 플레이 모드는 120FPS 정도는 나온다고 합니다
