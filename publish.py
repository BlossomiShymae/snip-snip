#!/usr/bin/env python3
# Script to publish needlework-core for release on platforms in rid_list below.
import subprocess
import os
import zipfile
import shutil

# Clear previous dist files
try:
  shutil.rmtree(os.path.abspath("dist"))
except:
  pass

assembly_name = "snip-snip"
static_files = [os.path.abspath("README.md"), os.path.abspath("LICENSE")]
rid_list = ['win-x64', 'linux-x64', 'osx-x64']
zipfile_list = []
# Publish release for platform
for rid in rid_list:
  result = subprocess.run(f"dotnet publish -c Release -r {rid} --no-self-contained", shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
  
  out = result.stdout.decode('utf-8').rstrip()
  if out:
    print(out)
  err = result.stderr.decode('utf-8').rstrip()
  if err:
    print(err)
  
  files = [f for f in os.listdir(os.path.abspath("dist"))]
  
  zip_file = os.path.abspath(os.path.join("dist", f"{assembly_name}-{rid}.zip"))
  if os.path.isfile(zip_file):
    os.remove(zip_file)
  
  with zipfile.ZipFile(zip_file, "w", zipfile.ZIP_DEFLATED) as archive:
    for s in static_files:
      archive.write(s, os.path.basename(s))
    for file in files:
      file_path = os.path.abspath(os.path.join("dist", file))
      if ".pdb" in file:
        os.remove(file_path)
        continue
      if file not in rid_list and file_path not in zipfile_list:
        archive.write(file_path, os.path.basename(file_path))
        os.remove(file_path)
  zipfile_list.append(zip_file)
  
