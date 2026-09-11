# -*- coding: utf-8 -*-
"""Pass 1: mechanical translation fixes on copied .cs files.

  1. sbyte -> byte (Plan A: unify to unsigned byte; keep & 0xFF; sign spots reviewed later)
  2. Java wrapper class static members -> .NET equivalents (helper: JavaLangCompat)
  3. .class literals -> typeof(), wrapper typeof(Xxx) -> typeof(xxx)
  4. Math.toRadians / Math.toDegrees -> arithmetic

Writes back in-place (UTF-8 with BOM to match originals).
Generates tools/sign-review.txt and tools/wrapper-leftover.txt.
Usage: python tools/pass1.py
"""
import os
import re
import sys

ROOT = r"C:\Users\fyq\Documents\dev\dotnet\NetcdfSharp"
DIRS = [
    os.path.join(ROOT, "NetcdfSharp.Core"),
    os.path.join(ROOT, "NetcdfSharp.Udunits"),
    os.path.join(ROOT, "NetcdfSharp.Grib"),
    os.path.join(ROOT, "NetcdfSharp.Bufr"),
]
BOM = b"\xef\xbb\xbf"


def read_text(path):
    with open(path, "rb") as f:
        data = f.read()
    if data.startswith(BOM):
        data = data[len(BOM):]
    return data.decode("utf-8")


def write_text(path, text):
    with open(path, "wb") as f:
        f.write(BOM + text.encode("utf-8"))


# ---- ordered literal replacements (exact, longest-first is naturally safe here) ----
LITERAL_PAIRS = [
    # Double
    ("Double.NaN", "double.NaN"),
    ("Double.MAX_VALUE", "double.MaxValue"),
    ("Double.MIN_NORMAL", "JavaLangCompat.DoubleMinNormal"),
    ("Double.BYTES", "sizeof(double)"),
    ("Double.toString(", "JavaLangCompat.DoubleToString("),
    ("Double.parseDouble(", "JavaLangCompat.ParseDouble("),
    ("Double.compare(", "JavaLangCompat.DoubleCompare("),
    ("Double.longBitsToDouble(", "BitConverter.Int64BitsToDouble("),
    # Float
    ("Float.NaN", "float.NaN"),
    ("Float.MIN_NORMAL", "JavaLangCompat.FloatMinNormal"),
    ("Float.BYTES", "sizeof(float)"),
    ("Float.floatToRawIntBits(", "BitConverter.SingleToInt32Bits("),
    ("Float.floatToIntBits(", "BitConverter.SingleToInt32Bits("),
    ("Float.intBitsToFloat(", "BitConverter.Int32BitsToSingle("),
    ("Float.isFinite(", "float.IsFinite("),
    ("Float.isNaN(", "float.IsNaN("),
    ("Float.compare(", "JavaLangCompat.FloatCompare("),
    # Integer
    ("Integer.BYTES", "sizeof(int)"),
    ("Integer.parseInt(", "JavaLangCompat.ParseInt("),
    ("Integer.toString(", "JavaLangCompat.IntToString("),
    ("Integer.compare(", "JavaLangCompat.IntCompare("),
    ("Integer.toUnsignedLong(", "JavaLangCompat.ToUnsignedLong("),
    ("Integer.reverseBytes(", "JavaLangCompat.ReverseBytes("),
    ("Integer.decode(", "JavaLangCompat.IntDecode("),
    # Long
    ("Long.BYTES", "sizeof(long)"),
    ("Long.parseLong(", "JavaLangCompat.ParseLong("),
    ("Long.compare(", "JavaLangCompat.LongCompare("),
    ("Long.bitCount(", "JavaLangCompat.BitCount("),
    ("Long.decode(", "JavaLangCompat.LongDecode("),
    # Byte
    ("Byte.BYTES", "sizeof(byte)"),
    ("Byte.toUnsignedInt(", "JavaLangCompat.ToUnsignedInt("),
    # Character
    ("Character.isJavaIdentifierPart(", "JavaLangCompat.IsJavaIdentifierPart("),
    # .class literals
    ("boolean.class", "typeof(bool)"),
    ("byte.class", "typeof(byte)"),
    ("short.class", "typeof(short)"),
    ("int.class", "typeof(int)"),
    ("long.class", "typeof(long)"),
    ("float.class", "typeof(float)"),
    ("double.class", "typeof(double)"),
    ("char.class", "typeof(char)"),
    ("String.class", "typeof(string)"),
    ("Object.class", "typeof(object)"),
    ("ByteBuffer.class", "typeof(ByteBuffer)"),
    ("StructureDataIterator.class", "typeof(StructureDataIterator)"),
    ("StructureData.class", "typeof(StructureData)"),
    # typeof(wrapper) -> typeof(primitive)
    ("typeof(Float)", "typeof(float)"),
    ("typeof(Double)", "typeof(double)"),
    ("typeof(Short)", "typeof(short)"),
    ("typeof(Integer)", "typeof(int)"),
    ("typeof(Long)", "typeof(long)"),
    ("typeof(Byte)", "typeof(byte)"),
    ("typeof(Character)", "typeof(char)"),
    ("typeof(Boolean)", "typeof(bool)"),
]


