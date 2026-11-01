[CmdletBinding()]
param([ValidateSet('Install','Rollback','Check')][string]$Mode='Install',[string]$GamePath)
. (Join-Path $PSScriptRoot 'Common.ps1')
$root=Select-Game $GamePath
if(-not $root){exit 2}
if($Mode -eq 'Rollback'){Undo-Latest $root;return}
$plan=Get-Plan $root
Write-Host "Compatibility checks passed. 11 components, $(@($plan.changes).Count) file changes planned."
if($Mode -eq 'Check'){Write-Host 'Check only: no game files changed.';return}
Apply-Plan $root $plan
