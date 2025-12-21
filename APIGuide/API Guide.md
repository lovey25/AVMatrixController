# 8x8 Matrix Switcher API Guide (Ver 1.1)

## 1. RS232 제어 가이드 (API Guide for RS232)

### 연결 설정 (Connection Settings)

- **Port Settings:** Bps 9600, Data bits 8, Parity None, Stop Bits 1, Flow Control None

### 통신 프로토콜 (Communication Protocol)

프로토콜은 아래의 3가지 형식을 따릅니다. ASCII 코드로 전송되며 별도의 피드백(응답)은 없습니다.

1. **`[x]v[y].`**: 입력 "x"를 출력 "y"에 연결
2. **`[x]v[y],[z].`**: 입력 "x"를 출력 "y"와 "z"에 연결
3. **`[x]All.`**: 입력 "x"를 모든 출력에 연결

### 예시 (Examples)

- `1v2.`: Input 1을 Output 2로 연결
- `1v2,3,4,5.`: Input 1을 Output 2, 3, 4, 5로 연결
- `5All.`: Input 5를 모든 Output으로 연결
- `All#.`: 모든 채널을 1:1로 대응 (1->1, 2->2...)

### 주의사항

> - 모든 명령어는 반드시 마침표(`.`)로 끝나야 합니다.
> - "v" 형식을 사용하여 하나의 입력에 여러 출력을 할당할 수 있습니다.
> - "All"은 항상 출력 채널(Output Channels)을 의미합니다.
> - 각 명령줄에서는 하나의 입력(Input)만 라우팅할 수 있습니다.

---

## 2. LAN 설정 및 제어 가이드 (Configuration and Control API Guide for LAN)

### 개요 (Description)

이 문서는 매트릭스 스위치 설정 및 제어 API(OMSCC API)를 설명합니다. 이 API는 Broadcast 및 Unicast 주소를 활용하는 **HTTP UDP 패킷 전송**을 사용합니다.

### 2.1 네트워크상에서 스위처 검색 (Locating a Switcher on the Network)

- **방식 (Method):** UDP Broadcast
- **목적지 주소 (Destination Address):** Broadcast `255.255.255.255`
- **목적지 포트 (Destination Port):** 7000
- **전송 패킷 (Packet Format):**
  ```text
  a5 6c 14 00 81 ff 01 00 00 00 00 00 00 00 00 00 ff a5 03 ae
  ```
- **응답 예시 (Return):**
  ```text
  a5 6c 23 00 82 ff 01 00 00 00 00 00 00 00 00 00 ff 00 4d 53 53 30 38 31 31 2d 10 2d 43 04 31 55 a9 06 ae
  ```

> **응답 코드 분석:**
>
> - `82` (Red marked): 장치 타입 0x82 (매트릭스 스위처)를 의미
> - `00` (Red marked): 데이터 반환 성공을 의미
> - `4D...55` (Green marked): 장치 모델명 (예: 8x8 Matrix Switcher). 장비마다 코드가 다름

### 2.2 출력 포트 설정 - 영상 절체 (Configuring Output Ports)

- **설명:** 입력 포트를 기반으로 출력 포트를 구성합니다.
- **방식 (Method):** UDP Unicast
- **목적지 주소:** 매트릭스 스위처의 IP 주소
- **목적지 포트:** 7000
- **명령어:** 모든 명령어는 ASCII 코드로 전송해야 합니다.

| 명령어 형식    | 설명                             |
| :------------- | :------------------------------- |
| `[x]v[y].`     | 입력 "x"를 출력 "y"에 연결       |
| `[x]v[y],[z].` | 입력 "x"를 출력 "y"와 "z"에 연결 |
| `[x]All.`      | 입력 "x"를 모든 출력에 연결      |

- **예시 (Examples):**
  - `1v2.` (Input 1 -> Output 2)
  - `1v2,3,4,5.` (Input 1 -> Outputs 2,3,4,5)
  - `5All.` (Input 5 -> All Outputs)
  - `All#.` (1:1 대응)

