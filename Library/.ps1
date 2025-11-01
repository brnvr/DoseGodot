# Remove all .uid files recursively from a directory

param (
    [string]$Path = "."
)

Write-Host "Deleting all .uid files under: $Path" -ForegroundColor Cyan

# Get all .uid files recursively and delete them
Get-ChildItem -Path $Path -Filter *.uid -Recurse -Force |
    ForEach-Object {
        Remove-Item $_.FullName -Force
        Write-Host "Deleted:" $_.FullName -ForegroundColor DarkGray
    }

Write-Host "Done!" -ForegroundColor Green