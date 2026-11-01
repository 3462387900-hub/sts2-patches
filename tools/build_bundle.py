"""Build the existing combined installer with per-patch metadata. No game access."""
from pathlib import Path
import argparse, hashlib, json, shutil, zipfile
import notice_tool

ROOT=Path(__file__).resolve().parents[1]
NAME='STS2-All-Fixes-20261102-v2'
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest().upper()
def write(p,d):p.write_text(json.dumps(d,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
def player_notice(d):
    return notice_tool.render(d).replace(
        '此条目尚不能用于新安装。补齐发布信息并审核通过后，管理器才能提供安装。',
        '此条目尚未开放管理器独立安装。当前可使用整合包的 Install.cmd，选择包含 SlayTheSpire2.exe 的游戏目录；Check.cmd 只检查，Rollback.cmd 撤销整合包安装。')

def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--refresh-notices',action='store_true',help='Regenerate player notices and refresh only patch-info checksums, preserving previous hashes for upgrades.')
    args=parser.parse_args()
    plan=read(ROOT/'installer/plan.json');index=read(ROOT/'index.json')
    components={c['id']:c for c in plan['components']}
    assert len(components)==len(plan['components'])==len(index['entries'])==11
    records={}
    for entry in index['entries']:
        cid=entry['component_id'];assert cid in components
        assert entry['metadata']==f'patches/{cid}/patch-info/patch.json'
        info=ROOT/'patches'/cid/'patch-info';data=read(info/'patch.json')
        assert data['id']==entry['id'] and data['id'] not in records
        errors=notice_tool.validate(data,read(info/'patch.schema.json'))
        assert not errors,(cid,errors)
        records[data['id']]=data
        rendered=player_notice(data)
        if args.refresh_notices:(info/'README.zh-CN.md').write_text(rendered,encoding='utf-8')
        assert (info/'README.zh-CN.md').read_text(encoding='utf-8')==rendered, f'{cid}: stale player notice'
        for f in components[cid]['files']:
            assert f['source']==f'payload/{cid}/{f["name"]}'
            path=ROOT/'patches'/cid/f['name']
            assert path.resolve().is_relative_to((ROOT/'patches'/cid).resolve())
            actual=sha(path)
            if args.refresh_notices and f['name'].startswith('patch-info/'):
                f['allowedExisting']=sorted(set(f['allowedExisting']+[f['sha256'],actual]))
                f['sha256']=actual
            assert actual==f['sha256'], f'{cid}/{f["name"]}: changed payload'
        fingerprints={(f['relative_path'],f['sha256']) for f in data['detection']['installed_fingerprints']}
        binary_files={(f['name'],f['sha256']) for f in components[cid]['files'] if not f['name'].startswith('patch-info/')}
        assert fingerprints==binary_files, f'{cid}: installed fingerprints differ'
        actual_paths={p.relative_to(ROOT/'patches'/cid).as_posix() for p in (ROOT/'patches'/cid).rglob('*') if p.is_file()}
        assert actual_paths=={f['name'] for f in components[cid]['files']}, f'{cid}: unlisted payload'
    for data in records.values():
        for dep in data['relationships']['requires_patches']+data['relationships']['requires_when_mod_present']:
            assert dep['patch_id'] in records, f'Unresolved reference {dep}'
    if args.refresh_notices:write(ROOT/'installer/plan.json',plan)
    dest=ROOT/'dist'/NAME;dest.mkdir(parents=True,exist_ok=True)
    # No deletions: refuse unknown leftovers instead of silently bundling them.
    expected={p.name for p in (ROOT/'installer').iterdir() if p.is_file()}
    for p in (ROOT/'installer').iterdir():
        if p.is_file():shutil.copy2(p,dest/p.name)
    for c in plan['components']:
        for f in c['files']:
            target=dest/f['source'];target.parent.mkdir(parents=True,exist_ok=True)
            shutil.copy2(ROOT/'patches'/c['id']/f['name'],target);expected.add(f['source'])
    expected.add('checksums.json')
    actual={p.relative_to(dest).as_posix() for p in dest.rglob('*') if p.is_file()}
    assert actual<=expected, f'Unexpected build leftovers: {actual-expected}'
    checks=[{'path':p.relative_to(dest).as_posix(),'sha256':sha(p)} for p in sorted(dest.rglob('*')) if p.is_file() and p.name!='checksums.json']
    write(dest/'checksums.json',checks)
    archive=ROOT/'dist'/(NAME+'.zip')
    with zipfile.ZipFile(archive,'w',zipfile.ZIP_DEFLATED) as z:
        for p in sorted(dest.rglob('*')):
            if p.is_file():z.write(p,NAME+'/'+p.relative_to(dest).as_posix())
    with zipfile.ZipFile(archive) as z:
        assert z.testzip() is None
        for c in components:assert f'{NAME}/payload/{c}/patch-info/patch.json' in z.namelist()
    (ROOT/'dist'/'SHA256SUMS.txt').write_text(sha(archive)+'  '+archive.name+'\n',encoding='ascii')
    print('PASS: 11 embedded records, player notices, payload hashes, dependency references and ZIP verified.')
    print(archive)
if __name__=='__main__':main()
