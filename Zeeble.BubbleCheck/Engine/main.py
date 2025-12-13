"""

 OMRChecker

 Author: Udayraj Deshmukh
 Github: https://github.com/Udayraj123

"""

import argparse
import sys
from src.entry import entry_point

if __name__ == "__main__":
    parser  = argparse.ArgumentParser()
    parser .add_argument("input_file", help="Path to the input file")
    parser .add_argument("template_file", help="Path to the template file")
    args = parser.parse_args()

    entry_point(args.input_file, args.template_file)
