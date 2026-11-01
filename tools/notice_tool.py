"""Validate patch notice metadata and generate the player notice. No game access."""
from pathlib import Path
import argparse,json,re,sys
from jsonschema import Draft202012Validator,FormatChecker

ROLE={'affected':'受影响模组','trigger':'触发条件涉及的模组','dependency':'运行前置','companion':'需联动处理的组件'}
TYPE={'single_mod_bug':'单个模组问题','mod_conflict':'模组组合冲突','game_api_compat':'游戏接口兼容','patch_compat':'补丁间兼容','display_error':'显示层问题','other':'其他／尚待分类'}
CONF={'confirmed':'已确认','partial':'部分确认','suspected':'疑似','unknown':'尚未确认'}
TEST={'code_review':'代码审查','automated':'自动化检查','local_game':'本机游戏测试','multiplayer':'完整联机测试'}
RESULT={'pass':'通过','partial':'部分验证','fail':'失败','not_run':'未执行'}
MULTI={'all_players':'所有玩家安装','host_only':'仅主机安装','client_only':'指定客户端安装','not_required':'无需保持相同安装状态','unknown':'尚未确认'}
SAVE={'supported':'已验证支持','unsupported':'不支持','unknown':'尚不能确认'}

def validate(data,schema):
    errors=[f'{".".join(str(v) for v in e.absolute_path) or "$"}: {e.message}' for e in Draft202012Validator(schema,format_checker=FormatChecker()).iter_errors(data)]
    if errors:return errors
    profiles=data['compatibility']['profiles']
    if len({p['profile_id'] for p in profiles})!=len(profiles):errors.append('profile_id 必须唯一。')
    if data['issue']['type']=='mod_conflict':
        involved={c['mod_id'] for c in data['components'] if c['role'] in ('affected','trigger')}
        if len(involved)<2:errors.append('mod_conflict 至少需要两个不同的受影响/触发模组；前置依赖和联动补丁不算冲突方。')
    if data['issue']['diagnosis']['confidence']=='confirmed' and not data['issue']['diagnosis']['evidence']:errors.append('已确认的诊断必须列出证据。')
    if data['changelog'][0]['version']!=data['version']:errors.append('更新记录首项版本必须等于本条目版本。')
    if len({(c['mod_id'],c['role']) for c in data['components']})!=len(data['components']):errors.append('重复的组件角色。')
    for p in profiles:
        if len({m['mod_id'] for m in p['required_mods']})!=len(p['required_mods']):errors.append('同一匹配配置中 required_mods 不得重复。')
        if {m['mod_id'] for m in p['required_mods']} & set(p['excluded_mod_ids']):errors.append('同一配置不能同时要求和排除一个模组。')
    relations=data['relationships']
    ids=[r['patch_id'] for r in relations['requires_patches']+relations['requires_when_mod_present']]+relations['conflicts_patch_ids']+relations['supersedes_patch_ids']
    if data['id'] in ids:errors.append('补丁不能依赖、冲突或替代自身。')
    def walk(x):
        if isinstance(x,dict):
            for k,v in x.items():
                if k=='relative_path':
                    if any(s in ('','.','..') for s in re.split(r'[/\\]',v)) or re.search(r'[:<>"|?*\x00-\x1f]',v):errors.append('文件路径必须是规范相对路径，不能使用盘符、上级目录或特殊路径。')
                yield k,v
                yield from walk(v)
        elif isinstance(x,list):
            for v in x:yield from walk(v)
    if data['publication_status']=='published':
        for k,v in walk(data):
            if isinstance(v,str) and (v.startswith('待填写') or 'REPLACE_ME' in v or v=='replace-me' or v=='0'*64):errors.append(f'发布前必须替换占位内容：{k}')
        if any(t['result']=='fail' for t in data['validation']):errors.append('仍有失败验证，不能标为 published。')
        if not any(t['result']=='pass' for t in data['validation']):errors.append('发布至少需要一项明确通过的验证。')
        if data['id']=='local.replace-me':errors.append('发布前必须设置稳定补丁 ID。')
    else:
        list(walk(data))
    return errors

