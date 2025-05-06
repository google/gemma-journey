import kagglehub
import os
import shutil
from pathlib import Path
import sys

# --- Configuration ---
MODEL_HANDLE = "google/gemma-3/gemmaCpp/3.0-4b-it-sfp"

# Determine the project root.
# If running as a script, it's the script's directory.
# If frozen by PyInstaller and run from project root, Path.cwd() is the project root.
if getattr(sys, 'frozen', False):
    # Executable mode (e.g., PyInstaller bundle)
    # Assumes the executable is run from the project root directory.
    PROJECT_ROOT = Path.cwd()
else:
    # Script mode
    PROJECT_ROOT = Path(__file__).resolve().parent

TARGET_DIR = PROJECT_ROOT / "Assets" / "StreamingAssets" / "gemma-3.0-4b"
# --- End Configuration ---

def main():
    print(f"Gemma Downloader Script")
    print(f"-----------------------")
    print(f"Model to download: {MODEL_HANDLE}")
    print(f"Target directory for model files: {TARGET_DIR}")
    
    try:
        print(f"\nEnsuring target directory exists: {TARGET_DIR}")
        TARGET_DIR.mkdir(parents=True, exist_ok=True)
        print(f"Target directory ready.")

        print(f"\nDownloading model: {MODEL_HANDLE}...")
        print("This may take some time depending on your internet connection and model size.")
        # kagglehub.model_download returns the path to the downloaded model assets
        # in a local cache directory.
        download_path_str = kagglehub.model_download(MODEL_HANDLE)
        download_path = Path(download_path_str)
        
        if not download_path.exists():
            print(f"\nError: Download path {download_path} does not exist after download attempt.")
            sys.exit(1)
        print(f"Model downloaded to cache: {download_path}")

        print(f"\nPreparing to copy model files to {TARGET_DIR}...")
        
        # Clear the target directory first to ensure a clean copy.
        # This is useful if the script is re-run.
        print(f"Cleaning target directory: {TARGET_DIR} (if it contains old files)...")
        for item in TARGET_DIR.iterdir():
            if item.is_file():
                item.unlink()
            elif item.is_dir():
                shutil.rmtree(item)
        print(f"Target directory cleaned.")

        print(f"Copying model files from cache ({download_path}) to target ({TARGET_DIR})...")
        copied_count = 0
        if download_path.is_dir():
            for item in download_path.iterdir():
                destination_item = TARGET_DIR / item.name
                if item.is_file():
                    shutil.copy2(item, destination_item)
                    print(f"  Copied file: {item.name}")
                    copied_count += 1
                elif item.is_dir():
                    # If there are subdirectories in the model assets, copy them too.
                    shutil.copytree(item, destination_item, dirs_exist_ok=True)
                    print(f"  Copied directory: {item.name}")
                    copied_count += 1
            
            if copied_count == 0:
                print(f"Warning: No files or directories were found in the downloaded path {download_path} to copy.")
            else:
                print(f"Successfully copied {copied_count} item(s) to {TARGET_DIR}")
        elif download_path.is_file(): 
            # This case is less likely for model_download, which usually gives a directory.
            shutil.copy2(download_path, TARGET_DIR / download_path.name)
            print(f"Successfully copied single file {download_path.name} to {TARGET_DIR}")
        else:
            print(f"\nError: Downloaded path {download_path} is neither a file nor a directory.")
            sys.exit(1)
            
        print("\n-------------------------------------")
        print("Gemma model download and setup complete!")
        print(f"Model files are now in: {TARGET_DIR}")
        print("-------------------------------------")

    except ImportError:
        print("\nError: The 'kagglehub' library is not installed.")
        print("Please install it by running: pip install kagglehub")
        sys.exit(1)
    except Exception as e:
        print(f"\nAn error occurred: {e}")
        print("\nPlease ensure you have Kaggle API credentials configured.")
        print("This typically involves:")
        print("1. Installing the Kaggle CLI: pip install kaggle")
        print("2. Setting up your API token: Download 'kaggle.json' from your Kaggle account page and place it in ~/.kaggle/kaggle.json (Linux/macOS) or C:\\Users\\<Your-Username>\\.kaggle\\kaggle.json (Windows).")
        print("Alternatively, set KAGGLE_USERNAME and KAGGLE_KEY environment variables.")
        print("Also, ensure you have accepted the terms for the Gemma model on Kaggle.")
        sys.exit(1)

if __name__ == "__main__":
    main()
