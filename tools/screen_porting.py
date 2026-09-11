#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""Screening script for netcdf-java -> NetcdfSharp porting.

For every not-yet-ported Java file under a module's src/main/java, classify it into one of:

  PORTABLE  - small (<=250 lines) and in an in-scope package: the Tangible Free Edition
              converter can translate it in one piece; needs pass1 mechanical fixes only.
  MANUAL    - in scope but too large for the free converter (truncates at ~250 lines):
              tool output + hand completion against the Java original (the usual workflow).
  EXCLUDE   - out-of-scope package (write/http/cache/rc/thredds/ft/ft2/ncml/stream/...):
              Direction A - do NOT port; add <Compile Remove> or a stub instead.

Rename-aware: a Java type is considered "already ported" if a C# file exists under the
port target whose *renamed* basename matches (see RENAMES). Run from the repo root.

Usage:  python tools/screen_porting.py [--module cdm|grib|bufr|udunits]
Output: three sorted lists printed to stdout (and a per-module summary).
"""

import os
import sys

ROOT = r"C:\Users\fyq\Documents\dev\dotnet\NetcdfSharp"
JAVA_ROOT = os.path.join(ROOT, "netcdf-java-maint-5.x")

# 250-line truncation limit of the Tangible Free Edition converter.
TOOL_LIMIT = 250

# Modules: java source root -> C# port root.
MODULES = {
    "cdm": (
        os.path.join(JAVA_ROOT, "cdm", "core", "src", "main", "java"),
        os.path.join(ROOT, "NetcdfSharp.Core"),
    ),
    "grib": (
        os.path.join(JAVA_ROOT, "grib", "src", "main", "java"),
        os.path.join(ROOT, "NetcdfSharp.Grib"),
    ),
    "bufr": (
        os.path.join(JAVA_ROOT, "bufr", "src", "main", "java"),
        os.path.join(ROOT, "NetcdfSharp.Bufr"),
    ),
    "udunits": (
        os.path.join(JAVA_ROOT, "udunits", "src", "main", "java"),
        os.path.join(ROOT, "NetcdfSharp.Udunits"),
    ),
}

# Java basename -> ported C# basename for every rename so far (see git log).
# Keep this list in sync when new renames land.
RENAMES = {
    "Array": "NcArray",                # ucar.ma2.Array (collides System.Array) 57c4b86
    "Range": "NcRange",                # ucar.ma2.Range 97c080c
    "Index": "NcIndex",                # ucar.ma2.Index b95dc3e
    "Attribute": "NcAttribute",        # ucar.nc2.Attribute 3d8bfa7
    "System": "JSystem",               # java.lang.System ae0f027
    "DateTime": "JDateTime",           # org.joda.time.DateTime ad64288
    "TimeZone": "JTimeZone",           # java.util.TimeZone 7d2456e
    "Type": "GribType",                # GribCollectionImmutable.Type 784b4c9
    "Version": "GribVersion",          # WmoCodeFlagTables.Version 0b627d8
    "Unit": "ShimUnit",                # Core ucar.units shims 0a34de3
    "UnitName": "ShimUnitName",
    "UnitDimension": "ShimUnitDimension",
    "ConversionException": "ShimConversionException",
    "UnknownUnit": "ShimUnknownUnit",
    "DerivedUnit": "ShimDerivedUnit",
    "ScaledUnit": "ShimScaledUnit",
    "Factor": "ShimFactor",
    "Converter": "ShimConverter",
    "UnitFormat": "ShimUnitFormat",
    "UnitFormatManager": "ShimUnitFormatManager",
    "Exception": "JDOMException",      # org.jdom2.JDOMException (J-prefix collision rule)
}

# Out-of-scope packages (Direction A: exclude, do not port). Keys are path fragments
# relative to the java source root; a Java file whose relative path contains one of
# these fragments is classified EXCLUDE.
EXCLUDE_FRAGMENTS = [
    # nc2 subsystems (feature types / ncml / stream / write / enhancement layer)
    "nc2\\ft",
    "nc2\\ft2",
    "nc2\\ncml",
    "nc2\\stream",
    "nc2\\write",
    "nc2\\dt",
    "nc2\\rc",
    "nc2\\dataset",
    "nc2\\internal\\ncml",
    "nc2\\internal\\dataset",   # enhancement layer (conv/Coards/Conventions) is not ported
    "nc2\\util\\rc",            # RC config subsystem
    "nc2\\util\\reflect",       # java.lang.reflect Proxy codegen (not applicable to C#)
    "nc2\\util\\net",           # URLStreamHandlerFactory etc.
    # thredds (catalog / featurecollection / inventory) - not ported
    "thredds",
    # remote / cache / http / vertical / gml
    "unidata\\io\\http",
    "unidata\\io\\cache",
    "unidata\\geoloc\\vertical",
    "unidata\\geoloc\\ogc",          # GML / OGC coord-ref-sys
    "unidata\\geoloc\\projection\\proj4",  # proj4 projection classes
    "unidata\\geoloc\\projection\\sat",    # satellite projection classes
    "nc2\\util\\cache",
    "nc2\\util\\xml",
    # edal / resc 360-day calendars copied into netcdf-java (not ported)
    "uk\\ac\\rdg",
    # old iosp variants replaced by the *New implementations
    "nc2\\iosp\\netcdf3",
    "nc2\\iosp\\hdf4",
    "nc2\\iosp\\hdf5",
    "nc2\\iosp\\netcdf4",
    # opendap / dap4 stubs
    "opendap",
]

# Module-specific excludes (Direction A): these are represented by stubs in the port,
# so the Java originals must NOT be translated.
EXCLUDE_MODULE = {
    "cdm": [
        "nc2\\NetcdfFileWriter",       # writing is out of scope
        "nc2\\FileWriter2",
        "nc2\\NCdumpW",                # command-line dump tool
        "unidata\\geoloc\\Bearing",    # not needed by the read path
        # removed as dead code in commit ee1db7c (refactor: remove dead deprecated
        # members and classes) - the Java originals must NOT be ported
        "ma2\\StructureDataScalar",
        "nc2\\VariableIF",
        "nc2\\VariableSimpleImpl",
        "nc2\\VariableSimpleAdapter",
        "nc2\\util\\NamedAnything",
        "nc2\\util\\NamedObject",
        "nc2\\util\\IOIterator",
        "nc2\\util\\UnsynchronizedBufferedWriter",
    ],
    "grib": [
        "grib\\collection\\Grib1CollectionWriter",   # index/writer -> grib/stubs/GribCollectionWriters.cs
        "grib\\collection\\Grib2CollectionWriter",
        "grib\\collection\\GribCollectionWriter",
        "grib\\collection\\PartitionCollection",      # partition -> grib/stubs/PartitionCollection.cs
        "grib\\collection\\GribPartitionBuilder",
        "grib\\collection\\GribBestDatasetBuilder",
        "grib\\collection\\Grib1Partition",
        "grib\\collection\\Grib2Partition",
        "grib\\collection\\Grib1CollectionBuilderFromIndex",  # index builders -> GribIndexStubs.cs
        "grib\\collection\\Grib2CollectionBuilderFromIndex",
        "grib\\collection\\Grib1PartitionBuilderFromIndex",
        "grib\\collection\\Grib2PartitionBuilderFromIndex",
        "grib\\collection\\GribCollectionBuilderFromIndex",
        "grib\\GribIndex",                            # index cache -> grib/stubs/GribIndexStubs.cs
        "grib\\GribIndexCache",
        "grib\\grib1\\Grib1Index",
        "grib\\grib2\\Grib2Index",
    ],
    "bufr": [
        "ft\\point\\bufr\\BufrCdmIndexProto",         # protobuf, not generated -> excluded
    ],
}


def count_lines(path):
    try:
        with open(path, "r", encoding="utf-8", errors="replace") as f:
            return sum(1 for _ in f)
    except Exception:
        return -1


def ported_basenames(cs_root):
    """Set of C# file basenames (both as-is and via the reverse rename map)."""
    names = set()
    for dirpath, _, files in os.walk(cs_root):
        for f in files:
            if f.endswith(".cs"):
                names.add(f[:-3])
    return names