def render(d):
    q=lambda values:'\n'.join('- '+s for s in values)
    issue=d['issue'];diag=issue['diagnosis'];multi=d['multiplayer'];rel=d['relationships']
    state={'draft':'草稿 · 不可自动安装','published':'已发布','withdrawn':'已撤回 · 不再推荐新安装'}[d['publication_status']]
    out=[f'# {d["title"]}',f'版本：{d["version"]}　|　{state}',d['summary'],
         '## 出现什么问题',q(issue['symptoms']),'## 什么时候可能遇到',q(issue['trigger_conditions']),
         '## 涉及哪些模组',f'问题类型：{TYPE[issue["type"]]}',
         q([f'{ROLE[c["role"]]}：{c["display_name"]}（{c["mod_id"]}）——{c["explanation"]}' for c in d['components']]),
         '## 适用版本',q([f'配置 {p["profile_id"]}：游戏 {p["game_version_display"]}；'+ '、'.join(m['mod_id']+' '+m['version_display'] for m in p['required_mods']) for p in d['compatibility']['profiles']]),
         '版本号仅帮助阅读，安装前仍要核对文件。未知版本停止安装，不凭相似名称强行适配。',
         '## 修了什么',q(d['changes']['description']),f'玩法影响：{d["changes"]["gameplay_impact"]}',
         '涉及文件：',q([f'{f["mod_id"]}/{f["relative_path"]}：{f["purpose"]}' for f in d['changes']['files']]),
         '## 我需要和队友一起装吗',f'{MULTI[multi["policy"]]}。{multi["explanation"]}',
         '安装后需要重启游戏。' if multi['restart_required'] else '此补丁声明无需重启；以实际安装计划为准。',
         '## 能继续已有存档吗',f'{SAVE[d["saved_runs"]["support"]]}。{d["saved_runs"]["explanation"]}',
         '## 关联补丁']
    relations=[]
    for r in rel['requires_patches']:relations.append(f'必须安装：{r["patch_id"]}，版本至少 {r["minimum_version"]}。')
    for r in rel['requires_when_mod_present']:relations.append(f'检测到 {r["mod_id"]} 时，还需要 {r["patch_id"]}（至少 {r["minimum_version"]}）。')
    if rel['conflicts_patch_ids']:relations.append('已声明不能共存：'+'、'.join(rel['conflicts_patch_ids']))
    if rel['supersedes_patch_ids']:relations.append('可替代的旧补丁：'+'、'.join(rel['supersedes_patch_ids'])+'。移除旧包仍需安装清单明确规定。')
    out += [q(relations) if relations else '尚未声明关联补丁。这不代表已验证与所有其他补丁兼容。',
            '## 依据与验证',f'诊断结论：{CONF[diag["confidence"]]}。{diag["conclusion"]}',q(diag['evidence']) if diag['evidence'] else '暂无证据记录。',
            q([f'{TEST[t["kind"]]}：{RESULT[t["result"]]}。{t["summary"]}'+(f'（{t["date"]}）' if t['date'] else '') for t in d['validation']]),
            '## 限制与未解决事项',q(d['limitations']),
            '## 安装与撤销',('此条目尚不能用于新安装。补齐发布信息并审核通过后，管理器才能提供安装。' if d['publication_status']!='published' else '在管理器中选择自己的游戏目录，查看改动计划。文件与依赖核对通过后，再通过已审核的安装清单安装。'),
            ('支持回滚。' if d['rollback']['supported'] else '未声明支持回滚。')+d['rollback']['description'],
            '## 更新记录',q([c['version']+' · '+c['date']+'：'+'；'.join(c['changes']) for c in d['changelog']]),
            '<details>\n<summary>技术详情：文件与校验信息</summary>\n']
    for p in d['compatibility']['profiles']:
        out.append('匹配配置：'+p['profile_id'])
        for f in p['game_files']:out.append(f'- 游戏/{f["relative_path"]}：`{f["sha256"]}`')
        for m in p['required_mods']:
            for f in m['files']:out.append(f'- {m["mod_id"]}/{f["relative_path"]}：`{f["sha256"]}`')
    out+=['修复后文件特征：']
    out+= [f'- {f["mod_id"]}/{f["relative_path"]}：`{f["sha256"]}`' for f in d['detection']['installed_fingerprints']]
    out+=['文件符合只代表磁盘状态，不能证明补丁已在运行中加载或本场故障已解决。','</details>']
    return '\n\n'.join(out)+'\n'

def main():
    p=argparse.ArgumentParser(description=__doc__);p.add_argument('metadata',type=Path);p.add_argument('--render',type=Path);a=p.parse_args()
    schema=json.loads((Path(__file__).parent/'patch.schema.json').read_text(encoding='utf-8-sig'))
    data=json.loads(a.metadata.read_text(encoding='utf-8-sig'));errors=validate(data,schema)
    if errors:
        for e in errors:print('ERROR:',e,file=sys.stderr)
        return 1
    print('PASS metadata structure and local semantic checks;',data['publication_status'])
    if a.render:a.render.write_text(render(data),encoding='utf-8');print('Generated:',a.render.name)
    print('Not a release approval, registry dependency check, recipe validator or installer.')
    return 0
if __name__=='__main__':sys.exit(main())
