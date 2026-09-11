# NetcdfReadBenchmark

BenchmarkDotNet 对比 **NetcdfSharp** vs **SDSLite** vs **SwatRecords.NetCDF** 的 netCDF-3 读取性能。

## 数据集

- 文件：`C:\Users\fyq\Documents\data\archive\monthlydata\china-monthly-2007-2023.nc`（714 MB，NETCDF3_64BIT_OFFSET）
- 变量：`t, u, v, w` int16 `[107,2,6,225,309]`（约 178 MB/变量）
- 路径在 `Program.cs` 的 `NetcdfReadBenchmarksBase.Setup` 中配置，可按需修改。

## 运行

```powershell
# 完整基准（较长）
dotnet run -c Release

# 快速（CI 友好，3 次迭代）
dotnet run -c Release -- --job short

# 只跑某类基准（如全量读）
dotnet run -c Release -- --filter *ReadAll*

# 避免新进程（杀毒软件/启动开销干扰时）
dotnet run -c Release -- --job short --inprocess
```

## 基准组

| 组 | 方法 | 测什么 |
|---|---|---|
| `Netcdf3OpenBenchmarks` | `*_Open`, `*_Metadata` | 打开文件、定位变量+读 shape |
| `Netcdf3SliceBenchmarks` | `*_Read2D`, `*_Read3D` | 固定 2D/3D 切片读取（含 open） |
| `Netcdf3ReadAllBenchmarks` | `*_ReadAll` | 整变量读取（178 MB） |

每个操作都包含一次 `open`（公平对比库的整体读路径）。

## 正确性校验

三个库读同一文件、同一切片，逐元素比对：

```powershell
dotnet run -c Release --correctness
```

预期输出 `TOTAL MISMATCHES: 0`（三库读取结果一致）。

## 文件结构

| 文件 | 内容 |
|---|---|
| `Program.cs` | 入口 + 公共 `NetcdfReadBenchmarksBase`（setup / sink） |
| `Benchmarks.Open.cs` | Open / Metadata |
| `Benchmarks.Slices.cs` | Read2D / Read3D |
| `Benchmarks.ReadAll.cs` | ReadAll |
| `Correctness.cs` | 三库逐元素正确性校验 |
