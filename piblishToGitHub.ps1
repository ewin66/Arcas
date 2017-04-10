param ([string]$TargetDir)

Write-Host $TargetDir

exit

$pubDir = $TargetDir + $PublishUrl
$arcFile = $TargetDir+'publish.zip'

if (!(Test-Path $pubDir))
{    
    exit
}

 if (!(Test-Path -Path $arcDir -PathType Leaf))
    {
        Remove-Item $arcDir -Recurse -force        
    }

Write-Host '---- Архивация публикации'

Add-Type -A System.IO.Compression.FileSystem
[IO.Compression.ZipFile]::CreateFromDirectory($pubDir, $arcFile)

Remove-Item $pubDir -Recurse -force