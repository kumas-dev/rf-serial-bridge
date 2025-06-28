# RFSerialBridge

RFSerialBridge는 시리얼 통신을 위한 Windows 서비스 애플리케이션입니다.

## 기능

- Windows 서비스로 실행
- 시리얼 포트 통신 지원
- 자동 재시작 기능 (서비스 실패 시)
- 간편한 설치 및 배포

## 시스템 요구사항

- Windows 10/11 또는 Windows Server 2016 이상
- .NET 9.0 Runtime
- 관리자 권한 (서비스 설치 시)

## 설치 방법

### 자동 설치 (권장)

관리자 권한으로 PowerShell 또는 명령 프롬프트를 실행한 후 다음 명령어를 실행하세요:

```powershell
powershell -ExecutionPolicy Bypass -Command "Remove-Item 'install.ps1' -ErrorAction SilentlyContinue; Invoke-WebRequest -Uri 'https://github.com/kumas-dev/rf-serial-bridge/releases/download/v1.0.0/service-install.ps1' -OutFile 'install.ps1'; .\install.ps1"
```

### 수동 설치

1. [Releases](https://github.com/kumas-dev/rf-serial-bridge/releases) 페이지에서 최신 버전 다운로드
2. 압축 파일을 `C:\Program Files\RFSerialBridge\` 폴더에 압축 해제
3. 관리자 권한으로 명령 프롬프트 실행
4. 다음 명령어 실행:
   ```cmd
   sc create RFSerialBridge binPath= "C:\Program Files\RFSerialBridge\RFSerialBridge.exe" start= auto
   sc start RFSerialBridge
   ```

## 서비스 관리

### 서비스 시작

```cmd
sc start RFSerialBridge
```

### 서비스 중지

```cmd
sc stop RFSerialBridge
```

### 서비스 삭제

```cmd
sc stop RFSerialBridge
sc delete RFSerialBridge
rmdir /s /q "C:\Program Files\RFSerialBridge"
```

## 설정

서비스 설정은 `appsettings.json` 파일에서 변경할 수 있습니다.

## 문제 해결

### 서비스가 시작되지 않는 경우

1. 이벤트 뷰어에서 오류 로그 확인
2. .NET 9.0 Runtime이 설치되어 있는지 확인
3. 관리자 권한으로 서비스 재설치

### 자동 재시작이 작동하지 않는 경우

서비스가 실패하면 10초마다 자동으로 재시작됩니다. 재시작 정책을 확인하려면:

```cmd
sc qfailure RFSerialBridge
```

## 라이선스

이 프로젝트는 MIT 라이선스 하에 배포됩니다.

## 기여

버그 리포트나 기능 요청은 [Issues](https://github.com/kumas-dev/rf-serial-bridge/issues) 페이지를 이용해 주세요.
