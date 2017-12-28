import sys
import numpy as np
import h5py
import math
import time

from keras.models import Sequential, Model, load_model
from keras.layers import Dense, Dropout, Activation, Flatten, BatchNormalization,Input
from keras.utils import np_utils
from keras import regularizers
from numpy import genfromtxt
from matplotlib import pyplot as plt
from keras.utils import plot_model
from keras.callbacks import EarlyStopping
import  socket
from threading import  Thread
import select


#For this to be working you should change the paths to your own.
def loadModels(AccelModel,AccelModel2,loader):
    if loader>0:
        return  AccelModel,AccelModel2,loader-1
    AccelModel= load_model(r'C:\Users\aborge01.cs8451\work\Simulator\AmyrSimData\AccelModel.h5')
    AccelModel2=load_model(r'C:\Users\aborge01.cs8451\work\Simulator\AmyrSimData\AccelModel2.h5')
    return AccelModel,AccelModel2,LOADAFTER
def processInput(input,inputs,agentInputs):
    if throw==1:
        return 0,inputs,agentInputs
    AllInputs = input.split(sep=':')
    newTimeStep=[]
    for aString in AllInputs:
        if len(aString)==0:
            pass
        else:
            arr = np.fromstring(aString, sep=';')
            arr = np.reshape(arr, (1, int(features/timesteps)))
            if len(inputs) == 0:
                inputs = arr
            else:
                inputs = np.append(inputs, arr, 0)
            newTimeStep.append(arr)
    newTimeStep=np.reshape(newTimeStep,[agents,int(features/timesteps)])
    #remove oldest step in time
    agentInputs = np.delete(agentInputs, np.s_[:(int(features/timesteps))], axis=1)
    #add new step
    agentInputs = np.append(agentInputs, np.array(newTimeStep), axis=1)
    #i dont even think that it's needed :P
    inputs=agentInputs
    return 0,inputs,agentInputs

def predict(inputs):
    Accel=AccelModel.predict([inputs], batch_size=8, verbose=0)
    Accel2=AccelModel2.predict([inputs], batch_size=8, verbose=0)
    stringToRet=''
    for acc,acc1 in zip(Accel,Accel2):
        stringToRet=stringToRet+str(acc[0])+','+str(acc1[0])+':'


    return stringToRet
def predictZeros():
    agents=len(inputs)
    stringToRet=''
    for i in range(int(agents)):
        stringToRet=stringToRet+str(0)+','+str(0)+':'
    return stringToRet

throw = 1
LOADAFTER=1500
timesteps=4
features=6*11+1
features*=timesteps
inputs=np.array([])
loader=1500+LOADAFTER
loader=10
AccelModel=None
AccelModel2=None
ok = sys.stdin.readline()
#File to check states -Debug purposes.
#trFile = open(r'C:\Users\kokos\Desktop\DiploGit\Simulator\SimulatorTests\out.txt', 'w')
agents=1
agentInputs=[]
for i in range(agents):
    agentInputs.append([0]*features)
agentInputs=np.reshape(agentInputs,[agents,features])
while True:
    ok = sys.stdin.readline()
    ok = ok[:-2]
    #Take input from Unity,process it,properly
    [throw, inputs,agentInputs] = processInput(ok, inputs,agentInputs)
    [AccelModel,AccelModel2,loader]=loadModels(AccelModel,AccelModel2,loader)
    if AccelModel!=None:
        print(predict(inputs),end='\n')
    else:
        print(predictZeros(),end='\n')
    sys.stdout.flush()
    #trFile.write(np.array_str(inputs))
    #trFile.flush()
    inputs = np.array([])
