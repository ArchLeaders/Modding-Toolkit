# python add_entry.py actorinfo hkrbsize fullname partialname log

from pathlib import Path
from oead import byml, yaz0, S32
from sys import argv as args
from zlib import crc32

def main():
    actorinfo = Path(args[1])
    hkrb = int(args[2])
    full_name = args[3]
    partial_name = args[4]
    is_log = len(args) > 4
    log = ''

    if is_log == True:
        log = args[5]

    data = byml.from_binary(yaz0.decompress(actorinfo.read_bytes()))
    new_actor = {}

    for actor in data['Actors']:
        new_actor = actor

    new_actor['bfres'] = partial_name
    new_actor['instSize'] = S32(int(new_actor['instSize']) + hkrb)
    new_actor['mainModel'] = full_name
    new_actor['name'] = full_name
    new_actor['profile'] = 'MapDynamicActive'

    if is_log == True:
        yaml_data = {}

        if Path(log).is_file:
            byml.from_text(Path(log).read_text())

        yaml_data[crc32(full_name)] = new_actor
        yaml_data = byml.to_text(yaml_data)

        Path(log).write_text(yaml_data)

    else:

        data['Actors'].append(new_actor)
        data['Hashes'].append(crc32(full_name))

        data = byml.to_binary(data)
        data = yaz0.compress(data)
        actorinfo.write_bytes(data)

if __name__ == '__main__':
    main()