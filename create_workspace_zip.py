#!/usr/bin/env python3
"""
創建整個工作區的 ZIP 檔案
"""
import os
import zipfile
import datetime
from pathlib import Path

def create_workspace_zip():
    """創建工作區 ZIP 檔案"""
    workspace_root = Path('/workspace')
    timestamp = datetime.datetime.now().strftime('%Y%m%d-%H%M%S')
    zip_filename = f'workspace-complete-{timestamp}.zip'
    zip_path = workspace_root / 'artifacts' / zip_filename
    
    # 確保 artifacts 目錄存在
    zip_path.parent.mkdir(exist_ok=True)
    
    # 排除的目錄和檔案
    exclude_patterns = {
        'node_modules',
        '__pycache__',
        '.pytest_cache',
        '.coverage',
        '*.pyc',
        '*.pyo',
        '*.pyd',
        '.DS_Store',
        'Thumbs.db'
    }
    
    def should_exclude(path):
        """檢查路徑是否應該被排除"""
        path_str = str(path)
        for pattern in exclude_patterns:
            if pattern in path_str:
                return True
        return False
    
    print(f"開始創建 ZIP 檔案: {zip_path}")
    
    with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for root, dirs, files in os.walk(workspace_root):
            # 過濾排除的目錄
            dirs[:] = [d for d in dirs if not should_exclude(Path(root) / d)]
            
            for file in files:
                file_path = Path(root) / file
                
                if should_exclude(file_path):
                    continue
                
                # 計算相對路徑
                arcname = file_path.relative_to(workspace_root)
                
                try:
                    zipf.write(file_path, arcname)
                    print(f"已添加: {arcname}")
                except Exception as e:
                    print(f"跳過檔案 {file_path}: {e}")
    
    print(f"ZIP 檔案創建完成: {zip_path}")
    print(f"檔案大小: {zip_path.stat().st_size / (1024*1024):.2f} MB")
    
    return zip_path

if __name__ == '__main__':
    zip_path = create_workspace_zip()
    print(f"完成！ZIP 檔案位置: {zip_path}")