import os
import shutil
import subprocess
from pathlib import Path
import sys

print("\n\n - Running Tests - \n")
try:
    subprocess.run("dotnet test", shell=True, check=True)
except subprocess.CalledProcessError as e:
    print(
        f"\n[ERROR] Tests failed with exit code {e.returncode}.",
        file=sys.stderr,
    )
    sys.exit(1)

print("\n\n\n - Building Package - \n")
try:
    subprocess.run("dotnet pack -c Release", shell=True, check=True)
except subprocess.CalledProcessError as e:
    print(
        f"\n[ERROR] Build failed with exit code {e.returncode}.",
        file=sys.stderr,
    )
    sys.exit(1)


print("\n\n\n - Publishing - \n")

source_dir = Path(r"AIAgentAdapter\bin\Release")
glob = "AIAgentAdapter.*.nupkg"

if not source_dir.exists():
    raise FileNotFoundError(f"The source directory '{source_dir}' does not exist relative to {Path.cwd()}")

CSHARP_PKGS = os.getenv("CSHARP_PKGS")

if CSHARP_PKGS is None:
    raise ValueError("The environment variable 'CSHARP_PKGS' is not set.")

dest_dir = Path(CSHARP_PKGS)

dest_dir.mkdir(parents=True, exist_ok=True)


for source_file in source_dir.glob(glob):
    if source_file.is_file():  
        destination_file = dest_dir / source_file.name
        shutil.move(str(source_file), str(destination_file))
        print(f"Moved: {source_file.name} -> {dest_dir}")
