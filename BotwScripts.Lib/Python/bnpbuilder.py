# python.exe bnpbuilder.py mod/folder output/mod.bnp

import json

from bcml.dev import create_bnp_mod
from pathlib import Path
from sys import argv as args

def main():
    
    meta = json.loads(Path(f'{args[1]}\\info.json').read_text())

    create_bnp_mod(
        mod = Path(args[1]),
        output = Path(args[2]),
        meta=meta,
        options={}
    )

if __name__ == '__main__':
    main()