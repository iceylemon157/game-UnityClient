from flask import Flask, Response, jsonify
from flask_cors import CORS
import json
import random


app = Flask(__name__)
CORS(app)

@app.route('/api/operation', methods=['GET'])
def random_operation():
    operations = ['w', 'a', 's', 'd', 'e', 'f']
    response = jsonify(random.choice(operations))
    response.headers.add('Access-Control-Allow-Origin', '*')
    response.headers.add('Content-Type', 'text/plain')
    return response


if __name__ == '__main__':
    app.run()
