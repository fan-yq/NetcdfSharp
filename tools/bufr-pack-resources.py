#!/usr/bin/env python3
"""Packs the BUFR table files under resources/bufrTables into a single zip.

Run at build time (NetcdfSharp.Bufr.csproj -> PackBufrTables target) to keep the
embedded payload small: ~55 MB of text tables compress to a few MB. At runtime
NetcdfSharp.ucar.nc2.iosp.bufr.tables.BufrTables.openStream() reads entries from
the zip lazily and decompresses on demand.

Entries are stored with their path relative to the source dir, using forward
slashes, so "wmo/BUFR_37_0_0_TableB_en.xml" etc. Deterministic timestamps so
the zip is byte-identical across rebuilds (incremental build friendly).

Usage:
  python tools/bufr-pack-resources.py --src NetcdfSharp.Bufr/resources/bufrTables \
      --out obj/Debug/net10.0/bufrTables.zip
"""
import argparse
import os
import sys
import zipfile

# Fixed timestamp (zip requires >= 1980). Deterministic output.
FIXED_TIME = (1980, 1, 1, 0, 0, 0)


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--src", required=True, help="source directory with the raw table files")
    parser.add_argument("--out", required=True, help="output .zip path")
    args = parser.parse_args()

    src = args.src
    if not os.path.isdir(src):
        print(f"error: source dir not found: {src}", file=sys.stderr)
        return 1

    out_dir = os.path.dirname(os.path.abspath(args.out))
    os.makedirs(out_dir, exist_ok=True)

    entries = []
    for dirpath, _dirnames, filenames in os.walk(src):
        for name in sorted(filenames):
            full = os.path.join(dirpath, name)
            rel = os.path.relpath(full, src).replace("\\", "/")
            entries.append((full, rel))
    entries.sort(key=lambda p: p[1])

    raw = sum(os.path.getsize(f) for f, _ in entries)
    with zipfile.ZipFile(args.out, "w", zipfile.ZIP_DEFLATED, compresslevel=9) as zf:
        for full, rel in entries:
            info = zipfile.ZipInfo(rel, date_time=FIXED_TIME)
            info.compress_type = zipfile.ZIP_DEFLATED
            info.external_attr = 0o644 << 16
            with open(full, "rb") as fh:
                zf.writestr(info, fh.read())

    packed = os.path.getsize(args.out)
    print(f"packed {len(entries)} files, {raw} -> {packed} bytes ({raw - packed} saved, "
          f"{packed * 100 // raw if raw else 0}% of original) -> {args.out}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
