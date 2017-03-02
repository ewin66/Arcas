del publishArcas.zip
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& Add-Type -A System.IO.Compression.FileSystem; [IO.Compression.ZipFile]::CreateFromDirectory('publish', 'publishArcas.zip')"
pause