param([Parameter(Mandatory=$true)][string]$FixtureRoot,[Parameter(Mandatory=$true)][string]$PackageDirectory,[Parameter(Mandatory=$true)][string]$SelectionBridge)
$ErrorActionPreference='Stop'
$root=[IO.Path]::GetFullPath($FixtureRoot)
if(-not(Test-Path -LiteralPath (Join-Path $root '.spirepatch-fixture')) -or (Get-Item -LiteralPath (Join-Path $root 'SlayTheSpire2.exe')).Length -ne 0){throw 'Not a fixture'}
$package=[IO.Path]::GetFullPath($PackageDirectory)
. (Join-Path $package 'Common.ps1')
. $SelectionBridge
$ids=@('TurnEndOrderFix','VictoryBreedsArroganceSyncFix','KakarotElbowTargetFix','NymphKazdelRewardFix','Act3SnapshotSaveFix','NeowOptionOrderFix')
$script:checks=0
function Check($ok,$message){if(-not $ok){throw $message};$script:checks++;Write-Output "PASS $message"}
function Snapshot { $map=@{};foreach($f in Get-ChildItem -LiteralPath (Join-Path $root 'mods') -File -Recurse){$map[$f.FullName]=Get-Sha $f.FullName};return $map }
function Same($a,$b){if($a.Count -ne $b.Count){return $false};foreach($k in $a.Keys){if($a[$k] -ne $b[$k]){return $false}};return $true}
function Reject($action,$message){$failed=$false;try{& $action|Out-Null}catch{$failed=$true};Check $failed $message}
$before=Snapshot
$plan=Get-SelectedPlan $root $ids
Check ((Same $before (Snapshot)) -and -not(Test-Path -LiteralPath (Join-Path $root 'mod_fix_backups'))) 'preview is read-only and cancellation before apply writes nothing'
Check ($plan.changes.Count -eq 12) 'six standalone patches install exactly twelve runtime files'
Check (@($plan.changes|Where-Object{$_.relative -like '*patch-info*'}).Count -eq 0) 'metadata and schema remain in cache, not mod loader folders'
foreach($id in $ids){$single=Get-SelectedPlan $root @($id);Check ($single.changes.Count -eq 2) "$id has independent selection recipe"}
Reject {Select-Game (Join-Path $root 'mods')} 'wrong game directory rejected'
$dependency=Join-Path $root 'data_sts2_windows_x86_64/sts2.dll';$bytes=[IO.File]::ReadAllBytes($dependency)
try{[IO.File]::WriteAllBytes($dependency,[byte[]]@(1,2,3));Reject {Get-SelectedPlan $root $ids} 'unknown game build rejected before writes'}finally{[IO.File]::WriteAllBytes($dependency,$bytes)}
$duplicate=Join-Path $root 'mods/duplicate-test.json';try{[IO.File]::WriteAllText($duplicate,'{"id":"BaseLib","has_dll":true}');Reject {Get-SelectedPlan $root $ids} 'duplicate mod identity rejected'}finally{Remove-Item -LiteralPath $duplicate}
$payload=Join-Path $package 'payload/TurnEndOrderFix/TurnEndOrderFix.dll';$bytes=[IO.File]::ReadAllBytes($payload)
try{[IO.File]::WriteAllBytes($payload,[byte[]]@(1,2,3));Reject {Get-SelectedPlan $root $ids} 'corrupt cached payload rejected'}finally{[IO.File]::WriteAllBytes($payload,$bytes)}
Apply-Plan $root (Get-SelectedPlan $root $ids)
$installed=Snapshot
Check (($installed.Count-$before.Count) -eq 12) 'selected install writes only twelve new runtime files'
Check ((Get-SelectedPlan $root $ids).changes.Count -eq 0) 'repeated install has no changes'
$target=Join-Path $root 'mods/TurnEndOrderFix/TurnEndOrderFix.dll';$bytes=[IO.File]::ReadAllBytes($target)
try{[IO.File]::WriteAllBytes($target,[byte[]]@(1,2,3));Reject {Get-SelectedPlan $root $ids} 'unknown installed patch rejected';Reject {Undo-Latest $root} 'rollback refuses a later modified file'}finally{[IO.File]::WriteAllBytes($target,$bytes)}
Undo-Latest $root
Check (Same $before (Snapshot)) 'rollback restores original file hashes exactly'
Write-Output "PASS $script:checks recipe/transaction checks; isolated fixture only, not current manager UI integration."
