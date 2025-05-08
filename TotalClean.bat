del /q *.ncb
del /a:h /q *.suo
del /s /q *.user
del /s /q *.pdb
del /s /q *.ilk
del /s /q *.exe
for /d %%G in ("*") do rmdir /s /q "%%G/Debug/"
for /d %%G in ("*") do rmdir /s /q "%%G/Release/"
for /d %%G in ("*") do rmdir /s /q "%%G/Vtune/"
for /d %%G in ("*") do rmdir /s /q "%%G/bin/"
for /d %%G in ("*") do rmdir /s /q "%%G/obj/"
for /d %%G in ("*") do rmdir /s /q "%%G/Content/bin/"
for /d %%G in ("*") do rmdir /s /q "%%G/Content/obj/"