> **주의사항:**
>
> - 모든 명령어는 `.`으로 끝나야 합니다.
> - "v" 형식을 사용하여 하나의 입력에 여러 출력을 할당할 수 있습니다.
> - "All"은 항상 출력 채널을 의미합니다.
> - 각 명령줄에서는 하나의 입력만 라우팅할 수 있습니다.
> - **Return:** 없음 (None)

---

## 3. 포트 상태 쿼리 및 제어 (Query Status of Ports)

- **설명:** 출력 포트 상태, LCD, IP, 장치 이름 등을 확인하고 설정합니다.
- **방식 (Method):** UDP Unicast
- **목적지 주소:** 매트릭스 스위처 IP
- **목적지 포트:** 7000

### 3.1 패킷 포맷: PC -> Switcher (Send)

| Data Packet        | Value (hex)         | Byte Length | Description                                             |
| :----------------- | :------------------ | :---------- | :------------------------------------------------------ |
| **Packet Header**  | `0xa5 0x6c`         | 2           | 데이터 패킷의 시작                                      |
| **Data Length**    | `0x0000` ~ `0x0420` | 2           | 헤더부터 끝까지의 전체 길이 (하위 바이트가 앞)          |
| **Device Type**    | `0x00` ~ `0xff`     | 1           | 장치 유형 정의 (`0xff`: Broadcast)                      |
| **Device ID**      | `0x00` ~ `0xff`     | 1           | 동일 LAN 내 장치 구분 (`0xff`: Broadcast)               |
| **Interface Type** | `0x00` ~ `0xff`     | 1           | `0x00`: UART (Serial), `0x01`: LAN                      |
| **Reserve**        | `0x00`              | 9           | 예약됨 (Reserve)                                        |
| **Command**        | `0x00` ~ `0xff`     | 1           | 각 기능별 명령어                                        |
| **Packet Data**    | 가변                | <= 1024     | 데이터 내용                                             |
| **Checksum**       | `0x0000` ~ `0xfff`  | 2           | 헤더부터 체크섬 직전까지의 대수적 합 (하위 바이트가 앞) |
| **Packet End**     | `0xae`              | 1           | 패킷의 끝                                               |

### 3.2 패킷 포맷: Switcher -> PC (Response)

| Data Packet                     | Value (hex)     | Byte Length | Description                                       |
| :------------------------------ | :-------------- | :---------- | :------------------------------------------------ |
| **Packet Header / Length**      | `0xa5 0x6c` ... | 2 / 2       | 시작 및 전체 길이                                 |
| **Device Type**                 | `0x00` ~ `0xff` | 1           | 장치 유형                                         |
| **Device ID / Interface**       | `0x00` ~ `0xff` | 1 / 1       | 장치 ID 및 인터페이스 (`0x00`: UART, `0x01`: LAN) |
| **Reserve**                     | `0x00`          | 9           | 예약됨                                            |
| **Command**                     | `0x00` ~ `0xff` | 1           | 기능별 명령어                                     |
| **Response Status**             | `0x00` ~ `0xff` | 1           | `0x00`: 성공, `0x01`: 에러                        |
| **Response Content / Checksum** | 가변 /`0x...`   | 가변 / 2    | 응답 데이터 및 체크섬                             |
| **Packet End**                  | `0xae`          | 1           | 패킷의 끝                                         |

> **Note:** Send = CMD + data; Return = CMD + status + data

### 3.3 기능 및 명령어 목록 (Function & Command)

- **Device Type:** `0x03`

| Function                        | Command (hex) | Description                                                            |
| :------------------------------ | :------------ | :--------------------------------------------------------------------- |
| **Read Status of Switcher**     | `0x53`        | IP 상태, 입출력 정보, 장치 이름 등 현재 상태 읽기                      |
| **Read Status of LCD**          | `0x50`        | LCD 백라이트 시간 및 밝기 정보 읽기                                    |
| **Setting Device Name**         | `0x0f`        | 장치 이름 설정 (최대 16자, Unicode)                                    |
| **Setting LCD Backlight Time**  | `0x51`        | `0`: 15s Dim, `1`: 60s Dim, `2`: 15s Off, `3`: 60s Off, `4`: Always On |
| **Setting LCD Brightness**      | `0x52`        | LCD 밝기 설정 (10-100)                                                 |
| **Setting IP (Static/Dynamic)** | `0x05`        | 데이터의 13번째 바이트:`0x01` Dynamic IP, `0x00` Static IP             |

