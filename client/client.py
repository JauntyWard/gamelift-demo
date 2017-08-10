#!/usr/bin/python

import boto3
import random
import requests
import time
import json

local = False


if local:
    client = boto3.client('gamelift', endpoint_url='http://localhost:8090')
    fleet = "foobar"
else:
    client = boto3.client('gamelift')

    fleets = client.list_fleets()
    fleet = fleets['FleetIds'][0]

players = []


    game_id = str('game-'+str(random.randint(0,100)))

    for i in range(8):
        rnd = random.randint(0,10)

        player_id = str(random.randint(5000,30000))
        players.append(player_id)
        item = dict()

    response = client.create_game_session(
        AliasId="alias-bf34a0e7-9c29-4440-a572-4de38b8c5ac7",
        MaximumPlayerSessionCount=8,
        Name=game_id,
    )

    time.sleep(5)


    game_session_id = response["GameSession"]["GameSessionId"]

    print game_session_id

    early_quit_prob = random.randint(0,100)

    if not ml_matchmaking:
        threshold = 20
    else:
        threshold = 80

    if early_quit_prob > threshold:
        early_quit = True
    else:
        early_quit = False

    matchmaking_stats = "{}, {}, {}\n".format(game_id, early_quit, ml_matchmaking)

    firehose_client.put_record(
        DeliveryStreamName='matchmaking_stats',
        Record={
        'Data': matchmaking_stats
        }
    )
    for player in players:
        player_id = player

        response = client.create_player_sessions(
            GameSessionId=game_session_id,
            PlayerIds=[
                player_id,
            ]
        )

        server_ip = response["PlayerSessions"][0]["IpAddress"]
        server_port = response["PlayerSessions"][0]["Port"]
        player_session_id = response["PlayerSessions"][0]["PlayerSessionId"]

        # RESTful call to the game server to kill the game
        r = requests.post("http://"+server_ip+":"+str(server_port)+"/start", data={'player_id': player_id, 'session_id': player_session_id})


    time.sleep(700)
    r = requests.get("http://"+server_ip+":"+str(server_port)+"/end")
    print r.status_code

    # r = requests.get("http://"+server_ip+":"+str(server_port)+"/shutdown")
    # print r.status_code
