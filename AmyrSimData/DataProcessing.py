import numpy as np
import h5py
import math
import time
from numpy import genfromtxt

#data = genfromtxt('outputFile.csv', delimiter=':')
data = open('outputFile.csv','r')
agents=2
agentState=[]
for i in range(agents):
    for line in data:
        agentState.append(line.split(':')[i].split(';'))

data=np.array(agentState,float)

#width=data[0,0]
#height=data[0,1]
#X=data[0,2]
#Z=data[0,3]
#Should be width*height
features=2*9+1
data=data[1:,:]
data=data[:,1:]

outputs=data[:,features:]
outputs=np.delete(outputs,1,axis=1)
data=data[:,:features]

timesteps=10
processedData=[]
for i in range(len(data)-timesteps):
    line=data[i]
    for j in range(1,timesteps):
        line=np.append(line,data[i+j],axis=0)

    line=np.append(line,outputs[i+timesteps-1])
    processedData.append(line)


processedData=np.array(processedData)
print(np.shape(processedData))

#processedData=np.delete(processedData, np.s_[:(int(features/timesteps))],axis=1)
#processedData=np.append(processedData,[0]*(int(features/timesteps)),axis=1)

#processedData=np.delete(processedData, np.s_[:(int(features))],axis=1)
#processedData=np.append(processedData,[0]*(int(features)),axis=1)
