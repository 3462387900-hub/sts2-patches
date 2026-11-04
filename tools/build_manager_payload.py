"""Build the manager's reviewed data cache; no standalone launchers are emitted."""
from pathlib import Path
import hashlib,json,zipfile
import notice_tool
ROOT=Path(__file__).resolve().parents[1]
NAME='STS2-Manager-Payload-20261105-v4'
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
def data(d):return (json.dumps(d,ensure_ascii=False,indent=2)+'\n').encode('utf-8')
def sha(b):return hashlib.sha256(b).hexdigest().upper()
def main():
 plan=read(ROOT/'manager-plan.json');index=read(ROOT/'index.json');schema=read(ROOT/'tools/patch.schema.json')
 components={c['id']:c for c in plan['components']}
 assert len(components)==len(index['entries'])==18
 script=(ROOT/'installer/Common.ps1').read_text(encoding='utf-8-sig').replace('Installed 11-component fix bundle;','Installed selected patch files;')
 files={'Common.ps1':b'\xef\xbb\xbf'+script.encode('utf-8')}
 ids={e['id'] for e in index['entries']}
 dependencies={(d['modId'],d['path']):d for d in plan['dependencies']}
 for e in index['entries']:
  cid=e['component_id'];folder=ROOT/'patches'/cid;metadata=read(folder/'patch-info/patch.json')
  assert metadata['id']==e['id'];assert not notice_tool.validate(metadata,schema)
  for dep in metadata['relationships']['requires_patches']+metadata['relationships']['requires_when_mod_present']:
   assert dep['patch_id'] in ids,dep
  for f in components[cid]['files']:
   assert f['source']==f'payload/{cid}/{f["name"]}'
   p=(folder/f['name']).resolve();assert p.is_relative_to(folder.resolve())
   b=p.read_bytes();assert sha(b)==f['sha256'];files[f['source']]=b
  # Cache metadata separately from the install recipe. New fixes install only DLL + mod manifest.
  for p in (folder/'patch-info').iterdir():
   if p.is_file():files[f'payload/{cid}/patch-info/{p.name}']=p.read_bytes()
  if not (ROOT/'src'/cid/'baseline.json').exists():continue
  for profile in metadata['compatibility']['profiles']:
   assert len(metadata['compatibility']['profiles'])==1
   for f in profile['game_files']:
    item={'modId':'','path':f['relative_path'],'sha256':f['sha256']};key=('',item['path'])
    assert key not in dependencies or dependencies[key]['sha256']==item['sha256'];dependencies[key]=item
   for mod in profile['required_mods']:
    for f in mod['files']:
     item={'modId':mod['mod_id'],'path':f['relative_path'],'sha256':f['sha256']};key=(item['modId'],item['path'])
     assert key not in dependencies or dependencies[key]['sha256']==item['sha256'];dependencies[key]=item
 plan['dependencies']=list(dependencies.values());files['plan.json']=data(plan)
 files['checksums.json']=data([{'path':p,'sha256':sha(b)} for p,b in sorted(files.items())])
 out=ROOT/'dist';out.mkdir(exist_ok=True);archive=out/(NAME+'.zip')
 with zipfile.ZipFile(archive,'w',zipfile.ZIP_DEFLATED) as z:
  for name,b in sorted(files.items()):
   info=zipfile.ZipInfo(NAME+'/'+name,(2026,11,5,0,0,0));info.compress_type=zipfile.ZIP_DEFLATED;z.writestr(info,b)
 with zipfile.ZipFile(archive) as z:
  assert z.testzip() is None
  assert not any(n.lower().endswith('.cmd') for n in z.namelist())
 (out/'manager-payload-checksum.json').write_bytes(data({'file_name':archive.name,'sha256':sha(archive.read_bytes()),'size':archive.stat().st_size}))
 print(archive);print(sha(archive.read_bytes()),archive.stat().st_size)
if __name__=='__main__':main()

