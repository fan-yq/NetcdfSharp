# Benchmark 运行说明

> 用 BenchmarkDotNet 对比 **NetcdfSharp**（本库）与 **SDSLite**、**SwatRecords.NetCDF** 的 netCDF-3 读取性能，并做逐元素正确性校验。
> 构建/测试见根目录 [`README.md`](../README.md)。

> ⚠️ **本基准依赖作者本地的数据文件，仓库不提供该文件。** `Program.cs` 的
> `NetcdfReadBenchmarksBase.Setup()` 中硬编码了 `C:\Users\fyq\Documents\data\archive\monthlydata\china-monthly-2007-2023.nc`
> （714 MB）；其他人要在本机运行，必须先把该路径改成自己的 netCDF-3 文件。
> 因此 §4 / §5 的实测结果**仅供作者本机参考，别人无法直接复现**——结果已附在下方。

---

## 1. 概述

| 项 | 说明 |
|---|---|
| 项目 | `benchmarks/NetcdfReadBenchmark`（独立 exe，引用 NetcdfSharp.Core + SDSLite 3.0.1 + SwatRecords.NetCDF 1.0.2） |
| 框架 | BenchmarkDotNet 0.14（`[MemoryDiagnoser]`，`[ShortRunJob]` 可选） |
| 对比库 | **NetcdfSharp**（本仓库 Core 读路径）、**SDSLite**（NuGet `SDSLite` 3.0.1）、**SwatRecords.NetCDF**（NuGet `SwatRecords.NetCDF` 1.0.2） |
| 目标 | 本地文件 netCDF-3 读取（与本库"本地文件读取"目标一致） |

### 1.1 数据集

- 文件：`C:\Users\fyq\Documents\data\archive\monthlydata\china-monthly-2007-2023.nc`（714 MB，**作者本地文件，不随仓库提供**）
- 格式：`NETCDF3_64BIT_OFFSET`（CDF-2）
- 变量：`t, u, v, w` 均为 `int16 [107,2,6,225,309]`（约 178 MB/变量）
- 路径在 `Program.cs` 的 `NetcdfReadBenchmarksBase.Setup()` 中硬编码，运行前需改成你自己的文件（结构与变量名可不同，相应改基准方法即可）。

> 三库都支持 CDF-2（64-bit offset）；SDSLite/Swat 对 netCDF-4（HDF5）的支持不在此基准范围。

---

## 2. 运行方法

从 `benchmarks/NetcdfReadBenchmark/` 目录执行：

```powershell
# 完整基准（较长时间）
dotnet run -c Release

# 快速（CI 友好，3 次迭代）
dotnet run -c Release -- --job short

# 只跑某类基准（如全量读）
dotnet run -c Release -- --filter *ReadAll*
dotnet run -c Release -- --filter *_Open*        # 只跑 Open

# 避免新进程（杀毒软件/进程启动开销干扰时）
dotnet run -c Release -- --job short --inprocess

# 正确性校验（三库逐元素比对）
dotnet run -c Release --correctness
```

> **注意**：`--inprocess` 与 `--filter` 组合会返回 0 个基准（BenchmarkDotNet 已知行为），两者分开使用。

---

## 3. 基准组

每个操作都**包含一次 `open`**（公平对比各库的整体读路径）；`sink` 字段防止 JIT 消除死读。

| 组 | 方法 | 测什么 |
|---|---|---|
| `Netcdf3OpenBenchmarks` | `*_Open` / `*_Metadata` | 打开文件；定位变量 + 读 shape |
| `Netcdf3SliceBenchmarks` | `*_Read2D` / `*_Read3D` | 固定切片：`t[0,0,0,:,:]`（[225,309]，139 KB）、`t[0,0,0:2,:,:]`（278 KB） |
| `Netcdf3ReadAllBenchmarks` | `*_ReadAll` | 整变量读取（178 MB） |

每组的三个方法分别对应 NetcdfSharp / SDSLite / SwatRecords。

---

## 4. 结果（作者本机 short job 实测，2026-08-14；因上述硬编码文件，他人无法直接复现）

