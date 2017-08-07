@echo off
echo 修改系统组策略安全设置...
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Lsa\FipsAlgorithmPolicy" /v Enabled /t REG_DWORD /d 0 /f
pause