# Pass 1: mechanical translation fixes on copied .cs files.
#  1. sbyte -> byte (Plan A: unify to unsigned byte; keep & 0xFF; sign spots reviewed later)
#  2. Java wrapper class static members -> .NET equivalents (helper: JavaLangCompat)
#  3. .class literals -> typeof(), wrapper typeof(Xxx) -> typeof(xxx)
#  4. Math.toRadians / Math.toDegrees -> arithmetic
# Generates tools\sign-review.txt with sign-sensitive lines for manual review.
# Usage: pwsh -NoProfile -File tools\pass1.ps1

$ErrorActionPreference = 'Stop'
$root = 'C:\Users\fyq\Documents\dev\dotnet\NetcdfSharp'
$dirs = @(
  "$root\NetcdfSharp.Core",
  "$root\NetcdfSharp.Udunits",
  "$root\NetcdfSharp.Grib",
  "$root\NetcdfSharp.Bufr"
)

# UTF-8 with BOM (matches original files)
$utf8Bom = New-Object System.Text.UTF8Encoding($true)
$review = New-Object System.Collections.Generic.List[string]

function Get-Utf8Text([string]$path) {
  $bytes = [System.IO.File]::ReadAllBytes($path)
  # detect BOM
  if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
    return [System.Text.Encoding]::UTF8.GetString($bytes, 3, $bytes.Length - 3)
  }
  return [System.Text.Encoding]::UTF8.GetString($bytes)
}

# Ordered literal replacements (exact, longest-first to avoid partial)
$literalPairs = @(
  # --- Double ---
  @{ from = 'Double.NaN';        to = 'double.NaN' },
  @{ from = 'Double.MAX_VALUE';  to = 'double.MaxValue' },
  @{ from = 'Double.MIN_NORMAL'; to = 'JavaLangCompat.DoubleMinNormal' },
  @{ from = 'Double.BYTES';      to = 'sizeof(double)' },
  @{ from = 'Double.toString(';  to = 'JavaLangCompat.DoubleToString(' },
  @{ from = 'Double.parseDouble('; to = 'JavaLangCompat.ParseDouble(' },
  @{ from = 'Double.compare(';   to = 'JavaLangCompat.DoubleCompare(' },
  @{ from = 'Double.longBitsToDouble('; to = 'BitConverter.Int64BitsToDouble(' },
  # --- Float ---
  @{ from = 'Float.NaN';         to = 'float.NaN' },
  @{ from = 'Float.MIN_NORMAL';  to = 'JavaLangCompat.FloatMinNormal' },
  @{ from = 'Float.BYTES';       to = 'sizeof(float)' },
  @{ from = 'Float.floatToRawIntBits('; to = 'BitConverter.SingleToInt32Bits(' },
  @{ from = 'Float.floatToIntBits(';   to = 'BitConverter.SingleToInt32Bits(' },
  @{ from = 'Float.intBitsToFloat(';   to = 'BitConverter.Int32BitsToSingle(' },
  @{ from = 'Float.isFinite(';   to = 'float.IsFinite(' },
  @{ from = 'Float.isNaN(';      to = 'float.IsNaN(' },
  @{ from = 'Float.compare(';    to = 'JavaLangCompat.FloatCompare(' },
  # --- Integer ---
  @{ from = 'Integer.BYTES';       to = 'sizeof(int)' },
  @{ from = 'Integer.parseInt(';   to = 'JavaLangCompat.ParseInt(' },
  @{ from = 'Integer.toString(';   to = 'JavaLangCompat.IntToString(' },
  @{ from = 'Integer.compare(';    to = 'JavaLangCompat.IntCompare(' },
  @{ from = 'Integer.toUnsignedLong('; to = 'JavaLangCompat.ToUnsignedLong(' },
  @{ from = 'Integer.reverseBytes(';   to = 'JavaLangCompat.ReverseBytes(' },
  @{ from = 'Integer.decode(';     to = 'JavaLangCompat.IntDecode(' },
  # --- Long ---
  @{ from = 'Long.BYTES';       to = 'sizeof(long)' },
  @{ from = 'Long.parseLong(';  to = 'JavaLangCompat.ParseLong(' },
  @{ from = 'Long.compare(';    to = 'JavaLangCompat.LongCompare(' },
  @{ from = 'Long.bitCount(';   to = 'JavaLangCompat.BitCount(' },
  @{ from = 'Long.decode(';     to = 'JavaLangCompat.LongDecode(' },
  # --- Byte ---
  @{ from = 'Byte.BYTES';        to = 'sizeof(byte)' },
  @{ from = 'Byte.toUnsignedInt('; to = 'JavaLangCompat.ToUnsignedInt(' },
  # --- Character ---
  @{ from = 'Character.isJavaIdentifierPart('; to = 'JavaLangCompat.IsJavaIdentifierPart(' },
  # --- .class literals ---
  @{ from = 'boolean.class'; to = 'typeof(bool)' },
  @{ from = 'byte.class';    to = 'typeof(byte)' },
  @{ from = 'short.class';   to = 'typeof(short)' },
  @{ from = 'int.class';     to = 'typeof(int)' },
  @{ from = 'long.class';    to = 'typeof(long)' },
  @{ from = 'float.class';   to = 'typeof(float)' },
  @{ from = 'double.class';  to = 'typeof(double)' },
  @{ from = 'char.class';    to = 'typeof(char)' },
  @{ from = 'String.class';  to = 'typeof(string)' },
  @{ from = 'Object.class';  to = 'typeof(object)' },
  @{ from = 'ByteBuffer.class'; to = 'typeof(ByteBuffer)' },
  @{ from = 'StructureDataIterator.class'; to = 'typeof(StructureDataIterator)' },
  @{ from = 'StructureData.class'; to = 'typeof(StructureData)' },
  # --- typeof(wrapper) -> typeof(primitive) ---
  @{ from = 'typeof(Float)';     to = 'typeof(float)' },
  @{ from = 'typeof(Double)';    to = 'typeof(double)' },
  @{ from = 'typeof(Short)';     to = 'typeof(short)' },
  @{ from = 'typeof(Integer)';   to = 'typeof(int)' },
  @{ from = 'typeof(Long)';      to = 'typeof(long)' },
  @{ from = 'typeof(Byte)';      to = 'typeof(byte)' },
  @{ from = 'typeof(Character)'; to = 'typeof(char)' },
  @{ from = 'typeof(Boolean)';   to = 'typeof(bool)' }
)

