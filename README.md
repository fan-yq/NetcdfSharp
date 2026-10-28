# NetcdfSharp

A C# / .NET 10 port of the [netcdf-java](https://github.com/Unidata/netcdf-java)
reading stack, for **local-file reading** of scientific data formats:
netCDF-3, netCDF-4 (HDF5), HDF4, HDF5, GRIB1, GRIB2 and BUFR. netCDF-3 writing is
also implemented.

> **Unofficial port.** This project is not affiliated with, or endorsed by,
> UCAR/Unidata. It is a machine-assisted translation (derivative work) of
> netcdf-java (maint-5.x); see [NOTICE](#license-and-acknowledgements).

[简体中文](README.zh-CN.md)

## Status

| Project | Build | Notes |
|---|---|---|
| `NetcdfSharp.Core` | 0 warnings | ma2 arrays, the nc2 model + IOSP layer, netCDF-3/4 (HDF5), HDF4/HDF5 read paths, `java.*`/guava/slf4j/joda shims |
| `NetcdfSharp.Udunits` | 0 warnings | full `ucar.units` port |
| `NetcdfSharp.Grib` | 0 warnings | GRIB1/GRIB2 decoding; WMO/ECMWF/NCEP/... tables embedded as a build-time zip |
| `NetcdfSharp.Bufr` | 0 warnings | BUFR reading; WMO/tables embedded as a build-time zip |
| `NetcdfSharp.Tests` | 344 passing\* | xUnit |

\* The suite skips tests whose sample data is absent (generated mock files under
`mock-datasets/`, or netcdf-java sample files). See [Build & test](#build--test).

### Supported / not supported

- Supported: local netCDF-3, netCDF-4 (HDF5), HDF4, plain HDF5, GRIB1, GRIB2, BUFR.
- netCDF-3 writing (`NetcdfFormatWriter`): fixed and unlimited dimensions, all
  numeric types, CHAR, scalar, global/variable attributes.
- Not supported: remote access (DAP, gCDM, HTTP streaming), netCDF-4 writing,
  GRIB/BUFR index caches (`.ncx`/`.gbx9`) — each open scans the file.
- Not ported data encodings: GRIB2 JPEG2000 (template 40) and PNG (template 41)
  packing; those records are skipped.

## Disclaimer

- **No correctness guarantee.** This is a machine-assisted port; results may
  differ from netcdf-java in edge cases. Validate against your own data before
  relying on it.
- **Best-effort maintenance.** The project may not be actively maintained.
- **Issues are welcome.** Bug reports and questions can be filed via GitHub
  Issues.
- **Test coverage.** Only `NetcdfSharp.Core` is tested against real netCDF data.
  The GRIB/BUFR tests use generated mock files or sample files; when those are
  absent the tests skip.

## Install

```bash
dotnet add package NetcdfSharp.Core     # netCDF/HDF core
dotnet add package NetcdfSharp.Grib     # GRIB1/2 (references Core)
dotnet add package NetcdfSharp.Bufr     # BUFR (references Core)
dotnet add package NetcdfSharp.Udunits  # units library
```

Requires the .NET 10 SDK / runtime.

## Quick start

```csharp
using NetcdfSharp.ucar.ma2;
using NetcdfSharp.ucar.nc2;

using NetcdfFile nc = NetcdfFiles.open("data.nc");

// metadata
foreach (Variable v in nc.getVariables())
    Console.WriteLine($"{v.getName()} {v.DataType} [{string.Join(",", v.Shape)}]");

// read a variable
Variable temp = nc.findVariable("temperature");
NcArray data = temp.read();
Console.WriteLine(temp.findAttributeString("units", "?"));
Console.WriteLine($"first = {data.getDouble(0)}");

// hyperslab read
NcArray slab = temp.read(new int[] { 0, 0, 0 }, new int[] { 1, 10, 10 });

// copy to a regular C# rectangular array (same rank and shape, row-major order)
double[,] grid = (double[,])data.copyToCSharpArray();
Console.WriteLine($"grid[0,0] = {grid[0, 0]}");

// wrap a C# rectangular array as an NcArray
NcArray wrapped = NcArray.makeFromCSharpArray(grid);

// GRIB2 works the same way through IOSP discovery:
using NetcdfFile grib = NetcdfFiles.open("forecast.grib2");
```

### Nullability

This is a Java-to-C# port, so nullable reference types are disabled and Java reference
semantics are preserved. Many APIs can return `null` where Java returns `null`; for
example `findVariable("missing")` returns `null` rather than throwing. Check returned
objects before using them (`if (v != null)`, `?.`, or explicit validation).

## Architecture

Treat this as a Java codebase wearing C# syntax. The rule is **1 Java file =
1 C# file**, Java naming is preserved (`getName`, `field`, Java class names),
namespaces carry the `NetcdfSharp.` prefix, `byte` is unsigned, `Number` maps to
`object` + `Convert.ToXxx()`, and Java stdlib/types are provided by a thin shim
layer:

```
NetcdfSharp/
├── NetcdfSharp.Core/     reads + java.* / org.slf4j / org.joda / guava / commons-math shims
├── NetcdfSharp.Udunits/  ucar.units
├── NetcdfSharp.Grib/     GRIB1/2 + embedded table zip (BUFR-style packing)
├── NetcdfSharp.Bufr/     BUFR   + embedded table zip
├── NetcdfSharp.Tests/    xUnit tests
├── docs/                 design/usage notes (Chinese)
└── NetcdfSharp.slnx
```

`NetcdfSharp.Core/GlobalUsings.cs` is the shim wiring point (global aliases for
every `java.*` shim type). `ucar.ma2.Array` is renamed **`NcArray`** to avoid the
`System.Array` collision.

## Build & test

```bash
dotnet build ./NetcdfSharp.slnx -c Release -v q -nologo
dotnet test  ./NetcdfSharp.Tests/NetcdfSharp.Tests.csproj --nologo -v q
```

On a Chinese-locale Windows console, set UTF-8 output first so the log is
readable:

```powershell
$env:DOTNET_CLI_UI_LANGUAGE="en"; [Console]::OutputEncoding=[System.Text.Encoding]::UTF8
```

The GRIB/BUFR test tables are vendored under the project `resources/` folders and
packed into a zip at build time (see the `PackGribTables` / `PackBufrTables`
MSBuild targets and `tools/bufr-pack-resources.py`). The Python script is needed
to build; the packed zip is ~1.3 MB (GRIB) and ~3.4 MB (BUFR).

Optional: generate additional mock datasets for the read tests:

```bash
python generate_mock_data.py
```

## Documentation

User notes (Chinese) live in [`docs/`](docs/):

| Document | Contents |
|---|---|
| `docs/使用指南.md` | usage: referencing, reading variables/subsets/attributes, netCDF-3 writing, per-format support and limitations |
| `docs/功能范围.md` | complete implemented vs. out-of-scope list |

Internal design/porting notes are kept in `docs/archive/` and are excluded from
source exports (`export-ignore`). Benchmark methodology and results are in
[`benchmarks/README.md`](benchmarks/README.md).

## License and acknowledgements

- This project is licensed under **BSD 3-Clause** — see [LICENSE](LICENSE).
- It is a derivative work of [netcdf-java](https://github.com/Unidata/netcdf-java)
  (BSD 3-Clause, UCAR/Unidata); upstream copyright headers are retained.
- Redistributed third-party data tables (WMO, ECMWF ecCodes, NCEP, NOAA, ...) are
  listed in [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).
- "netCDF" and "Unidata" are trademarks of UCAR; this is an unofficial port.
