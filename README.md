# SDJK
현재 개발중인 리듬게임입니다

요약하자면, [osu!](https://osu.ppy.sh/)에서 ADOFAI와 같은 시야 방해 요소가 포함된 여러 이펙트가 추가된 단순하기 짝이없는 리듬게임에 불과합니다  
내장 맵은 없으며, 1인 개발이고 심지어 유니티, C#을 다루는 실력 또한 허접하기 때문에 디자인과 최적화 등등 여러 요소 또한 ~~개~~허접 할 수 있습니다

대부분의 UI는 [osu!lazer](https://github.com/ppy/osu)에서 영감을 받았습니다 ~~(라기엔 사실상 복사)~~  
이 부분이 문제가 된다면 잔말 없이 바로 수정하갰습니다

## 주의
메인 화면이 매우매우매우 불안정 합니다!  
메모리 누수는 기본이요 디자인도 중구 난방이고 렉도 심하고 코드가 스파게티 마냥 매우매우 이리저리 꼬여있습니다  
그렇기 때문에 메인 화면의 90% 이상 갈아엎을 예정입니다 (언제가 될진 모르갰지만 말이죠)

게임 플레이는 안심하세요  
어셈블리로 코드가 완벽하게 분리되어있으니까요

... 사실 유니티 자채가 유저 맵 같이 컴파일 타임에 결과를 예측할 수 없는 (런타임에 결과가 항상 바뀌는) 시스템에는 정말 안맞습니다

## Ruleset
* SDJK
  * .sdjk
  * .sdjk (SDJK 1.0 file format)
* Super Hexagon
  * .super_hexagon
* A Dance of Fire and Ice (Hidden)
  * .adofai (ADOFAI file format)
* osu!mania (Hidden)
  * .osu (osu!mania file format v14)
  
## [MapFile](Assets/Map/MapFile.cs#L21) 클래스가 지원하는 타입
[ITypeList]: Assets/SC%20KRM/TypeList.cs#L3
[object]: https://learn.microsoft.com/ko-kr/dotnet/api/System.Object

* [object]
* [ValueType](https://learn.microsoft.com/ko-kr/dotnet/api/System.ValueType)
* [bool](https://learn.microsoft.com/ko-kr/dotnet/api/System.Boolean)
* [byte](https://learn.microsoft.com/ko-kr/dotnet/api/System.Byte)
* [sbyte](https://learn.microsoft.com/ko-kr/dotnet/api/System.SByte)
* [short](https://learn.microsoft.com/ko-kr/dotnet/api/System.Int16)
* [ushort](https://learn.microsoft.com/ko-kr/dotnet/api/System.UInt16)
* [int](https://learn.microsoft.com/ko-kr/dotnet/api/System.Int32)
* [uint](https://learn.microsoft.com/ko-kr/dotnet/api/System.UInt32)
* [long](https://learn.microsoft.com/ko-kr/dotnet/api/System.Int64)
* [ulong](https://learn.microsoft.com/ko-kr/dotnet/api/System.UInt64)
* [float](https://learn.microsoft.com/ko-kr/dotnet/api/System.Single)
* [double](https://learn.microsoft.com/ko-kr/dotnet/api/System.Double)
* [decimal](https://learn.microsoft.com/ko-kr/dotnet/api/System.Decimal)
* [nint](https://learn.microsoft.com/ko-kr/dotnet/api/System.IntPtr)
* [unint](https://learn.microsoft.com/ko-kr/dotnet/api/System.UIntPtr)
* [char](https://learn.microsoft.com/ko-kr/dotnet/api/System.Char)
* [string](https://learn.microsoft.com/ko-kr/dotnet/api/System.String)
* [BigInteger](https://learn.microsoft.com/ko-kr/dotnet/api/System.Numerics.BigInteger)
* [BigDecimal](https://github.com/AdamWhiteHat/BigDecimal)
* [JVector2](Assets/SC%20KRM/Json/JsonManager.cs#L90)
* [JVector3](Assets/SC%20KRM/Json/JsonManager.cs#L126)
* [JVector4](Assets/SC%20KRM/Json/JsonManager.cs#L169)
* [JRect](Assets/SC%20KRM/Json/JsonManager.cs#L221)
* [JColor](Assets/SC%20KRM/Json/JsonManager.cs#L278)
* [JColor32](Assets/SC%20KRM/Json/JsonManager.cs#L340)
* [AnimationCurve](https://docs.unity3d.com/ScriptReference/AnimationCurve.html)
* [Enum](https://learn.microsoft.com/ko-kr/dotnet/api/System.Enum)
* [IBeatValuePair](Assets/SC%20KRM/Rhythm/RhythmMapFile.cs#L356)
* [IBeatValuePairAni](Assets/SC%20KRM/Rhythm/RhythmMapFile.cs#L369)
* [ITypeList]

위에 서술 된 타입이 아니더라도 정상 작동은 하나, 에디터에서 표시되지 않음  
(주의: [ITypeList] 인터페이스를 상속하지 않았지만 [ICollection](https://learn.microsoft.com/ko-kr/dotnet/api/System.Collections.ICollection) 인터페이스는 상속한 경우 [object] 클래스를 상속했더라도 제외됩니다)
  
## 라이선스
[Blacklist]: https://docs.google.com/document/d/1A8kz4DJOdLEtf-kybrKnGR51XDNZVHmojCU86KaDgKg
[SC KRM]: https://github.com/SimsimhanChobo/SC-KRM-1.0
[SDJK]: https://github.com/SimsimhanChobo/SDJK
[Simsimhan Chobo]: https://github.com/SimsimhanChobo
[discord_check.dll]: Assets/SC%20KRM/Discord/Library/discord_check.dll
[C++ Namu]: https://namu.wiki/w/C%2B%2B
[C++]: https://ko.wikipedia.org/wiki/C%2B%2B
[MIT]: https://opensource.org/licenses/mit
[GPL]: https://opensource.org/license/gpl-3-0

[TEAM Bucket 블랙리스트][Blacklist]에 등록된 사람은 [SDJK] 및 [SDJK]의 파생 저작물을 이용할 수 없습니다.

또한 [discord_check.dll] 파일을 디컴파일하거나 (내용을 보거나) 수정할 수 없습니다.  
[discord_check.dll] 파일은 [C++][C++ Namu] 프로그래밍 언어로 작성되었으며 저작권은 [Simsimhan Chobo] 에게 있습니다.

위의 내용에 영향을 받지 않는다면 [SDJK]은 [GPL 3.0 라이선스][GPL]를 따르게 됩니다.

## English Translation
[SDJK] and derivative works of [SDJK] cannot be used by persons on the [TEAM Bucket Blacklist][Blacklist]

Also, you cannot decompile (view contents) or modify the file [discord_check.dll]
The [discord_check.dll] file is written in [C++] programming language and copyrighted by [Simsimhan Chobo].

Unless affected by the above, [SDJK] is under the [GPL 3.0 License][GPL]

## 사용된 패키지와 DLL, 오픈 소스
- [SC KRM 1.0](https://github.com/SimsimhanChobo/SC-KRM-1.0)
- [UI Soft Mask](https://github.com/mob-sakai/SoftMaskForUGUI)
- [Super Blur](https://github.com/PavelDoGreat/Super-Blur)
- [ColorBands](https://github.com/rstecca/ColorBands)

제가 멍청해서 빼먹은 출처가 있을 수 있습니다...  
만약 그럴 경우, 이슈에 올려주세요

## 사용한 아이콘
- 제가 직접 만들었거나, [여기](https://www.iconfinder.com/search?q=&price=free&family=bootstrap)에서 가져왔습니다

## 버전 표기 규칙
- [Semantic Versioning](https://semver.org/)

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
