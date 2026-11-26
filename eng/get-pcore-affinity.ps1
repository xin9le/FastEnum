<#
.SYNOPSIS
    Prints the affinity mask that keeps only the performance cores.

.DESCRIPTION
    Used by run-benchmarks.bat. On a hybrid CPU (Intel P/E cores, Snapdragon X,
    Zen 5c) the scheduler may move a benchmark from a P core to an E core in the
    middle of an iteration. That is not noise within StdDev but a several-fold
    difference, and the mean of a whole run shifts with the share of time spent
    on E cores. BenchmarkDotNet does not reject such a run; it only shows a
    spread, and two runs of the same code disagree by 10-15%.

    The cores come from GetLogicalProcessorInformationEx(RelationProcessorCore):
    every physical core carries an EfficiencyClass. A higher class means a more
    performant core, and the class is non-zero ONLY on hybrid machines. So:

      * all cores of one class (a usual Xeon, Ryzen, any pre-hybrid Intel) -
        nothing to restrict, the script prints nothing;
      * several classes - the mask has every logical processor of the cores of
        the highest class, SMT siblings included: the benchmarks are single
        threaded, and an idle sibling does not disturb them.

    A mask belongs to a processor group (KAFFINITY) and a process lives in one
    group, so on machines with more than 64 logical processors only group 0 is
    taken; if it has no P cores, the script prints nothing again.

    Diagnostics go to stderr, in ASCII. stdout gets exactly one line with the
    mask, or nothing; the caller must treat empty output as "no restriction",
    not as an error.

.PARAMETER AsDecimal
    Print the mask as a decimal number instead of hexadecimal (start /affinity
    takes hexadecimal).

.OUTPUTS
    System.String - the mask, or nothing.

.EXAMPLE
    PS> .\get-pcore-affinity.ps1
    3C03
#>
[CmdletBinding()]
param(
    [switch] $AsDecimal
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

function Write-Diagnostic([string] $message)
{
    [Console]::Error.WriteLine($message)
}

# The buffer is read by offsets rather than through a [StructLayout] class:
# SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX has a variable length (GroupMask is an
# ANYSIZE_ARRAY), which the marshaller cannot express.
#
#   SYSTEM_LOGICAL_PROCESSOR_INFORMATION_EX { DWORD Relationship; DWORD Size; union {...} }
#   PROCESSOR_RELATIONSHIP { BYTE Flags; BYTE EfficiencyClass; BYTE Reserved[20];
#                            WORD GroupCount; GROUP_AFFINITY GroupMask[ANYSIZE_ARRAY]; }
#   GROUP_AFFINITY { KAFFINITY Mask; WORD Group; WORD Reserved[3]; }
$source = @'
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace FastEnumBench
{
    public struct CoreInfo
    {
        public byte EfficiencyClass;
        public ushort Group;
        public ulong Mask;
    }

    public static class TopologyProbe
    {
        private const int RelationProcessorCore = 0;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetLogicalProcessorInformationEx(
            int relationshipType, IntPtr buffer, ref int returnedLength);

        public static CoreInfo[] GetCores()
        {
            int length = 0;
            GetLogicalProcessorInformationEx(RelationProcessorCore, IntPtr.Zero, ref length);
            if (length <= 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            IntPtr buffer = Marshal.AllocHGlobal(length);
            try
            {
                if (!GetLogicalProcessorInformationEx(RelationProcessorCore, buffer, ref length))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // GROUP_AFFINITY: ULONG_PTR + WORD + WORD[3], so 16 bytes on x64 and 12 on x86.
                int groupAffinitySize = IntPtr.Size + 8;
                var cores = new List<CoreInfo>();

                int offset = 0;
                while (offset + 8 <= length)
                {
                    int relationship = Marshal.ReadInt32(buffer, offset);
                    int size = Marshal.ReadInt32(buffer, offset + 4);
                    if (size <= 0)
                    {
                        break;
                    }

                    if (relationship == RelationProcessorCore)
                    {
                        int processor = offset + 8;
                        byte efficiencyClass = Marshal.ReadByte(buffer, processor + 1);
                        int groupCount = (ushort)Marshal.ReadInt16(buffer, processor + 22);

                        for (int i = 0; i < groupCount; i++)
                        {
                            int groupAffinity = processor + 24 + (i * groupAffinitySize);
                            ulong mask = IntPtr.Size == 8
                                ? (ulong)Marshal.ReadInt64(buffer, groupAffinity)
                                : (ulong)(uint)Marshal.ReadInt32(buffer, groupAffinity);

                            cores.Add(new CoreInfo
                            {
                                EfficiencyClass = efficiencyClass,
                                Group = (ushort)Marshal.ReadInt16(buffer, groupAffinity + IntPtr.Size),
                                Mask = mask
                            });
                        }
                    }

                    offset += size;
                }

                return cores.ToArray();
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
'@

function Get-BitCount([uint64] $mask)
{
    $count = 0
    while ($mask -ne 0)
    {
        $mask = $mask -band ($mask - 1)
        $count++
    }

    return $count
}

try
{
    if (-not ('FastEnumBench.TopologyProbe' -as [type]))
    {
        Add-Type -TypeDefinition $source -Language CSharp -ErrorAction Stop
    }

    $cores = @([FastEnumBench.TopologyProbe]::GetCores())
}
catch
{
    Write-Diagnostic ("P-core detection failed: " + $_.Exception.Message)
    Write-Diagnostic "Falling back to the default affinity (all cores)."
    exit 0
}

if ($cores.Count -eq 0)
{
    Write-Diagnostic "P-core detection: the OS reported no processor cores, using all cores."
    exit 0
}

$classes = @($cores | ForEach-Object { [int] $_.EfficiencyClass } | Sort-Object -Unique)
if ($classes.Count -le 1)
{
    Write-Diagnostic ("P-core detection: uniform CPU, all " + $cores.Count +
        " cores share efficiency class " + $classes[0] + " - no affinity mask needed.")
    exit 0
}

$topClass = $classes[-1]
$fast = @($cores | Where-Object { [int] $_.EfficiencyClass -eq $topClass -and $_.Group -eq 0 })
if ($fast.Count -eq 0)
{
    Write-Diagnostic ("P-core detection: processor group 0 has no cores of the fastest class " +
        $topClass + " - a mask would only narrow the run, using all cores.")
    exit 0
}

$mask = [uint64] 0
foreach ($core in $fast)
{
    $mask = $mask -bor $core.Mask
}

if ($mask -eq 0)
{
    Write-Diagnostic "P-core detection: the fastest cores carry an empty affinity mask, using all cores."
    exit 0
}

$groupZeroTotal = 0
foreach ($core in $cores)
{
    if ($core.Group -eq 0)
    {
        $groupZeroTotal += Get-BitCount $core.Mask
    }
}

Write-Diagnostic ("P-core detection: hybrid CPU, efficiency classes " + ($classes -join ',') +
    "; " + $fast.Count + " core(s) of class " + $topClass + " -> " + (Get-BitCount $mask) +
    " of " + $groupZeroTotal + " logical CPUs in group 0.")

if ($AsDecimal)
{
    Write-Output ([string] $mask)
}
else
{
    Write-Output ('{0:X}' -f $mask)
}