---

## 4. 제어 예시 (Examples) - 전체 HEX 코드

### 1) 스위처 현재 상태 읽기 (Read Current Status of Switcher)

- **Send:**
  ```text
  a5 6c 14 00 82 01 01 00 00 00 00 00 00 00 00 00 53 fc 01 ae
  ```
- **Return:** (줄바꿈 없이 이어진 전체 데이터입니다)

  ```text
  a5 6c 40 00 82 01 01 00 00 00 00 00 00 00 00 00 53 00 01 02 03 04 05 06 07 08 00 01 1c 61 00 20 00 74 00 65 00 73 00 74 00 20 00 64 00 65 00 76 00 69 00 63 00 55 00 20 00 41 00 41 00 dc 07 ae
  ```

  - `53`: Command
  - `00`: Response Success
  - `01 02 ... 08`: Output status (출력 상태)
  - `00`: Reserve
  - `00`: IP Status (00: Static, 01: Dynamic)
  - 나머지 데이터: Device name (첫 바이트는 길이, 이후 32바이트는 이름)

### 2) LCD 상태 읽기 (Read Status of LCD)

- **Send:**
  ```text
  a5 6c 14 00 03 01 01 00 00 00 00 00 00 00 00 00 50 7a 01 ae
  ```
- **Return:**

  ```text
  a5 6c 17 00 03 01 02 00 00 00 00 00 00 00 00 00 50 00 00 2b a9 01 ae
  ```

  - `00`: 현재 백라이트 시간 (15s Dim)
  - `2b`: 현재 밝기 (43)

### 3) 장치 이름 설정 ("this is a matrix")

- **Send:**
  ```text
  a5 6c 34 00 82 ff 01 00 00 00 00 00 00 00 00 00 0f 74 00 68 00 69 00 73 00 20 00 69 00 73 00 20 00 61 00 20 00 6d 00 61 00 74 00 72 00 69 00 78 00 c0 08 ae
  ```
- **Return:**
  ```text
  a5 6c 15 00 82 ff 01 00 00 00 00 00 00 00 00 00 0f 00 b7 02 ae
  ```

### 4) LCD 백라이트 시간 설정 ("Always On")

- **Send:**
  ```text
  a5 6c 15 00 03 ff 01 00 00 00 00 00 00 00 00 00 51 04 7e 02 ae
  ```
- **Return:**

  ```text
  a5 6c 15 00 03 01 02 00 00 00 00 00 00 00 00 00 52 00 7e 01 ae
  ```

  > _참고: LCD 설정 시 응답 커맨드는 항상 `0x52`입니다._

### 5) LCD 밝기 설정 (100)

- **Send:**
  ```text
  a5 6c 15 00 03 ff 01 00 00 00 00 00 00 00 00 00 52 64 df 02 ae
  ```
- **Return:**
  ```text
  a5 6c 15 00 03 01 02 00 00 00 00 00 00 00 00 00 52 00 7e 01 ae
  ```

### 6) 정적 IP 설정 (Static IP, 192.168.1.219)

- **Send:**

  ```text
  a5 6c 21 00 82 ff 01 00 00 00 00 00 00 00 00 00 05 c0 a8 01 db ff ff ff 00 c0 a8 01 01 00 64 09 ae
  ```

  - `c0 a8 01 db`: IP Address (192.168.1.219)
  - `ff ff ff 00`: Subnet Mask
  - `c0 a8 01 01`: Default Gateway
  - `00`: Static IP 설정 (`01`은 DHCP)

- **Return:**
  ```text
  a5 6c 15 00 82 ff 01 00 00 00 00 00 00 00 00 00 05 00 ad 02 ae
  ```

> **주의:** 정적 IP 주소와 컴퓨터의 IP 주소는 동일한 네트워크 세그먼트에 있어야 합니다.