def reverse_renames():
    rev = {v: k for k, v in RENAMES.items()}
    rev.update(RENAMES)  # both directions
    return rev


def classify(rel_path, lines, ported, module):
    rel = rel_path.replace("/", "\\")
    for frag in EXCLUDE_FRAGMENTS:
        if frag in rel:
            return "EXCLUDE"
    for frag in EXCLUDE_MODULE.get(module, []):
        if frag in rel:
            return "EXCLUDE"
    base = os.path.basename(rel_path)[:-5]  # strip .java
    if base in ported or RENAMES.get(base) in ported:
        return "PORTED"
    if lines < 0:
        return "UNKNOWN"
    return "PORTABLE" if lines <= TOOL_LIMIT else "MANUAL"


def main():
    module = "cdm"
    if len(sys.argv) > 1 and sys.argv[1] == "--module":
        module = sys.argv[2]
    if module not in MODULES:
        print("unknown module; choose from:", ", ".join(MODULES))
        sys.exit(1)
    java_root, cs_root = MODULES[module]

    ported = ported_basenames(cs_root)
    rev = reverse_renames()

    results = {"PORTABLE": [], "MANUAL": [], "EXCLUDE": [], "PORTED": [], "UNKNOWN": []}
    for dirpath, _, files in os.walk(java_root):
        for f in files:
            if not f.endswith(".java"):
                continue
            abs_path = os.path.join(dirpath, f)
            rel_path = os.path.relpath(abs_path, java_root)
            lines = count_lines(abs_path)
            cat = classify(rel_path, lines, ported, module)
            results[cat].append((lines, rel_path))

    for cat in ("PORTABLE", "MANUAL", "EXCLUDE"):
        lst = sorted(results[cat], key=lambda t: (-t[0], t[1]))
        print(f"\n===== {cat} ({len(lst)}) =====")
        for lines, rel in lst:
            print(f"{lines:5d}  {rel}")

    print("\n===== SUMMARY =====")
    print(f"module: {module}")
    print(f"already ported (basename or renamed match): {len(results['PORTED'])}")
    print(f"PORTABLE (<= {TOOL_LIMIT} lines, tool-safe): {len(results['PORTABLE'])}")
    print(f"MANUAL (> {TOOL_LIMIT} lines, needs hand completion): {len(results['MANUAL'])}")
    print(f"EXCLUDE (out of scope, Direction A): {len(results['EXCLUDE'])}")
    print(f"UNKNOWN: {len(results['UNKNOWN'])}")
    total = sum(len(v) for v in results.values())
    print(f"total java files: {total}")


if __name__ == "__main__":
    main()