function Apply-Literals([string]$text) {
  foreach ($p in $literalPairs) {
    $text = $text.Replace($p.from, $p.to)
  }
  return $text
}

function Apply-Math([string]$text) {
  # Math.toRadians(X) -> ((X) * (Math.PI / 180.0))
  $text = [regex]::Replace($text, 'Math\.toRadians\(([^()]*)\)', '((`$1) * (Math.PI / 180.0))')
  # Math.toDegrees(X) -> ((X) * (180.0 / Math.PI))
  $text = [regex]::Replace($text, 'Math\.toDegrees\(([^()]*)\)', '((`$1) * (180.0 / Math.PI))')
  return $text
}

function Apply-SbyteToByte([string]$text, [string]$path, [ref]$changed) {
  # capture sign-sensitive lines BEFORE replacement for review
  $lines = $text -split "`n"
  foreach ($line in $lines) {
    if ($line -match '\bsbyte\b') {
      if ($line -match '<\s*0|>\s*0|==\s*-?1|!=\s*-?1|byteValue\(\)\s*[<>]|\(sbyte\)|& 0x80|>>\s*7|\*\s*-1|minus') {
        $review.Add(("SIGN: {0}: {1}" -f $path, $line.Trim()))
      }
    }
  }
  # global word-boundary sbyte -> byte
  $new = [regex]::Replace($text, '\bsbyte\b', 'byte')
  $changed.Value = ($new -ne $text)
  return $new
}

$totalFiles = 0
$changedFiles = 0
$files = Get-ChildItem -Path $dirs -Recurse -Filter *.cs -File
foreach ($file in $files) {
  $text = Get-Utf8Text $file.FullName
  $orig = $text
  $text = Apply-Literals $text
  $text = Apply-Math $text
  $c = $false
  $text = Apply-SbyteToByte $text $file.FullName.Replace($root, '') ([ref]$c)
  if ($text -ne $orig) {
    [System.IO.File]::WriteAllText($file.FullName, $text, $utf8Bom)
    $changedFiles++
  }
  $totalFiles++
}

# report remaining Java wrapper references (should be ~0; leftover needs manual)
$leftover = New-Object System.Collections.Generic.List[string]
foreach ($file in (Get-ChildItem -Path $dirs -Recurse -Filter *.cs -File)) {
  $text = Get-Utf8Text $file.FullName
  foreach ($line in ($text -split "`n")) {
    if ($line -match '\b(Double|Float|Integer|Long|Byte|Character)\.(NaN|MAX_VALUE|MIN_|parse|toString|compare|decode|reverseBytes|toUnsigned|bitCount|longBitsToDouble|intBitsToFloat|floatToIntBits|isNaN|isFinite|BYTES|isJavaIdentifierPart)\b' -and
        $line -notmatch '^\s*//' -and $line -notmatch '^\s*\*' -and $line -notmatch 'JavaLangCompat' -and $line -notmatch 'ArrayDouble|ArrayFloat') {
      $leftover.Add(("{0}: {1}" -f $file.FullName.Replace($root, ''), $line.Trim()))
    }
  }
}

$reviewPath = Join-Path $root 'tools\sign-review.txt'
[System.IO.File]::WriteAllLines($reviewPath, $review.ToArray(), $utf8Bom)
$leftoverPath = Join-Path $root 'tools\wrapper-leftover.txt'
[System.IO.File]::WriteAllLines($leftoverPath, $leftover.ToArray(), $utf8Bom)

Write-Host "Files processed: $totalFiles, changed: $changedFiles"
Write-Host "Sign-sensitive lines for review: $($review.Count) -> $reviewPath"
Write-Host "Leftover wrapper refs (manual): $($leftover.Count) -> $leftoverPath"
