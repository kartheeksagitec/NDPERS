del C:\PERSLinkSource\PERSLink\Dev\Files\Correspondence\Generated\*.* /Q
del C:\PERSLinkSource\PERSLink\Dev\Files\Reports\Generated\*.* /Q
del C:\PERSLinkSource\PERSLink\Dev\Files\Reports\Generated1099r\*.* /Q
del C:\PERSLinkSource\PERSLink\Dev\Files\Reports\GeneratedESS\*.* /Q
del C:\PERSLinkSource\PERSLink\Dev\Files\Reports\GeneratedMAS\*.* /Q
del C:\PERSLinkSource\PERSLink\Dev\Files\Reports\GeneratedPayment\*.* /Q
del C:\PERSLinkSource\PERSLink\Dev\Files\Reports\PublishedToWSS\*.* /Q
del C:\PERSLinkSource\PERSLink\Dev\ErrorLog\*.* /Q
del C:\PERSLinkSource\PERSLink\Dev\Bin\*.pdb /Q
del C:\PERSLinkSource\PERSLink\Dev\slnNeoSpin\NeoSpinWebClient\Bin\*.pdb /Q
del C:\PERSLinkSource\PERSLink\Dev\slnNeoSpin\NeoSpinWSS\Bin\*.pdb /Q
del C:\PERSLinkSource\PERSLink\Dev\slnNeoSpin\NeoSpinMSS\Bin\*.pdb /Q
del C:\PERSLinkSource\PERSLink\Dev\Bin\*.vshost.exe.config /Q
del C:\PERSLinkSource\PERSLink\Dev\Bin\*.vshost.exe.manifest /Q
del C:\PERSLinkSource\PERSLink\Dev\Bin\*.vshost.exe /Q
for /f "delims=" %%x in ('dir /b /s /ad ..\obj') do rd /s /q "%%x"
pause
