# AV Matrix Controller

8x8 매트릭스 스위처 제어 애플리케이션

## 주요 기능

- 8x8 입출력 매트릭스 라우팅 제어
- UDP LAN 통신 지원
- 프리셋 저장 및 불러오기
- 장치 자동 검색 (Broadcast)
- 장치 상태 조회
- LCD 상태 조회
- Material Design UI
- 설정 자동 저장

## 시작하기

### 필수 요구사항
- .NET 10.0 이상
- Windows OS
- MaterialSkin.2 NuGet 패키지 (자동으로 설치됨)

### 설치 및 실행
1. 프로젝트 복제
```bash
git clone https://github.com/lovey25/AVMatrixController
cd AVMatrixController
```

2. 빌드
```bash
dotnet build
```

3. 실행
```bash
dotnet run
```

## 파일 구조

- `Form1.cs` - 메인 애플리케이션 로직
- `Form1.Designer.cs` - UI 컨트롤 정의
- `MatrixProtocol.cs` - API 프로토콜 구현
- `SettingsDialog.cs` - 장치 설정 대화상자
- `PresetNameDialog.cs` - 프리셋 이름 입력 대화상자
- `settings.json` - 장치 IP/포트 설정 (자동 생성)
- `presets.json` - 저장된 프리셋 목록 (자동 생성)

## 기술 스택

- **Framework**: .NET 10.0
- **UI Library**: MaterialSkin.2
- **Communication**: UDP Socket
- **Data Storage**: JSON 파일

## 통신 프로토콜

### 1. 라우팅 명령 (ASCII)
- **단일 출력**: `[입력]v[출력].` (예: `1v2.`)
- **다중 출력**: `[입력]v[출력1],[출력2],...` (예: `1v2,3,4.`)
- **모든 출력**: `[입력]All.` (예: `5All.`)
- **1:1 매핑**: `All#.`

### 2. 장치 검색 (UDP Broadcast)
- **포트**: 7000
- **주소**: 255.255.255.255
- **패킷**: `a5 6c 14 00 81 ff 01 00 00 00 00 00 00 00 00 00 ff a5 03 ae`

### 3. 상태 조회 (UDP Unicast)
- **장치 상태 조회**: Command `0x53`
- **LCD 상태 조회**: Command `0x50`
- **패킷 구조**:
  - Header: `0xa5 0x6c`
  - Data Length: 2 bytes (Little Endian)
  - Device Type: 1 byte (`0x82`: Matrix, `0x03`: LCD)
  - Device ID: 1 byte
  - Interface: 1 byte (`0x01`: LAN)
  - Reserve: 9 bytes
  - Command: 1 byte
  - Checksum: 2 bytes (Little Endian)
  - End: `0xae`

## API 가이드

전체 API 명세는 `APIGuide/API Guide.md` 파일을 참조하세요.

## 라이선스

이 프로젝트는 학습 및 개인적 용도로 제작되었습니다.
