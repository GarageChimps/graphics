import json
from collections import namedtuple


def _json_object_hook(d):
    return namedtuple('X', d.keys())(*d.values())


def json2obj(data, object_hook=None):
    if object_hook is None:
        object_hook = _json_object_hook
    return json.loads(data, object_hook=object_hook)


def jsonfile2obj(file, object_hook=None):
    with open(file, 'r') as myfile:
        data = myfile.read()
        return json2obj(data, object_hook)