def apply_literals(text):
    for frm, to in LITERAL_PAIRS:
        text = text.replace(frm, to)
    return text


def _extract_parens(text, start_idx):
    """Return (inner, end_idx) for the balanced parens starting at text[start_idx]=='('."""
    depth = 0
    i = start_idx
    while i < len(text):
        if text[i] == '(':
            depth += 1
        elif text[i] == ')':
            depth -= 1
            if depth == 0:
                return text[start_idx + 1:i], i
        i += 1
    return text[start_idx + 1:], start_idx


def fix_powershell_backtick_artifacts(text):
    """Remove backtick artifacts left by the earlier PowerShell run:
    '((`xxx) * ...)' -> '((xxx) * ...)'  (from regex replacement of Math.toRadians/toDegrees)."""
    text = re.sub(r"\(\(`([^()]*)\) \* \(Math\.PI / 180\.0\)\)", r"((\1) * (Math.PI / 180.0))", text)
    text = re.sub(r"\(\(`([^()]*)\) \* \(180\.0 / Math\.PI\)\)", r"((\1) * (180.0 / Math.PI))", text)
    return text


def apply_math(text):
    """Replace Math.toRadians(X) / Math.toDegrees(X) with arithmetic, X may contain nested parens."""
    out = []
    i = 0
    while i < len(text):
        m = re.compile(r"Math\.to(Radians|Degrees)\(").search(text, i)
        if not m:
            out.append(text[i:])
            break
        out.append(text[i:m.start()])
        inner, end = _extract_parens(text, m.end() - 1)
        if m.group(1) == "Radians":
            out.append("(({0}) * (Math.PI / 180.0))".format(inner))
        else:
            out.append("(({0}) * (180.0 / Math.PI))".format(inner))
        i = end + 1
    return "".join(out)


def apply_sbyte_to_byte(text, rel_path, review):
    # capture sign-sensitive lines for manual review
    for line in text.splitlines():
        if re.search(r"\bsbyte\b", line):
            if re.search(r"<\s*0|>\s*0|==\s*-?1|!=\s*-?1|byteValue\(\)\s*[<>]|\(sbyte\)|& 0x80|>>\s*7|\*\s*-1", line):
                review.append("SIGN: {}: {}".format(rel_path, line.strip()))
    return re.sub(r"\bsbyte\b", "byte", text)


def collect_files():
    files = []
    for d in DIRS:
        for dirpath, _dirnames, filenames in os.walk(d):
            for fn in filenames:
                if fn.endswith(".cs"):
                    files.append(os.path.join(dirpath, fn))
    return files


def main():
    review = []
    changed = 0
    total = 0
    for path in collect_files():
        text = read_text(path)
        orig = text
        text = apply_literals(text)
        text = fix_powershell_backtick_artifacts(text)
        text = apply_math(text)
        text = apply_sbyte_to_byte(text, os.path.relpath(path, ROOT), review)
        if text != orig:
            write_text(path, text)
            changed += 1
        total += 1

    # leftover wrapper refs (case-sensitive, real code only)
    leftover = []
    wrapper_re = re.compile(
        r"\b(Double|Float|Integer|Long|Byte|Character)\.(NaN|MAX_VALUE|MIN_|parse|toString|compare|"
        r"decode|reverseBytes|toUnsigned|bitCount|longBitsToDouble|intBitsToFloat|floatToIntBits|"
        r"isNaN|isFinite|BYTES|isJavaIdentifierPart)\b")
    for path in collect_files():
        text = read_text(path)
        for i, line in enumerate(text.splitlines(), 1):
            if wrapper_re.search(line):
                if line.lstrip().startswith("//") or line.lstrip().startswith("*"):
                    continue
                if "JavaLangCompat" in line or "ArrayDouble" in line or "ArrayFloat" in line:
                    continue
                leftover.append("{}:{}: {}".format(
                    os.path.relpath(path, ROOT), i, line.strip()))

    review_path = os.path.join(ROOT, "tools", "sign-review.txt")
    leftover_path = os.path.join(ROOT, "tools", "wrapper-leftover.txt")
    with open(review_path, "w", encoding="utf-8") as f:
        f.write("\n".join(review))
    with open(leftover_path, "w", encoding="utf-8") as f:
        f.write("\n".join(leftover))

    print("Files processed: {}, changed: {}".format(total, changed))
    print("Sign-sensitive lines for review: {} -> {}".format(len(review), review_path))
    print("Leftover wrapper refs (manual): {} -> {}".format(len(leftover), leftover_path))


if __name__ == "__main__":
    main()
