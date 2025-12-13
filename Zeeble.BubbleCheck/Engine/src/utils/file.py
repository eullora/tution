import argparse
import json 
import os
from pathlib import Path
from time import localtime, strftime

from src.logger import logger


def load_json(path, **rest):
    try:
        with open(path, "r", encoding='utf-8') as f:
            loaded = json.load(f, **rest)
    except json.decoder.JSONDecodeError as error:
        logger.critical(f"JSON Decode Error when loading file at: '{path}'\n{error}")
        exit(1)
    except FileNotFoundError:
        logger.critical(f"File not found when attempting to load JSON: '{path}'")
        exit(1)
    except Exception as e:
        logger.critical(f"Error loading JSON file at '{path}': {e}")
        exit(1)
    return loaded


class Paths:
    def __init__(self, output_dir_base):
        output_dir = Path(output_dir_base)
        self.output_dir = output_dir
        self.save_marked_dir = output_dir / "CheckedOMRs"
        self.results_dir = output_dir / "Results"
        self.manual_dir = output_dir / "Manual"
        self.evaluation_dir = output_dir / "Evaluation"
        self.errors_dir = self.manual_dir / "ErrorFiles"
        self.multi_marked_dir = self.manual_dir / "MultiMarkedFiles"
        # Default paths, might be overwritten in setup_outputs_for_template
        self.results_json = self.results_dir / "results.json"
        self.errors_json = self.errors_dir / "errors.json"
        self.multimarked_json = self.multi_marked_dir / "multimarked.json"


def setup_dirs_for_paths(paths: Paths):
    logger.info("Checking Output Directories...")
    dirs_to_create = [
        paths.output_dir,
        paths.save_marked_dir,
        paths.results_dir,
        paths.manual_dir,
        paths.evaluation_dir,
        paths.errors_dir,
        paths.multi_marked_dir,
        paths.save_marked_dir / "stack",
        paths.save_marked_dir / "_MULTI_",
        paths.save_marked_dir / "_MULTI_" / "stack"
    ]
    for dir_path in dirs_to_create:
        try:
            dir_path.mkdir(parents=True, exist_ok=True)
        except Exception as e:
            logger.error(f"Failed to create directory {dir_path}: {e}")
            raise
    logger.info(f"Output directories ensured under: {paths.output_dir}")


def setup_outputs_for_template(paths: Paths, template):
    ns = argparse.Namespace()
    logger.info("Setting up output namespace for JSON...")

    ns.paths = paths
    TIME_NOW_HRS = strftime("%I%p", localtime())
    ns.paths.results_json = paths.results_dir / f"Results_{TIME_NOW_HRS}.json"
    ns.paths.errors_json = paths.errors_dir / "errors.json"
    ns.paths.multimarked_json = paths.multi_marked_dir / "multimarked.json"

    ns.results_data = []
    ns.errors_data = []
    ns.multimarked_data = []

    empty_val = getattr(template, 'global_empty_val', "")
    ns.empty_resp = [empty_val] * len(template.output_columns)

    logger.info(f"JSON output paths configured:")
    logger.info(f"  Results: {ns.paths.results_json}")
    logger.info(f"  Errors: {ns.paths.errors_json}")
    logger.info(f"  MultiMarked: {ns.paths.multimarked_json}")
    logger.info("Output namespace ready for data collection.")

    return ns


def write_json_output(data_list, json_path):
    if not data_list: 
        logger.info(f"No data collected for {json_path.name}, skipping file writing.")
        return
    try:
        logger.info(f"Writing {len(data_list)} records to JSON: {json_path}")
        json_path.parent.mkdir(parents=True, exist_ok=True)
        with open(json_path, "w", encoding='utf-8') as f:
            json.dump(data_list, f, indent=4)
    except Exception as e:
        logger.error(f"Failed to write JSON file to {json_path}: {e}")

def write_all_outputs(outputs_namespace):
    write_json_output(outputs_namespace.results_data, outputs_namespace.paths.results_json)
    write_json_output(outputs_namespace.errors_data, outputs_namespace.paths.errors_json)
    write_json_output(outputs_namespace.multimarked_data, outputs_namespace.paths.multimarked_json)