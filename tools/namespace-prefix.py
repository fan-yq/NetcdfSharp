#!/usr/bin/env python3
"""Adds the `NetcdfSharp.` prefix to every namespace in the compiled projects.

Rule: any dotted identifier starting with a known top-level namespace root
(ucar, java, org, com, thredds, unidata) is prefixed with `NetcdfSharp.`.
Covers:
  - `namespace ucar.nc2.X` declarations
  - `using ucar.nc2;` / `using X = ucar.nc2.Y;` / `global using ucar.nc2;`
  - fully-qualified references in code (ucar.ma2.Array.factory(...), ...)
  - string literals used for reflection lookup (registerIOProvider("..."))
  - doc comments (harmless)

Two patterns avoid false positives on Java-translated identifiers that share a
root name (e.g. the field `org` = "original" in StructureDataProxy, used as
`org.getScalarInt(...)`):
  - namespace/using directive lines:   require >= 1 dotted segment
  - all other code:                    require >= 2 dotted segments
(Real namespace references in code always have >= 2 segments; 1-segment roots
only occur inside using/namespace lines.)

Non-compiled material (CSharpTranslateResult/, netcdf-java-maint-5.x/) is not touched.

Run from the repo root:
  python tools/namespace-prefix.py
"""
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
PROJECTS = [
    "NetcdfSharp.Core",
    "NetcdfSharp.Udunits",
    "NetcdfSharp.Grib",
    "NetcdfSharp.Bufr",
    "NetcdfSharp.Tests",
    "benchmarks",
]

ROOTS = r"ucar|java|org|com|thredds|unidata"
SEG = r"\.[A-Za-z_@][A-Za-z0-9_@]*"
# >= 1 dotted segment (namespace declarations and using directives)
PATTERN1 = re.compile(r"(?<![A-Za-z0-9_@])(?:%s)(?:%s)+" % (ROOTS, SEG))
# >= 2 dotted segments (code references)
PATTERN2 = re.compile(r"(?<![A-Za-z0-9_@])(?:%s)(?:%s){2,}" % (ROOTS, SEG))
PREFIX = "NetcdfSharp."

USING_DIR = re.compile(r"^(?:global\s+)?using\s+[A-Za-z_]")
NAMESPACE = re.compile(r"^namespace\s+")


def decode_bytes(data: bytes):
    """Return (text, encoding_name_for_write)."""
    if data.startswith(b"\xef\xbb\xbf"):
        return data.decode("utf-8-sig"), "utf-8-sig"
    if data.startswith(b"\xff\xfe"):
        return data.decode("utf-16-le"), "utf-16"
    if data.startswith(b"\xfe\xff"):
        return data.decode("utf-16-be"), "utf-16-be"
    return data.decode("utf-8"), "utf-8"


def encode_text(text: str, encoding: str) -> bytes:
    if encoding == "utf-16-be":
        return b"\xfe\xff" + text.encode("utf-16-be")
    if encoding == "utf-16":
        return text.encode("utf-16")  # 'utf-16' emits a BOM
    if encoding == "utf-8-sig":
        return text.encode("utf-8-sig")  # emits a BOM
    return text.encode("utf-8")


def transform_line(line: str) -> str:
    stripped = line.lstrip()
    if USING_DIR.match(stripped) or NAMESPACE.match(stripped):
        return PATTERN1.sub(lambda m: PREFIX + m.group(0), line)
    return PATTERN2.sub(lambda m: PREFIX + m.group(0), line)


def transform_file(path: str) -> bool:
    with open(path, "rb") as fh:
        data = fh.read()
    text, encoding = decode_bytes(data)
    # split on \n, keep the trailing \r for CRLF files
    lines = text.split("\n")
    out = []
    changed = False
    for raw in lines:
        had_cr = raw.endswith("\r")
        body = raw[:-1] if had_cr else raw
        new_body = transform_line(body)
        if new_body != body:
            changed = True
        out.append(new_body + ("\r" if had_cr else ""))
    if not changed:
        return False
    new_text = "\n".join(out)
    with open(path, "wb") as fh:
        fh.write(encode_text(new_text, encoding))
    return True


def main() -> int:
    total = changed = 0
    for proj in PROJECTS:
        proj_path = os.path.join(ROOT, proj)
        if not os.path.isdir(proj_path):
            continue
        for dirpath, dirnames, filenames in os.walk(proj_path):
            dirnames[:] = [d for d in dirnames if d not in ("bin", "obj")]
            for name in filenames:
                if not name.endswith(".cs"):
                    continue
                full = os.path.join(dirpath, name)
                total += 1
                if transform_file(full):
                    changed += 1
    print(f"Processed {total} files, changed {changed}.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
