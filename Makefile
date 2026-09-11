# Makefile for NetcdfSharp
#
# 目标:
#   build        - 编译全部 5 个项目 (Release)
#   test         - 运行 xUnit 测试
#   pack         - 打包 4 个库为 NuGet 包到 artifacts/
#   publish      - 打包 + 推送到本地 nuget 源 (NUGET_FEED 可覆盖)
#   bench        - 运行 BenchmarkDotNet 对比基准
#   clean        - 清理 bin/obj/artifacts
#
# 用法:
#   make build
#   make pack
#   make publish NUGET_FEED=/path/to/local-feed
#   make test
#
# 版本号唯一来源是 Directory.Build.props

CONFIG  ?= Release
SOLUTION = NetcdfSharp.slnx
ARTIFACTS = artifacts
NUGET_FEED ?= $(ARTIFACTS)

# 打包的库项目 (依赖顺序: Core -> Udunits -> Grib -> Bufr)
LIB_PROJECTS = \
	NetcdfSharp.Core/NetcdfSharp.Core.csproj \
	NetcdfSharp.Udunits/NetcdfSharp.Udunits.csproj \
	NetcdfSharp.Grib/NetcdfSharp.Grib.csproj \
	NetcdfSharp.Bufr/NetcdfSharp.Bufr.csproj

.PHONY: all build test pack publish bench clean

all: build

## build: 编译全部项目 (Release)
build:
	dotnet build $(SOLUTION) -c $(CONFIG) -v q -nologo

## test: 运行测试
test:
	dotnet test NetcdfSharp.Tests/NetcdfSharp.Tests.csproj -c $(CONFIG) -v q -nologo

## pack: 打包 4 个库为 NuGet 包
.PHONY: pack
pack:
	@echo "==> packing into $(ARTIFACTS)/"
	@mkdir -p $(ARTIFACTS)
	@for p in $(LIB_PROJECTS); do \
		dotnet pack "$$p" -c $(CONFIG) -o $(ARTIFACTS) -v q -nologo; \
	done
	@echo "==> packages:"
	@ls -1 $(ARTIFACTS)/*.nupkg 2>/dev/null || dir /b $(ARTIFACTS)\*.nupkg

## publish: 打包并推送到本地 NuGet 源
publish: pack
	@echo "==> publishing to $(NUGET_FEED)"
	@for pkg in $(ARTIFACTS)/*.nupkg; do \
		dotnet nuget push "$$pkg" --source "$(NUGET_FEED)" --skip-duplicate; \
	done

## bench: 运行 BenchmarkDotNet (short job, 含正确性校验)
bench:
	cd benchmarks/NetcdfReadBenchmark && dotnet run -c Release -- --correctness
	cd benchmarks/NetcdfReadBenchmark && dotnet run -c Release -- --job short

## clean: 清理构建与打包产物
clean:
	dotnet clean $(SOLUTION) -v q -nologo
	@rm -rf $(ARTIFACTS) 2>/dev/null || powershell -Command "Remove-Item -Recurse -Force $(ARTIFACTS) -ErrorAction SilentlyContinue"
	@powershell -Command "Get-ChildItem -Recurse -Directory | Where-Object { $$_.Name -eq 'bin' -or $$_.Name -eq 'obj' } | Remove-Item -Recurse -Force"
	@echo "==> cleaned"
