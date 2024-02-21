#Вариант на Python

import pandas as pd
import itertools
import math
#Parse file
def Parse(fileName='Data2.csv'):
    file = pd.read_csv(fileName,header=0,sep=";",encoding="utf8")
    file["Затраты времени на посещение"]=file["Затраты времени на посещение"].apply(lambda x:float(x.replace("ч","").replace(",",".")))
    file["Важность"]=file["Важность"].apply(lambda x:float(x))
    print(file.head())
    return file
file=Parse()

def calculate_route(target,sleep_time=2*8,travel_time=48):
   
    best_route = None
    best_time = float("inf")
    best_score=0
    max_Time=travel_time-sleep_time
    
    comb_count=0
    for count in range(1,len(target)):
        comb_count+=math.comb(len(target),count)
    print("Всего нужно проверить: "+str(comb_count))
    
    
    for count in range(1,len(target)):
        #print(file.index.values.tolist())
        combinations = itertools.combinations(file.index.values.tolist(),count)
        print("Combinations for "+str(count) +": "+str(math.comb(len(target),count)))
        for combination in combinations:
            #print("Comb "+str(combination))
            current_time = 0
            currect_score = 0
            route = []
            
            
            for index in combination:
                data=target.iloc[[index]].values[0]
                #print(data)
                name=data[0]
                duration=data[1]
                score=data[2]
                if current_time + duration <= max_Time:
                    route.append(name)
                    current_time += duration
                    currect_score+=score
                else:
                    break

            
            #print("New route:" +str(route)+" Importance: "+str(currect_score))
            #Проверяем, что новый маршрут лучше
            if current_time < max_Time and best_score<currect_score:
                best_time = current_time
                best_route = route
                best_score = currect_score
                print("New best route:" +str(best_route)+" Importance: "+str(best_score))

    return best_route,best_time,best_score

best_route,best_time,best_score = calculate_route(file)
print("Best route:")
print("Route:" +str(best_route))
print("Time: "+str(best_time))
print("Importance: "+str(best_score))
