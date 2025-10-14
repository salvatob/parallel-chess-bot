import os
import subprocess
import json

def run_command(cmd, cwd=None):
    result = subprocess.run(cmd, shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, cwd=cwd, text=True)
    if result.returncode != 0:
        raise Exception(f"Command failed: {cmd}\nstdout: {result.stdout}\nstderr: {result.stderr}")
    return result.stdout

def checkout_branch(branch_name, repo_path):
    run_command(f"git checkout {branch_name}", cwd=repo_path)

def run_benchmark(csproj_path, output_file):
    # Run dotnet benchmark with output to json file
    run_command(f"dotnet run -c Release --project {csproj_path} --exporters json --output {output_file}")

def merge_results(file1, file2, merged_file):
    # Load the two JSON benchmark result files
    with open(file1, 'r') as f1, open(file2, 'r') as f2:
        results1 = json.load(f1)
        results2 = json.load(f2)

    # Example merge strategy: combine all benchmark entries from both, labeling them by branch
    merged = {'benchmarks': []}

    for entry in results1.get('benchmarks', []):
        entry['branch'] = 'branch1'
        merged['benchmarks'].append(entry)

    for entry in results2.get('benchmarks', []):
        entry['branch'] = 'branch2'
        merged['benchmarks'].append(entry)

    with open(merged_file, 'w') as mf:
        json.dump(merged, mf, indent=4)

if __name__ == "__main__":
    repo_path = "./"  # Set to your local repo path where the git repo .git directory is present
    branch1 = "master"  # First branch name
    branch2 = "improve-move-generation"  # Second branch name
    csproj_path = ".\Benchmarks"  # Path to the benchmark csproj file
    output1 = "benchmark_branch1.json"
    output2 = "benchmark_branch2.json"
    merged_output = "merged_benchmark.json"

    print(f"Checking out {branch1}...")
    checkout_branch(branch1, repo_path)
    print(f"Running benchmark on {branch1}...")
    run_benchmark(csproj_path, output1)

    print(f"Checking out {branch2}...")
    checkout_branch(branch2, repo_path)
    print(f"Running benchmark on {branch2}...")
    run_benchmark(csproj_path, output2)

    print("Merging results...")
    merge_results(output1, output2, merged_output)

    print(f"Merged results saved to {merged_output}")
