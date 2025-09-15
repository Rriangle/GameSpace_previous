#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT_DIR"
OUT_DIR="reports/_latest"
mkdir -p "$OUT_DIR"

# 1) TODO/佔位掃描（排除產物與第三方）
if command -v rg >/dev/null 2>&1; then
  rg -n -i \
    --glob '!**/obj/**' --glob '!**/bin/**' \
    --glob '!**/vendor/**' --glob '!**/wwwroot/lib/**' \
    --glob '!**/*.min.*' \
    -e 'TODO' -e 'FIXME' -e 'TBD' -e 'NotImplementedException' \
    -e 'lorem ipsum' -e 'xx/yy' -e '\?\?\?' -e '待補' -e '佔位' -e '暫定' -e '示意' -e '隨便' -e '草稿' \
    -- . > "$OUT_DIR/todo_scan.txt" || true
else
  grep -RInE "TODO|FIXME|TBD|NotImplementedException|lorem ipsum|xx/yy|\?\?\?|待補|佔位|暫定|示意|隨便|草稿" \
    --exclude-dir=obj --exclude-dir=bin --exclude-dir=vendor --exclude-dir=wwwroot/lib --exclude='*.min.*' \
    . > "$OUT_DIR/todo_scan.txt" || true
fi

# 2) cloc（若可用）
if command -v cloc >/dev/null 2>&1; then
  cloc . --by-file --out="$OUT_DIR/cloc.txt" || true
fi

# 3) dotnet build/test（非互動式）
if command -v dotnet >/dev/null 2>&1; then
  dotnet build GameSpace.sln -c Release --nologo > "$OUT_DIR/dotnet_build.txt" 2>&1 || true
  dotnet test GameSpace.sln -c Release --no-build --logger "trx;LogFileName=TestResults.trx" > "$OUT_DIR/dotnet_test.txt" 2>&1 || true
fi

# 4) DB 資訊（若可用 jq）
if command -v jq >/dev/null 2>&1; then
  jq '.tables | length' My_Own_Source_Of_Data/database.json > "$OUT_DIR/db_table_count.txt" 2>&1 || true
fi

# 5) 簡短摘要
BUILD_ERR_LINES=$(grep -Ei "(Build FAILED|error CS)" "$OUT_DIR/dotnet_build.txt" 2>/dev/null | wc -l || true)
TEST_SUMMARY=$(grep -Ei "(Total tests|Passed:|Failed:)" "$OUT_DIR/dotnet_test.txt" 2>/dev/null | tr -d '\r' || true)
{
  echo "# Audit Summary"
  echo "- build error lines: ${BUILD_ERR_LINES}"
  echo "- test summary:"
  echo "${TEST_SUMMARY}"
  if [ -f "$OUT_DIR/todo_scan.txt" ]; then
    echo "- todo lines: $(wc -l < "$OUT_DIR/todo_scan.txt" 2>/dev/null || echo 0)"
  fi
} > "$OUT_DIR/AUDIT_SUMMARY.md"