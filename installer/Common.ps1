Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$PackageRoot = $PSScriptRoot
function Read-Json([string]$Path) { Get-Content -LiteralPath $Path -Raw -Encoding UTF8 | ConvertFrom-Json }
function Write-Json($Value,[string]$Path) { $Value | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $Path -Encoding UTF8 }
function Get-Sha([string]$Path) {
    $stream=[IO.File]::OpenRead($Path);$algorithm=[Security.Cryptography.SHA256]::Create()
    try {return [BitConverter]::ToString($algorithm.ComputeHash($stream)).Replace('-','')}
    finally {$stream.Dispose();$algorithm.Dispose()}
}
function Safe-Child([string]$Base,[string]$Relative) {
    $baseFull = [IO.Path]::GetFullPath($Base).TrimEnd('\')
    $full = [IO.Path]::GetFullPath((Join-Path $baseFull $Relative))
    if (-not $full.StartsWith($baseFull+'\',[StringComparison]::OrdinalIgnoreCase)) { throw "Path outside allowed folder: $full" }
    $walk = $full
    while ($walk.Length -ge $baseFull.Length) {
        if ((Test-Path -LiteralPath $walk) -and ((Get-Item -LiteralPath $walk -Force).Attributes -band [IO.FileAttributes]::ReparsePoint)) { throw "Linked paths are not supported: $walk" }
        if ($walk -eq $baseFull) { break }
        $walk = Split-Path -Parent $walk
    }
    return $full
}
function Select-Game([string]$GamePath) {
    if ([string]::IsNullOrWhiteSpace($GamePath)) {
        Add-Type -AssemblyName System.Windows.Forms
        [System.Windows.Forms.Application]::EnableVisualStyles()
        $dialog=New-Object System.Windows.Forms.FolderBrowserDialog
        $dialog.Description='选择杀戮尖塔 2 游戏根目录（包含 SlayTheSpire2.exe），不要选 mods 文件夹。'
        $dialog.ShowNewFolderButton=$false
        $dialog.RootFolder=[Environment+SpecialFolder]::MyComputer
        try {
            while ($true) {
                if ($dialog.ShowDialog() -ne [Windows.Forms.DialogResult]::OK) { Write-Host 'Cancelled. No game files changed.'; return $null }
                $candidate=$dialog.SelectedPath
                if ((Test-Path -LiteralPath (Join-Path $candidate 'SlayTheSpire2.exe') -PathType Leaf) -and (Test-Path -LiteralPath (Join-Path $candidate 'mods') -PathType Container)) { $GamePath=$candidate; break }
                [void][Windows.Forms.MessageBox]::Show('没有找到游戏程序和 mods 目录，请重新选择游戏根目录。','目录不正确')
            }
        } finally { $dialog.Dispose() }
    }
    $root=[IO.Path]::GetFullPath($GamePath).TrimEnd('\')
    if (-not (Test-Path -LiteralPath (Safe-Child $root 'SlayTheSpire2.exe') -PathType Leaf)) { throw 'Game executable not found.' }
    if (-not (Test-Path -LiteralPath (Safe-Child $root 'mods') -PathType Container)) { throw 'Mods folder not found.' }
    return $root
}
function Assert-Closed([string]$Root) {
    foreach ($p in @(Get-Process -Name 'SlayTheSpire2','sts2' -ErrorAction SilentlyContinue)) {
        if (-not $p.Path -or $p.Path.StartsWith($Root+'\',[StringComparison]::OrdinalIgnoreCase)) { throw 'Close the game before installing or rolling back.' }
    }
}
function Inventory([string]$Root) {
    $map=@{}; $mods=Safe-Child $Root 'mods'
    foreach ($file in Get-ChildItem -LiteralPath $mods -Filter '*.json' -File -Recurse) {
        try {$m=Read-Json $file.FullName} catch {continue}
        if (-not $m -or -not $m.PSObject.Properties['id'] -or -not ($m.PSObject.Properties['has_dll'] -or $m.PSObject.Properties['has_pck'])) {continue}
        $id=[string]$m.id
        if (-not $id) {continue}
        [void](Safe-Child $mods $file.FullName.Substring($mods.Length+1))
        if ($map.ContainsKey($id)) {throw "Duplicate mod ID: $id. Remove duplicate copies before installation."}
        $map[$id]=$file
    }
    return $map
}
function Check-Package {
    $checks=Read-Json (Join-Path $PackageRoot 'checksums.json')
    foreach ($f in $checks) {
        $path=Safe-Child $PackageRoot $f.path
        if (-not (Test-Path -LiteralPath $path -PathType Leaf) -or (Get-Sha $path) -ne $f.sha256) {throw "Package checksum failed: $($f.path)"}
    }
}
function Get-Plan([string]$Root) {
    Check-Package
    $spec=Read-Json (Join-Path $PackageRoot 'plan.json')
    $map=Inventory $Root
    $all=@(); $changes=@(); $projected=@{}
    $mods=Safe-Child $Root 'mods'
    foreach ($c in $spec.components) {
        $mf=$null
        if ($map.ContainsKey($c.id)) {$mf=$map[$c.id]}
        if ($c.required -and -not $mf) {throw "Required original mod missing: $($c.id)"}
        $folder=if($mf){$mf.DirectoryName}else{Safe-Child $mods $c.id}
        if ($c.id -eq 'StartingDeckTweaks' -and (Test-Path -LiteralPath (Join-Path $folder 'StartingDeckTweaks.pck'))) {throw 'StartingDeckTweaks has an unexpected PCK; this package targets the verified DLL-only build.'}
        foreach ($f in $c.files) {
            $target=if($f.isManifest -and $mf){$mf.FullName}else{Safe-Child $folder $f.name}
            $relative=$target.Substring($Root.Length+1)
            [void](Safe-Child $Root $relative)
            $exists=Test-Path -LiteralPath $target -PathType Leaf
            if ($c.required -and -not $exists -and -not ($f.PSObject.Properties['allowMissing'] -and $f.allowMissing -eq $true -and $f.name.StartsWith('patch-info/'))) {throw "Original mod file missing: $target"}
            $oldHash=if($exists){Get-Sha $target}else{''}
            if ($exists -and $oldHash -notin $f.allowedExisting) {throw "Unsupported existing file: $target. No game files changed."}
            $item=[pscustomobject]@{relative=$relative;target=$target;source=(Safe-Child $PackageRoot $f.source);existed=$exists;oldHash=$oldHash;newHash=$f.sha256;operation='write';backup=''}
            $all+=,$item; $projected[$target]=$f.sha256
            if ($oldHash -ne $f.sha256) {$changes+=,$item}
        }
    }
    foreach ($d in $spec.dependencies) {
        $folder=$Root
        if ($d.modId) {
            if (-not $map.ContainsKey($d.modId)) {throw "Required dependency mod missing: $($d.modId)"}
            $folder=$map[$d.modId].DirectoryName
        }
        $path=Safe-Child $folder $d.path
        $hash=if($projected.ContainsKey($path)){$projected[$path]}elseif(Test-Path -LiteralPath $path -PathType Leaf){Get-Sha $path}else{''}
        if ($hash -ne $d.sha256) {throw "Dependency version mismatch: $($d.modId) / $($d.path). No game files changed."}
    }
    foreach ($retired in $spec.retired) {
        if (-not $map.ContainsKey($retired.id)) {continue}
        $mf=$map[$retired.id]; $expected=@()
        foreach ($f in $retired.files) {
            $target=if($f.isManifest){$mf.FullName}else{Safe-Child $mf.DirectoryName $f.name}
            if (-not (Test-Path -LiteralPath $target -PathType Leaf) -or (Get-Sha $target) -ne $f.sha256) {throw "Unrecognized legacy fix: $target"}
            $expected+=,$target
            $changes+=,[pscustomobject]@{relative=$target.Substring($Root.Length+1);target=$target;source='';existed=$true;oldHash=$f.sha256;newHash='';operation='remove';backup=''}
        }
        foreach ($extra in Get-ChildItem -LiteralPath $mf.DirectoryName -Force) {if($extra.FullName -notin $expected){throw "Legacy fix has unknown files: $($extra.FullName)"}}
    }
    return [pscustomobject]@{all=$all;changes=$changes;version=$spec.version}
}
function Set-File([string]$Source,[string]$Target,[string]$Hash,[string]$BackupFolder) {
    New-Item -ItemType Directory -Path (Split-Path -Parent $Target) -Force | Out-Null
    $temp=Join-Path (Split-Path -Parent $Target) ('.sts2-fix-'+[guid]::NewGuid().ToString('N')+'.tmp')
    try {
        Copy-Item -LiteralPath $Source -Destination $temp
        if ((Get-Sha $temp) -ne $Hash) {throw 'Staged file checksum mismatch.'}
        if (Test-Path -LiteralPath $Target) {
            $swap=Safe-Child $BackupFolder ('swap-'+[guid]::NewGuid().ToString('N')+'.bak')
            [IO.File]::Replace($temp,$Target,$swap)
        } else { [IO.File]::Move($temp,$Target) }
        if ((Get-Sha $Target) -ne $Hash) {throw "Write verification failed: $Target"}
    } finally {if(Test-Path -LiteralPath $temp){Remove-Item -LiteralPath $temp}}
}
function Apply-Plan([string]$Root,$Plan) {
    if (@($Plan.changes).Count -eq 0) {Write-Host 'Already installed: all files match. No game files changed.';return}
    Assert-Closed $Root
    $backup=Safe-Child $Root ('mod_fix_backups\STS2-AllFixes-20261102-'+[DateTime]::UtcNow.ToString('yyyyMMddHHmmssfff')+'-'+[guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory -Path $backup -Force | Out-Null
    $i=0
    foreach($f in $Plan.changes) {
        $f.backup="original-$i.bak"; $i++
        if ($f.existed) {
            if ((Get-Sha $f.target) -ne $f.oldHash) {throw 'Target changed since preflight.'}
            Copy-Item -LiteralPath $f.target -Destination (Safe-Child $backup $f.backup)
            if ((Get-Sha (Safe-Child $backup $f.backup)) -ne $f.oldHash) {throw 'Backup verification failed; no game files changed.'}
        }
    }
    $record=[pscustomobject]@{version=$Plan.version;status='prepared';files=@($Plan.changes|Select-Object relative,existed,oldHash,newHash,operation,backup)}
    $journal=Join-Path $backup 'restore.json';Write-Json $record $journal
    $attempted=@()
    try {
        foreach($f in $Plan.changes) {
            if ($f.existed) {if((Get-Sha $f.target) -ne $f.oldHash){throw 'File changed during install.'}}
            elseif(Test-Path -LiteralPath $f.target){throw 'File appeared during install.'}
            $attempted+=,$f
            if ($f.operation -eq 'remove') {Remove-Item -LiteralPath $f.target}
            else {Set-File $f.source $f.target $f.newHash $backup}
        }
        foreach($f in $Plan.all){if((Get-Sha $f.target) -ne $f.newHash){throw 'Final payload verification failed.'}}
        $record.status='installed';Write-Json $record $journal
    } catch {
        $failure=$_
        foreach($f in $attempted) {
            if($f.existed){Set-File (Safe-Child $backup $f.backup) $f.target $f.oldHash $backup}
            elseif(Test-Path -LiteralPath $f.target){Remove-Item -LiteralPath $f.target}
        }
        $record.status='install-failed-restored';Write-Json $record $journal
        throw $failure
    }
    Write-Host "Installed 11-component fix bundle; $(@($Plan.changes).Count) files changed. Restart all players."
    Write-Host "Backup: $backup"
}
function Undo-Latest([string]$Root) {
    Assert-Closed $Root
    $parent=Safe-Child $Root 'mod_fix_backups'
    if(-not(Test-Path -LiteralPath $parent)){throw 'No integrated-package backup found in this game folder.'}
    $chosen=$null
    foreach($dir in @(Get-ChildItem -LiteralPath $parent -Directory -Filter 'STS2-AllFixes-20261102-*' | Sort-Object Name -Descending)) {
        $recordPath=Safe-Child $dir.FullName 'restore.json'
        if(-not(Test-Path -LiteralPath $recordPath)){continue}
        $r=Read-Json $recordPath
        if($r.status -eq 'installed'){$chosen=[pscustomobject]@{folder=$dir.FullName;record=$r;journal=$recordPath};break}
    }
    if(-not $chosen){Write-Host 'No installed integrated package needs rollback.';return}
    $backup=$chosen.folder; $r=$chosen.record
    foreach($f in $r.files) {
        if(-not $f.relative.StartsWith('mods\',[StringComparison]::OrdinalIgnoreCase)){throw 'Invalid backup target.'}
        $target=Safe-Child $Root $f.relative
        if($f.operation -eq 'remove'){if(Test-Path -LiteralPath $target){throw "Retired file reappeared; refusing rollback: $target"}}
        elseif(-not(Test-Path -LiteralPath $target -PathType Leaf) -or (Get-Sha $target) -ne $f.newHash){throw "File changed since installation; refusing rollback: $target"}
        if($f.existed){$original=Safe-Child $backup $f.backup;if(-not(Test-Path -LiteralPath $original) -or (Get-Sha $original) -ne $f.oldHash){throw 'Original backup checksum failed.'}}
    }
    # Keep current files so rollback can itself be reversed if any write fails.
    $recovery=@();$i=0
    foreach($f in $r.files){
        $target=Safe-Child $Root $f.relative;$present=Test-Path -LiteralPath $target
        $copy=Safe-Child $backup ('undo-current-'+[guid]::NewGuid().ToString('N')+'.bak')
        if($present){Copy-Item -LiteralPath $target -Destination $copy;if((Get-Sha $copy) -ne $f.newHash){throw 'Rollback staging failed.'}}
        $recovery+=,[pscustomobject]@{target=$target;present=$present;copy=$copy;hash=$f.newHash;file=$f}
    }
    $attempted=@()
    try {
        foreach($x in $recovery){
            $attempted+=,$x
            if($x.file.existed){Set-File (Safe-Child $backup $x.file.backup) $x.target $x.file.oldHash $backup}
            else{Remove-Item -LiteralPath $x.target}
        }
        $r.status='rolled-back';Write-Json $r $chosen.journal
    } catch {
        $failure=$_
        foreach($x in $attempted){if($x.present){Set-File $x.copy $x.target $x.hash $backup}elseif(Test-Path -LiteralPath $x.target){Remove-Item -LiteralPath $x.target}}
        throw $failure
    }
    Write-Host 'Rollback completed: exact pre-install files restored; newly added fix files removed. Backups retained.'
}
