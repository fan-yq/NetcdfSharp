# NetcdfSharp

将 [netcdf-java](https://github.com/Unidata/netcdf-java)（maint-5.x）机器翻译为
C# / .NET 10 的读取实现，目标是支持 **netCDF-3 / netCDF-4(HDF5) / HDF4 / HDF5 /
GRIB1 / GRIB2 / BUFR** 的**本地文件读取**；**netCDF-3 写入**亦已实现。

> **非官方移植**：本项目与 UCAR/Unidata 无关联、未获其背书，是对 netcdf-java
> 的机器辅助翻译（衍生作品），详见[许可与致谢](#许可与致谢)。

[English](README.md)

## 当前状态

| 项目 | 编译 | 说明 |
|---|---|---|
| `NetcdfSharp.Core` | ✅ 0 警告 | ma2 数组、nc2 模型/iosp、netCDF-3/4(HDF5)、HDF4/HDF5 读路径、`java.*`/guava/slf4j/joda shim |
| `NetcdfSharp.Udunits` | ✅ 0 警告 | `ucar.units` 完整移植 |
| `NetcdfSharp.Grib` | ✅ 0 警告 | GRIB1/2 解码；WMO/ECMWF/NCEP 等表以 build 期 zip 内嵌 |
| `NetcdfSharp.Bufr` | ✅ 0 警告 | BUFR 读取；表以 build 期 zip 内嵌 |
| `NetcdfSharp.Tests` | ✅ 340 通过\* | xUnit |

\* 缺少样本数据（`mock-datasets/` 生成文件或 netcdf-java 样本）的测试会自动跳过。

### 支持 / 不支持

- 支持：本地 netCDF-3、netCDF-4(HDF5)、HDF4、普通 HDF5、GRIB1、GRIB2、BUFR。
- netCDF-3 写入（`NetcdfFormatWriter`）：定长/无限维、各数值类型、CHAR、标量、
  全局/变量属性。
- 不支持：远程访问（DAP/gCDM/stream）、netCDF-4 写入、GRIB/BUFR 索引缓存
  （`.ncx`/`.gbx9`，每次打开全量扫描）。
- 未移植的数据编码：GRIB2 的 JPEG2000（模板 40）与 PNG（模板 41）打包，遇到则跳过。

## 免责声明

- **不保证正确性**：本项目为机器辅助移植，边界情况下结果可能与 netcdf-java
  不一致，请在自测通过后再使用。
- **不保证持续维护**：项目可能不再积极维护。
- **欢迎提 Issue**：问题与反馈请通过 GitHub Issues 提出。
- **测试覆盖**：仅 `NetcdfSharp.Core` 使用真实 netCDF 数据进行读取测试；
  GRIB/BUFR 测试依赖生成的 mock 文件或样本文件，文件缺失时会自动跳过。

## 安装

```bash
dotnet add package NetcdfSharp.Core     # netCDF/HDF 核心
dotnet add package NetcdfSharp.Grib     # GRIB1/2（依赖 Core）
dotnet add package NetcdfSharp.Bufr     # BUFR（依赖 Core）
dotnet add package NetcdfSharp.Udunits  # 单位库
```

需要 .NET 10 SDK / 运行时。

## 快速开始

```csharp
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;

using NetcdfFile nc = NetcdfFiles.open("data.nc");

// 元数据
foreach (Variable v in nc.getVariables())
    Console.WriteLine($"{v.getName()} {v.DataType} [{string.Join(",", v.Shape)}]");

// 读变量
Variable temp = nc.findVariable("temperature");
NcArray data = temp.read();
Console.WriteLine(temp.findAttributeString("units", "?"));
Console.WriteLine($"first = {data.getDouble(0)}");

// 子集（hyperslab）
NcArray slab = temp.read(new int[] { 0, 0, 0 }, new int[] { 1, 10, 10 });

// GRIB2 同样经 IOSP 自动识别打开
using NetcdfFile grib = NetcdfFiles.open("forecast.grib2");
```

## 架构与翻译方法

把它当作「穿 C# 外衣的 Java 代码库」：**1 个 Java 文件 = 1 个 C# 文件**，
保留 Java 命名（`getName`、`field`、Java 类名）与 `ucar.*` 命名空间（统一加
`NetcdfSharp.` 前缀）；`byte` 为无符号，`Number`→`object` + `Convert.ToXxx()`，
Java 标准库由 shim 层提供。

```
NetcdfSharp/
├── NetcdfSharp.Core/    读取核心 + java.*/org.slf4j/org.joda/guava/commons-math shim
├── NetcdfSharp.Udunits/ ucar.units
├── NetcdfSharp.Grib/    GRIB1/2 + 内嵌表 zip（BUFR 式打包）
├── NetcdfSharp.Bufr/    BUFR  + 内嵌表 zip
├── NetcdfSharp.Tests/   xUnit 测试
├── docs/                设计与使用文档（中文）
└── NetcdfSharp.slnx
```

`NetcdfSharp.Core/GlobalUsings.cs` 是 shim 接线点；`ucar.ma2.Array` 重命名为
**`NcArray`** 以避免与 `System.Array` 冲突。

## 构建与测试

```bash
dotnet build ./NetcdfSharp.slnx -c Release -v q -nologo
dotnet test  ./NetcdfSharp.Tests/NetcdfSharp.Tests.csproj --nologo -v q
```

中文 Windows 控制台先转 UTF-8 输出，避免日志乱码：

```powershell
$env:DOTNET_CLI_UI_LANGUAGE="en"; [Console]::OutputEncoding=[System.Text.Encoding]::UTF8
```

GRIB/BUFR 测试表 vendored 在各自项目的 `resources/` 下，build 时由
`PackGribTables` / `PackBufrTables` target 调 `tools/bufr-pack-resources.py`
打包成 zip（GRIB ~1.3 MB，BUFR ~3.4 MB），因此构建需要 Python。

可选：生成更多 mock 数据用于读取测试：

```bash
python generate_mock_data.py
```

## 文档

面向使用者的中文文档在 [`docs/`](docs/)：

| 文档 | 内容 |
|---|---|
| `docs/使用指南.md` | 面向使用者：引用方式、读变量/子集/属性、netCDF-3 写入、各格式支持与限制 |
| `docs/功能范围.md` | 已实现 vs 未实现/范围外的完整清单 |

内部设计与移植笔记保留在 `docs/archive/`，从源码导出中排除（`export-ignore`）。
Benchmark 方法与结果见 [`benchmarks/README.md`](benchmarks/README.md)。

## 许可与致谢

- 本项目采用 **BSD 3-Clause**，见 [LICENSE](LICENSE)。
- 本项目是 [netcdf-java](https://github.com/Unidata/netcdf-java)
  （BSD 3-Clause，UCAR/Unidata）的衍生作品，保留上游版权声明。
- 再分发的第三方数据表（WMO、ECMWF ecCodes、NCEP、NOAA 等）见
  [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md)。
- 「netCDF」「Unidata」是 UCAR 的商标；本项目为非官方移植。
