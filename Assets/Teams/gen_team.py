import os

for i in range(1, 13):

    # create a folder named i

    os.mkdir(str(i))
    
    with open(f'{str(i)}/team.txt', 'w') as f:
        print(f'{str(i)}\nshirakami_{str(i)}', file=f)

