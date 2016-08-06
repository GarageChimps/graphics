import json


def jsonfile2obj(file, object_hook=None):
    with open(file, 'r') as myfile:
        data = myfile.read()
        return json.loads(data, object_hook=object_hook)