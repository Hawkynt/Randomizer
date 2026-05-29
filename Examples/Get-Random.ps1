# PowerShell offers two ways to get random data:
#
# 1. Get-Random uses System.Random under the hood (NOT cryptographically secure)
#    — fine for scripting and shuffling, not for security-sensitive use.
#
# 2. [System.Security.Cryptography.RandomNumberGenerator] uses the OS-level
#    cryptographic generator (BCryptGenRandom on Windows, /dev/urandom-equivalent
#    on Linux/macOS via PowerShell Core) — use this for keys, salts and tokens.

# Non-cryptographic (fast, scripting use):
$nonCrypto = Get-Random -Minimum 0 -Maximum ([UInt64]::MaxValue)
"Random 64-bit number (Get-Random):           {0:X16}" -f $nonCrypto | Write-Output

# Cryptographic (recommended for secrets):
$buf = New-Object byte[] 8
[System.Security.Cryptography.RandomNumberGenerator]::Fill($buf)
$crypto = [System.BitConverter]::ToUInt64($buf, 0)
"Random 64-bit number (cryptographic):        {0:X16}" -f $crypto | Write-Output
