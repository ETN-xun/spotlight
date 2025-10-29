import json
import sys

def rename_animation_in_json(file_path, old_name, new_name):
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            data = json.load(f)
        
        if 'animations' in data and old_name in data['animations']:
            # 重命名动画
            data['animations'][new_name] = data['animations'].pop(old_name)
            
            # 保存修改后的文件
            with open(file_path, 'w', encoding='utf-8') as f:
                json.dump(data, f, ensure_ascii=False, separators=(',', ':'))
            
            print(f'Successfully renamed animation "{old_name}" to "{new_name}" in {file_path}')
            return True
        else:
            print(f'Animation "{old_name}" not found in {file_path}')
            return False
    except Exception as e:
        print(f'Error: {e}')
        return False

# 修改影角色的BreakpointKill动画
rename_animation_in_json(r'c:\Users\LWX\spotlight\Assets\Art\Character\影\shuchu\Shadow.json', 'BreakpointKill', 'Attack')