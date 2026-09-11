# Third-Party Notices

NetcdfSharp is a machine-assisted C# translation (derivative work) of
[netcdf-java](https://github.com/Unidata/netcdf-java). It also redistributes
several third-party parameter/code tables. This file lists those components and
their licenses. The full text of the primary license is in [LICENSE](LICENSE).

## netcdf-java (source of the port)

- License: BSD 3-Clause
- Copyright (c) 1998-2025, University Corporation for Atmospheric Research/Unidata
- https://github.com/Unidata/netcdf-java

The C# port retains the original copyright headers in the source files and the
BSD 3-Clause terms in [LICENSE](LICENSE).

## WMO GRIB2 tables

- Files: `NetcdfSharp.Grib/resources/gribTables/grib2/wmo/*`
  (`GRIB2_37_Template.xml`, `GRIB2_37_CodeFlag.xml`)
- License: MIT — see `NetcdfSharp.Grib/resources/gribTables/grib2/wmo/LICENSE.md`
- Source: WMO Manual on Codes (WMO-No. 306), as redistributed by netcdf-java.

## ECMWF ecCodes tables

- Files: `NetcdfSharp.Grib/resources/gribTables/grib2/ecmwf/*`, `grib1/ecmwf*/*`,
  and related local tables.
- License: Apache License 2.0 (ECMWF ecCodes)
- https://github.com/ecmwf/eccodes

These tables are derived from ECMWF ecCodes. If you redistribute this project in
a jurisdiction where the table data terms differ, verify the applicable terms.

## BUFR tables

- Files: `NetcdfSharp.Bufr/resources/bufrTables/*`
- Source: WMO Manual on Codes (WMO-No. 306) and NOAA/other originating centres,
  as redistributed by netcdf-java.

## Other GRIB1/GRIB2 local tables

- Files: `NetcdfSharp.Grib/resources/gribTables/grib1/*` and
  `gribTables/grib2/{ncep,mrms,noaa_gsd,local}/*`
- Source: the respective originating centres (NCEP, AFWA, FNMOC, JMA, DSS,
  CFSR, MRMS, FSL/HRRR, ...), as redistributed by netcdf-java. See the
  `readme*` files next to each table set.

Modification notice: the vendored FSL-HRRR file
`gribTables/grib2/noaa_gsd/Fsl-hrrr4.csv` had an upstream typo (a missing comma
on line 218) corrected by this project; tables were otherwise reformatted and
repackaged only.

## NuGet dependencies

See the `PackageReference` entries in the `*.csproj` files. The principal
runtime/analyzers dependencies are Microsoft.Extensions.* (MIT) and, for tests,
xUnit.net (Apache-2.0).

## Full license texts

The verbatim license texts are bundled in the [`licenses/`](licenses/) directory
and are included in every NuGet package:

- `licenses/BSD-3-Clause-netcdf-java.txt` — netcdf-java (BSD 3-Clause)
- `licenses/MIT-WMO-GRIB2-tables.md` — WMO GRIB2 tables (MIT)
- `licenses/Apache-2.0.txt` — ECMWF ecCodes tables (Apache License 2.0)

If you redistribute this project (source or binaries), keep these files together
with the distribution.

## Trademarks

"netCDF" and "Unidata" are trademarks of the University Corporation for
Atmospheric Research. This project is an unofficial port and is not affiliated
with or endorsed by UCAR/Unidata.