| 操作 | NetcdfSharp | SDSLite | SwatRecords.NetCDF |
|---|---|---|---|
| **Open** | 107.9 ms / 119 MB | 1.9 ms / 77 KB | **38 µs** / 26 KB |
| **Metadata** | 109.4 ms | 1.9 ms | **37 µs** |
| **Read 2D**（139 KB） | 109.3 ms | 2.2 ms / 351 KB | **1.8 ms** / 5.6 MB |
| **Read 3D**（278 KB） | 193.8 ms | 2.7 ms / 897 KB | **5.2 ms** / 16.7 MB |
| **ReadAll**（178 MB） | 536.9 ms / 293 MB | **239.8 ms** / 349 MB | 682.8 ms / 4.5 GB |
| Gen0 GC / op | ~7000 | 86~333 | 1.5~249000 |

> `--job short` 仅 3 次迭代，方差较大（尤其 ReadAll）；正式报告建议 `--job medium` 或默认 job 复核。

### 4.1 结论

1. **NetcdfSharp 的 `NetcdfFiles.open()` 是大文件读取的主要瓶颈**：约 108 ms + 119 MB 分配 + 7000 Gen0 GC/操作。由于每个基准都含 open，2D/3D 切片慢（109~194 ms）的根因是 open 解析 714 MB 文件头并构建完整 `Group/Variable/Dimension` 对象图，而非数据读取本身。
2. **SwatRecords 打开/元数据极快**（37 µs，惰性解析），小切片最快；但 **ReadAll 最慢**（683 ms）且分配 4.5 GB（疑似 4 字节对齐/冗余拷贝）。
3. **SDSLite 中等**：open 1.9 ms，2D/3D 切片为三库最快（2.2/2.7 ms），ReadAll 最快（240 ms，但 349 MB 分配）。
4. **场景建议**：
   - 元数据/小切片（打开多、读少）：**Swat > SDSLite >> NetcdfSharp**
   - 全量大数组读：**SDSLite > NetcdfSharp > Swat**
   - NetcdfSharp 若**复用同一句柄连续读**，切片性能会显著提升（当前每次 open）。

---

## 5. 正确性校验

```powershell
dotnet run -c Release --correctness
```

用 NetcdfSharp 作参考，将 SDSLite、SwatRecords 读同一文件同一变量逐元素比对：

| 校验 | 元素数 | NetcdfSharp vs SDSLite | NetcdfSharp vs SwatRecords |
|---|---|---|---|
| `t[0,0,0,:,:]`（2D 切片） | 69,525 | ✅ 0 mismatch | ✅ 0 mismatch |
| `t[0,0,0:2,:,:]`（3D 切片） | 208,575 | ✅ 0 mismatch | ✅ 0 mismatch |

**结论**：三库读取结果逐元素一致，性能差异纯粹是性能问题，非正确性问题。

> 注：NetcdfSharp 的 `read()` 返回保留 rank（`[1,1,1,225,309]`），SDSLite/Swat 类似；逐元素展开后三者一致。

---

## 6. 文件结构

| 文件 | 内容 |
|---|---|
| `Program.cs` | 入口 `Main` + 公共 `NetcdfReadBenchmarksBase`（setup / sink）+ `--correctness` 开关 |
| `Benchmarks.Open.cs` | `Netcdf3OpenBenchmarks`：Open / Metadata |
| `Benchmarks.Slices.cs` | `Netcdf3SliceBenchmarks`：Read2D / Read3D |
| `Benchmarks.ReadAll.cs` | `Netcdf3ReadAllBenchmarks`：ReadAll |
| `Correctness.cs` | `Correctness.Run()`：三库逐元素正确性校验 |
| `README.md` | 项目内运行说明 |
| `NetcdfReadBenchmark.csproj` | 引用 Core + SDSLite + SwatRecords + BenchmarkDotNet |

`bin/`、`obj/`、`BenchmarkDotNet.Artifacts/` 已被 `.gitignore` 忽略。

---

## 7. 维护要点

- 改数据集路径 → 更新 `NetcdfReadBenchmarksBase.Setup()` 与本文 §1.1。
- 新增基准操作 → 在对应 `Benchmarks.*.cs` 加 `[Benchmark]` 方法，并更新 §3 表。
- 重跑结果 → 更新 §4 表格，标注测量日期与 job 参数。
- 三库 API 签名变化（尤其 SDSLite `GetData`/Swat `Read<T>`）→ 同步修正基准方法。
