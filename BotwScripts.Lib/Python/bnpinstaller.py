# python.exe bnpinstaller.py folder/mod.bnp [true|false]

from bcml.install import install_mod, link_master_mod
from pathlib import Path
from sys import argv as args

def main():

    if args.count() > 3:
        remerge: bool = args[2] == "true"
    else:
        remerge: bool = False

    install_mod(
        Path(args[1]),
        merge_now=remerge
    )

    if remerge == True:
        link_master_mod()

    input()

if __name__ == '__main__':
    main()