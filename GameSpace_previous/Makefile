SHELL := /usr/bin/bash
.ONESHELL:

.PHONY: audit build test reports ci-local

audit:
	@echo "[audit] running scans..."
	@mkdir -p reports/_latest
	@./scripts/run_audit.sh || true

build:
	@echo "[build] dotnet build Release"
	@mkdir -p reports/_latest
	@dotnet build GameSpace.sln -c Release --nologo > reports/_latest/dotnet_build.txt 2>&1 || true

test:
	@echo "[test] dotnet test Release"
	@mkdir -p reports/_latest
	@dotnet test GameSpace.sln -c Release --no-build --logger "trx;LogFileName=TestResults.trx" > reports/_latest/dotnet_test.txt 2>&1 || true

reports:
	@echo "[reports] summarize"
	@build_err=$$(grep -Ei "(Build FAILED|error CS)" reports/_latest/dotnet_build.txt | wc -l || true); \
	test_sum=$$(grep -Ei "(Total tests|Passed:|Failed:)" reports/_latest/dotnet_test.txt | tr -d "\r" || true); \
	printf "# Build/Test 摘要\n- build error lines: %s\n- test summary:\n%s\n" "$$build_err" "$$test_sum" > reports/_latest/BT_LATEST.md

ci-local: audit build test reports
	@echo "[ci-local] done"
