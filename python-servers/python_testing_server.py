from flask import Flask, Response, jsonify
from flask_cors import CORS
import json
import random


app = Flask(__name__)
# CORS(app)

valid_operations = ['w', 'a', 's', 'd', 'e', 'f']
operations = ['a'] * 20 + ['w'] * 100 + ['e'] * 100 + ['d'] * 60 + ['w'] * 5 + ['e'] + ['f'] * 3 \
+ ['d'] * 80 + ['w'] + ['e'] + ['a'] * 30 + ['w'] + ['e'] + ['d'] * 30 + \
['s'] * 50 + ['d'] + ['e'] * 150 + ['a'] * 80 + ['w'] * 50 + ['e'] + \
['d'] * 40 + ['w'] + ['e'] * 300 + ['a'] * 60 + ['w'] + ['e'] + \
['a'] * 60 + ['s'] * 200 + ['e'] + ['d'] * 100 + ['s'] + ['e'] + ['f'] * 3 + ['e'] + \
['a'] * 30 + ['s'] + ['e'] + ['a'] * 120 + ['s'] + ['e'] + ['d'] * 150 + ['s'] + ['e'] + ['f'] * 5 + \
['a'] * 200 + ['w'] * 150 + ['e'] + ['d'] * 150 + ['w'] + ['e'] * 2 + ['s'] * 120 + ['e'] + ['d'] * 30 + ['s'] + ['e'] + \
['w'] * 120 + ['d'] * 60 + ['e']
print(operations)
print(*operations[::-1], sep='", "')
cnt = 0

@app.route('/api/operation', methods=['GET'])
def random_operation():
    global cnt, operations
    if cnt >= len(operations):
        response = Response('z')
    else:
        response = Response(operations[cnt])
    cnt += 1
    # add user agent
    # response.headers.add('User-Agent', 'Mozilla/5.0')

    # response.headers.add('Access-Control-Allow-Origin', '*')
    # response.headers.add('Content-Type', 'text/plain')
    return response


if __name__ == '__main__':
    app.run(port=8888)
